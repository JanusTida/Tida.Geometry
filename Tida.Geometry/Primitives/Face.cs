using System;
using System.Collections.Generic;
using System.Linq;
using Tida.Geometry.External;
namespace Tida.Geometry.Primitives
{

    public class Face : Object3D
    {
        private Vector3D origin;
        
        public Vector3D Origin
        {
            get
            {
                return origin;
            }
            set
            {
                origin = value;
            }
        }//平面原点


        private Vector3D normal;
        
        public Vector3D Normal
        {
            get
            {
                return normal;
            }
            set
            {
                normal = value;
            }
        }
        public Face()
        {

        }


        public static Face Create(List<Vector3D> points)
        {
            Face face = new Face(points);
            face.origin = face.Edges.GetOrigin();
            face.normal = face.Edges.GetNormal();

            return face;
        }

        public static Face Create(List<Line3D> lines)
        {
            if(lines[0].Direction.Cross(lines[1].Direction).Normalize().IsAlmostEqualTo(Vector3D.BasisZ)||
                lines[0].Direction.Cross(lines[1].Direction).Normalize().IsAlmostEqualTo(-Vector3D.BasisZ))
            {
                if (!lines.First().Start.IsAlmostEqualTo(lines.Last().End))
                    return null;
            }

            Face face = new Face(lines);
            face.origin = face.Edges.GetOrigin();
            face.normal = face.Edges.GetNormal();

            return face;
            //return new Face(lines);
        }
        
        public List<Line3D> Edges
        {
            get;
             set;
        }
        
        public ShapeType ShapeType
        {
            get
            {
                if (this.Edges.Count == 3)
                {

                    return ShapeType.Triangle;
                }
                else if (this.Edges.Count == 4)
                {
                    return ShapeType.Quadrilateral;
                }
                else
                {
                    return ShapeType.Multi;
                }
            }
            set { }
        }

        public bool IsContain(Line3D line)
        {

            foreach (Line3D cl in Edges)
            {
                if (cl.IsAlmostEqualTo(line))
                {
                    return true;
                }
                //if (cl.Equals(line))
                //{
                //    return true;
                //}
            }
            return false;
        }

        public List<Line3D> GetHorizontalEdge()
        {

            List<Line3D> lines = new List<Line3D>();

            foreach (Line3D ls in Edges)
            {

                if (ls.IsHorizontal())
                {

                    lines.Add(ls);
                }
            }
            return lines;
        }
        public List<Line3D> GetVerticalEdge()
        {

            List<Line3D> lines = new List<Line3D>();

            foreach (Line3D ls in Edges)
            {

                if (ls.IsVertical())
                {

                    lines.Add(ls);
                }
            }
            return lines;

        }

        
        public Vector3D Oritention
        {
            get;
            set;
        }


        /// <summary>
        /// 有限面相交获得相交线段集合
        /// </summary>
        /// <param name="source">与之相交的面</param>
        /// <returns></returns>
        public List<Line3D> Intersect(Face source)
        {
            Line3D intersectStratightLine = IntersectUnlimitedFace(source);
            if (intersectStratightLine == null)
                return new List<Line3D>();

            List<Line3D> originalEdges = this.Edges;
            List<Line3D> sourceEdges = source.Edges;

            //直线与多边形相交的交点
            List<Vector3D> originalFaceInterPoint = new List<Vector3D>();
            List<Vector3D> sourceFaceInterpoint = new List<Vector3D>();
            foreach (var edge in originalEdges)
            {
                Vector3D intersectPoint = intersectStratightLine.IntersectStraightLine2(edge);
                if (intersectPoint != null)
                    originalFaceInterPoint.Add(intersectPoint);
            }
            foreach (var edge in sourceEdges)
            {
                Vector3D intersectPoint = intersectStratightLine.IntersectStraightLine2(edge);
                if (intersectPoint != null)
                    sourceFaceInterpoint.Add(intersectPoint);
            }
            if (originalFaceInterPoint.Count == 0 || sourceFaceInterpoint.Count == 0)
                return new List<Line3D>();


            //直线与多边形相交的交线

            List<Line3D> originalFaceInterLine = GetLinesFromIntersectPoints(originalFaceInterPoint, originalEdges);
            List<Line3D> sourceFaceInterLine = GetLinesFromIntersectPoints(sourceFaceInterpoint, sourceEdges);

            //List<Line3D> originalFaceInterLine = GetLinesFromIntersectPoints2(originalFaceInterPoint, originalEdges);
            //List<Line3D> sourceFaceInterLine = GetLinesFromIntersectPoints2(sourceFaceInterpoint, sourceEdges);

            Merge(originalFaceInterLine);
            Merge(sourceFaceInterLine);

            //两个面所有的共有交线
            List<Line3D> coownInterLine = new List<Line3D>();
            foreach (var line1 in originalFaceInterLine)
            {
                foreach (var line2 in sourceFaceInterLine)
                {
                    Line3D tempLine = GetPublicLine(line1, line2);
                    if (tempLine != null)
                        coownInterLine.Add(tempLine);
                }
            }

            return coownInterLine;
        }



