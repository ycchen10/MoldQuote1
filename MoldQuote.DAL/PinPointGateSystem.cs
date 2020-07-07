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
        public PinPointGateSystem(AnalysisMold mold) : base(mold)
        {

        }
        protected override void GetBaseModel()
        {
            List<MoldBaseModel> down = analysis.GetDownModel(this.moldbase);
            if (UMathUtils.IsEqual(down[0].CenterPt.Z + down[0].DisPt.Z, this.AMoldBase.CenterPt.Z - this.AMoldBase.DisPt.Z))
            {
                this.SupportPlate = down[0];
                this.SupportPlate.Name = "托板";
            }
            this.Baseplate = down[down.Count - 1];
            this.Baseplate.Name = "底板";
            MoldBaseModel kon = this.analysis.GetKnockoutPlate(this.moldbase);
            if(kon!=null)
            {
                this.PushPlate = kon;
                this.PushPlate.Name = "推板";
            }


        }
        public override AbstractCylinderBody GetCylinder()
        {
            throw new NotImplementedException();
        }
    }
}
