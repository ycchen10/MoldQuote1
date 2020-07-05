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
    /// 圆柱面
    /// </summary>
    public class CylinderFace : AbstractCircleFace
    {
        /// <summary>
        /// 半径
        /// </summary>
        public double Radius { get { return this.Data.Radius; } }
        /// <summary>
        /// 长度
        /// </summary>
        public double Length { get; private set; }
        public CylinderFace(FaceData data) : base(data)
        {
            GetFacePoint();
            this.IsHole = data.IntNorm == -1;
            this.IsStep = data.IntNorm == 1;
        }
        /// <summary>
        /// 设置圆柱的属性点
        /// </summary>
        private  void GetFacePoint()
        {
            Part workPart = Session.GetSession().Parts.Work;
            Matrix3x3 mat3 = workPart.WCS.CoordinateSystem.Orientation.Element;
            Point3d centerPt = new Point3d();
            Point3d disPt = new Point3d();
            Point3d start = new Point3d();
            Point3d end = new Point3d();
            Matrix4 inve = this.Matr.GetInversMatrix();
            double angleX = UMathUtils.Angle(this.Direction, new Vector3d(mat3.Xx, mat3.Xy, mat3.Xz));
            double angleY = UMathUtils.Angle(this.Direction, new Vector3d(mat3.Yx, mat3.Yy, mat3.Yz));
            double angleZ = UMathUtils.Angle(this.Direction, new Vector3d(mat3.Zx, mat3.Zy, mat3.Zz));

            if ((UMathUtils.IsEqual(angleX, 0) || UMathUtils.IsEqual(angleX, Math.PI)) ||
                (UMathUtils.IsEqual(angleY, 0) || UMathUtils.IsEqual(angleY, Math.PI)) ||
                (UMathUtils.IsEqual(angleZ, 0) || UMathUtils.IsEqual(angleZ, Math.PI)))
            {
                this.GetCenterAndDisForData(out centerPt, out disPt);
            }
            else
            {
                this.GetFaceBoundingBox(out centerPt, out disPt);
            }
            start.Z = centerPt.Z - disPt.Z;
            end.Z = centerPt.Z + disPt.Z;
            inve.ApplyPos(ref centerPt);
            inve.ApplyPos(ref start);
            inve.ApplyPos(ref end);
            this.CenterPt = centerPt;
            this.StartPt = start;
            this.EndPt = end;
            this.Length = disPt.Z * 2;
        }
        /// <summary>
        /// 通过面数据获得最大外形点（相对Matr）
        /// </summary>
        /// <param name="centerPt"></param>
        /// <param name="disPt"></param>
        private void GetCenterAndDisForData(out Point3d centerPt, out Point3d disPt)
        {
            centerPt = new Point3d();
            disPt = new Point3d();
            Point3d min = this.Data.BoxMinCorner;
            Point3d max = this.Data.BoxMaxCorner;
            this.Matr.ApplyPos(ref min);
            this.Matr.ApplyPos(ref max);

            centerPt = UMathUtils.GetMiddle(min, max);
            disPt.X = this.Data.Radius;
            disPt.Y = this.Data.Radius;
            disPt.Z = UMathUtils.GetDis(min, max) / 2;

        }
        public override string ToString()
        {
            return this.Radius.ToString("f3") + "+" + this.Length.ToString("f3");

        }
    }
}
