using System.Collections.Generic;
using System.Linq;
using Tida.Geometry.External;
using Tida.Geometry.Alternation;

namespace Tida.Geometry.Primitives
{

    public class Polygon3D
    {
        /// <summary>
        /// 顶点
        /// </summary>

        public List<Vector3D> Vertexes { get { return GetVertexes(); } set { } }
        /// <summary>
        /// 面积
        /// </summary>

        public double Area { get { return GetArea(); } set { } }

        /// <summary>
        /// 形心
        /// </summary>


        public Vector3D Centroid { get { return GetCentroid(); } set { } }
        /// <summary>
        /// 轮廓线
        /// </summary>

        public List<Line3D> Edges { get; set; }

        /// <summary>
        /// 法向量
        /// </summary>

        public Vector3D Normal { get { return GetNormal(); } set { } }

        /// <summary>
        /// 转换矩阵算法
        /// </summary>
        /// 
        private Matrix4 Matrix
        {
            get
            {
                return GetMatrix();
            }
            set { }
        }


        private Matrix4 GetMatrix()
        {
            Vector3D origin = Edges[0].Start;
            Vector3D xAxis = Edges[0].Direction;
            Vector3D zAxis = Edges[0].Direction.Cross(Edges[1].Direction).Normalize();
            Vector3D yAxis = zAxis.Cross(xAxis).Normalize();
            Matrix4 matrix = TransformUtil.ViewMatrix(origin, xAxis, yAxis);
            return matrix;
        }

        //private GlobalToLocalCoordinateAlgorithm tAlgorithm { get { return GetTransform(); } set { } }

        private Polygon2D Plane { get { return ProjectToPlane(); } set { } }
        /// <summary>
        /// lines集合必须为一个平面内的
        /// </summary>
        /// <param name="lines"></param>
        public Polygon3D(List<Line3D> lines)
        {
            List<Line3D> newLines = new List<Line3D>(lines);
            Edges = newLines;
        }

        private List<Vector3D> GetVertexes()
        {
            return Edges.Select(t => t.Start).ToList();
        }

        private Polygon2D ProjectToPlane()
        {
            List<Line2D> lines = Edges.Select(edge => TransformLine(Matrix, edge)).ToList();
            return new Polygon2D(lines);
        }

        private double GetArea()
        {
            return Plane.Area;
        }

        private Vector3D GetCentroid()
        {
            Vector3D v = new Vector3D(Plane.Centroid.X, Plane.Centroid.Y, 0);
            Matrix4 reverse = Matrix.GetInverse();
            return reverse * v;
        }

        private Vector3D GetNormal()
        {
            return Edges[0].Direction.Cross(Edges[1].Direction).Normalize();
        }

        private Line2D TransformLine(Matrix4 transMatrix, Line3D line)
        {

            Vector3D start = line.Start;
            Vector3D end = line.End;

            //开始转换
            Vector3D newStart = transMatrix * start;
            Vector3D newEnd = transMatrix * end;


            Line2D targetLine = new Line2D(new Vector2D(newStart.X, newStart.Y),
                new Vector2D(newEnd.X, newEnd.Y));
            return targetLine;
        }

        //private GlobalToLocalCoordinateAlgorithm GetTransform()
        //{
        //    Vector3D origin = Edges[0].Evaluate(0.5);
        //    Vector3D axisX = Edges[0].Direction;
        //    Vector3D axisY = Normal.Cross(axisX).Normalize();
        //    return new GlobalToLocalCoordinateAlgorithm(origin, axisX, axisY);
        //}
    }
}
