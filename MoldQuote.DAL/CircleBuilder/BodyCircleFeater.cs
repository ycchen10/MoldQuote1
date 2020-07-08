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
    /// 体圆柱特征
    /// </summary>
    public class BodyCircleFeater
    {
        private List<AbstractCircleFace> circleFaces = new List<AbstractCircleFace>();
        private List<HoleBuilder> holeBuilders = new List<HoleBuilder>();
        private List<StepBuilder> steps = new List<StepBuilder>();
        private Body body;
        public BodyCircleFeater(Body body)
        {
            this.body = body;
            foreach (Face face in body.GetFaces())
            {
                AbstractCircleFace af = CircleFaceFactory.Create(face);
                if (af != null)
                    circleFaces.Add(af);
            }
        }
        /// <summary>
        /// 获取孔特征
        /// </summary>
        private void GetHoleBuilder()
        {

            if (holeBuilders.Count == 0)
            {
                List<CircularFaceList> hole = CircularCollection.GetHoleList(this.circleFaces);
                foreach (CircularFaceList cl in hole)
                {
                    if (cl.IsCylinder())
                        holeBuilders.Add(new HoleBuilder(cl));
                }
            }
        }

        /// <summary>
        /// 获取圆柱特征
        /// </summary>
        private void GetStepBuilder()
        {

            if (holeBuilders.Count == 0)
            {
                List<CircularFaceList> step = CircularCollection.GetStepList(this.circleFaces);
                foreach (CircularFaceList cl in step)
                {
                    if (cl.IsCylinder())
                        steps.Add(new StepBuilder(cl));
                }
            }
        }
        /// <summary>
        /// 判断是否事圆柱体
        /// </summary>
        /// <returns></returns>
        public bool IsCylinderBody(out StepBuilder step)
        {
            GetStepBuilder();
            if (this.steps.Count == 1)
            {
                double rid = this.steps[0].CylFeater[0].Cylinder.Radius;
                Matrix4 mat = this.steps[0].CylFeater[0].Cylinder.Matr;
                Matrix4 inv = mat.GetInversMatrix();
                Point3d centerPt = new Point3d();
                Point3d disPt = new Point3d();
                NXObject[] obj = { this.body };
                CartesianCoordinateSystem csys = BoundingBoxUtils.CreateCoordinateSystem(mat, inv);
                BoundingBoxUtils.GetBoundingBoxInLocal(obj, csys, mat, ref centerPt, ref disPt);
                if (UMathUtils.IsEqual(disPt.X, rid) && UMathUtils.IsEqual(disPt.Y, rid))
                {
                    step = this.steps[0];
                    return true;
                }
            }
            step = null;
            return false;
        }


        /// <summary>
        /// 获取单一盲孔
        /// </summary>
        /// <returns></returns>
        public List<OnlyBlindHoleFeature> GetOnlyBlindHoleFeature()
        {
            GetHoleBuilder();
            List<OnlyBlindHoleFeature> feat = new List<OnlyBlindHoleFeature>();
            foreach (HoleBuilder hb in holeBuilders)
            {
                if (hb.CylFeater.Count == 1 && hb.IsBlindHole())
                {
                    feat.Add(new OnlyBlindHoleFeature(hb));
                }
            }
            return feat;
        }
        /// <summary>
        /// 获取单一通孔
        /// </summary>
        /// <returns></returns>
        public List<OnlyThroughHoleFeature> GetOnlyThroughHoleFeature()
        {
            GetHoleBuilder();
            List<OnlyThroughHoleFeature> feat = new List<OnlyThroughHoleFeature>();
            foreach (HoleBuilder hb in holeBuilders)
            {
                if (hb.CylFeater.Count == 1 && !hb.IsBlindHole())
                {
                    feat.Add(new OnlyThroughHoleFeature(hb));
                }
            }
            return feat;
        }

        /// <summary>
        /// 获取平底盲孔
        /// </summary>
        /// <returns></returns>
        public List<StepHoleFeature> GetStepHoleFeature()
        {
            GetHoleBuilder();
            List<StepHoleFeature> feat = new List<StepHoleFeature>();
            foreach (HoleBuilder hb in holeBuilders)
            {
                if (hb.CylFeater.Count == 1 && hb.IsBlindHole())
                {
                    bool isok = false;
                    foreach (AbstractCircleFace af in hb.List.CircleFaceList)
                    {
                        if (af is CircleAnnylusFace)
                            isok = true;
                    }
                    if (isok)
                        feat.Add(new StepHoleFeature(hb));
                }
            }
            return feat;
        }
        /// <summary>
        /// 获取台阶盲孔
        /// </summary>
        /// <returns></returns>
        public List<StepBlindHoleFeature> GetStepBlindHoleFeature()
        {
            GetHoleBuilder();
            List<StepBlindHoleFeature> feat = new List<StepBlindHoleFeature>();
            foreach (HoleBuilder hb in holeBuilders)
            {
                if (hb.CylFeater.Count > 1 && hb.IsBlindHole())
                {
                    feat.Add(new StepBlindHoleFeature(hb));
                }
            }
            return feat;
        }
        /// <summary>
        /// 获取台阶通孔
        /// </summary>
        /// <returns></returns>
        public List<StepThroughHoleFeature> GetStepThroughHoleFeature()
        {
            GetHoleBuilder();
            List<StepThroughHoleFeature> feat = new List<StepThroughHoleFeature>();
            foreach (HoleBuilder hb in holeBuilders)
            {
                if (hb.CylFeater.Count > 1 && !hb.IsBlindHole())
                {
                    feat.Add(new StepThroughHoleFeature(hb));
                }
            }
            return feat;
        }

    }
}