        #region 私有方法

        private Face(List<Vector3D> points)
        {
            this.Edges = new List<Line3D>();

            for (int i = 0; i < points.Count; i++)
            {
                Line3D line3d = null;
                if (i < points.Count - 1)
                {
                    line3d = Line3D.Create(points[i], points[i + 1]);
                }
                else
                {
                    line3d = Line3D.Create(points[i], points[0]);
                }
                //if (!IsLineForces(line3d))
                //{
                this.Edges.Add(line3d);
                // }
                //else
                //{

                //    throw new Exception("传入点的无法构建闭合空间");

                //}
            }

            if (this.Edges.Count < 3)
            {

                throw new Exception("无法构建一个面");
            }
        }

        private Face(List<Line3D> lines)
        {
            this.Edges = new List<Line3D>();
            for (int i = 0; i < lines.Count; i++)
            {

                this.Edges.Add(lines[i]);

            }
            if (this.Edges.Count < 3)
            {

                throw new Exception("无法构建一个面");
            }
        }

        private bool IsLineForces(Line3D line)
        {

            foreach (Line3D ld in Edges)
            {

                if (ld.Intersect(line) != null)
                {

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 面和面相交获得一条直线
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Line3D IntersectUnlimitedFace(Face source)
        {
            Vector3D origin1 = this.Origin;
            Vector3D normal1 = this.Normal;//n1
            Vector3D origin2 = source.Origin;
            Vector3D normal2 = source.Normal;//n2

            Vector3D direction = normal1.Cross(normal2).Normalize();
            if (direction.Modulus().AreEqual(0))
            {
                return null;//无交点
            }

            double s1, s2, a, b;
            s1 = normal1.Dot(origin1);
            s2 = normal2.Dot(origin2);
            double n1n2dot = normal1.Dot(normal2);
            double n1normsqr = normal1.Dot(normal1);
            double n2normsqr = normal2.Dot(normal2);
            a = (s2 * n1n2dot - s1 * n2normsqr) / (Math.Pow(n1n2dot, 2) - n1normsqr * n2normsqr);
            b = (s1 * n1n2dot - s2 * n1normsqr) / (Math.Pow(n1n2dot, 2) - n1normsqr * n2normsqr);
            Vector3D point = a * normal1 + b * normal2;

            //Vector3D pointNext = point + 10 * direction;
            //Line3D intersectLine = new Line3D(point, pointNext);
            Line3D intersectLine = Line3D.CreateUnbound(point, direction);
            return intersectLine;

        }

        private void Merge(List<Line3D> curves)
        {

            for (int i = 0; i < curves.Count - 1; i++)
            {
                if (curves[i].End.IsAlmostEqualTo(curves[i + 1].Start))
                {
                    Line3D tempCur = new Line3D(curves[i].Start, curves[i + 1].End);
                    curves.Remove(curves[i]);//移除线段
                    curves.Insert(i, tempCur);//在指定索引位置加入线段
                    curves.Remove(curves[i + 1]);

                    i--;
                }
            }
        }

        /// <summary>
        /// 通过直线和多边形交点的奇偶性，获得相交的线段
        /// </summary>
        /// <param name="intersectPoints"></param>
        /// <param name="faceEdges"></param>
        /// <returns></returns>
        private List<Line3D> GetLinesFromIntersectPoints(List<Vector3D> intersectPoints, List<Line3D> faceEdges)
        {
            List<Line3D> interLines = new List<Line3D>();
            List<Vector3D> tempIntersectPoints = new List<Vector3D>();
            foreach (var point in intersectPoints)
            {
                if (tempIntersectPoints.Contains(point, new Vector3DEqualityComparer()))
                    continue;
                tempIntersectPoints.Add(point);
            }
            tempIntersectPoints = tempIntersectPoints.OrderByXYZ();
            List<Vector3D> newIntersectPoints = new List<Vector3D>(tempIntersectPoints);
            bool firstPointIsEndPoint = false;//起点是否线的端点
            for (int i = 0; i < newIntersectPoints.Count; i++)
            {
                bool isEndPoint = PointIsEndPointOfCurves(newIntersectPoints[i], faceEdges);
                if (i == 0 && isEndPoint)
                    firstPointIsEndPoint = true;
                if (isEndPoint)
                {
                    Vector3D newPoint = new Vector3D(newIntersectPoints[i].X, newIntersectPoints[i].Y, newIntersectPoints[i].Z);
                    newIntersectPoints.Insert(i, newPoint);
                    i++;
                }
            }
            if (firstPointIsEndPoint)
            {
                //取偶数线段
                for (int i = 0; i < newIntersectPoints.Count - 1; i++)
                {
                    if (i % 2 != 0)
                    {
                        if (newIntersectPoints[i].IsAlmostEqualTo(newIntersectPoints[i + 1]))
                            continue;
                        Line3D temp = new Line3D(newIntersectPoints[i], newIntersectPoints[i + 1]);
                        interLines.Add(temp);
                    }
                }
            }
            else
            {
                //取奇数线段
                for (int i = 0; i < newIntersectPoints.Count - 1; i++)
                {
                    if (i % 2 == 0)
                    {
                        if (newIntersectPoints[i].IsAlmostEqualTo(newIntersectPoints[i + 1]))
                            continue;
                        Line3D temp = new Line3D(newIntersectPoints[i], newIntersectPoints[i + 1]);
                        interLines.Add(temp);
                    }
                }
            }

            return interLines;
        }

        private List<Line3D> GetLinesFromIntersectPoints2(List<Vector3D> intersectPoints, List<Line3D> faceEdges)
        {
            List<Line3D> interLines = new List<Line3D>();
            List<Vector3D> newIntersectPoints = new List<Vector3D>();
            foreach (var point in intersectPoints)
            {
                if (newIntersectPoints.Contains(point, new Vector3DEqualityComparer()))
                    continue;
                newIntersectPoints.Add(point);
            }
            newIntersectPoints = newIntersectPoints.OrderByXYZ();
            for (int i = 0; i < newIntersectPoints.Count - 1; i++)
            {
                Line3D temp = new Line3D(newIntersectPoints[i], newIntersectPoints[i + 1]);
                Vector3D middlePoint = temp.Evaluate(0.5);
                //判断点是否在多边形区域的内部或者边上
                bool isPointInRegion = IsPointInsideRegion(middlePoint, faceEdges) || IsPointInRegionOutLine(middlePoint, faceEdges);
                if (isPointInRegion)
                    interLines.Add(temp);
            }
            return interLines;
        }


        /// <summary>
        /// 点在多边形的内部
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool IsPointInsideRegion(Vector3D point, List<Line3D> lines)
        {

            //采用累积角度法判断点是否在区域内，首先判断点是否在轮廓线上，如果是，则返回false；
            //再对轮廓线排序，判断点是否在区域内部，如果是，则返回true.
            bool pointInCurves = IsPointInRegionOutLine(point, lines);
            if (pointInCurves)
                return false;

            List<Line3D> newLines = lines.SortLinesContinuously();
            Vector3D refNomal = Extension.GetNormal(newLines);

            //List<Line3D> newLines = lines.SortCurvesByCounterClockwise();//排序

            double sum = 0;
            foreach (var line in newLines)
            {
                Vector3D vector1 = (line.Start - point).Normalize();
                Vector3D vector2 = (line.End - point).Normalize();
                double angle = vector2.AngleFrom(vector1, refNomal);
                if (angle > Math.PI)
                    angle -= Math.PI * 2;
                sum += angle;

            }
            //为0（或2PI的偶数倍）时，点位于多边形之外；
            //为+-2PI（或2PI的奇数倍）时，点位于多边形之内；
            if (((sum / (Math.PI * 2)) % 2).AreEqual(0))
                return false;
            else if (!((sum / (Math.PI * 2)) % 2).AreEqual(0))
                return true;

            return false;
        }

        /// <summary>
        ///点在多边形的轮廓线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private bool IsPointInRegionOutLine(Vector3D point, List<Line3D> lines)
        {
            foreach (var line in lines)
            {
                bool pointOnCurve = point.IsOnLine(line);
                if (pointOnCurve)
                    return true;
            }
            return false;
        }

        private bool PointIsEndPointOfCurves(Vector3D point, List<Line3D> curves)
        {
            foreach (var line in curves)
            {
                if (point.IsAlmostEqualTo(line.Start) || point.IsAlmostEqualTo(line.End))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 获得两条线段的公共部分
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        private Line3D GetPublicLine(Line3D line1, Line3D line2)
        {
            Line3D publicLine = null;
            List<Vector3D> points = new List<Vector3D>();
            if (line1.Start.IsOnLine(line2))
                points.Add(line1.Start);
            if (line1.End.IsOnLine(line2))
                points.Add(line1.End);
            if (line2.Start.IsOnLine(line1))
                points.Add(line2.Start);
            if (line2.End.IsOnLine(line1))
                points.Add(line2.End);
            List<Vector3D> newPoints = new List<Vector3D>();
            foreach (var point in points)
            {
                if (newPoints.Contains(point, new Vector3DEqualityComparer()))
                    continue;
                newPoints.Add(point);
            }
            if (newPoints.Count == 2)
                publicLine = new Line3D(newPoints[0], newPoints[1]);
            return publicLine;
        }

        #endregion

        /// <summary>
        /// 进行坐标转换
        /// </summary>
        protected override void Vary()
        {
            this.Edges.ForEach(x =>
            {
                x.Transform = this.Transform;
            });
        }

    }

    public enum ShapeType
    {

        Quadrilateral,
        Triangle,
        Multi
    }
}
