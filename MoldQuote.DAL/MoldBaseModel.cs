using System;
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
    public class MoldBaseModel
    {
        private Matrix4 matr;
        public string Name { get; set; }

        public Body Body { get; private set; }

        public Point3d CenterPt { get; private set; }

        public Point3d DisPt { get; private set; }

        public Node node { get; set; }
        /// <summary>
        /// 材质
        /// </summary>
        public string Materials { get; set; }
        public MoldBaseModel(Body body, Matrix4 mat)
        {
            this.Body = body;
            this.matr = mat;
            Matrix4 inv = mat.GetInversMatrix();
            CartesianCoordinateSystem cs = BoundingBoxUtils.CreateCoordinateSystem(mat, inv);
            NXObject[] obj = { body };
            Point3d center = new Point3d();
            Point3d dis = new Point3d();
            BoundingBoxUtils.GetBoundingBoxInLocal(obj, cs, mat, ref center, ref dis);
            this.CenterPt = center;
            this.DisPt = dis;
            this.Materials = "王牌";
        }
        /// <summary>
        /// 获取模板信息
        /// </summary>
        /// <returns></returns>
        public MoldQuoteNameInfo GetMoldBaseInfo()
        {
            MoldQuoteNameInfo info = new MoldQuoteNameInfo()
            {
                Name = this.Name,
                Body = this.Body,
                Materials = this.Materials

            };
            if (this.DisPt.X >= this.DisPt.Y)
            {
                info.Length = Math.Round(this.DisPt.X * 2, 2).ToString();
                info.Width = Math.Round(this.DisPt.Y * 2, 2).ToString();
            }
            else
            {
                info.Length = Math.Round(this.DisPt.Y * 2, 2).ToString();
                info.Width = Math.Round(this.DisPt.X * 2, 2).ToString();
            }
            info.Height = Math.Round(this.DisPt.Z * 2, 2).ToString();
            return info;
        }
        /// <summary>
        /// 获取螺栓
        /// </summary>
        /// <returns></returns>
        public List<AbstractCylinderBody> GetBolt(List<AbstractCylinderBody> cylinder, MoldBaseModel other)
        {
            Vector3d vec = UMathUtils.GetVector(new Point3d(0, 0, this.CenterPt.Z), new Point3d(0, 0, other.CenterPt.Z));
            List<AbstractCylinderBody> temp = cylinder.FindAll(a => UMathUtils.IsEqual(UMathUtils.Angle(vec, a.Direction), 0));
            List<AbstractCylinderBody> bolt = new List<AbstractCylinderBody>();
            foreach (AbstractCylinderBody ab in temp)
            {
                Point3d start = ab.StratPt;
                Point3d end = ab.EndPt;
                this.matr.ApplyPos(ref start);
                this.matr.ApplyPos(ref end);
                if ((start.Z > this.CenterPt.Z - this.DisPt.Z && start.Z < this.CenterPt.Z + this.DisPt.Z)
                    && (end.Z > other.CenterPt.Z - other.DisPt.Z && end.Z <= other.CenterPt.Z + other.DisPt.Z))
                {
                    // ab.Name = "M" + Math.Ceiling(ab.Radius * 2).ToString();
                    ab.Name = "螺栓";
                    bolt.Add(ab);
                    continue;
                }

            }
            return bolt;
        }
        /// <summary>
        /// 获取导套
        /// </summary>
        /// <param name="cylinder"></param>
        /// <returns></returns>
        public List<AbstractCylinderBody> GetGuideBushing(List<AbstractCylinderBody> cylinder)
        {
            List<AbstractCylinderBody> bush = new List<AbstractCylinderBody>();
            foreach (AbstractCylinderBody ab in cylinder)
            {
                if (ab.Radius >= 8)
                {
                    Vector3d vec1 = ab.Direction;
                    Vector3d vec2 = new Vector3d(-vec1.X, -vec1.Y, -vec1.Z);
                    int count1 = TraceARay.AskTraceARay(this.Body, ab.StratPt, vec1);
                    int count2 = TraceARay.AskTraceARay(this.Body, ab.StratPt, vec2);
                    if (count1 == 0 && count2 == 0)
                    {
                        List<Body> bodys = new List<Body>();
                        NXOpen.GeometricAnalysis.SimpleInterference.Result res = AnalysisUtils.SetInterferenceOutResult(this.Body, ab.Body, out bodys);
                        if (res == NXOpen.GeometricAnalysis.SimpleInterference.Result.OnlyEdgesOrFacesInterfere && bodys.Count == 0)
                        {
                            if (ab is CylinderBody)
                            {
                                ab.Name = "直导套";
                            }
                            else
                            {
                                ab.Name = "有托导套";
                            }
                            bush.Add(ab);
                            continue;
                        }
                    }
                }
            }
            return bush;
        }

    }
}
