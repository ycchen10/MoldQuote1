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
    /// 创建孔特征
    /// </summary>
    public class StepBuilder : IDisplayObject
    {
        public CircularFaceList List { get; private set; }
        public List<CylinderFeater> CylFeater { get; private set; } = new List<CylinderFeater>();
        public StepBuilder(CircularFaceList cir)
        {
            this.List = cir;
            CylFeater = this.List.GetCylinderFeaters();
            CylFeater.Sort(); //以半径排序
        }

        /// <summary>
        /// 设置轴向方向
        /// </summary>
        /// <param name="dir"></param>
        public void SetDirection(Vector3d dir)
        {
            foreach (CylinderFeater cf in CylFeater)
            {
                cf.SetDirection(dir);
            }
            CylinderFeaterSort(dir);
        }
        /// <summary>
        /// 以向量排序
        /// </summary>
        /// <param name="dir"></param>
        private void CylinderFeaterSort(Vector3d dir)
        {           
            this.CylFeater.Sort(delegate (CylinderFeater a, CylinderFeater b)
            {
                Point3d pt1 = UMathUtils.GetMiddle(a.StartPt, a.EndPt);
                Point3d pt2 = UMathUtils.GetMiddle(b.StartPt, b.EndPt);
                this.CylFeater[0].Cylinder.Matr.ApplyPos(ref pt1);
                this.CylFeater[0].Cylinder.Matr.ApplyPos(ref pt2);
                return pt1.Z.CompareTo(pt2.Z);
            });
        }
        public void Highlight(bool highlight)
        {
            this.List.Highlight(highlight);
        }
    }
}
