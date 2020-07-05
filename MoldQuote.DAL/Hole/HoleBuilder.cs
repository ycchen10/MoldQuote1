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
    public class HoleBuilder : IDisplayObject
    {
        public CircularFaceList List { get; private set; }
        public List<CylinderFeater> CylFeater { get; private set; } = new List<CylinderFeater>();
        public HoleBuilder(CircularFaceList cir)
        {
            this.List = cir;
            CylFeater = this.List.GetCylinderFeaters();
        }

        /// <summary>
        /// 判断是否是盲孔
        /// </summary>
        /// <returns></returns>
        public bool IsBlindHole()
        {
            Vector3d vec1 = this.List.CircleFaceList[0].Direction;
            Vector3d vec2 = new Vector3d(-vec1.X, -vec1.Y, -vec1.Z);
            int k1 = TraceARay.AskTraceARay(this.List.CircleFaceList[0].Data.Face.GetBody(), this.List.CircleFaceList[0].StartPt, vec1);
            int k2 = TraceARay.AskTraceARay(this.List.CircleFaceList[0].Data.Face.GetBody(), this.List.CircleFaceList[0].StartPt, vec1);
            return k1 != 0 || k2 != 0;
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
        }
        public void Highlight(bool highlight)
        {
            this.List.Highlight(highlight);
        }
    }
}
