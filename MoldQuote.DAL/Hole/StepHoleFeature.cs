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
    /// 平底盲孔
    /// </summary>
    public class StepHoleFeature : AbstractHoleFeater
    {
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius
        {
            get { return Builder.CylFeater[0].Radius; }
        }

        public StepHoleFeature(HoleBuilder builder) : base(builder)
        {
            this.Type = HoleType.StepHole;
        }
        protected override void GetDirection()
        {
            Vector3d dir = this.Builder.CylFeater[0].Direction;
            Point3d cylCenterPt = this.Builder.CylFeater[0].Cylinder.CenterPt;
            Body body = this.Builder.CylFeater[0].Cylinder.Data.Face.GetBody();
            int k = TraceARay.AskTraceARay(body, cylCenterPt, dir);
            if (k != 0)
            {
                this.Direction = dir;
            }
            else
            {
                this.Direction = new Vector3d(-dir.X, -dir.Y, -dir.Z);
                this.Builder.SetDirection(this.Direction);
            }
        }
        protected override void GetStartAndEndPt()
        {
            this.StratPt = this.Builder.CylFeater[0].StartPt;
            this.EndPt = this.Builder.CylFeater[0].EndPt;
        }
        public override string ToString()
        {
            return "D" + (this.Radius * 2).ToString("f3") + "H" + this.Length.ToString("f3");
        }
    }
}
