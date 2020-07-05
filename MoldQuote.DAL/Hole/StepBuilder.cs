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
            CylFeater.Sort();
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
