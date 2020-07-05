using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;

namespace MoldQuote.DAL
{
    /// <summary>
    /// 圆形收集
    /// </summary>
    public class CircularCollection
    {
        /// <summary>
        /// 获取孔特征
        /// </summary>
        /// <param name="circles"></param>
        /// <returns></returns>
        public static List<CircularFaceList> GetHoleList(List<AbstractCircleFace> circles)
        {
            bool isok = false;
            List<CircularFaceList> holeList = new List<CircularFaceList>();
            foreach (AbstractCircleFace af in circles)
            {
                if (af.IsHole)
                {
                    if (holeList.Count == 0)
                    {
                        CircularFaceList list = new CircularFaceList();
                        list.IsInThisHole(af);
                        holeList.Add(list);
                        continue;
                    }
                    foreach (CircularFaceList cl in holeList)
                    {
                        if (cl.IsInThisHole(af))
                        {
                            isok = true;
                            break;
                        }
                    }
                    if (!isok)
                    {
                        CircularFaceList list = new CircularFaceList();
                        list.IsInThisHole(af);
                        holeList.Add(list);
                    }
                    isok = false;
                }
            }
            return holeList;
        }
        /// <summary>
        /// 获取圆台特征
        /// </summary>
        /// <param name="circles"></param>
        /// <returns></returns>
        public static List<CircularFaceList> GetStepList(List<AbstractCircleFace> circles)
        {
            bool isok = false;
            List<CircularFaceList> stepList = new List<CircularFaceList>();
            foreach (AbstractCircleFace af in circles)
            {
                if (af.IsStep)
                {
                    if (stepList.Count == 0)
                    {
                        CircularFaceList list = new CircularFaceList();
                        list.IsInThisStep(af);
                        stepList.Add(list);
                        continue;
                    }
                    foreach (CircularFaceList cl in stepList)
                    {
                        if (cl.IsInThisStep(af))
                        {
                            isok = true;
                            break;
                        }
                    }
                    if (!isok)
                    {
                        CircularFaceList list = new CircularFaceList();
                        list.IsInThisStep(af);
                        stepList.Add(list);
                    }
                    isok = false;
                }
            }
            return stepList;
        }
    }
}
