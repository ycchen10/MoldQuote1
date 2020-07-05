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
    /// 圆柱形
    /// </summary>
    public class CylinderBuilder
    {
        /// <summary>
        /// 获取圆柱特征
        /// </summary>
        /// <param name="circle"></param>
        /// <param name="cyl"></param>
        /// <returns></returns>
        public static CylinderFeater GetCylinderFeater(List<AbstractCircleFace> circle, CylinderFace cyl)
        {

            List<AbstractCircleFace> cylinder = new List<AbstractCircleFace>();
            cylinder.Add(cyl);
            int index = circle.IndexOf(cyl);
            if (index != -1)
            {
                for (int i = index - 1; i >= 0; i--) 
                {
                    if (!(circle[i] is CylinderFace) )
                    {
                        cylinder.Add(circle[i]);
                    }
                    if ((circle[i] is CylinderFace) && (UMathUtils.IsEqual((circle[i] as CylinderFace).Radius, cyl.Radius)))
                    {
                        cylinder.Add(circle[i]);
                    }
                    if(circle[i] is CircleAnnylusFace)
                    {
                        break;
                    }
                }
                for (int i = index + 1; i < circle.Count; i++)
                {
                    if (!(circle[i] is CylinderFace) )
                    {
                        cylinder.Add(circle[i]);

                    }
                    if ((circle[i] is CylinderFace) && (UMathUtils.IsEqual((circle[i] as CylinderFace).Radius, cyl.Radius)))
                    {
                        cylinder.Add(circle[i]);
                    }
                    if (circle[i] is CircleAnnylusFace)
                    {
                        break;
                    }
                }
            }
            return new CylinderFeater(cylinder, cyl);
        }

    }
}
