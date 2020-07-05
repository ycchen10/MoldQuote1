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
    /// 台阶通孔
    /// </summary>
    public class StepThroughHoleFeature : AbstractHoleFeater
    {
        /// <summary>
        /// 台阶数
        /// </summary>
        public double Count
        {
            get { return this.Builder.CylFeater.Count; }
        }

        public StepThroughHoleFeature(HoleBuilder builder) : base(builder)
        {
            this.Type = HoleType.StepThroughHole;

        }

        protected override void GetDirection()
        {
            Point3d pt1 = this.Builder.CylFeater[0].Cylinder.CenterPt;
            Point3d pt2 = this.Builder.CylFeater[1].Cylinder.CenterPt;
            Vector3d vec = UMathUtils.GetVector(pt1, pt2);
            this.Direction = vec;
            this.Builder.SetDirection(vec);

        }
        protected override void GetStartAndEndPt()
        {
            this.StratPt = this.Builder.CylFeater[0].StartPt;
            this.EndPt = this.Builder.CylFeater[this.Builder.CylFeater.Count - 1].CylinderFace[this.Builder.CylFeater[this.Builder.CylFeater.Count - 1].CylinderFace.Count - 1].EndPt;
        }
        public override string ToString()
        {
            string temp = "";
            foreach (CylinderFeater cf in this.Builder.CylFeater)
            {
                temp += "D" + (cf.Radius * 2).ToString("f3") +"H"+ cf.Length.ToString("f3");
            }
            return temp;
        }
    }
}
