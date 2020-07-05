using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using Basic;

namespace MoldQuote.DAL
{
    /// <summary>
    /// 创建圆形面工厂
    /// </summary>
    public class CircleFaceFactory
    {
        public static AbstractCircleFace Create(Face face)
        {
            FaceData data = FaceUtils.AskFaceData(face);
            AbstractCircleFace abs = null;
            switch (face.SolidFaceType)
            {
                case Face.FaceType.Cylindrical:
                    abs = new CylinderFace(data);
                    break;
                case Face.FaceType.Conical:
                    abs = new CircularConeFace(data);
                    break;
                case Face.FaceType.Planar:
                    List<ArcEdgeData> edge = new List<ArcEdgeData>();
                    if (CircleAnnylusFace.IsCircleAnnylus(face, out edge))
                        abs = new CircleAnnylusFace(data, edge);
                    break;
                default:
                    break;
            }
            return abs;
        }
    }
}
