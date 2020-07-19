using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.Utilities;
using Basic;

namespace MoldQuote.DAL
{
    /// <summary>
    ///二台阶圆柱体类
    /// </summary>
    public class CylinderManyStepBody : AbstractCylinderBody
    {
        /// <summary>
        /// 半径
        /// </summary>
        public override double Radius
        {
            get
            {
                double length = this.Builder.CylFeater.Max(a => a.Length);
                return this.Builder.CylFeater.Find(a => UMathUtils.IsEqual(a.Length, length)).Radius;
            }
        }

        public CylinderManyStepBody(StepBuilder builder) : base(builder)
        {

        }
        public override string ToString()
        {
            return "D" + (this.Radius * 2).ToString() + "H" + this.Length.ToString() + "T" + this.Builder.CylFeater[0].Length.ToString();
        }

        protected override void GetDirection()
        {
            this.Direction = UMathUtils.GetVector(this.Builder.CylFeater[0].Cylinder.CenterPt, this.Builder.CylFeater[1].Cylinder.CenterPt);
            this.Builder.SetDirection(this.Direction);
        }

        protected override void GetStartAndEndPt()
        {
            int count = this.Builder.CylFeater.Count;
            this.StratPt = this.Builder.CylFeater[0].StartPt;
            this.EndPt = this.Builder.CylFeater[count - 1].CylinderFace[this.Builder.CylFeater[count-1].CylinderFace.Count - 1].EndPt;
        }

    }
}
