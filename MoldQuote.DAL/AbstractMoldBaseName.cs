using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using MoldQuote.Model;

namespace MoldQuote.DAL
{
    /// <summary>
    /// 模板名称
    /// </summary>
    public abstract class AbstractMoldBaseName
    {
        protected AnalysisMold analysis;
        protected List<MoldBaseModel> moldbase = new List<MoldBaseModel>();
        protected List<AbstractCylinderBody> cylinderBody = new List<AbstractCylinderBody>();
        /// <summary>
        /// A板
        /// </summary>
        public MoldBaseModel AMoldBase
        {
            get { return analysis.AMoldBase; }
        }
        /// <summary>
        /// B板
        /// </summary>
        public MoldBaseModel BMoldBase
        {
            get { return analysis.BMoldBase; }
        }
        /// <summary>
        /// 上底板
        /// </summary>
        public MoldBaseModel UpBaseplate { get; protected set; }
        /// <summary>
        /// 底板
        /// </summary>
        public MoldBaseModel Baseplate { get; protected set; }
        /// <summary>
        /// 托板
        /// </summary>
        public MoldBaseModel SupportPlate { get; protected set; }
        /// <summary>
        /// 推板
        /// </summary>
        public MoldBaseModel PushPlate { get; protected set; }
        /// <summary>
        /// 方铁
        /// </summary>
        public List<MoldBaseModel> Spacer { get; protected set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 顶针板
        /// </summary>
        public List<MoldBaseModel> EiectorPlates { get; protected set; } = new List<MoldBaseModel>();

      
        public AbstractMoldBaseName(AnalysisMold analysis)
        {
            this.analysis = analysis;
            analysis.GetBase(out moldbase, out cylinderBody);
            GetBaseModel();
        }
        /// <summary>
        /// 获取模架
        /// </summary>
        /// <returns></returns>
        protected abstract void GetBaseModel();

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public abstract AbstractCylinderBody GetCylinder();


    }
}
