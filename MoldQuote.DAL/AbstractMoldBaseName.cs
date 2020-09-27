using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;
using MoldQuote.Model;

namespace MoldQuote.DAL
{
    /// <summary>
    /// 模板名称
    /// </summary>
    public abstract class AbstractMoldBaseName
    {
        protected AnalysisMold analysis;
        protected List<MoldBaseModel> moldbase = new List<MoldBaseModel>();
        protected List<AbstractCylinderBody> cylinderBody = new List<AbstractCylinderBody>();
        /// <summary>
        /// A板
        /// </summary>
        public MoldBaseModel AMoldBase
        {
            get { return analysis.AMoldBase; }
        }
        /// <summary>
        /// B板
        /// </summary>
        public MoldBaseModel BMoldBase
        {
            get { return analysis.BMoldBase; }
        }
        /// <summary>
        /// 上底板
        /// </summary>
        public MoldBaseModel UpBaseplate { get; protected set; } = null;
        /// <summary>
        /// 底板
        /// </summary>
        public MoldBaseModel Baseplate { get; protected set; } = null;
        /// <summary>
        /// 托板
        /// </summary>
        public MoldBaseModel SupportPlate { get; protected set; } = null;
        /// <summary>
        /// 推板
        /// </summary>
        public MoldBaseModel PushPlate { get; protected set; } = null;
        /// <summary>
        /// 方铁
        /// </summary>
        public List<MoldBaseModel> Spacer { get; protected set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 面顶针板
        /// </summary>
        public List<MoldBaseModel> FaceEiectorPlates { get; protected set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 底针板
        /// </summary>
        public List<MoldBaseModel> DowEiectorPlates { get; protected set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 其他无名板
        /// </summary>
        public List<MoldBaseModel> OtherBaseModel { get; protected set; } = new List<MoldBaseModel>();

        protected List<AbstractCylinderBody> pin = new List<AbstractCylinderBody>();
        public AbstractMoldBaseName(AnalysisMold analysis)
        {
            this.analysis = analysis;
            analysis.GetBase(out moldbase, out cylinderBody);
            GetDowBaseModel();
        }

        private void GetDowBaseModel()
        {
            List<MoldBaseModel> down = analysis.GetDownModel(this.moldbase);
            if (down.Count > 0)
            {
                if (down[down.Count - 1].DisPt.Z * 2 <= 10 && down.Count != 1)
                {
                    down.RemoveAt(down.Count - 1);
                }
                if (UMathUtils.IsEqual(down[0].CenterPt.Z + down[0].DisPt.Z, this.BMoldBase.CenterPt.Z - this.BMoldBase.DisPt.Z)
                    && UMathUtils.IsEqual(down[0].CenterPt.X, 0) && UMathUtils.IsEqual(down[0].CenterPt.Y, 0))
                {
                    this.SupportPlate = down[0];
                    this.SupportPlate.Name = "托板";
                }
                if (down.Count > 1)
                {
                    this.Baseplate = down[down.Count - 1];
                    this.Baseplate.Name = "底板";
                }

                MoldBaseModel kon = this.analysis.GetKnockoutPlate(this.moldbase);
                if (kon != null)
                {
                    this.PushPlate = kon;
                    this.PushPlate.Name = "推板";
                }
                List<MoldBaseModel> spa = this.analysis.GetSpacer(down); //方铁
                if (spa.Count > 0)
                {
                    this.Spacer.AddRange(spa);
                    List<MoldBaseModel> eiec = this.analysis.GetEiectorPlates(this.moldbase, spa);
                    if (eiec.Count != 0)
                    {

                        double max = eiec.Max(a => a.CenterPt.Z);
                        foreach (MoldBaseModel mm in eiec)
                        {
                            if (UMathUtils.IsEqual(mm.CenterPt.Z, max))
                            {
                                mm.Name = "面针板";
                                FaceEiectorPlates.Add(mm);
                            }
                            else
                            {
                                mm.Name = "底针板";
                                DowEiectorPlates.Add(mm);
                            }
                        }
                    }
                }
            }
            foreach (MoldBaseModel mm in down) //无名板
            {
                if (mm.Name == null || mm.Name.Equals(""))
                {
                    this.OtherBaseModel.Add(mm);
                }
            }
        }
        protected bool IsPassThrough(AbstractCylinderBody ab, MoldBaseModel start, MoldBaseModel end)
        {
            double anlge = UMathUtils.Angle(analysis.Matr.GetZAxis(), ab.Direction);
            Point3d startPt = ab.StratPt;
            Point3d endPt = ab.EndPt;
            this.analysis.Matr.ApplyPos(ref startPt);
            this.analysis.Matr.ApplyPos(ref endPt);

            if (UMathUtils.IsEqual(anlge, 0))
            {
                if (startPt.Z > start.CenterPt.Z - start.DisPt.Z && endPt.Z < end.CenterPt.Z + end.DisPt.Z && startPt.Z < end.CenterPt.Z - end.DisPt.Z)
                    return true;
            }
            if (UMathUtils.IsEqual(anlge, Math.PI))
            {
                if (startPt.Z < start.CenterPt.Z + start.DisPt.Z && endPt.Z > end.CenterPt.Z - end.DisPt.Z && startPt.Z > end.CenterPt.Z + end.DisPt.Z)
                    return true;
            }
            return false;
        }

        protected bool IsPassThrough(AbstractCylinderBody ab, double start, double end)
        {
            double anlge = UMathUtils.Angle(analysis.Matr.GetZAxis(), ab.Direction);
            Point3d startPt = ab.StratPt;
            Point3d endPt = ab.EndPt;
            this.analysis.Matr.ApplyPos(ref startPt);
            this.analysis.Matr.ApplyPos(ref endPt);

            if (UMathUtils.IsEqual(anlge, 0))
            {
                if (startPt.Z > start && endPt.Z < end)
                    return true;
            }
            if (UMathUtils.IsEqual(anlge, Math.PI))
            {
                if (startPt.Z < start && endPt.Z > end)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 获取模架
        /// </summary>
        /// <returns></returns>
        protected abstract void GetUpBaseModel();
        /// <summary>
        /// 获取模板信息
        /// </summary>
        /// <returns></returns>
        public abstract List<MoldQuoteNameInfo> GetBaseInfo();
        /// <summary>
        /// 获取导套信息
        /// </summary>
        /// <returns></returns>
        public abstract List<StandardPartsName> GetGuideBushing();
        /// <summary>
        /// 获取导柱信息
        /// </summary>
        /// <returns></returns>
        public abstract List<StandardPartsName> GetGuidePillar();
        /// <summary>
        /// 获取螺栓信息
        /// </summary>
        /// <returns></returns>
        public abstract List<StandardPartsName> GetBolt();
        /// <summary>
        /// 获取圆柱名称
        /// </summary>
        /// <param name="cyl"></param>
        /// <returns></returns>
        protected List<StandardPartsName> GetCyliderName(List<AbstractCylinderBody> cyl)
        {
            var temp = cyl.GroupBy(a => a.ToString());
            List<StandardPartsName> st = new List<StandardPartsName>();
            foreach (var item in temp)
            {
                StandardPartsName spn = new StandardPartsName();
                spn.Count = item.Count();
                spn.Name = item.First().Name;
                spn.Dia = Math.Ceiling(item.First().Radius * 2).ToString();
                spn.Length = Math.Ceiling(item.First().Length).ToString();
                foreach (AbstractCylinderBody cy in item)
                {
                    spn.Bodys.Add(cy.Body);
                }
                st.Add(spn);
            }
            return st;
        }
        /// <summary>
        /// 添加板件
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool AddMoldBody(Body body, string name)
        {
            StepBuilder builder;
            BodyCircleFeater bf = new BodyCircleFeater(body);
            Matrix4 inv = this.analysis.Matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.analysis.Matr, inv);
            if (bf.IsCylinderBody(out builder))
            {
                return false;
            }
            else
            {
                MoldBaseModel mold = new MoldBaseModel(body, this.analysis.Matr, csys);
                mold.Name = name;
                this.OtherBaseModel.Add(mold);
                return true;
            }
        }

    }
}
