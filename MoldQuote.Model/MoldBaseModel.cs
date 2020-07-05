﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen;
using NXOpen.BlockStyler;
using Basic;

namespace MoldQuote.Model
{
    public class MoldBaseModel
    {
        private Matrix4 matr;
        public string Name { get; set; }

        public Body Body { get; private set; }

        public Point3d CenterPt { get; private set; }

        public Point3d DisPt { get; private set; }

        public Node node { get; set; }
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
        }

    }
}
