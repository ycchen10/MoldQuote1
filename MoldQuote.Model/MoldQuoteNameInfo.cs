using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
namespace MoldQuote.Model
{
    public class MoldQuoteNameInfo: IDisplayObject
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        public string Length { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        public string Width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public string Height { get; set; }
        /// <summary>
        /// 材质
        /// </summary>
        public string Materials { get; set; }

        public Node Node { get; set; }

        public Body Body { get; set; }
     

        public void Highlight(bool highlight)
        {
            if (highlight)
                this.Body.Unblank();
            else
                this.Body.Blank();
        }
    }
}
