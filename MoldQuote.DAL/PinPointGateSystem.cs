﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;
using MoldQuote.Model;

namespace MoldQuote.DAL
{
    /// <summary>
    /// 细水口模架
    /// </summary>
    public class PinPointGateSystem : AbstractMoldBaseName
    {
        /// <summary>
        /// 水口推板
        /// </summary>
        public MoldBaseModel ShuiSupportPlate { get; private set; }
        public PinPointGateSystem(AnalysisMold mold) : base(mold)
        {
            GetUpBaseModel();
        }

        public override List<MoldQuoteNameInfo> GetBaseInfo()
        {
            List<MoldQuoteNameInfo> infos = new List<MoldQuoteNameInfo>();
            if (this.UpBaseplate != null)
                infos.Add(this.UpBaseplate.GetMoldBaseInfo());
            if (this.ShuiSupportPlate != null)
                infos.Add(this.ShuiSupportPlate.GetMoldBaseInfo());
            if (this.AMoldBase != null)
                infos.Add(this.AMoldBase.GetMoldBaseInfo());
            if (this.PushPlate != null)
                infos.Add(this.PushPlate.GetMoldBaseInfo());
            if (this.BMoldBase != null)
                infos.Add(this.BMoldBase.GetMoldBaseInfo());
            if (this.SupportPlate != null)
                infos.Add(this.SupportPlate.GetMoldBaseInfo());
            if (this.Baseplate != null)
                infos.Add(this.Baseplate.GetMoldBaseInfo());
            foreach (MoldBaseModel mm in this.FaceEiectorPlates)
            {
                infos.Add(mm.GetMoldBaseInfo());
            }
            foreach (MoldBaseModel mm in this.DowEiectorPlates)
            {
                infos.Add(mm.GetMoldBaseInfo());
            }
            foreach (MoldBaseModel mm in this.Spacer)
            {
                infos.Add(mm.GetMoldBaseInfo());
            }
            foreach (MoldBaseModel mm in this.OtherBaseModel)
            {
                infos.Add(mm.GetMoldBaseInfo());
            }
            return infos;
        }
        /*
                public override List<StandardPartsName> GetBolt()
                {
                    List<AbstractCylinderBody> bolt = new List<AbstractCylinderBody>();
                    List<AbstractCylinderBody> cyls = this.cylinderBody.FindAll(a => a is CylinderTwoStepBody && (a as CylinderTwoStepBody).IsBolt());
                    List<MoldBaseModel> dowSp = this.Spacer.FindAll(a => a.CenterPt.Z < 0);
                    List<MoldBaseModel> upSp = this.Spacer.FindAll(a => a.CenterPt.Z > 0);
                    List<MoldBaseModel> dowFace = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z < 0);
                    List<MoldBaseModel> upFace = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z > 0);
                    List<MoldBaseModel> dow = this.DowEiectorPlates.FindAll(a => a.CenterPt.Z < 0);
                    List<MoldBaseModel> up = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z > 0);
                    foreach (AbstractCylinderBody ab in cyls)
                    {
                        Point3d start = ab.StratPt;
                        Point3d end = ab.EndPt;
                        this.analysis.Matr.ApplyPos(ref start);
                        this.analysis.Matr.ApplyPos(ref end);
                        CylinderTwoStepBody cy = ab as CylinderTwoStepBody;
                        cy.Name = "M" + Math.Ceiling(cy.Radius * 2).ToString();
                        if (IsPassThrough(ab, this.ShuiSupportPlate, this.AMoldBase))//推板到A板
                        {
                            bolt.Add(cy);
                        }
                        if (IsPassThrough(ab, this.Baseplate, this.BMoldBase)) //底板到B板
                        {
                            bolt.Add(cy);
                        }
                        if (upSp.Count > 0 && IsPassThrough(ab, this.ShuiSupportPlate, upSp[0])) //推板到上方铁
                        {
                            bolt.Add(cy);
                        }
                        if (dowSp.Count > 0 && IsPassThrough(ab, this.Baseplate, dowSp[0]))//底板到下方铁
                        {
                            bolt.Add(cy);
                        }
                        if (up.Count > 0 && upFace.Count > 0 && IsPassThrough(ab, up[0], upFace[0]))//上顶针板
                        {
                            bolt.Add(cy);
                        }
                        if (dow.Count > 0 && dowFace.Count > 0 && IsPassThrough(ab, dow[0], dowFace[0]))//下顶针板
                        {
                            bolt.Add(cy);
                        }
                    }
                    return this.GetCyliderName(bolt);



                }
        */
        public override List<StandardPartsName> GetBolt()
        {
            List<AbstractCylinderBody> bolt = new List<AbstractCylinderBody>();
            List<AbstractCylinderBody> cyls = this.cylinderBody.FindAll(a => a is CylinderTwoStepBody && (a as CylinderTwoStepBody).IsBolt());
            List<MoldBaseModel> dowSp = this.Spacer.FindAll(a => a.CenterPt.Z < 0);
            List<MoldBaseModel> upSp = this.Spacer.FindAll(a => a.CenterPt.Z > 0);
            List<MoldBaseModel> dowFace = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z < 0);
            List<MoldBaseModel> upFace = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z > 0);
            List<MoldBaseModel> dow = this.DowEiectorPlates.FindAll(a => a.CenterPt.Z < 0);
            List<MoldBaseModel> up = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z > 0);

            if (this.UpBaseplate != null)//上底板到A板
            {
                bolt.AddRange(this.UpBaseplate.GetBolt(cyls, this.AMoldBase));
            }
            if (this.Baseplate != null) //底板到B板
            {
                bolt.AddRange(this.Baseplate.GetBolt(cyls, this.BMoldBase));
            }
            if (upSp.Count > 0 && this.UpBaseplate != null) //上底板到上方铁
            {
                bolt.AddRange(this.UpBaseplate.GetBolt(cyls, upSp[0]));
            }
            if (dowSp.Count > 0 && this.Baseplate != null)//底板到下方铁
            {
                bolt.AddRange(this.Baseplate.GetBolt(cyls, dowSp[0]));
            }
            if (this.SupportPlate != null && this.Baseplate != null)//底板到托板
            {
                bolt.AddRange(this.Baseplate.GetBolt(cyls, this.SupportPlate));
            }
            if (up.Count > 0 && upFace.Count > 0)//上顶针板
            {
                bolt.AddRange(up[0].GetBolt(cyls, upFace[0]));
            }
            if (dow.Count > 0 && dowFace.Count > 0)//下顶针板
            {
                bolt.AddRange(dow[0].GetBolt(cyls, dowFace[0]));
            }
            return this.GetCyliderName(bolt);
        }

        public override List<StandardPartsName> GetGuideBushing()
        {
            List<AbstractCylinderBody> pins = this.cylinderBody.FindAll(a => a.IsGuidePin());
            List<MoldBaseModel> dowFace = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z < 0);
            List<MoldBaseModel> upFace = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z > 0);
            if (this.ShuiSupportPlate != null) // 水口托板
            {
                foreach (AbstractCylinderBody ab in this.ShuiSupportPlate.GetGuideBushing(pins))
                {
                    Vector3d vec1 = this.analysis.Matr.GetZAxis();
                    int count1 = TraceARay.AskTraceARay(this.AMoldBase.Body, ab.StratPt, new Vector3d(vec1.X, vec1.Y, -vec1.Z));
                    int count2 = TraceARay.AskTraceARay(this.BMoldBase.Body, ab.StratPt, new Vector3d(vec1.X, vec1.Y, -vec1.Z));
                    if (count1 == 0 && count2 == 0)
                    {
                        pin.Add(ab);
                    }
                }
            }
            foreach (AbstractCylinderBody ab in this.AMoldBase.GetGuideBushing(pins)) //A板
            {
                Vector3d vec1 = this.analysis.Matr.GetZAxis();
                int count1 = TraceARay.AskTraceARay(this.BMoldBase.Body, ab.StratPt, new Vector3d(vec1.X, vec1.Y, -vec1.Z));
                if (count1 == 0)
                {
                    pin.Add(ab);
                }
            }
            if (this.PushPlate != null) //推板
            {
                foreach (AbstractCylinderBody ab in this.ShuiSupportPlate.GetGuideBushing(pins))
                {
                    Vector3d vec1 = this.analysis.Matr.GetZAxis();
                    int count1 = TraceARay.AskTraceARay(this.AMoldBase.Body, ab.StratPt, vec1);
                    int count2 = TraceARay.AskTraceARay(this.BMoldBase.Body, ab.StratPt, new Vector3d(vec1.X, vec1.Y, -vec1.Z));
                    if (count1 == 0 && count2 == 0)
                    {
                        pin.Add(ab);
                    }
                }
            }
            foreach (AbstractCylinderBody ab in this.BMoldBase.GetGuideBushing(pins)) //B板
            {
                Vector3d vec1 = this.analysis.Matr.GetZAxis();
                int count1 = TraceARay.AskTraceARay(this.AMoldBase.Body, ab.StratPt, vec1);
                if (count1 == 0)
                {
                    pin.Add(ab);
                }
            }
            foreach (MoldBaseModel mb in dowFace)
            {
                pin.AddRange(mb.GetGuideBushing(pins));
            }
            foreach (MoldBaseModel mb in upFace)
            {
                pin.AddRange(mb.GetGuideBushing(pins));
            }
            return GetCyliderName(pin);
        }


        public override List<StandardPartsName> GetGuidePillar()
        {
            List<MoldBaseModel> dowFace = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z < 0);
            List<MoldBaseModel> dow = this.DowEiectorPlates.FindAll(a => a.CenterPt.Z < 0);
            List<AbstractCylinderBody> pillars = new List<AbstractCylinderBody>();
            List<AbstractCylinderBody> pillar = this.cylinderBody.FindAll(a => a.Radius >= 8 && a is CylinderTwoStepBody && (a as CylinderTwoStepBody).IsGuidePillar());
            foreach (AbstractCylinderBody ab in pin)
            {
                AbstractCylinderBody pi = pillar.Find(a => UMathUtils.IsEqual(a.StratPt.X, ab.StratPt.X) && UMathUtils.IsEqual(a.StratPt.Y, ab.StratPt.Y));
                //if (!pillars.Exists(a => a.Equals(pi)))
                //{
                //    pillars.Add(pi);
                //}
                if (pi != null)
                {
                    pi.Name = "导柱";
                    pillars.Add(pi);
                    pillar.Remove(pi);
                }
            }
            if (this.SupportPlate != null && dowFace.Count != 0 && dow.Count != 0)
            {
                foreach (AbstractCylinderBody ab in pillar)
                {
                    Point3d start = ab.StratPt;
                    Point3d end = ab.EndPt;
                    this.analysis.Matr.ApplyPos(ref start);
                    this.analysis.Matr.ApplyPos(ref end);
                    Vector3d vec = UMathUtils.GetVector(this.DowEiectorPlates[0].CenterPt, this.SupportPlate.CenterPt);
                    if (UMathUtils.IsEqual(UMathUtils.Angle(vec, ab.Direction), 0) &&
                       UMathUtils.IsEqual(ab.StratPt.Z, dow[0].CenterPt.Z + dow[0].DisPt.Z) &&
                       ab.EndPt.Z > this.SupportPlate.CenterPt.Z - this.SupportPlate.DisPt.Z)
                    {
                        ab.Name = "回针";
                        pillars.Add(ab);
                    }

                }
            }
            else if (this.SupportPlate == null && dowFace.Count != 0 && dow.Count != 0)
            {
                foreach (AbstractCylinderBody ab in pillar)
                {
                    Point3d start = ab.StratPt;
                    Point3d end = ab.EndPt;
                    this.analysis.Matr.ApplyPos(ref start);
                    this.analysis.Matr.ApplyPos(ref end);
                    Vector3d vec = UMathUtils.GetVector(this.DowEiectorPlates[0].CenterPt, this.SupportPlate.CenterPt);
                    if (UMathUtils.IsEqual(UMathUtils.Angle(vec, ab.Direction), 0) &&
                       UMathUtils.IsEqual(ab.StratPt.Z, dow[0].CenterPt.Z + dow[0].DisPt.Z) &&
                       ab.EndPt.Z > this.BMoldBase.CenterPt.Z - this.BMoldBase.DisPt.Z)
                    {
                        ab.Name = "回针";
                        pillars.Add(ab);
                    }

                }
            }
            return GetCyliderName(pillars);
        }
        /// <summary>
        /// 获取上模板
        /// </summary>
        protected override void GetUpBaseModel()
        {
            List<MoldBaseModel> up = analysis.GetUpModel(this.moldbase);
            if (up.Count > 0)
            {
                if (up[up.Count - 1].DisPt.Z <= 5 && up.Count != 1)
                {
                    up.RemoveAt(up.Count - 1);
                }
                if (up.Count == 1)
                {
                    this.UpBaseplate = up[0];
                    this.UpBaseplate.Name = "水口板";
                }
                else
                {
                    if (UMathUtils.IsEqual(up[0].CenterPt.Z - up[0].DisPt.Z, this.AMoldBase.CenterPt.Z + this.AMoldBase.DisPt.Z))
                    {

                        this.ShuiSupportPlate = up[0];
                        this.ShuiSupportPlate.Name = "水口推板";
                    }
                    this.UpBaseplate = up[up.Count - 1];
                    this.UpBaseplate.Name = "水口板";
                }
                List<MoldBaseModel> spa = this.analysis.GetSpacer(up); //方铁
                if (spa.Count > 0)
                {
                    this.Spacer.AddRange(spa);
                    List<MoldBaseModel> eiec = this.analysis.GetEiectorPlates(this.moldbase, spa);
                    if (eiec.Count > 0)
                    {
                        double max = eiec.Max(a => a.CenterPt.Z);
                        foreach (MoldBaseModel mm in eiec)
                        {
                            if (UMathUtils.IsEqual(mm.CenterPt.Z, max))
                            {
                                mm.Name = "底针板";
                                DowEiectorPlates.Add(mm);
                            }
                            else
                            {
                                mm.Name = "面针板";
                                this.FaceEiectorPlates.Add(mm);
                            }
                        }
                    }
                }
            }
            foreach (MoldBaseModel mm in up) //无名板
            {
                if (mm.Name == null || mm.Name.Equals(""))
                {
                    this.OtherBaseModel.Add(mm);
                }
            }

        }

    }
}
