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
    /// 孔特征抽象类
    /// </summary>
    public abstract class AbstractHoleFeater : IDisplayObject, IEquatable<AbstractHoleFeater>
    {
        /// <summary>
        /// 起点
        /// </summary>
        public Point3d StratPt { get; protected set; }
        /// <summary>
        /// 终点
        /// </summary>
        public Point3d EndPt { get; protected set; }
        /// <summary>
        /// 方向
        /// </summary>
        public Vector3d Direction { get; protected set; }

        public double Length
        {
            get
            {
                return UMathUtils.GetDis(this.StratPt, this.EndPt);
            }
        }
        /// <summary>
        /// 孔类型
        /// </summary>
        public HoleType Type { get; protected set; }
        /// <summary>
        /// 孔特征
        /// </summary>
        public HoleBuilder Builder { get; private set; }

        public AbstractHoleFeater(HoleBuilder builder)
        {
            this.Builder = builder;
            this.Builder.CylFeater.Sort();
            GetDirection();
            GetStartAndEndPt();
        }

        public void Highlight(bool highlight)
        {
            this.Builder.Highlight(highlight);
        }
        /// <summary>
        /// 获取起点和终点
        /// </summary>
        protected abstract void GetStartAndEndPt();
        /// <summary>
        /// 获取顶边
        /// </summary>
        /// <returns></returns>
        public  ArcEdgeData GetTopEdge()
        {
            string err = "";
            List<ArcEdgeData> arcs = new List<ArcEdgeData>();
            foreach (Edge eg in this.Builder.CylFeater[0].Cylinder.Data.Face.GetEdges())
            {
                if (eg.SolidEdgeType == Edge.EdgeType.Circular)
                {
                    ArcEdgeData data = EdgeUtils.GetArcData(eg, ref err);
                    arcs.Add(data);
                }
            }
            arcs.Sort(delegate (ArcEdgeData a, ArcEdgeData b)
            {
                Matrix4 mat = this.Builder.CylFeater[0].Cylinder.Matr;
                Point3d centerPt1 = a.Center;
                Point3d centerPt2 = b.Center;
                mat.ApplyPos(ref centerPt1);
                mat.ApplyPos(ref centerPt2);
                return centerPt2.Z.CompareTo(centerPt1.Z);
            });
            return arcs[0];
        }
        /// <summary>
        /// 获取轴向方向
        /// </summary>
        protected abstract void GetDirection();

        public bool Equals(AbstractHoleFeater other)
        {
            double angle = UMathUtils.Angle(this.Direction, other.Direction);
            if (this.Type == other.Type && this.ToString().Equals(other.ToString(), StringComparison.CurrentCultureIgnoreCase) && UMathUtils.IsEqual(angle, 0))
            {
                Matrix4 mat = new Matrix4();
                mat.Identity();
                mat.TransformToZAxis(this.StratPt, this.Direction);
                Point3d strat = this.StratPt;
                Point3d otherStrat = other.StratPt;
                mat.ApplyPos(ref strat);
                mat.ApplyPos(ref otherStrat);
                if (UMathUtils.IsEqual(strat.Z, otherStrat.Z))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public enum HoleType
    {
        OnlyBlindHole = 1,             //单一盲孔
        OnlyThroughHole = 2,           //单一通孔
        StepBlindHole = 3,            // 台阶盲孔
        StepHole = 4,                 //平底盲孔
        StepThroughHole = 5           //台阶通孔
    }
}
