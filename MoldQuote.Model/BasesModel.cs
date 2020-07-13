using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;

namespace MoldQuote.Model
{
    /// <summary>
    /// 模架
    /// </summary>
    public class BasesModel
    {
        /// <summary>
        /// A板
        /// </summary>
        public MoldBaseModel1 AMoldBase { get; set; }
        /// <summary>
        /// B板
        /// </summary>
        public MoldBaseModel1 BMoldBase { get; set; }
        /// <summary>
        /// 上模板
        /// </summary>
        public List<MoldBaseModel1> UpModel { get; set; } = new List<MoldBaseModel1>();
        /// <summary>
        /// 下模板
        /// </summary>
        public List<MoldBaseModel1> DownModel { get; set; } = new List<MoldBaseModel1>();
        /// <summary>
        /// 垫脚
        /// </summary>
        public List<MoldBaseModel1> Spacer { get; set; } = new List<MoldBaseModel1>();
        /// <summary>
        /// 顶针板
        /// </summary>
        public List<MoldBaseModel1> EiectorPlates { get; set; } = new List<MoldBaseModel1>();

      
    }
}
