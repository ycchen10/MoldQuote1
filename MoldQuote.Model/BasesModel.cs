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
        public MoldBaseModel AMoldBase { get; set; }
        /// <summary>
        /// B板
        /// </summary>
        public MoldBaseModel BMoldBase { get; set; }
        /// <summary>
        /// 上模板
        /// </summary>
        public List<MoldBaseModel> UpModel { get; set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 下模板
        /// </summary>
        public List<MoldBaseModel> DownModel { get; set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 垫脚
        /// </summary>
        public List<MoldBaseModel> Spacer { get; set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 顶针板
        /// </summary>
        public List<MoldBaseModel> EiectorPlates { get; set; } = new List<MoldBaseModel>();
    }
}
