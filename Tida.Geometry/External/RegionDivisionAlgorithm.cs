using Tida.Geometry.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tida.Geometry.External
{
    public class RegionDivisionAlgorithm
    {
        #region 属性
        //楼板外侧轮廓
        private Polygon2D FloorRegion { get; set; }
        //内部的墙线
        private List<Line2D> InnerWalls { get; set; }
        //楼板内洞口边线
        private List<Line2D> OpeningRegionOutline;
        //手动添加的模型线
        private List<Line2D> AddedLines { get; set; }
        #endregion

        public RegionDivisionAlgorithm(List<Line2D> floorOutLine, List<Polygon2D> openingRegion, List<Line2D> innerWalls)
        {
            OpeningRegionOutline = new List<Line2D>();
            FloorRegion = new Polygon2D(floorOutLine);
            InnerWalls = innerWalls;
            if (openingRegion != null && openingRegion.Count != 0)
                openingRegion.ForEach(x => OpeningRegionOutline.AddRange(x.Edges));
        }

        public List<Rectangle2D> AutoDivide(ref List<Line2D> remainingLines)
        {
            List<Line2D> allLineInsideFloorRegion = GetAllLineInsideFloorRegion();
            List<Rectangle2D> minimunRectangle = GetMinimunRectangle(allLineInsideFloorRegion);
            minimunRectangle = GetRectangleInFloorOutline(minimunRectangle);
            AddedLines = GetAddedLines(minimunRectangle);
            AddedLines = AddedLines.OrderByDescending(x => x.Length).ToList();
            List<Line2D> orderAddedLines = new List<Line2D>(AddedLines);
            List<Line2D> leftLines = new List<Line2D>(AddedLines);
            List<Rectangle2D> minimumRectangles = new List<Rectangle2D>(minimunRectangle);
            GetBlockAfterMerge(orderAddedLines, minimumRectangles, leftLines);
            remainingLines = leftLines;
            return minimumRectangles;
        }

        private void GetBlockAfterMerge(List<Line2D> orderAddedLines, List<Rectangle2D> minimumRectangles, List<Line2D> leftLines)
        {
            List<Line2D> remainLines = new List<Line2D>();
            Merge(orderAddedLines, minimumRectangles, leftLines);
            remainLines.AddRange(leftLines);
            GraphicAlgorithm.MergeLine(leftLines);
            GraphicAlgorithm.OrderLinesByLength(leftLines);
            if (remainLines.Count != leftLines.Count)
            {
                orderAddedLines = new List<Line2D>(leftLines);
                GetBlockAfterMerge(orderAddedLines, minimumRectangles, leftLines);
            }
        }

        #region 融合矩形
        private void Merge(List<Line2D> orderAddedLines, List<Rectangle2D> minimumRectangles, List<Line2D> leftLines)
        {
            List<Rectangle2D> sameEdgeRec = new List<Rectangle2D>();
            List<Line2D> removeLines = new List<Line2D>();
            foreach (var source in orderAddedLines)
            {
                sameEdgeRec.Clear();
                removeLines.Add(source);
                foreach (var rectangle in minimumRectangles)
                {
                    if (IsRectangleEdge(source, rectangle))
                        sameEdgeRec.Add(rectangle);
                    if (sameEdgeRec.Count > 1)
                    {
                        leftLines.Remove(source);
                        break;
                    }
                }
                if (sameEdgeRec.Count > 1)
                    break;
            }
            removeLines.ForEach(x => orderAddedLines.Remove(x));
            if (sameEdgeRec.Count > 1)
            {
                sameEdgeRec.ForEach(x => minimumRectangles.Remove(x));
                minimumRectangles.Add(Rebuild(sameEdgeRec));
                if (orderAddedLines.Count != 0)
                    Merge(orderAddedLines, minimumRectangles, leftLines);
            }
        }

        private bool IsRectangleEdge(Line2D source, Rectangle2D rec)
        {
            if (IsRectangleEndPoint(source, rec) && IsRectangleWidth(source, rec))
                return true;
            return false;
        }

        private bool IsRectangleWidth(Line2D source, Rectangle2D rec)
        {
            return ((source.Length - rec.Top.Length).AreEqual(0) || (source.Length - rec.Left.Length).AreEqual(0));
        }

        private bool IsRectangleEndPoint(Line2D source, Rectangle2D rec)
        {
            List<Vector2D> newPoints = new List<Vector2D>();
            List<Vector2D> points = new List<Vector2D> { rec.Top.Start, rec.Right.Start, rec.Bottom.Start, rec.Left.Start };
            points.ForEach(x => { if (x.IsAlmostEqualTo(source.Start) || x.IsAlmostEqualTo(source.End)) newPoints.Add(x); });
            return newPoints.Count >= 2;
        }

        private Rectangle2D Rebuild(List<Rectangle2D> targets)
        {
            Rectangle2D rec1 = targets[0];
            Rectangle2D rec2 = targets[1];
            List<Line2D> recEdges = new List<Line2D> { rec1.Bottom, rec1.Top, rec1.Left, rec1.Right, rec2.Bottom, rec2.Top, rec2.Left, rec2.Right };
            List<Line2D> leftLines = new List<Line2D>();
            List<Rectangle2D> newRec = new List<Rectangle2D>(targets);
            List<Line2D> sortedLines = new List<Line2D>(recEdges);
            foreach (var edge in recEdges)
            {
                if (IsRectangleEdge(edge, rec1) && IsRectangleEdge(edge, rec2))
                    sortedLines.Remove(edge);
            }
            GraphicAlgorithm.MergeLine(sortedLines);
            GraphicAlgorithm.HuntCurveByStartPoint(sortedLines, sortedLines.First().Start, leftLines);
            return new Rectangle2D(leftLines);
        }
        #endregion

        #region 生成小矩形
        private List<Rectangle2D> GetMinimunRectangle(List<Line2D> lines)
        {
            var recPoints = new List<Vector2D>();
            var endPoints = lines.GetEndPoints();
            var tempLines = new List<Line2D>();
            foreach (var point in endPoints)
            {
                Line2D line1 = point.CreateLine(GetBasicDirection()[0]);
                Line2D line2 = point.CreateLine(GetBasicDirection()[1]);
                tempLines.Add(line1);
                tempLines.Add(line2);
            }
            EliminateCollinearLines(tempLines);
            for (int i = 0; i < tempLines.Count; i++)
            {
                for (int j = tempLines.Count - 1; j > i; j--)
                {
                    Vector2D p = tempLines[i].Intersect(tempLines[j]);
                    if (p != null)
                        recPoints.Add(p);
                }
            }
            recPoints = recPoints.DistinctPoint();
            return GetRecrangle(recPoints);
        }
        private List<Rectangle2D> GetRecrangle(List<Vector2D> points)
        {
            List<Rectangle2D> rectangles = new List<Rectangle2D>();
            Vector2D floorLocalCoord = GetBasicDirection()[0];
            Vector2D GlobalCoord = new Vector2D(1, 0);
            double xiTa = 0;
            if (floorLocalCoord.Dot(GlobalCoord) > 0 && GlobalCoord.Cross(floorLocalCoord) < 0)
            {
                xiTa = GlobalCoord.AngleWith(floorLocalCoord);
            }
            else if (floorLocalCoord.Dot(GlobalCoord) < 0 && GlobalCoord.Cross(-floorLocalCoord) < 0)
            {
                xiTa = GlobalCoord.AngleWith(floorLocalCoord);
            }
            else if (floorLocalCoord.Dot(GlobalCoord) > 0 && GlobalCoord.Cross(floorLocalCoord) > 0)
            {
                xiTa = Math.PI / 2 - GlobalCoord.AngleWith(floorLocalCoord);
            }
            else if (floorLocalCoord.Dot(GlobalCoord) < 0 && GlobalCoord.Cross(-floorLocalCoord) > 0)
            {
                xiTa = Math.PI / 2 - GlobalCoord.AngleWith(floorLocalCoord);
            }

            points.ForEach(x => x.X = Math.Floor(x.X * 1e9) / 1e9);
            points.ForEach(x => x.Y = Math.Floor(x.Y * 1e9) / 1e9);
            List<Vector2D> newCoord = new List<Vector2D>();
            points.ForEach(x => newCoord.Add(CoordTransform(x, xiTa)));
            List<double> newXCoord = new List<double>();
            List<double> newYCoord = new List<double>();
            newCoord.ForEach(x =>
            {
                newXCoord.Add(x.X);
                newYCoord.Add(x.Y);
            });
            newXCoord = newXCoord.Distinct().OrderBy(x => x).ToList();
            newYCoord = newYCoord.Distinct().OrderBy(y => y).ToList();


            for (int i = 0; i < newXCoord.Count - 1; i++)
            {
                for (int j = 0; j < newYCoord.Count - 1; j++)
                {
                    Vector2D p1 = new Vector2D(newXCoord[i], newYCoord[j]);
                    Vector2D p2 = new Vector2D(newXCoord[i + 1], newYCoord[j]);
                    Vector2D p3 = new Vector2D(newXCoord[i + 1], newYCoord[j + 1]);
                    Vector2D p4 = new Vector2D(newXCoord[i], newYCoord[j + 1]);

                    p1 = CoordTransform(p1, -xiTa);
                    p2 = CoordTransform(p2, -xiTa);
                    p3 = CoordTransform(p3, -xiTa);
                    p4 = CoordTransform(p4, -xiTa);

                    Rectangle2D rec = new Rectangle2D(p1, p2, p3, p4);
                    rectangles.Add(rec);
                }
            }
            return rectangles;
        }

        private Vector2D CoordTransform(Vector2D point, double angle)
        {
            return new Vector2D(point.X * Math.Cos(angle) - point.Y * Math.Sin(angle),
                        point.X * Math.Sin(angle) + point.Y * Math.Cos(angle));
        }


        private void EliminateCollinearLines(List<Line2D> oldLines)
        {
            int k = 0;
            List<Line2D> removeLines = new List<Line2D>();
            for (int i = 0; i < oldLines.Count; i++)
            {
                k = 0;
                for (int j = i + 1; j < oldLines.Count; j++)
                {
                    if (oldLines[i].IsCollinearWith(oldLines[j]))
                    {
                        k++;
                        removeLines.Add(oldLines[j]);
                        break;
                    }
                }
                if (k > 0)
                    break;
            }
            if (k > 0)
            {
                removeLines.ForEach(x => oldLines.Remove(x));
                EliminateCollinearLines(oldLines);
            }
        }

        private void EliminateCoincideLines(List<Line2D> oldLines)
        {
            int k = 0;
            List<Line2D> removeLines = new List<Line2D>();
            for (int i = 0; i < oldLines.Count; i++)
            {
                k = 0;
                for (int j = i + 1; j < oldLines.Count; j++)
                {
                    GraphicAlgorithm.TwoParallelLines2D twoLines = new GraphicAlgorithm.TwoParallelLines2D(oldLines[i], oldLines[j]);
                    if (twoLines.Relationship == GraphicAlgorithm.TwoParallelLinesRelationship.完全相同)
                    {
                        k++;
                        removeLines.Add(oldLines[j]);
                        break;
                    }
                }
                if (k > 0)
                    break;
            }
            if (k > 0)
            {
                removeLines.ForEach(x => oldLines.Remove(x));
                EliminateCoincideLines(oldLines);
            }
        }

        private List<Rectangle2D> GetMinRectangle(List<Line2D> basicLines)
        {
            List<Rectangle2D> fakeRecs = GetMinimunRectangle(basicLines);
            List<Rectangle2D> trueRecs = new List<Rectangle2D>();
            foreach (var item in fakeRecs)
            {
                if (item.Centroid.IsInRegion(FloorRegion.Edges))
                    trueRecs.Add(item);
            }
            return trueRecs;
        }

        private List<Vector2D> GetBasicDirection()
        {
            List<Vector2D> directions = new List<Vector2D>();
            if (Math.Abs(FloorRegion.Edges.First().Direction.Dot(FloorRegion.Edges.Last().Direction)) < Extension.SMALL_NUMBER)
            {
                directions.Add(FloorRegion.Edges.First().Direction);
                directions.Add(FloorRegion.Edges.Last().Direction);
            }
            else
            {
                for (int i = 0; i < FloorRegion.Edges.Count - 1; i++)
                {
                    if (Math.Abs(FloorRegion.Edges[i].Direction.Dot(FloorRegion.Edges[i + 1].Direction)) < Extension.SMALL_NUMBER)
                    {
                        directions.Add(FloorRegion.Edges[i].Direction);
                        directions.Add(FloorRegion.Edges[i + 1].Direction);
                        break;
                    }
                }
            }
            if (directions.Count == 0)
            {
                directions.Add(new Vector2D(1, 0));
                directions.Add(new Vector2D(0, 1));
            }
            return directions;
        }

        private List<Line2D> GetAllLineInsideFloorRegion()
        {
            List<Line2D> lines = new List<Line2D>();
            lines.AddRange(FloorRegion.Edges);
            lines.AddRange(OpeningRegionOutline);
            lines.AddRange(InnerWalls);
            return lines;
        }

        private List<Rectangle2D> GetMinRectangle()
        {
            List<Line2D> basicLines = GetAllLineInsideFloorRegion();
            List<Rectangle2D> fakeRecs = GetMinimunRectangle(basicLines);
            List<Rectangle2D> trueRecs = new List<Rectangle2D>();
            foreach (var item in fakeRecs)
            {
                if (item.Centroid.IsInRegion(FloorRegion.Edges))
                    trueRecs.Add(item);
            }
            return trueRecs;
        }

        private List<Rectangle2D> GetRectangleInFloorOutline(List<Rectangle2D> allRec)
        {
            List<Rectangle2D> uselessRec = new List<Rectangle2D>();
            foreach (var item in allRec)
            {
                if (!item.Centroid.IsInRegion(FloorRegion.Edges))
                    uselessRec.Add(item);
            }
            uselessRec.ForEach(x => allRec.Remove(x));
            return allRec;
        }

        public List<Line2D> GetAddedLines(List<Rectangle2D> miniRectangles)
        {
            List<Line2D> addedLines = new List<Line2D>();
            //miniRectangles = GetMinRectangle();
            List<Line2D> originalLines = GetAllLineInsideFloorRegion();
            List<Line2D> tempLines = new List<Line2D>();
            miniRectangles.ForEach(x => tempLines.AddRange(x.GetEdges()));
            EliminateCoincideLines(tempLines);
            foreach (var item in tempLines)
            {
                foreach (var line in originalLines)
                {
                    GraphicAlgorithm.TwoParallelLines2D twoLines = new GraphicAlgorithm.TwoParallelLines2D(item, line);
                    if (twoLines.Relationship == GraphicAlgorithm.TwoParallelLinesRelationship.线段1全在线段2内 ||
                        twoLines.Relationship == GraphicAlgorithm.TwoParallelLinesRelationship.完全相同)
                        addedLines.Add(item);
                }
            }
            addedLines.ForEach(x => tempLines.Remove(x));
            return tempLines;
        }
        #endregion
    }
}
