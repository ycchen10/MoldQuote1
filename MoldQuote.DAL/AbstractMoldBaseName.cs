﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;
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
        public MoldBaseModel UpBaseplate { get; protected set; } = null;
        /// <summary>
        /// 底板
        /// </summary>
        public MoldBaseModel Baseplate { get; protected set; } = null;
        /// <summary>
        /// 托板
        /// </summary>
        public MoldBaseModel SupportPlate { get; protected set; } = null;
        /// <summary>
        /// 推板
        /// </summary>
        public MoldBaseModel PushPlate { get; protected set; } = null;
        /// <summary>
        /// 方铁
        /// </summary>
        public List<MoldBaseModel> Spacer { get; protected set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 面顶针板
        /// </summary>
        public List<MoldBaseModel> FaceEiectorPlates { get; protected set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 底针板
        /// </summary>
        public List<MoldBaseModel> DowEiectorPlates { get; protected set; } = new List<MoldBaseModel>();
        /// <summary>
        /// 其他无名板
        /// </summary>
        public List<MoldBaseModel> OtherBaseModel { get; protected set; } = new List<MoldBaseModel>();
        public AbstractMoldBaseName(AnalysisMold analysis)
        {
            this.analysis = analysis;
            analysis.GetBase(out moldbase, out cylinderBody);
            GetDowBaseModel();
        }

        private void GetDowBaseModel()
        {
            List<MoldBaseModel> down = analysis.GetDownModel(this.moldbase);
            if (UMathUtils.IsEqual(down[0].CenterPt.Z + down[0].DisPt.Z, this.BMoldBase.CenterPt.Z - this.BMoldBase.DisPt.Z))
            {
                this.SupportPlate = down[0];
                this.SupportPlate.Name = "托板";
            }
            this.Baseplate = down[down.Count - 1];
            this.Baseplate.Name = "底板";
            MoldBaseModel kon = this.analysis.GetKnockoutPlate(this.moldbase);
            if (kon != null)
            {
                this.PushPlate = kon;
                this.PushPlate.Name = "推板";
            }
            List<MoldBaseModel> spa = this.analysis.GetSpacer(down);
            if (spa.Count > 0)
            {
                this.Spacer.AddRange(spa);
                List<MoldBaseModel> eiec = this.analysis.GetEiectorPlates(this.moldbase, spa);
                double max = eiec.Max(a => a.CenterPt.Z);
                foreach (MoldBaseModel mm in eiec)
                {
                    if (UMathUtils.IsEqual(mm.CenterPt.Z, max))
                    {
                        mm.Name = "面针板";
                        FaceEiectorPlates.Add(mm);
                    }
                    else
                    {
                        mm.Name = "底针板";
                        DowEiectorPlates.Add(mm);
                    }
                }
            }
            foreach (MoldBaseModel mm in down)
            {
                if (mm.Name == null || mm.Name.Equals(""))
                {
                    this.OtherBaseModel.Add(mm);
                }
            }
        }

      
        /// <summary>
        /// 获取模架
        /// </summary>
        /// <returns></returns>
        protected abstract void GetUpBaseModel();
        /// <summary>
        /// 获取模板信息
        /// </summary>
        /// <returns></returns>
        public abstract List<MoldQuoteNameInfo> GetBaseInfo();
        /// <summary>
        /// 获取导套信息
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetGuideBushing();
        /// <summary>
        /// 获取导柱信息
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetGuidePillar();
        /// <summary>
        /// 获取螺栓信息
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetBolt();

    }
}
