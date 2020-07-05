using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.UF;
using Basic;
using NXOpen.Utilities;

namespace MoldQuote.DAL
{
    /// <summary>
    /// 圆环面
    /// </summary>
    public class CircleAnnylusFace : AbstractCircleFace
    {
        private List<ArcEdgeData> edgeData = new List<ArcEdgeData>();
        /// <summary>
        /// 最小半径
        /// </summary>
        public double MinRadius
        {
            get
            {
                return this.Data.RadData;
            }
        }
        /// <summary>
        /// 最大半径
        /// </summary>
        public double MaxRadius { get { return this.Data.Radius; } }

        public CircleAnnylusFace(FaceData data, List<ArcEdgeData> edge) : base(data)
        {
            this.edgeData = edge;
            GetFacePoint();
            this.IsHole = true;
            this.IsStep = true;
        }
        private void GetFacePoint()
        {
            this.CenterPt = edgeData[0].Center;
            this.StartPt = this.CenterPt;
            this.EndPt = this.CenterPt;
        }
        /// <summary>
        /// 判断是否是圆环面
        /// </summary>
        /// <param name="face"></param>
        /// <param name="arcEdge"></param>
        /// <returns></returns>
        public static bool IsCircleAnnylus(Face face, out List<ArcEdgeData> arcEdge)
        {
            arcEdge = new List<ArcEdgeData>();
            FaceLoopUtils.LoopList[] loops = FaceLoopUtils.AskFaceLoops(face.Tag);
            List<ArcEdgeData> arc = new List<ArcEdgeData>();
            string err = "";
            foreach (FaceLoopUtils.LoopList lt in loops)
            {
                if (lt.Type == 1)
                {
                    foreach (Tag egTag in lt.EdgeList)
                    {
                        Edge ed = NXObjectManager.Get(egTag) as Edge;
                        if (ed.SolidEdgeType == Edge.EdgeType.Circular)
                        {
                            ArcEdgeData arcData = EdgeUtils.GetArcData(ed, ref err);
                            if (arcData.Angle >= Math.PI)
                                arc.Add(arcData);
                        }
                    }

                }
            }
            if (arc.Count == 0)
            {
                return false;
            }
            else if (arc.Count == 1)
            {
                arcEdge.Add(arc[0]);
                return true;
            }
            else
            {

                var temp = arc.GroupBy(a => a.Center);
                int cout = 0;
                foreach (var tp in temp)
                {
                    int tpCont = tp.Count();
                    if (cout < tpCont)
                        cout = tpCont;
                }
                if (cout == 1)
                {
                    double anlge = arc.Max(a => a.Angle);
                    arcEdge.Add(arc.Find(a => a.Angle == anlge));
                    return true;
                }
                else
                {
                    foreach (var tp in temp)
                    {
                        int tpCont = tp.Count();
                        if (cout == tpCont)
                        {
                            arcEdge.AddRange(tp);
                            return true;
                        }
                    }
                }

            }

            return false;
        }

        public override string ToString()
        {
            return this.MinRadius.ToString("f3") + "+" + this.MaxRadius.ToString("f3");
        }
    }
}
