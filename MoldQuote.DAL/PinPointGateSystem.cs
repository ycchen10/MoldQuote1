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
    /// 细水口模架
    /// </summary>
    public class PinPointGateSystem : AbstractMoldBaseName
    {
        /// <summary>
        /// 水口推板
        /// </summary>
        public MoldBaseModel ShuiSupportPlate { get; private set; }
        public PinPointGateSystem(AnalysisMold mold) : base(mold)
        {
            GetUpBaseModel();
        }

        public override List<MoldQuoteNameInfo> GetBaseInfo()
        {
            List<MoldQuoteNameInfo> infos = new List<MoldQuoteNameInfo>();
            if (this.UpBaseplate != null)
                infos.Add(this.UpBaseplate.GetMoldBaseInfo());
            if (this.ShuiSupportPlate != null)
                infos.Add(this.ShuiSupportPlate.GetMoldBaseInfo());
            if (this.AMoldBase != null)
                infos.Add(this.AMoldBase.GetMoldBaseInfo());
            if (this.PushPlate != null)
                infos.Add(this.PushPlate.GetMoldBaseInfo());
            if (this.BMoldBase != null)
                infos.Add(this.BMoldBase.GetMoldBaseInfo());
            if (this.SupportPlate != null)
                infos.Add(this.SupportPlate.GetMoldBaseInfo());
            if (this.Baseplate != null)
                infos.Add(this.Baseplate.GetMoldBaseInfo());
            foreach (MoldBaseModel mm in this.FaceEiectorPlates)
            {
                infos.Add(mm.GetMoldBaseInfo());
            }
            foreach (MoldBaseModel mm in this.DowEiectorPlates)
            {
                infos.Add(mm.GetMoldBaseInfo());
            }
            foreach (MoldBaseModel mm in this.Spacer)
            {
                infos.Add(mm.GetMoldBaseInfo());
            }
            foreach (MoldBaseModel mm in this.OtherBaseModel)
            {
                infos.Add(mm.GetMoldBaseInfo());
            }
            return infos;
        }

        public override List<string> GetBolt()
        {
            List<CylinderTwoStepBody> bolt = new List<CylinderTwoStepBody>();
            List<AbstractCylinderBody> cyls = this.cylinderBody.FindAll(a => a is CylinderTwoStepBody && (a as CylinderTwoStepBody).IsBolt());
            foreach (AbstractCylinderBody ab in cyls)
            {
                Point3d start = ab.StratPt;
                Point3d end = ab.EndPt;
                this.analysis.Matr.ApplyPos(ref start);
                this.analysis.Matr.ApplyPos(ref end);
                if(start.Z< this.ShuiSupportPlate.CenterPt.Z+this.ShuiSupportPlate.DisPt.Z&&end.Z>this.AMoldBase.CenterPt.Z-this.AMoldBase.DisPt.Z)//推板到A板
                {
                    bolt.Add(ab as CylinderTwoStepBody);
                }
            }

        }

        public override List<string> GetGuideBushing()
        {
            throw new NotImplementedException();
        }

        public override List<string> GetGuidePillar()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 获取上模板
        /// </summary>
        protected override void GetUpBaseModel()
        {
            List<MoldBaseModel> up = analysis.GetUpModel(this.moldbase);
            if (UMathUtils.IsEqual(up[0].CenterPt.Z - up[0].DisPt.Z, this.AMoldBase.CenterPt.Z + this.AMoldBase.DisPt.Z))
            {
                this.ShuiSupportPlate = up[0];
                this.ShuiSupportPlate.Name = "水口推板";
            }
            this.UpBaseplate = up[up.Count - 1];
            this.UpBaseplate.Name = "水口板";

            List<MoldBaseModel> spa = this.analysis.GetSpacer(up);
            if (spa.Count > 0)
            {
                this.Spacer.AddRange(spa);
                List<MoldBaseModel> eiec = this.analysis.GetEiectorPlates(this.moldbase, spa);
                double max = eiec.Max(a => a.CenterPt.Z);
                foreach (MoldBaseModel mm in eiec)
                {
                    if (UMathUtils.IsEqual(mm.CenterPt.Z, max))
                    {
                        mm.Name = "底针板";
                        DowEiectorPlates.Add(mm);
                    }
                    else
                    {
                        mm.Name = "面针板";
                        this.FaceEiectorPlates.Add(mm);
                    }
                }
            }
            foreach (MoldBaseModel mm in up)
            {
                if (mm.Name == null || mm.Name.Equals(""))
                {
                    this.OtherBaseModel.Add(mm);
                }
            }

        }

    }
}
