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
        protected BasesModel basesModel;

        public AbstractMoldBaseName(AnalysisMold analysis)
        {
            this.analysis = analysis;
            this.basesModel = new BasesModel();
            this.basesModel.AMoldBase = this.analysis.AMoldBase;
            this.basesModel.AMoldBase.Name = "A板";
            this.basesModel.BMoldBase = this.analysis.BMoldBase;
            this.basesModel.BMoldBase.Name = "B板";
        }
        public abstract BasesModel GetBaseModel();

        public abstract AbstractCylinderBody GetCylinder();
    }
}
