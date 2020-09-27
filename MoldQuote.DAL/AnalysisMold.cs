using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MoldQuote.Model;

namespace MoldQuote.DAL
{
    /// <summary>
    /// 分析模架
    /// </summary>
    public class AnalysisMold
    {
        private Body aBody;
        private Body bBody;
        public Matrix4 Matr { get; private set; } = new Matrix4();
        private Part workPart;
        private CartesianCoordinateSystem csys;
        /// <summary>
        /// 上模
        /// </summary>
        public List<MoldBaseModel> UpModel { get; private set; } = new List<MoldBaseModel>();
        /// <summary>
        /// A板
        /// </summary>
        public MoldBaseModel AMoldBase { get; private set; }
        /// <summary>
        /// 下模
        /// </summary>
        public List<MoldBaseModel> DownModel { get; private set; } = new List<MoldBaseModel>();
        /// <summary>
        /// B版
        /// </summary>
        public MoldBaseModel BMoldBase { get; private set; }
        public AnalysisMold(Body aBody, Body bBody)
        {
            this.aBody = aBody;
            this.bBody = bBody;
            workPart = Session.GetSession().Parts.Work;
            this.Matr = GetMatr();
            Matrix4 inv = this.Matr.GetInversMatrix();
            csys = BoundingBoxUtils.CreateCoordinateSystem(this.Matr, inv);
            MoldBaseModel aMold = new MoldBaseModel(aBody, Matr, csys);
            aMold.Name = "A板";
            this.AMoldBase = aMold;
            MoldBaseModel bMold = new MoldBaseModel(bBody, Matr, csys);
            bMold.Name = "B板";
            this.BMoldBase = bMold;
        }
        /// <summary>
        /// 根据AB板得到矩阵
        /// </summary>
        /// <returns></returns>
        private Matrix4 GetMatr()
        {
            CoordinateSystem wcs = workPart.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToCsys(wcs, ref mat);
            Matrix4 inv = mat.GetInversMatrix();
            CartesianCoordinateSystem cs = BoundingBoxUtils.CreateCoordinateSystem(mat, inv);
            MoldBaseModel aMold = new MoldBaseModel(aBody, mat, cs);
            MoldBaseModel bMold = new MoldBaseModel(bBody, mat, cs);
            Vector3d vec = UMathUtils.GetVector(bMold.CenterPt, aMold.CenterPt);
            Point3d center = UMathUtils.GetMiddle(bMold.CenterPt, aMold.CenterPt);
            inv.ApplyPos(ref center);
            mat.TransformToZAxis(center, vec);
            return mat;
        }
        /// <summary>
        /// 获取模板和圆柱形
        /// </summary>
        /// <returns></returns>
        public void GetBase(out List<MoldBaseModel> moldBase, out List<AbstractCylinderBody> cylinder)
        {
            moldBase = new List<MoldBaseModel>();
            cylinder = new List<AbstractCylinderBody>();

            foreach (Body by in workPart.Bodies)
            {
                MoldBaseModel mm = new MoldBaseModel(by, this.Matr, csys);
                if ((UMathUtils.IsEqual(mm.CenterPt.X, 0) && UMathUtils.IsEqual(mm.CenterPt.Y, 0)) &&
                    ((Math.Round(mm.DisPt.X, 4) >= Math.Round(AMoldBase.DisPt.X, 4) &&
                    Math.Round(mm.DisPt.Y, 4) >= Math.Round(AMoldBase.DisPt.Y, 4))))
                {
                    moldBase.Add(mm);
                }
                else
                {
                    StepBuilder builder;
                    if (mm.DisPt.Z > mm.DisPt.X && mm.DisPt.Z > mm.DisPt.Y)
                    {
                        BodyCircleFeater bf = new BodyCircleFeater(by);
                        if (bf.IsCylinderBody(out builder))
                        {
                            AbstractCylinderBody ab = CylinderBodyFactory.Create(builder);
                            if (ab != null)
                            {
                                double angle = UMathUtils.Angle(ab.Direction, this.Matr.GetZAxis());
                                if (UMathUtils.IsEqual(angle, 0) || UMathUtils.IsEqual(angle, Math.PI))
                                    cylinder.Add(ab);
                            }
                        }
                    }
                    else if ((UMathUtils.IsEqual(mm.CenterPt.X, 0) || UMathUtils.IsEqual(mm.CenterPt.Y, 0)) &&
                        ((Math.Round(mm.DisPt.X, 4) >= Math.Round(AMoldBase.DisPt.X, 4) ||
                         Math.Round(mm.DisPt.Y, 4) >= Math.Round(AMoldBase.DisPt.Y, 4))))
                    {
                        moldBase.Add(mm);
                    }
                }
            }
        }
        /// <summary>
        /// 获取上模板
        /// </summary>
        /// <param name="molds">上模板</param>
        /// <returns></returns>
        public List<MoldBaseModel> GetUpModel(List<MoldBaseModel> molds)
        {
            List<MoldBaseModel> up = molds.FindAll(a => a.CenterPt.Z > 0);

            double maxZ = AMoldBase.CenterPt.Z + AMoldBase.DisPt.Z;
            List<MoldBaseModel> mold = SeekUpMoldBaseModel(up, ref maxZ);
            List<MoldBaseModel> upModel = new List<MoldBaseModel>();
            while (mold.Count > 0)
            {
                upModel.AddRange(mold);
                mold = SeekUpMoldBaseModel(molds, ref maxZ);
            }
            upModel.Sort(delegate (MoldBaseModel a, MoldBaseModel b
                )
            {
                return a.CenterPt.Z.CompareTo(b.CenterPt.Z);
            });
            return upModel;
        }
        /// <summary>
        /// 获取下模板
        /// </summary>
        /// <param name="molds"></param>
        public List<MoldBaseModel> GetDownModel(List<MoldBaseModel> molds)
        {
            List<MoldBaseModel> down = molds.FindAll(a => a.CenterPt.Z < 0);
            double minZ = BMoldBase.CenterPt.Z - BMoldBase.DisPt.Z;
            List<MoldBaseModel> mold = SeekDownMoldBaseModel(down, ref minZ);
            List<MoldBaseModel> down1 = new List<MoldBaseModel>();
            while (mold.Count > 0)
            {
                down1.AddRange(mold);
                mold = SeekDownMoldBaseModel(molds, ref minZ);
            }
            down1.Sort(delegate (MoldBaseModel a, MoldBaseModel b)
            {
                return b.CenterPt.Z.CompareTo(a.CenterPt.Z);
            });
            return down1;
        }
        /// <summary>
        /// 向上搜索
        /// </summary>
        /// <param name="molds"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private List<MoldBaseModel> SeekUpMoldBaseModel(List<MoldBaseModel> molds, ref double maxz)
        {
            List<MoldBaseModel> mb = new List<MoldBaseModel>();
            foreach (MoldBaseModel mm in molds)
            {
                double mmz = mm.CenterPt.Z - mm.DisPt.Z;
                if (UMathUtils.IsEqual(maxz, mmz))
                {
                    mb.Add(mm);
                }
            }
            if (mb.Count > 0)
                maxz = mb[0].CenterPt.Z + mb[0].DisPt.Z;
            return mb;
        }
        /// <summary>
        /// 向下搜索
        /// </summary>
        /// <param name="molds"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private List<MoldBaseModel> SeekDownMoldBaseModel(List<MoldBaseModel> molds, ref double minz)
        {
            List<MoldBaseModel> mb = new List<MoldBaseModel>();
            foreach (MoldBaseModel mm in molds)
            {
                double mmz = mm.CenterPt.Z + mm.DisPt.Z;
                if (UMathUtils.IsEqual(minz, mmz))
                {
                    mb.Add(mm);
                }
            }
            if (mb.Count > 0)
                minz = mb[0].CenterPt.Z - mb[0].DisPt.Z;
            return mb;
        }
        /// <summary>
        /// 获取垫脚
        /// </summary>
        /// <param name="molds"></param>
        /// <returns></returns>
        public List<MoldBaseModel> GetSpacer(List<MoldBaseModel> molds)
        {
            List<MoldBaseModel> spacer = new List<MoldBaseModel>();
            foreach (MoldBaseModel mm in molds)
            {
                if (!(UMathUtils.IsEqual(mm.CenterPt.X, 0) && UMathUtils.IsEqual(mm.CenterPt.Y, 0)))
                {
                    mm.Name = "方铁";
                    spacer.Add(mm);
                }
            }
            return spacer;
        }
        /// <summary>
        /// 获取顶针板
        /// </summary>
        /// <param name="molds">模板</param>
        /// <param name="spacer">垫脚</param>
        /// <returns></returns>
        public List<MoldBaseModel> GetEiectorPlates(List<MoldBaseModel> molds, List<MoldBaseModel> spacer)
        {
            double minZ = Math.Round(spacer[0].CenterPt.Z - spacer[0].DisPt.Z, 3);
            double maxZ = Math.Round(spacer[0].CenterPt.Z + spacer[0].DisPt.Z, 3);
            List<MoldBaseModel> moldBases = molds.FindAll(a => Math.Round(a.CenterPt.Z - a.DisPt.Z, 3) > minZ && Math.Round(a.CenterPt.Z + a.DisPt.Z, 3) < maxZ).ToList();
            List<MoldBaseModel> eiector = new List<MoldBaseModel>();
            if (moldBases.Count > 0)
            {
                if (spacer[0].DisPt.X > spacer[0].DisPt.Y)
                {
                    double max = moldBases.Max(a => a.DisPt.X);
                    eiector = moldBases.FindAll(a => a.DisPt.X == max).ToList();
                }
                else
                {
                    double max = moldBases.Max(a => a.DisPt.Y);
                    eiector = moldBases.FindAll(a => a.DisPt.Y == max).ToList();
                }
            }
            return eiector;
        }

        /// <summary>
        ///获取推板
        /// </summary>
        /// <param name="molds"></param>
        /// <returns></returns>
        public MoldBaseModel GetKnockoutPlate(List<MoldBaseModel> molds)
        {
            return molds.Find(a => (a.CenterPt.Z > this.BMoldBase.CenterPt.Z && a.CenterPt.Z < this.AMoldBase.CenterPt.Z
             && UMathUtils.IsEqual(a.DisPt.X, this.AMoldBase.DisPt.X) && UMathUtils.IsEqual(a.DisPt.Y, this.AMoldBase.DisPt.Y)));
        }
    }
}
