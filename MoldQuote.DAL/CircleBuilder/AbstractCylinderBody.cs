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
    }
}
