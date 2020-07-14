using System;
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
                bolt.AddRange(this.AMoldBase.GetBolt(cyls, this.UpBaseplate));
            }
            if (this.Baseplate != null) //底板到B板
            {
                bolt.AddRange(this.BMoldBase.GetBolt(cyls, this.Baseplate));
            }
            if (upSp.Count > 0 && this.UpBaseplate != null) //上底板到上方铁
            {
                bolt.AddRange(upSp[0].GetBolt(cyls, this.UpBaseplate));
            }
            if (dowSp.Count > 0 && this.Baseplate != null)//底板到下方铁
            {
                bolt.AddRange(dowSp[0].GetBolt(cyls, this.Baseplate));
            }
            if (up.Count > 0 && upFace.Count > 0)//上顶针板
            {
                bolt.AddRange(upFace[0].GetBolt(cyls, up[0]));
            }
            if (dow.Count > 0 && dowFace.Count > 0)//下顶针板
            {
                bolt.AddRange(dowFace[0].GetBolt(cyls, dow[0]));
            }
            return this.GetCyliderName(bolt);
        }

        public override List<string> GetGuideBushing()
        {
            List<AbstractCylinderBody> pin = new List<AbstractCylinderBody>();
            List<AbstractCylinderBody> pins = this.cylinderBody.FindAll(a => a.IsGuidePin());
            List<MoldBaseModel> dowFace = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z < 0);
            List<MoldBaseModel> upFace = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z > 0);
            List<MoldBaseModel> dow = this.DowEiectorPlates.FindAll(a => a.CenterPt.Z < 0);
            List<MoldBaseModel> up = this.FaceEiectorPlates.FindAll(a => a.CenterPt.Z > 0);
            foreach (AbstractCylinderBody ab in pins)
            {
                if (ab.Radius >= 8)
                {
                    ab.Name = "D" + Math.Ceiling(ab.Radius * 2).ToString();
                    Point3d centerPt = UMathUtils.GetMiddle(ab.StratPt, ab.EndPt);
                    Vector3d vec1 = ab.Direction;
                    Vector3d vec2 = new Vector3d(-vec1.X, -vec1.Y, -vec1.Z);
                    int count3 = TraceARay.AskTraceARay(this.AMoldBase.Body, ab.StratPt, vec1);
                    int count4 = TraceARay.AskTraceARay(this.AMoldBase.Body, ab.StratPt, vec2);
                    int count5 = TraceARay.AskTraceARay(this.BMoldBase.Body, ab.StratPt, vec1);
                    int count6 = TraceARay.AskTraceARay(this.BMoldBase.Body, ab.StratPt, vec2);
                    if (count3 == 0 && count4 == 0 && count5 == 0 && count6 == 0)
                    {
                        List<Body> bodys = new List<Body>();
                        NXOpen.GeometricAnalysis.SimpleInterference.Result res = AnalysisUtils.SetInterferenceOutResult(this.AMoldBase.Body, ab.Body, out bodys); //A板
                        if (res == NXOpen.GeometricAnalysis.SimpleInterference.Result.OnlyEdgesOrFacesInterfere && bodys.Count == 0)
                        {
                            pin.Add(ab);
                            continue;
                        }
                        res = AnalysisUtils.SetInterferenceOutResult(this.BMoldBase.Body, ab.Body, out bodys); //B板
                        if (res == NXOpen.GeometricAnalysis.SimpleInterference.Result.OnlyEdgesOrFacesInterfere && bodys.Count == 0)
                        {
                            pin.Add(ab);
                            continue;
                        }
                    }
                    if (this.ShuiSupportPlate != null && centerPt.Z > this.ShuiSupportPlate.CenterPt.Z - this.ShuiSupportPlate.DisPt.Z &&
                   centerPt.Z < this.ShuiSupportPlate.CenterPt.Z - this.ShuiSupportPlate.DisPt.Z) //推板上的导套
                    {
                        int count1 = TraceARay.AskTraceARay(this.ShuiSupportPlate.Body, ab.StratPt, vec1);
                        int count2 = TraceARay.AskTraceARay(this.ShuiSupportPlate.Body, ab.StratPt, vec2);
                        if (count1 == 0 && count2 == 0 && count3 == 0 && count4 == 0 && count5 == 0 && count6 == 0)
                        {
                            List<Body> bodys = new List<Body>();
                            NXOpen.GeometricAnalysis.SimpleInterference.Result res = AnalysisUtils.SetInterferenceOutResult(this.ShuiSupportPlate.Body, ab.Body, out bodys);
                            if (res == NXOpen.GeometricAnalysis.SimpleInterference.Result.OnlyEdgesOrFacesInterfere && bodys.Count == 0)
                            {
                                pin.Add(ab);
                                continue;
                            }

                        }
                    }

                    if (up.Count > 0 && upFace.Count > 0)
                    {
                        if (centerPt.Z > upFace[0].CenterPt.Z - upFace[0].DisPt.Z && centerPt.Z < up[0].DisPt.Z + up[0].CenterPt.Z)
                        {
                            foreach (MoldBaseModel mm in up)
                            {
                                int count1 = TraceARay.AskTraceARay(this.ShuiSupportPlate.Body, ab.StratPt, vec1);
                                int count2 = TraceARay.AskTraceARay(this.ShuiSupportPlate.Body, ab.StratPt, vec2);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public override List<string> GetGuidePillar()
        {
            throw new NotImplementedException();
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



                List<MoldBaseModel> spa = this.analysis.GetSpacer(up);
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
            foreach (MoldBaseModel mm in up)
            {
                if (mm.Name == null || mm.Name.Equals(""))
                {
                    this.OtherBaseModel.Add(mm);
                }
            }

        }

    }
}
