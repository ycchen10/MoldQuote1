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
    public class CylinderTwoStepBody : AbstractCylinderBody
    {
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius
        {
            get { return Builder.CylFeater[1].Radius; }
        }

        public CylinderTwoStepBody(StepBuilder builder) : base(builder)
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
            this.StratPt = this.Builder.CylFeater[0].StartPt;
            this.EndPt = this.Builder.CylFeater[1].CylinderFace[this.Builder.CylFeater[1].CylinderFace.Count - 1].EndPt;
        }
        /// <summary>
        /// 判断是否是螺栓
        /// </summary>
        /// <returns></returns>
        public bool IsBolt()
        {
            Face face = this.Builder.CylFeater[0].CylinderFace[0].Data.Face;
            FaceLoopUtils.LoopList[] loopList = FaceLoopUtils.AskFaceLoops(face.Tag);
            if (loopList.Length != 2)
            {
                return false;
            }
            foreach (FaceLoopUtils.LoopList lt in loopList)
            {
                if (lt.Type == 2 && lt.EdgeList.Length == 6)
                {
                    return true;
                }

            }
            return false;
        }
        public bool IsGuidePin()
        {
            Face face = this.Builder.CylFeater[0].CylinderFace[0].Data.Face;
            FaceLoopUtils.LoopList[] loopList = FaceLoopUtils.AskFaceLoops(face.Tag);
            if (loopList.Length != 2)
            {
                return false;
            }
            foreach (FaceLoopUtils.LoopList lt in loopList)
            {
                if (lt.Type == 2 && lt.EdgeList.Length == 1)
                {
                    Edge edge = NXObjectManager.Get(lt.EdgeList[0]) as Edge;
                    if (edge.SolidEdgeType == Edge.EdgeType.Circular)

                        return true;
                }

            }
            return false;
        }
    }
}
