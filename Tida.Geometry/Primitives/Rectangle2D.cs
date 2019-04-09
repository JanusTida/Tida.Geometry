using System;
using System.Collections.Generic;
using System.Linq;

using Tida.Geometry.External;

namespace Tida.Geometry.Primitives
{

    public class Rectangle2D
    {
        public Line2D Bottom { get;  set; }
        
        public Line2D Top { get;  set; }
        
        public Line2D Right { get;  set; }
        
        public Line2D Left { get;  set; }

        public bool BSupport { get; set; }

        public bool TSupport { get; set; }

        public bool RSupport { get; set; }

        public bool LSupport { get; set; }

        public double SupportNum { get { return GetSuports(); } }

        public Rectangle2D(List<Line2D> lines)
        {
            lines = lines.MakeCounterclockwise();
            Bottom = lines[0];
            Right = lines[1];
            Top = lines[2];
            Left = lines[3];
            Vertexes = GetVertexes();
            Area = GetArea();
            Centroid = GetCentroid();
        }

        public Rectangle2D(Vector2D p0, Vector2D p1, Vector2D p2, Vector2D p3)
        {
            GetRectangle(p0, p1, p2, p3);
            Vertexes = GetVertexes();
            Area = GetArea();
            Centroid = GetCentroid();
        }

        /// <summary>
        /// 顶点
        /// </summary>
        
        public List<Vector2D> Vertexes { get;  set; }
        /// <summary>
        /// 面积
        /// </summary>
        
        public double Area { get;  set; }
        /// <summary>
        /// 形心
        /// </summary>
        
        public Vector2D Centroid { get;  set; }
        private List<Vector2D> GetVertexes()
        {
            return new List<Vector2D> { Bottom.Start, Right.Start, Top.Start, Left.Start };
        }

        private double GetSuports()
        { 
            double n=0;
            if (BSupport)
                n += 1;
            if (TSupport)
                n += 1;
            if (LSupport)
                n += 1;
            if (RSupport)
                n += 1;
            return n;
        }

        private double GetArea()
        {
            return Bottom.Length * Right.Length;
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

        public List<Line2D> GetEdges()
        {
            return new List<Line2D> { Bottom, Right, Top, Left };
        }

        public Line2D GetLengthSide()
        {
            return Bottom.Length > Right.Length ? Bottom : Right;
        }

        public Line2D GetWidthSide()
        {
            return Bottom.Length < Right.Length ? Bottom : Right;
        }

        public List<Line2D> GetLengthAndWidth()
        {
            List<Line2D> lengthAndWidth = new List<Line2D>();
            if (Bottom.Length > Right.Length)
            {
                lengthAndWidth.Add(Bottom);
                lengthAndWidth.Add(Right);
            }
            else
            {
                lengthAndWidth.Add(Left);
                lengthAndWidth.Add(Bottom);
            }
            return lengthAndWidth;
        }

        public bool IsCurvesThrough(List<Line2D> lines)
        {
            List<Line2D> region = new List<Line2D> { this.Bottom, this.Right, this.Top, this.Left };
            bool result = false;
            foreach (var item in lines)
            {
                List<Vector2D> intersects = new List<Vector2D>();
                foreach (var edge in region)
                {
                    Vector2D p1 = item.Intersect(edge);
                    if (p1 != null)
                        intersects.Add(p1);
                }
                if (intersects.Count == 2)
                {
                    int iteration = 0;
                    foreach (var line in region)
                    {
                        if (intersects[0].IsOnLine(line) && intersects[1].IsOnLine(line))
                        {
                            iteration++;
                        }
                    }
                    if (iteration == 0)
                    {
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }

        private void GetRectangle(Vector2D p0, Vector2D p1, Vector2D p2, Vector2D p3)
        {
            List<Vector2D> points = new List<Vector2D> { p0, p1, p2, p3 };
            points.ForEach(x => x.X = Math.Floor(x.X * 1e9) / 1e9);
            points.ForEach(x => x.Y = Math.Floor(x.Y * 1e9) / 1e9);
            Vector2D first = points.OrderBy(x => x.X).ThenBy(y => y.Y).First();
            List<Line2D> tryCurve = new List<Line2D>();
            for (int i = 0; i < points.Count - 1; i++)
            {
                for (int j = i + 1; j < points.Count; j++)
                {
                    tryCurve.Add(Line2D.Create(points[i], points[j]));
                }
            }
            tryCurve = tryCurve.OrderByDescending(x => x.Length).ToList();
            tryCurve.RemoveAt(1);
            tryCurve.RemoveAt(0);
            tryCurve = tryCurve.MakeCounterclockwise();
            Bottom = tryCurve[0];
            Right = tryCurve[1];
            Top = tryCurve[2];
            Left = tryCurve[3];
        }
        /// <summary>
        /// 当前当前多边形的线段进行排序
        /// </summary>
        public void MakeCounterclockwise()
        {
            List<Line2D> lines = new List<Line2D> { Bottom, Right, Top, Left };
            List<Line2D> newLines = lines.MakeCounterclockwise();
            Bottom = newLines[0];
            Right = newLines[1];
            Top = newLines[2];
            Left = newLines[3];
        }
    }
}
