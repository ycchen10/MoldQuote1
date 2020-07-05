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
    /// 圆柱特征
    /// </summary>
    public class CylinderFeater : IDisplayObject, IComparable<CylinderFeater>
    {
        public CylinderFace Cylinder { get; private set; }
        /// <summary>
        /// 圆形面（包括圆柱本身）
        /// </summary>
        public List<AbstractCircleFace> CylinderFace { get; private set; } = new List<AbstractCircleFace>();
        /// <summary>
        /// 起点
        /// </summary>
        public Point3d StartPt
        {
            get { return CylinderFace[0].StartPt; }
        }
        /// <summary>
        /// 终点
        /// </summary>
        public Point3d EndPt
        {
            get { return Cylinder.EndPt; }
        }
        /// <summary>
        /// 长度
        /// </summary>
        public double Length
        {
            get { return UMathUtils.GetDis(StartPt, this.CylinderFace[this.CylinderFace.Count - 1].EndPt); }
        }
        /// <summary>
        /// 方向
        /// </summary>
        public Vector3d Direction
        {
            get { return this.Cylinder.Direction; }
        }
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius
        {
            get { return Cylinder.Radius; }
        }

        public Node Node { get; set; }

        public CylinderFeater(List<AbstractCircleFace> circle, CylinderFace cyl)
        {
            this.Cylinder = cyl;
            this.CylinderFace = circle;
            SetDirection(cyl.Direction);
        }
        /// <summary>
        /// 设置轴向方向
        /// </summary>
        /// <param name="vec"></param>
        public void SetDirection(Vector3d vec)
        {
            //  Cylinder.SetReverseDirection(vec);
            foreach (AbstractCircleFace af in CylinderFace)
            {
                af.SetReverseDirection(vec);
            }
            CylinderFaceSort();
        }
        /// <summary>
        /// 排序
        /// </summary>
        private void CylinderFaceSort()
        {
            this.CylinderFace.Sort(delegate (AbstractCircleFace a, AbstractCircleFace b)
            {
                Point3d pt1 = a.CenterPt;
                Point3d pt2 = b.CenterPt;
                this.Cylinder.Matr.ApplyPos(ref pt1);
                this.Cylinder.Matr.ApplyPos(ref pt2);
                return pt1.Z.CompareTo(pt2.Z);
            });
        }
        public void Highlight(bool highlight)
        {
            foreach (AbstractCircleFace af in this.CylinderFace)
            {
                af.Highlight(highlight);
            }
        }

        public int CompareTo(CylinderFeater other)
        {
            return other.Radius.CompareTo(this.Radius);
        }
    }
}
