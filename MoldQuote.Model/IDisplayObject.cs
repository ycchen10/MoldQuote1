using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;

namespace MoldQuote.Model
{
    public interface IDisplayObject
    {
        /// <summary>
        /// 显示
        /// </summary>
        /// <param name="highlight"></param>
        void Highlight(bool highlight);

    }
}
