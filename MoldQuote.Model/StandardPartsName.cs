using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;


namespace MoldQuote.Model
{
    /// <summary>
    /// 标准件名字
    /// </summary>
    public class StandardPartsName : IDisplayObject
    {
        public string Name { get; set; }

        public string Length { get; set; }

        public string Dia { get; set; }

        public int Count { get; set; }
        public List<Body> Bodys { get; set; } = new List<Body>();

        public Node Node { get; set; }

        public void Highlight(bool highlight)
        {
            if (highlight)
                foreach (Body by in Bodys)
                {
                    by.Unblank();
                }
            else
                foreach (Body by in Bodys)
                {
                    by.Blank();
                }
        }
    }
}
