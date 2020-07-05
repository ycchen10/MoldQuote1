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
        private Matrix4 matr = new Matrix4();
        private Part workPart;
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
            this.matr = GetMatr();
            MoldBaseModel aMold = new MoldBaseModel(aBody, matr);
            aMold.Name = "A板";
            this.AMoldBase = aMold;
            MoldBaseModel bMold = new MoldBaseModel(bBody, matr);
            bMold.Name = "B板";
            this.BMoldBase = bMold;
        }

        private Matrix4 GetMatr()
        {
            CoordinateSystem wcs = workPart.WCS.CoordinateSystem;
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToCsys(wcs, ref matr);
            Matrix4 inv = mat.GetInversMatrix();
            MoldBaseModel aMold = new MoldBaseModel(aBody, mat);
            MoldBaseModel bMold = new MoldBaseModel(bBody, mat);
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
                MoldBaseModel mm = new MoldBaseModel(by, this.matr);
                if (UMathUtils.IsEqual(mm.CenterPt.X, 0) && UMathUtils.IsEqual(mm.CenterPt.Y, 0))
                {
                    moldBase.Add(mm);
                }
                else
                {
                    StepBuilder builder;
                    BodyCircleFeater bf = new BodyCircleFeater(by);
                    if (bf.IsCylinderBody(out builder))
                    {
                        AbstractCylinderBody ab = CylinderBodyFactory.Create(builder);

                        if (ab != null)
                        {
                            double angle = UMathUtils.Angle(ab.Direction, this.matr.GetZAxis());
                            if (UMathUtils.IsEqual(angle, 0) || UMathUtils.IsEqual(angle, Math.PI))
                                cylinder.Add(ab);
                        }
                    }
                    else if (UMathUtils.IsEqual(mm.CenterPt.X, 0) || UMathUtils.IsEqual(mm.CenterPt.Y, 0))
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
            List<MoldBaseModel> mold = SeekUpMoldBaseModel(up, maxZ);
            List<MoldBaseModel> upModel = new List<MoldBaseModel>();
            while (mold.Count > 0)
            {
                upModel.AddRange(mold);
                maxZ = mold[0].CenterPt.Z + mold[0].DisPt.Z;
                mold = SeekUpMoldBaseModel(molds, maxZ);
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
            List<MoldBaseModel> mold = SeekDownMoldBaseModel(down, minZ);
            List<MoldBaseModel> down1 = new List<MoldBaseModel>();
            while (mold.Count > 0)
            {
                down1.AddRange(mold);
                minZ = mold[0].CenterPt.Z - mold[0].DisPt.Z;
                mold = SeekDownMoldBaseModel(molds, minZ);
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
        private List<MoldBaseModel> SeekUpMoldBaseModel(List<MoldBaseModel> molds, double maxz)
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
            return mb;
        }
        /// <summary>
        /// 向下搜索
        /// </summary>
        /// <param name="molds"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private List<MoldBaseModel> SeekDownMoldBaseModel(List<MoldBaseModel> molds, double maxz)
        {
            List<MoldBaseModel> mb = new List<MoldBaseModel>();
            foreach (MoldBaseModel mm in molds)
            {
                double mmz = mm.CenterPt.Z + mm.DisPt.Z;
                if (UMathUtils.IsEqual(maxz, mmz))
                {
                    mb.Add(mm);
                }
            }
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
            double minZ = spacer[0].CenterPt.Z - spacer[0].DisPt.Z;
            double maxZ = spacer[0].CenterPt.Z + spacer[0].DisPt.Z;
            List<MoldBaseModel> moldBases = molds.FindAll(a => a.CenterPt.Z > minZ && a.CenterPt.Z < maxZ).ToList();
            List<MoldBaseModel> eiector = new List<MoldBaseModel>();
            if (spacer[0].DisPt.X > spacer[0].DisPt.Y)
            {
                double max = 0;
                foreach (MoldBaseModel mm in moldBases)
                {
                    if (mm.DisPt.X > max)
                        max = mm.DisPt.X;
                }
                eiector = moldBases.FindAll(a => a.DisPt.X == max).ToList();
            }
            else
            {
                double max = 0;
                foreach (MoldBaseModel mm in moldBases)
                {
                    if (mm.DisPt.Y > max)
                        max = mm.DisPt.Y;
                }
                eiector = moldBases.FindAll(a => a.DisPt.X == max).ToList();
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
