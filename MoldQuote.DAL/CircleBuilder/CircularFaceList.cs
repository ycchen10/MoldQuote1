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
    /// 同一中轴线圆柱形
    /// </summary>
    public class CircularFaceList : IDisplayObject
    {
        public List<AbstractCircleFace> CircleFaceList { get; private set; } = new List<AbstractCircleFace>();
        /// <summary>
        /// 判断是否是同一个孔并添加
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        public bool IsInThisHole(AbstractCircleFace cf)
        {
            if (this.CircleFaceList.Count == 0 || this.CircleFaceList == null)
            {
                this.CircleFaceList.Add(cf);
                return true;
            }
            else
            {
                if (this.CircleFaceList[0].IsTheSameHole(cf))
                {
                    this.CircleFaceList.Add(cf);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断是否是一个凸台并添加
        /// </summary>
        /// <param name="cf"></param>
        /// <returns></returns>
        public bool IsInThisStep(AbstractCircleFace cf)
        {
            if (this.CircleFaceList.Count == 0 || this.CircleFaceList == null)
            {
                this.CircleFaceList.Add(cf);
                return true;
            }
            else
            {
                if (this.CircleFaceList[0].IsCircleStep(cf))
                {
                    this.CircleFaceList.Add(cf);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取圆柱特征
        /// </summary>
        /// <returns></returns>
        public List<CylinderFeater> GetCylinderFeaters()
        {
            List<AbstractCircleFace> cyls = CircleFaceList.FindAll(a => a is CylinderFace).ToList();
            List<CylinderFeater> featers = new List<CylinderFeater>();
            if (cyls != null && cyls.Count > 0)
            {
                Sort(cyls[0].Direction);

                foreach (CylinderFace cy in cyls)
                {
                    featers.Add(CylinderBuilder.GetCylinderFeater(this.CircleFaceList, cy));
                }
            }

            return featers;
        }
        /// <summary>
        /// 排序
        /// </summary>
        public void Sort(Vector3d vec)
        {
            Matrix4 mat = new Matrix4();
            mat.Identity();
            mat.TransformToZAxis(new Point3d(0, 0, 0), vec);
            this.CircleFaceList.Sort(delegate (AbstractCircleFace a, AbstractCircleFace b)
            {
                Point3d pt1 = a.CenterPt;
                Point3d pt2 = b.CenterPt;
                mat.ApplyPos(ref pt1);
                mat.ApplyPos(ref pt2);
                return pt1.Z.CompareTo(pt2.Z);
            });
        }
        public void Highlight(bool highlight)
        {
            foreach (AbstractCircleFace af in this.CircleFaceList)
            {
                af.Highlight(highlight);
            }
        }
    }
}
