using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;

namespace MoldQuote.DAL
{
    /// <summary>
    /// 单一通孔
    /// </summary>
    public class OnlyThroughHoleFeature : AbstractHoleFeater
    {
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius
        {
            get { return Builder.CylFeater[0].Radius; }
        }

        public OnlyThroughHoleFeature(HoleBuilder builder) : base(builder)
        {
            this.Type = HoleType.OnlyThroughHole;

        }

        protected override void GetDirection()
        {
            this.Direction = this.Builder.CylFeater[0].Direction;
        }
        protected override void GetStartAndEndPt()
        {
            this.StratPt = this.Builder.CylFeater[0].StartPt;
            this.EndPt = this.Builder.CylFeater[0].CylinderFace[this.Builder.CylFeater[0].CylinderFace.Count - 1].EndPt;
        }
        public override string ToString()
        {
            return "D" + (this.Radius * 2).ToString("f3") + "H" + this.Length.ToString("f3");
        }
    }
}
