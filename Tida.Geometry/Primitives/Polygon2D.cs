using System;
using System.Collections.Generic;
using System.Linq;

using Tida.Geometry.External;

namespace Tida.Geometry.Primitives
{

    public class Polygon2D
    {
        /// <summary>
        /// 顶点
        /// </summary>
        
        public List<Vector2D> Vertexes { get { return GetVertexes(); } set { } }
        /// <summary>
        /// 面积
        /// </summary>
        
        public double Area { get { return GetArea(); } set { } }
        /// <summary>
        /// 形心
        /// </summary>
        
        public Vector2D Centroid { get { return GetCentroid(); } set { } }
        /// <summary>
        /// 轮廓线
        /// </summary>
        
        public List<Line2D> Edges { get;  set; }
        public Polygon2D(List<Line2D> lines)
        {
            List<Line2D> newLines = new List<Line2D>(lines);
            Edges = MakeCounterclockwise(newLines);
        }

        public Polygon2D()
        {
        }
        /// <summary>
        /// 对多边形边进行整体偏移
        /// </summary>
        /// <param name="offsetDist">正值为向外偏，负值为向内偏</param>
        /// <returns></returns>
        public Polygon2D CreateOffset(double offsetDist)
        {
            List<Line2D> extendLines = new List<Line2D>();
            Edges.ForEach(x => extendLines.Add(x.CreateOffset((new Vector2D(x.Direction.Y, -x.Direction.X)) * offsetDist)));
            List<Line2D> offsetLines = new List<Line2D>();
            Vector2D firstPoint = extendLines.First().IntersectStraightLine(extendLines.Last());
            List<Vector2D> offsetPoints = new List<Vector2D> { firstPoint };
            for (int i = 0; i < extendLines.Count - 1; i++)
            {
                Vector2D intersect = extendLines[i].IntersectStraightLine(extendLines[i + 1]);
                if (intersect != null)
                    offsetPoints.Add(intersect);
            }
            offsetPoints.Add(firstPoint);
            for (int i = 0; i < offsetPoints.Count - 1; i++)
                offsetLines.Add(Line2D.Create(offsetPoints[i], offsetPoints[i + 1]));
            return new Polygon2D(offsetLines);
        }

        private List<Line2D> MakeCounterclockwise(List<Line2D> lines)
        {
            lines = lines.MakeCounterclockwise();
            return lines;
        }

        private List<Vector2D> GetVertexes()
        {
            List<Vector2D> vertexes = new List<Vector2D>();
            for (int i = 0; i < Edges.Count; i++)
            {
                vertexes.Add(Edges[i].Start);
            }
            return vertexes;
        }

        private double GetArea()
        {
            List<Vector2D> points = new List<Vector2D>(Vertexes);
            points.Add(Vertexes[0]);
            double area = 0.0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                area += points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y;
            }
            return Math.Abs(area / 2);
        }

        private Vector2D GetCentroid()
        {
            List<Vector2D> points = new List<Vector2D>(Vertexes);
            points.Add(Vertexes[0]);
            double area = 0.0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                area += points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y;
            }
            area = area / 2;
            double x = 0.0;
            double y = 0.0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                x += (points[i].X + points[i + 1].X) * (points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y);
                y += (points[i].Y + points[i + 1].Y) * (points[i].X * points[i + 1].Y - points[i + 1].X * points[i].Y);
            }
            x = x / (area * 6);
            y = y / (area * 6);
            return new Vector2D(x, y);
        }
    }
}
