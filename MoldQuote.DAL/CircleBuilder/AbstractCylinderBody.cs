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
    ///圆柱体抽象类
    /// </summary>
    public abstract class AbstractCylinderBody : IDisplayObject, IEquatable<AbstractCylinderBody>
    {
        /// <summary>
        /// 圆柱台阶特征
        /// </summary>
        public StepBuilder Builder { get; private set; }
        /// <summary>
        /// 起点
        /// </summary>
        public Point3d StratPt { get; protected set; }
        /// <summary>
        /// 总点
        /// </summary>
        public Point3d EndPt { get; protected set; }
        /// <summary>
        /// 方向
        /// </summary>
        public Vector3d Direction { get; protected set; }
        /// <summary>
        /// 长度
        /// </summary>
        public double Length
        {
            get
            {
                return UMathUtils.GetDis(this.StratPt, this.EndPt);
            }
        }
        public string Name { get; set; }

        public Body Body
        {
            get
            {
                return this.Builder.CylFeater[0].CylinderFace[0].Data.Face.GetBody();
            }
        }

        public abstract double Radius { get; }
        public AbstractCylinderBody(StepBuilder builder)
        {
            this.Builder = builder;
            this.Builder.CylFeater.Sort();
            this.GetDirection();
            this.GetStartAndEndPt();
        }
        /// <summary>
        /// 获取轴向方向
        /// </summary>
        protected abstract void GetDirection();
        /// <summary>
        /// 获取起点和终点
        /// </summary>
        protected abstract void GetStartAndEndPt();
        public bool Equals(AbstractCylinderBody other)
        {
            return this.ToString().Equals(other.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        public void Highlight(bool highlight)
        {
            this.Builder.Highlight(highlight);
        }
        /// <summary>
        ///判断是否是导套
        /// </summary>
        /// <returns></returns>
        public bool IsGuidePin()
        {
            Face face = this.Builder.CylFeater[0].CylinderFace[0].Data.Face;
            FaceLoopUtils.LoopList[] loopList = FaceLoopUtils.AskFaceLoops(face.Tag);
            string err = "";
            Vector3d vec = new Vector3d(-this.Direction.X, -this.Direction.Y, -this.Direction.Z);
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

                    {
                        ArcEdgeData data = EdgeUtils.GetArcData(edge, ref err);
                        if (data.Radius >= 8 && UMathUtils.IsEqual(data.Angle, Math.PI * 2))
                        {
                            int count1 = TraceARay.AskTraceARay(this.Body, data.Center, vec);
                            int count2 = TraceARay.AskTraceARay(this.Body, data.Center, this.Direction);
                            if (count1 == 0 && count2 == 0)
                            {
                                return true;
                            }
                        }

                    }

                }

            }
            return false;
        }

        /// <summary>
        /// 判断是否是导柱
        /// </summary>
        /// <returns></returns>
        public bool IsGuidePillar()
        {
            Face face = this.Builder.CylFeater[0].CylinderFace[0].Data.Face;
            FaceLoopUtils.LoopList[] loopList = FaceLoopUtils.AskFaceLoops(face.Tag);
            if (loopList.Length == 1 && loopList[0].Type == 1)
            {
                return true;
            }
            else if (loopList.Length >= 2)
            {
                string err = "";
                Vector3d vec = new Vector3d(-this.Direction.X, -this.Direction.Y, -this.Direction.Z);
                foreach (FaceLoopUtils.LoopList lt in loopList)
                {
                    if (lt.Type == 2 && lt.EdgeList.Length == 1)
                    {
                        Edge edge = NXObjectManager.Get(lt.EdgeList[0]) as Edge;
                        if (edge.SolidEdgeType == Edge.EdgeType.Circular)

                        {
                            ArcEdgeData data = EdgeUtils.GetArcData(edge, ref err);
                            if (UMathUtils.IsEqual(data.Angle, Math.PI * 2))
                            {
                                int count1 = TraceARay.AskTraceARay(this.Body, data.Center, vec);
                                int count2 = TraceARay.AskTraceARay(this.Body, data.Center, this.Direction);
                                if (count1 != 0 || count2 != 0)
                                {
                                    return true;
                                }
                            }

                        }

                    }
                }
            }
            return false;
        }
    }
}
