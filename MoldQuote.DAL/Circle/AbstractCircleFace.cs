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
    /// 圆形面类型
    /// </summary>
    public abstract class AbstractCircleFace : IComparable<AbstractCircleFace>, IDisplayObject
    {
        protected Matrix4 matr;
        protected Vector3d dir;
        /// <summary>
        /// 起点
        /// </summary>
        public Point3d StartPt { get; protected set; }
        /// <summary>
        /// 终点
        /// </summary>
        public Point3d EndPt { get; protected set; }
        public FaceData Data { get; private set; }
        /// <summary>
        /// 方向
        /// </summary>
        public Vector3d Direction { get { return dir; } }
        /// <summary>
        /// 矩阵
        /// </summary>
        public Matrix4 Matr { get { return matr; } }
        /// <summary>
        /// 是否是孔
        /// </summary>
        public bool IsHole { get; protected set; }

        public bool IsStep { get; protected set; }
        /// <summary>
        /// 中心点
        /// </summary>
        public Point3d CenterPt { get; protected set; }

        public Node Node { get; set; }
        public AbstractCircleFace(FaceData data)
        {
            this.Data = data;
            dir = data.Dir;
            matr = new Matrix4();
            matr.Identity();
            matr.TransformToZAxis(this.CenterPt, data.Dir);
        }
        public void Highlight(bool highlight)
        {
            if (highlight)
                this.Data.Face.Highlight();
            else
                this.Data.Face.Unhighlight();
        }
        /// <summary>
        /// 判断是否是一个孔
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsTheSameHole(AbstractCircleFace other)
        {
            double angle = UMathUtils.Angle(this.Data.Dir, other.Data.Dir);
            if (UMathUtils.IsEqual(angle, 0) == false && UMathUtils.IsEqual(angle, Math.PI) == false)
            {
                return false;
            }
            Vector3d vec1 = new Vector3d();
            Vector3d vec2 = new Vector3d();
            if (UMathUtils.IsEqual(this.CenterPt, other.CenterPt))
            {
                vec1 = this.Data.Dir;
                vec2 = new Vector3d(-vec1.X, -vec1.Y, -vec1.Z);
            }
            else
            {
                vec1 = UMathUtils.GetVector(this.CenterPt, other.CenterPt);
                vec2 = UMathUtils.GetVector(other.CenterPt, this.CenterPt);
            }
            angle = UMathUtils.Angle(this.Data.Dir, vec1);
            if (UMathUtils.IsEqual(angle, 0) == false && UMathUtils.IsEqual(angle, Math.PI) == false)
            {
                return false;
            }
            if (TraceARay.AskTraceARay(this.Data.Face.GetBody(), this.CenterPt, vec1) > 1 && TraceARay.AskTraceARay(other.Data.Face.GetBody(), other.CenterPt, vec2) > 1)

            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 圆形凸台
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsCircleStep(AbstractCircleFace other)
        {
            double angle = UMathUtils.Angle(this.Data.Dir, other.Data.Dir);
            if (UMathUtils.IsEqual(angle, 0) == false && UMathUtils.IsEqual(angle, Math.PI) == false)
            {
                return false;
            }
            Vector3d vec1 = UMathUtils.GetVector(this.CenterPt, other.CenterPt);

            angle = UMathUtils.Angle(this.Data.Dir, vec1);
            if (UMathUtils.IsEqual(angle, 0) == false && UMathUtils.IsEqual(angle, Math.PI) == false)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 通过边界盒获得最大外形(相对Matr)
        /// </summary>
        /// <param name="centerPt"></param>
        /// <param name="disPt"></param>
        public void GetFaceBoundingBox(out Point3d centerPt, out Point3d disPt)
        {
            centerPt = new Point3d();
            disPt = new Point3d();
            NXObject[] obj = { this.Data.Face };
            Matrix4 inve = this.Matr.GetInversMatrix();
            CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(this.Matr, inve);
            BoundingBoxUtils.GetBoundingBoxInLocal(obj, csys, this.Matr, ref centerPt, ref disPt);
        }

        public int CompareTo(AbstractCircleFace other)
        {
            Point3d pt1 = this.CenterPt;
            Point3d pt2 = other.CenterPt;
            this.Matr.ApplyPos(ref pt1);
            this.Matr.ApplyPos(ref pt2);
            return pt1.Z.CompareTo(pt2.Z);
        }
        /// <summary>
        /// 设置轴线方向相反
        /// </summary>
        /// <param name="dir"></param>
        public void SetReverseDirection(Vector3d dir)
        {
            double angle = UMathUtils.Angle(this.dir, dir);
            if (UMathUtils.IsEqual(angle, 0))
                return;
            else if (UMathUtils.IsEqual(angle, Math.PI))
            {
                this.dir = dir;
                this.matr.TransformToZAxis(this.CenterPt, dir);
                Point3d temp = this.StartPt;
                this.StartPt = this.EndPt;
                this.EndPt = temp;
            }
            else
            {
                LogMgr.WriteLog("面反向错误！");
            }

        }


    }
}
