using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MoldQuote.DAL
{
    /// <summary>
    ///圆柱体类
    /// </summary>
    public class CylinderBody : AbstractCylinderBody
    {
        /// <summary>
        /// 半径
        /// </summary>
        public override double Radius
        {
            get { return Builder.CylFeater[0].Radius; }
        }

        public CylinderBody(StepBuilder builder) : base(builder)
        {

        }
        public override string ToString()
        {
            return "D" + (this.Radius * 2).ToString() + "H" + this.Length.ToString();
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
        public bool IsBolt()
        {
            Face topFace = this.Builder.CylFeater[0].CylinderFace[0].Data.Face;
            FaceLoopUtils.LoopList[] list = FaceLoopUtils.AskFaceLoops(topFace.Tag);
            List<FaceLoopUtils.LoopList> hole = new List<FaceLoopUtils.LoopList>();
            foreach (FaceLoopUtils.LoopList lt in list)
            {
                if (lt.Type == 2)
                {
                    hole.Add(lt);
                }
            }
            if (hole.Count == 1 && hole[0].EdgeList.Length == 5)
                return true;
            return false;
        }


    }
}
