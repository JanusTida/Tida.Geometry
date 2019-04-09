using System;
using System.Collections.Generic;
using System.Linq;
using Tida.Geometry.Primitives;
using Tida.Geometry.Alternation;

namespace Tida.Geometry.External
{
    public class GraphicAlgorithm
    {
        private const double SMALL_NUMBER = 1e-4;
        public class TwoParallelLines2D
        {
            public Line2D Line1 { get; set; }
            public Line2D Line2 { get; set; }
            public TwoParallelLinesRelationship Relationship { get; set; }
            public TwoParallelLines2D(Line2D line1, Line2D line2)
            {
                Line1 = line1;
                Line2 = line2;
                DetermineRelationship();

            }

            /// <summary>
            /// 得到重复部分的线段
            /// </summary>
            /// <returns></returns>
            public Line2D GetSuperpositionPart()
            {
                if (Relationship == TwoParallelLinesRelationship.两条线段部分搭接)
                {
                    if (Line1.Start.IsOnLine(Line2) && Line2.Start.IsOnLine(Line1))
                        return Line2D.Create(Line2.Start, Line1.Start);
                    if (Line1.Start.IsOnLine(Line2) && Line2.End.IsOnLine(Line1))
                        return Line2D.Create(Line2.End, Line1.Start);
                    if (Line1.End.IsOnLine(Line2) && Line2.End.IsOnLine(Line1))
                        return Line2D.Create(Line2.End, Line1.End);
                    return Line2D.Create(Line2.Start, Line1.End);
                }
                if (Relationship == TwoParallelLinesRelationship.线段1全在线段2内)
                    return Line1;
                if (Relationship == TwoParallelLinesRelationship.线段2全在线段1内)
                    return Line2;
                if (Relationship == TwoParallelLinesRelationship.完全相同)
                    return Line2;
                return null;
            }

            private void DetermineRelationship()
            {
                if (!Line1.IsParallelWith(Line2))
                    Relationship = TwoParallelLinesRelationship.不平行;
                else if (!Line1.IsCollinearWith(Line2))
                    Relationship = TwoParallelLinesRelationship.平行不共线;
                else
                {
                    double distanceMax = TwoLinesEndPointsBiggestDistance();
                    double distanceSum = TwoLinesLengthSum();
                    if (distanceMax - distanceSum > Extension.SMALL_NUMBER)
                        Relationship = TwoParallelLinesRelationship.两条线段共线但是无重合部分;
                    else if (Math.Abs(distanceMax - distanceSum) < Extension.SMALL_NUMBER)
                        Relationship = TwoParallelLinesRelationship.两条线段端点搭接;
                    else if (distanceSum - distanceMax > Extension.SMALL_NUMBER)
                    {
                        if (distanceMax - Line1.Length > Extension.SMALL_NUMBER && distanceMax - Line2.Length > Extension.SMALL_NUMBER)
                            Relationship = TwoParallelLinesRelationship.两条线段部分搭接;
                        else if (Line1.Length - Line2.Length > Extension.SMALL_NUMBER)
                            Relationship = TwoParallelLinesRelationship.线段2全在线段1内;
                        else if (Line2.Length - Line1.Length > Extension.SMALL_NUMBER)
                            Relationship = TwoParallelLinesRelationship.线段1全在线段2内;
                        else
                            Relationship = TwoParallelLinesRelationship.完全相同;
                    }
                }
            }

            private double TwoLinesEndPointsBiggestDistance()
            {
                List<double> distances = new List<double>();
                distances.Add(Line1.Start.Distance(Line2.Start));
                distances.Add(Line1.Start.Distance(Line2.End));
                distances.Add(Line1.End.Distance(Line2.Start));
                distances.Add(Line1.End.Distance(Line2.End));
                distances.Add(Line1.Length);
                distances.Add(Line2.Length);
                return distances.OrderByDescending(x => x).First();
            }

            private double TwoLinesLengthSum()
            {
                return Line1.Length + Line2.Length;
            }
        }
        public class TwoParallelLines3D
        {
            public Line3D Line1 { get; set; }
            public Line3D Line2 { get; set; }
            public TwoParallelLinesRelationship Relationship { get; set; }
            public TwoParallelLines3D(Line3D line1, Line3D line2)
            {
                Line1 = line1;
                Line2 = line2;
                DetermineRelationship();
            }

            /// <summary>
            /// 得到重复部分的线段
            /// </summary>
            /// <returns></returns>
            public Line3D GetSuperpositionPart()
            {
                if (Relationship == TwoParallelLinesRelationship.两条线段部分搭接)
                {
                    if (Line1.Start.IsOnLine(Line2) && Line2.Start.IsOnLine(Line1))
                        return Line3D.Create(Line2.Start, Line1.Start);
                    if (Line1.Start.IsOnLine(Line2) && Line2.End.IsOnLine(Line1))
                        return Line3D.Create(Line2.End, Line1.Start);
                    if (Line1.End.IsOnLine(Line2) && Line2.End.IsOnLine(Line1))
                        return Line3D.Create(Line2.End, Line1.End);
                    return Line3D.Create(Line2.Start, Line1.End);
                }
                if (Relationship == TwoParallelLinesRelationship.线段1全在线段2内)
                    return Line1;
                if (Relationship == TwoParallelLinesRelationship.线段2全在线段1内)
                    return Line2;
                if (Relationship == TwoParallelLinesRelationship.完全相同)
                    return Line2;
                return null;
            }

            private void DetermineRelationship()
            {
                if (!Line1.IsParallelWith(Line2))
                    Relationship = TwoParallelLinesRelationship.不平行;
                else if (!Line1.IsCollinearWith(Line2))
                    Relationship = TwoParallelLinesRelationship.平行不共线;
                else
                {
                    double distanceMax = TwoLinesEndPointsBiggestDistance();
                    double distanceSum = TwoLinesLengthSum();
                    if (distanceMax - distanceSum > Extension.SMALL_NUMBER)
                        Relationship = TwoParallelLinesRelationship.两条线段共线但是无重合部分;
                    else if (Math.Abs(distanceMax - distanceSum) < Extension.SMALL_NUMBER)
                        Relationship = TwoParallelLinesRelationship.两条线段端点搭接;
                    else if (distanceSum - distanceMax > Extension.SMALL_NUMBER)
                    {
                        if (distanceMax - Line1.Length > Extension.SMALL_NUMBER && distanceMax - Line2.Length > Extension.SMALL_NUMBER)
                            Relationship = TwoParallelLinesRelationship.两条线段部分搭接;
                        else if (Line1.Length - Line2.Length > Extension.SMALL_NUMBER)
                            Relationship = TwoParallelLinesRelationship.线段2全在线段1内;
                        else if (Line2.Length - Line1.Length > Extension.SMALL_NUMBER)
                            Relationship = TwoParallelLinesRelationship.线段1全在线段2内;
                        else
                            Relationship = TwoParallelLinesRelationship.完全相同;
                    }
                }
            }

            private double TwoLinesEndPointsBiggestDistance()
            {
                List<double> distances = new List<double>();
                distances.Add(Line1.Start.Distance(Line2.Start));
                distances.Add(Line1.Start.Distance(Line2.End));
                distances.Add(Line1.End.Distance(Line2.Start));
                distances.Add(Line1.End.Distance(Line2.End));
                distances.Add(Line1.Length);
                distances.Add(Line2.Length);
                return distances.OrderByDescending(x => x).First();
            }

            private double TwoLinesLengthSum()
            {
                return Line1.Length + Line2.Length;
            }
        }
        public enum TwoParallelLinesRelationship
        {
            线段1全在线段2内 = 0,
            线段2全在线段1内 = 1,
            两条线段部分搭接 = 2,
            两条线段端点搭接 = 3,
            两条线段共线但是无重合部分 = 4,
            完全相同 = 5,
            不共线 = 6,
            平行不共线 = 7,
            不平行 = 8,
        }

        #region 多边线几何算法
        /// <summary>
        /// 去掉共线的线
        /// </summary>
        /// <param name="oldLines"></param>
        public static void EliminateCollinearLines(List<Line2D> oldLines)
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
        /// <summary>
        /// 去掉重合的线段
        /// </summary>
        /// <param name="oldLines"></param>
        public static void EliminateCoincideLines(List<Line2D> oldLines)
        {
            int k = 0;
            List<Line2D> removeLines = new List<Line2D>();
            for (int i = 0; i < oldLines.Count; i++)
            {
                k = 0;
                for (int j = i + 1; j < oldLines.Count; j++)
                {
                    TwoParallelLines2D twoLines = new TwoParallelLines2D(oldLines[i], oldLines[j]);
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
        /// <summary>
        /// 获得一个多边形中任意垂直的一组边
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static List<Vector2D> GetCoupleofBasicDirection(Polygon2D polygon)
        {
            List<Vector2D> directions = new List<Vector2D>();
            if (Math.Abs(polygon.Edges.First().Direction.Dot(polygon.Edges.Last().Direction)) < SMALL_NUMBER)
            {
                directions.Add(polygon.Edges.First().Direction);
                directions.Add(polygon.Edges.Last().Direction);
            }
            else
            {
                for (int i = 0; i < polygon.Edges.Count - 1; i++)
                {
                    if (Math.Abs(polygon.Edges[i].Direction.Dot(polygon.Edges[i + 1].Direction)) < SMALL_NUMBER)
                    {
                        directions.Add(polygon.Edges[i].Direction);
                        directions.Add(polygon.Edges[i + 1].Direction);
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

        /// <summary>
        /// 获取所有的点通过给定的直线
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="XVector"></param>
        /// <param name="YVector"></param>
        /// <returns></returns>
        public static List<Vector2D> GetAllPointOfGivenLineExtension(List<Line2D> lines, Vector2D XVector, Vector2D YVector)
        {
            List<Vector2D> recPoints = new List<Vector2D>();
            //获取直线所有不重复的端点
            var endPoints = lines.GetEndPoints();
            //获取
            List<Line2D> tempLines = new List<Line2D>();

            foreach (var point in endPoints)
            {
                Line2D line1 = point.CreateLine(XVector);
                Line2D line2 = point.CreateLine(YVector);
                tempLines.Add(line1);
                tempLines.Add(line2);
            }

            GraphicAlgorithm.EliminateCollinearLines(tempLines);

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
            return recPoints;
        }

        /// <summary>
        /// 对线段进行前后排序
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="point"></param>
        /// <param name="sortedLines"></param>
        public static void HuntCurveByStartPoint(List<Line2D> lines, Vector2D point, List<Line2D> sortedLines)
        {
            Line2D startLine = null;
            Line2D nextLine = null;
            foreach (Line2D line in lines)
            {
                if (line.Start.IsAlmostEqualTo(point))
                {
                    startLine = line;
                    nextLine = line;
                    break;
                }
                if (line.End.IsAlmostEqualTo(point))
                {
                    startLine = line;
                    nextLine = new Line2D(line.End, line.Start);
                    break;
                }
            }
            //获取以point为起点或者终点的线
            if (nextLine != null)
            {
                sortedLines.Add(nextLine);
                lines.Remove(startLine);
                HuntCurveByStartPoint(lines, nextLine.End, sortedLines);
            }
        }


        #region Line3D的几何算法

        /// <summary>
        /// 对线段进行前后排序
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="point"></param>
        /// <param name="sortedLines"></param>
        public static void HuntCurveByStartPoint(List<Line3D> lines, Vector3D point, List<Line3D> sortedLines)
        {
            Line3D startLine = null;
            Line3D nextLine = null;
            foreach (Line3D line in lines)
            {
                if (line.Start.IsAlmostEqualTo(point))
                {
                    startLine = line;
                    nextLine = line;
                    break;
                }
                if (line.End.IsAlmostEqualTo(point))
                {
                    startLine = line;
                    nextLine = new Line3D(line.End, line.Start);
                    break;
                }
            }
            //获取以point为起点或者终点的线
            if (nextLine != null)
            {
                sortedLines.Add(nextLine);
                lines.Remove(startLine);
                HuntCurveByStartPoint(lines, nextLine.End, sortedLines);
            }
        }

        /// <summary>
        /// 对同一直线上首尾相连的线段进行融合
        /// </summary>
        /// <param name="originalSortedLines">线是首尾相连的</param>
        /// <returns></returns>
        public static List<Line3D> MergeLines(List<Line3D> originalSortedLines)
        {
            originalSortedLines = originalSortedLines.SortLinesContinuously();
            List<Line3D> sortLines = new List<Line3D>();
            HuntCurveByStartPoint(originalSortedLines, originalSortedLines[0].Start, sortLines);
            MergeOneLines(sortLines);
            return sortLines;

        }

        public static List<Line2D> MergeLines(List<Line2D> originalSortedLines)
        {
            List<Line2D> sortLines = new List<Line2D>();
            HuntCurveByStartPoint(originalSortedLines, originalSortedLines[0].Start, sortLines);
            MergeOneLines(sortLines);
            return sortLines;

        }

        /// <summary>
        /// 融合一直线上首尾相连的线段进行融合，但是要排除指定的点不融合
        /// </summary>
        /// <param name="originalSortedLines"></param>
        /// <param name="withoutpoint"></param>
        /// <returns></returns>
        public static List<Line3D> MergeLinesWithoutpoints(List<Line3D> originalSortedLines, List<Vector3D> withoutpoints)
        {
            List<Line3D> sortLines = new List<Line3D>();
            HuntCurveByStartPoint(originalSortedLines, originalSortedLines[0].Start, sortLines);
            MergeOneLines(sortLines, withoutpoints);
            return sortLines;
        }

        /// <summary>
        /// 融合一直线上首尾相连的线段进行融合，但是要排除指定的点不融合
        /// </summary>
        /// <param name="originalSortedLines"></param>
        /// <param name="withoutpoint"></param>
        /// <returns></returns>
        public static List<Line2D> MergeLinesWithoutpoints(List<Line2D> originalSortedLines, List<Vector2D> withoutpoints)
        {
            List<Line2D> sortLines = new List<Line2D>();
            HuntCurveByStartPoint(originalSortedLines, originalSortedLines[0].Start, sortLines);
            MergeOneLines(sortLines, withoutpoints);
            return sortLines;
        }

        /// <summary>
        /// 合并在一条线段上的所有线段
        /// </summary>
        /// <param name="sortLines"></param>
        private static void MergeOneLines(List<Line3D> sortLines)
        {
            for (int i = 0, j = sortLines.Count - 1; i < sortLines.Count; j = i++)
            {
                Line3D pre = sortLines[j];
                Line3D next = sortLines[i];
                if (pre.Direction.IsAlmostEqualTo(next.Direction) && pre.End.IsAlmostEqualTo(next.Start))
                {
                    Line3D newLine = Line3D.Create(pre.Start, next.End);
                    sortLines.Insert(i, newLine);
                    sortLines.Remove(pre);
                    sortLines.Remove(next);
                    MergeOneLines(sortLines);
                    break;
                }
            }

        }

        private static void MergeOneLines(List<Line2D> sortLines)
        {
            for (int i = 0, j = sortLines.Count - 1; i < sortLines.Count; j = i++)
            {
                Line2D pre = sortLines[j];
                Line2D next = sortLines[i];
                if (pre.Direction.IsAlmostEqualTo(next.Direction) && pre.End.IsAlmostEqualTo(next.Start))
                {
                    Line2D newLine = Line2D.Create(pre.Start, next.End);
                    sortLines.Insert(i, newLine);
                    sortLines.Remove(pre);
                    sortLines.Remove(next);
                    MergeOneLines(sortLines);
                    break;
                }
            }
        }
        /// <summary>
        /// 合并在一条线段上的所有线段,除了指定的点
        /// </summary>
        /// <param name="sortLines"></param>
        /// <param name="withoutpoints"></param>
        private static void MergeOneLines(List<Line3D> sortLines, List<Vector3D> withoutpoints)
        {
            for (int i = 0, j = sortLines.Count - 1; i < sortLines.Count; j = i++)
            {
                Line3D pre = sortLines[j];
                Line3D next = sortLines[i];
                if (pre.Direction.IsAlmostEqualTo(next.Direction) && pre.End.IsAlmostEqualTo(next.Start) && withoutpoints.Find(x => x.IsAlmostEqualTo(pre.End)) == null)
                {
                    Line3D newLine = Line3D.Create(pre.Start, next.End);
                    sortLines.Insert(i, newLine);
                    sortLines.Remove(pre);
                    sortLines.Remove(next);
                    MergeOneLines(sortLines, withoutpoints);
                    break;
                }
            }

        }

        private static void MergeOneLines(List<Line2D> sortLines, List<Vector2D> withoutpoints)
        {
            for (int i = 0, j = sortLines.Count - 1; i < sortLines.Count; j = i++)
            {
                Line2D pre = sortLines[j];
                Line2D next = sortLines[i];
                if (pre.Direction.IsAlmostEqualTo(next.Direction) && pre.End.IsAlmostEqualTo(next.Start) && withoutpoints.Find(x => x.IsAlmostEqualTo(pre.End)) == null)
                {
                    Line2D newLine = Line2D.Create(pre.Start, next.End);
                    sortLines.Insert(i, newLine);
                    sortLines.Remove(pre);
                    sortLines.Remove(next);
                    MergeOneLines(sortLines, withoutpoints);
                    break;
                }
            }

        }
        /// <summary>
        /// 一个点是否在指定区域内部和线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool IsPointInOrOnLinesArea(Vector3D point, List<Line3D> lines)
        {
            //点在线上或者区域内部

            return IsPointInLinesArea(point, lines) || IsPointOnLines(point, lines);
        }

        /// <summary>
        /// 判断点是否在curves内部
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool IsPointInLinesArea(Vector3D point, List<Line3D> lines)
        {
            //射线法
            //引射线法。就是从该点出发引一条射线，看这条射线和所有边的交点数目。
            //如果有奇数个交点，则说明在内部，如果有偶数个交点，则说明在外部。
            //这是所有方法中计算量最小的方法，在光线追踪算法中有大量的应用。
            //注释：此方法相交于DotIsInCurveList更准确，初步判断drawing的精度不够
            int num = lines.Count;
            double[] arrayX = new double[num];
            double[] arrayY = new double[num];
            Vector3D tempXYZ = new Vector3D(0, 0, 0);
            for (int n = 0; n < lines.Count(); n++)
            {
                tempXYZ = lines[n].Start;
                arrayX[n] = tempXYZ.X;
                arrayY[n] = tempXYZ.Y;
            }
            double testx = point.X;
            double testy = point.Y;
            int i, j, crossings = 0;
            for (i = 0, j = num - 1; i < num; j = i++)
            {
                bool IsMiddle = ((arrayY[i] > testy) != (arrayY[j] > testy));
                //将该点的y值带入，判断是否方程得到的x是否大于该点的x
                bool IsRight = (testx < (arrayX[j] - arrayX[i]) * (testy - arrayY[i]) / (arrayY[j] - arrayY[i]) + arrayX[i]);
                if (IsMiddle && IsRight)
                    crossings++;
            }
            return (crossings % 2 != 0) && !IsPointOnLines(point, lines);//排除在线段中和线段端点上的情况
        }

        /// <summary>
        /// 判断点是否在多条线的某个线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool IsPointOnLines(Vector3D point, List<Line3D> lines)
        {
            //判断一个点是否在一组curve上
            foreach (var item in lines)
            {
                if (point.IsOnLine(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断点point是否为curve的端点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        private static bool IsAtCurveEnd(Vector3D point, Line3D curve)
        {
            if (point.IsAlmostEqualTo(curve.Start) ||
                point.IsAlmostEqualTo(curve.End))
                return true;
            else
                return false;

        }



        #endregion


        public static void HuntCurveByStartPoint(List<Line2D> lines, Vector2D point, List<Line2D> sortedLines, double radius)
        {
            Line2D startLine = null;
            Line2D nextLine = null;
            foreach (Line2D line in lines)
            {
                if (line.Start.IsAlmostEqualTo(point, radius))
                {
                    startLine = line;
                    nextLine = line;
                    break;
                }
                if (line.End.IsAlmostEqualTo(point, radius))
                {
                    startLine = line;
                    nextLine = new Line2D(line.End, line.Start);
                    break;
                }
            }
            //获取以point为起点或者终点的线
            if (nextLine != null)
            {
                sortedLines.Add(nextLine);
                lines.Remove(startLine);
                HuntCurveByStartPoint(lines, nextLine.End, sortedLines, radius);
            }
        }

        public static Rectangle2D Rebuild(List<Rectangle2D> targets)
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

        public static bool IsRectangleEdge(Line2D source, Rectangle2D rec)
        {
            if (IsRectangleEndPoint(source, rec) && IsRectangleWidth(source, rec))
                return true;
            return false;
        }

        public static bool IsRectangleWidth(Line2D source, Rectangle2D rec)
        {
            return ((source.Length - rec.Top.Length).AreEqual(0) || (source.Length - rec.Left.Length).AreEqual(0));
        }

        public static bool IsRectangleEndPoint(Line2D source, Rectangle2D rec)
        {
            List<Vector2D> newPoints = new List<Vector2D>();
            List<Vector2D> points = new List<Vector2D> { rec.Top.Start, rec.Right.Start, rec.Bottom.Start, rec.Left.Start };
            points.ForEach(x => { if (x.IsAlmostEqualTo(source.Start) || x.IsAlmostEqualTo(source.End)) newPoints.Add(x); });
            return newPoints.Count >= 2;
        }

        /// <summary>
        /// 融合一条直线上的线段，符合精度为float的线的融合
        /// </summary>
        /// <param name="oldLines"></param>
        public static void MergeLine(List<Line2D> oldLines)
        {
            int k = 0;
            Line2D newLine = null;
            List<Line2D> removeLines = new List<Line2D>();
            for (int i = 0; i < oldLines.Count; i++)
            {
                removeLines.Clear();
                k = 0;
                for (int j = i + 1; j < oldLines.Count; j++)
                {
                    if (IsTwoLineSubset(oldLines[i], oldLines[j]))
                    {
                        k++;
                        newLine = MergeTwoLine(oldLines[i], oldLines[j]);
                        removeLines.Add(oldLines[i]);
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
                oldLines.Add(newLine);
                MergeLine(oldLines);
            }
        }

        /// <summary>
        /// 给定比较的小数位数， 融合一条直线上的线段
        /// </summary>
        /// <param name="oldLines"></param>
        /// <param name="tolerance">比较的小数位数</param>
        public static void MergeLine(List<Line2D> oldLines, int tolerance)
        {
            int k = 0;
            Line2D newLine = null;
            List<Line2D> removeLines = new List<Line2D>();
            for (int i = 0; i < oldLines.Count; i++)
            {
                removeLines.Clear();
                k = 0;
                for (int j = i + 1; j < oldLines.Count; j++)
                {
                    if (IsTwoLineSubset(oldLines[i], oldLines[j]))
                    {
                        k++;
                       // newLine = MergeTwoLine(oldLines[i], oldLines[j], tolerance);
                        newLine = MergeTwoLine2(oldLines[i], oldLines[j]); 
                        removeLines.Add(oldLines[i]);
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
                oldLines.Add(newLine);
                MergeLine(oldLines, tolerance);
            }
        }

        /// <summary>
        /// 是否是结点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static bool IsEndPoint(Vector3D point, Line3D line)
        {
            return point.IsAlmostEqualTo(line.Start, Extension.SMALL_NUMBER) |
                   point.IsAlmostEqualTo(line.End, Extension.SMALL_NUMBER);
        }



        public static double GlobalToLocalTransformRotationAngle(Vector2D baseDirection)
        {
            Vector2D floorLocalCoord = baseDirection;
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
            return xiTa;
        }

        public static Vector2D CoordTransform(Vector2D point, double angle)
        {
            return new Vector2D(point.X * Math.Cos(angle) - point.Y * Math.Sin(angle),
                        point.X * Math.Sin(angle) + point.Y * Math.Cos(angle));
        }
        #endregion
        #region 融合线的实现
        /// <summary>
        /// 适用于精度为float的线的融合（1e-6）
        /// </summary>
        /// <param name="line2D1"></param>
        /// <param name="line2D2"></param>
        /// <returns></returns>
        private static Line2D MergeTwoLine(Line2D line2D1, Line2D line2D2)
        {
            List<Vector2D> points = new List<Vector2D> { line2D1.Start, line2D1.End, line2D2.Start, line2D2.End };
            points.ForEach(x => x.X = Math.Floor(x.X * 1e6) / 1e6);
            points.ForEach(x => x.Y = Math.Floor(x.Y * 1e6) / 1e6);
            points = points.OrderBy(x => x.X).ThenBy(y => y.Y).ToList();
            return new Line2D(points.First(), points.Last());
        }

        /// <summary>
        /// 适用于指定精度为float的线的融合
        /// </summary>
        /// <param name="line2D1"></param>
        /// <param name="line2D2"></param>
        /// <param name="tolerance">需要保留的小数位数</param>
        /// <returns></returns>
        private static Line2D MergeTwoLine(Line2D line2D1, Line2D line2D2, int tolerance)
        {
            List<Vector2D> points = new List<Vector2D> { line2D1.Start, line2D1.End, line2D2.Start, line2D2.End };
            points.ForEach(x => x.X = Math.Floor(x.X * Math.Pow(10, tolerance)) / Math.Pow(10, tolerance));
            points.ForEach(x => x.Y = Math.Floor(x.Y * Math.Pow(10, tolerance)) / Math.Pow(10, tolerance));
            points = points.OrderBy(x => x.X).ThenBy(y => y.Y).ToList();
            return new Line2D(points.First(), points.Last());
        }

        /// <summary>
        ///线的融合
        /// </summary>
        /// <param name="line2D1"></param>
        /// <param name="line2D2"></param>
        /// <param name="tolerance">需要保留的小数位数</param>
        /// <returns></returns>
        private static Line2D MergeTwoLine2(Line2D line2D1, Line2D line2D2)
        {
            //两条线在同一条直线上，并且端点或者部分线段重合
            double distance = line2D1.Start.Distance(line2D2.Start);
            Line2D newLine = new Line2D(line2D1.Start, line2D2.Start);
            if (distance < line2D1.Start.Distance(line2D2.End))
            {
                distance = line2D1.Start.Distance(line2D2.End);
                newLine = new Line2D(line2D1.Start, line2D2.End);
            }
            if (distance < line2D1.End.Distance(line2D2.End))
            {
                distance = line2D1.End.Distance(line2D2.End);
                newLine = new Line2D(line2D1.End, line2D2.End);
            }
            if (distance < line2D1.End.Distance(line2D2.Start))
            {
                distance = line2D1.End.Distance(line2D2.Start);
                newLine = new Line2D(line2D1.End, line2D2.Start);
            }
            return newLine;
        }

        private static bool IsTwoLineSubset(Line2D line1, Line2D line2)
        {
            if (line1.Direction.IsParallelWith(line2.Direction))
            {
                Line2D line11 = line1;
                line1 = line1.Extend(1e9, 1e9);
                return line1.Distance(line2.Start).AreEqual(0) && IsMaxLengthLessTwoLineLength(line11, line2);
            }
            return false;
        }


        private static bool IsMaxLengthLessTwoLineLength(Line2D line1, Line2D line2)
        {
            List<double> distances = new List<double>
            { line1.Start.Distance(line2.Start), line1.Start.Distance(line2.End),
                line1.End.Distance(line2.Start), line1.End.Distance(line2.End), line1.Length, line2.Length };
            double distance = distances.OrderByDescending(x => x).First();
            //return distance - (line1.Length + line2.Length) <= SMALL_NUMBER;
            TwoParallelLines2D two = new TwoParallelLines2D(line1, line2);
            if (two.Relationship == TwoParallelLinesRelationship.两条线段部分搭接 ||
                two.Relationship == TwoParallelLinesRelationship.两条线段端点搭接)
                return true;
            else if ((line1.Length + line2.Length) - distance >= 0 && (line1.Length + line2.Length) - distance < 1e-6)
                return true;
            return false;
        }
        #endregion
        /// <summary>
        /// 按line的长度从大到小排列
        /// </summary>
        /// <param name="leftLines"></param>
        public static void OrderLinesByLength(List<Line2D> leftLines)
        {
            Line2D temp = null;
            for (int i = 0; i < leftLines.Count - 1; i++)
            {
                for (int j = i + 1; j < leftLines.Count; j++)
                {
                    if (leftLines[i].Length < leftLines[j].Length)
                    {
                        temp = leftLines[j];
                        leftLines[j] = leftLines[i];
                        leftLines[i] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// 用于对长的墙体进行打断
        /// </summary>
        /// <param name="partitionLength"></param>
        /// <param name="expandCoef"></param>
        /// <param name="seperations"></param>
        /// <param name="firstPartitionable"></param>
        /// <returns></returns>
        public static List<double> Partition(double partitionLength, double expandCoef, List<double> seperations, bool firstPartitionable)
        {
            PartitionAlgorithm partition = new PartitionAlgorithm(partitionLength, expandCoef);
            return partition.Partition(seperations, firstPartitionable);
        }

        /// <summary>
        /// 剔除存在没有共享端点的线，这个线有一个端点不和任何其他线连接（只考虑所有打断的线）
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line2D> Weed(List<Line2D> lines)
        {
            WeedIndependentAlgorithm weedIndependentPoint = new WeedIndependentAlgorithm();
            return weedIndependentPoint.Weed(lines);
        }
        /// <summary>
        /// 剔除存在没有共享端点的线，这个线有一个端点不和任何其他线连接（只考虑所有打断的线）
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line3D> Weed(List<Line3D> lines)
        {
            WeedIndependentAlgorithm weedIndependentPoint = new WeedIndependentAlgorithm();
            return weedIndependentPoint.Weed(lines);
        }

        /// <summary>
        /// 获取线段中，不属于闭合区域的所有线段
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line2D> WeedLess(List<Line2D> lines)
        {
            WeedIndependentAlgorithm weedIndependentPoint = new WeedIndependentAlgorithm();
            return weedIndependentPoint.WeedLess(lines);
        }

        /// <summary>
        /// 获取线段中，不属于闭合区域的所有线段
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line3D> WeedLess(List<Line3D> lines)
        {
            WeedIndependentAlgorithm weedIndependentPoint = new WeedIndependentAlgorithm();
            return weedIndependentPoint.WeedLess(lines);
        }

        /// <summary>
        /// 查找一个闭合区域
        /// </summary>
        /// <param name="searchLines">需要查找的线段</param>
        /// <param name="largeOrSmall">是查找最大区域，还是最小区域</param>
        /// <returns></returns>
        public static List<Line3D> FindClosedLines(List<Line3D> searchLines, bool isLargeRegion, bool isDecompose, Line3D orgin = null)
        {

            ClosedLinesAlgorithm closedLinesAlgorithm = new ClosedLinesAlgorithm(searchLines, isLargeRegion, isDecompose, orgin);

            return closedLinesAlgorithm.Find();
        }

        /// <summary>
        /// 查找一个闭合区域
        /// </summary>
        /// <param name="searchLines">需要查找的线段</param>
        /// <param name="largeOrSmall">是查找最大区域，还是最小区域</param>
        /// <returns></returns>
        public static List<Line2D> FindClosedLines(List<Line2D> searchLines, bool isLargeRegion, bool isDecompose, Line2D orgin = null)
        {

            ClosedLinesAlgorithm closedLinesAlgorithm = new ClosedLinesAlgorithm(searchLines, isLargeRegion, isDecompose, orgin);

            return closedLinesAlgorithm.Find2();
        }

        /// <summary>
        /// 获取与直线相交的所有线段,交点不包含后面2个点
        /// </summary>
        /// <param name="outLines"></param>
        /// <param name="unbound1"></param>
        /// <param name="wallStart"></param>
        /// <returns></returns>
        public static List<Vector3D> GetInjectionPoints(Line3D unbound, List<Line3D> lines, Vector3D v1 = null, Vector3D v2 = null)
        {
            List<Vector3D> injectionPoints = new List<Vector3D>();
            foreach (Line3D line in lines)
            {

                Vector3D point = unbound.IntersectStraightLine2(line);
                if (point != null && !point.IsAlmostEqualTo(v1) && !point.IsAlmostEqualTo(v2))//有交点且交点不是wallstart
                {
                    injectionPoints.Add(point);
                }
            }
            return injectionPoints;
        }

        /// <summary>
        /// 获取直线和面的交点
        /// </summary>
        /// <param name="unbound"></param>
        /// <param name="lines"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3D GetInjectionPoints(Line3D unbound, List<Line3D> lines)
        {
            //获取焦点
            Vector3D injectionPoint = new Vector3D();
            //获取面的向量
            Vector3D planeNormal = lines.GetNormal();
            if (planeNormal.Z > 0)
            {

                planeNormal = -planeNormal;
            }
            Vector3D point = unbound.End;
            Vector3D direct = unbound.Direction;
            Vector3D planePoint = lines[0].Start;
            //则当前点到面的距离
            double d = (planePoint - point).Dot(planeNormal) / direct.Dot(planeNormal);
            return point + direct * d;
        }

        /// <summary>
        /// 是否是平行关系
        /// </summary>
        /// <param name="l1"></param>
        /// <param name="l2"></param>
        /// <returns></returns>
        public static bool IsCollineation(Line3D l1, Line3D l2)
        {
            Vector3D xyz1 = l1.End - l1.Start;
            Vector3D xyz2 = l2.End - l2.Start;

            if (xyz1.AngleTo(xyz2) == 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 通过两条直线求平面方程
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <param name="D"></param>
        public static void CreatePlaneEquation(Line3D line1, Line3D line2, ref double A, ref double B, ref double C, ref double D)
        {
            //获得三个线性无关的点
            List<Vector3D> points = new List<Vector3D>() { line1.Start, line1.End, line2.Start, line2.End };
            points = points.Distinct(new Vector3DEqualityComparer()).ToList();

            //三个线性无关的点创建平面方程
            if (points.Count == 3)
            {
                CreatePlaneEquation(points, ref A, ref  B, ref  C, ref  D);
            }
            else { throw new Exception("只需三个线性无关的点就可创建平面方程，请去除多余的点"); }

        }

        /// <summary>
        /// 构建一般式平面方程：Ax+By+Cz+D=0
        /// </summary>
        /// <param name="points">三个线性无关的点</param>
        /// <param name="A">方程的系数</param>
        /// <param name="B">方程的系数</param>
        /// <param name="C">方程的系数</param>
        /// <param name="D">方程的系数</param>
        private static void CreatePlaneEquation(List<Vector3D> points, ref double A, ref double B, ref double C, ref double D)
        {
            if (points.Count != 3)
            {
                throw new Exception("只需三个线性无关的点就可创建平面方程，请去除多余的点");
            }

            //points[0]
            double x1 = points[0].X;
            double y1 = points[0].Y;
            double z1 = points[0].Z;
            //points[1]
            double x2 = points[1].X;
            double y2 = points[1].Y;
            double z2 = points[1].Z;
            //points[2]
            double x3 = points[2].X;
            double y3 = points[2].Y;
            double z3 = points[2].Z;

            A = (y2 - y1) * (z3 - z1) - (y3 - y1) * (z2 - z1);
            B = (z2 - z1) * (x3 - x1) - (z3 - z1) * (x2 - x1);
            C = (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);
            D = -(A * x1 + B * y1 + C * z1);

        }

        /// <summary>
        /// 创建平面方程
        /// </summary>
        /// <param name="layerOutLines"></param>
        /// <param name="A">方程的系数</param>
        /// <param name="B">方程的系数</param>
        /// <param name="C">方程的系数</param>
        /// <param name="D">方程的系数</param>
        public static void CreatePlaneEquation(List<Line3D> layerOutLines, ref double A, ref double B, ref double C, ref double D)
        {
            //从这四条线中取出不想交的直线
            Line3D line1 = null;
            Line3D line2 = null;
            foreach (Line3D linea in layerOutLines)
            {
                foreach (Line3D lineb in layerOutLines)
                {
                    Vector3D intersectPoint = linea.Intersect(lineb);
                    if (intersectPoint != null)
                    {
                        line1 = linea;
                        line2 = lineb;
                        break;
                    }
                }
                if (line1 != null && line2 != null)
                {
                    break;
                }
            }
            //两直线构建平面方程
            GraphicAlgorithm.CreatePlaneEquation(line1, line2, ref A, ref B, ref C, ref D);

        }

        /// <summary>
        /// 判断两个三维面是否平行
        /// </summary>
        /// <param name="lines1"></param>
        /// <param name="lines2"></param>
        /// <returns></returns>
        public static bool IsParallel(List<Line3D> lines, List<Line3D> compare)
        {
            Vector3D bodyNormal = lines.GetNormal();
            Vector3D compareNormal = compare.GetNormal();
            //判断当前的向量是否相同或者相反
            if (bodyNormal.IsAlmostEqualTo(compareNormal) || bodyNormal.IsAlmostEqualTo(-compareNormal))
            {
                //说明两个面是平行的
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断线和面是否平行
        /// </summary>
        public static bool IsParallel(List<Line3D> lines, Line3D unbound)
        {
            //获取直线的方向
            Vector3D V = unbound.Direction;

            double A = 0, B = 0, C = 0, D = 0;

            //创建平面方程
            CreatePlaneEquation(lines, ref A, ref B, ref C, ref D);

            //所有平面的法向量为
            Vector3D N = Vector3D.Create(A, B, C);

            double s = V.Dot(N);

            if (s.AreEqual(0.0))
            {

                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断线是否在面上面
        /// </summary>
        /// <param name="lines1"></param>
        /// <param name="lines2"></param>
        /// <returns></returns>
        public static bool IsCoplanarity(List<Line3D> lines, Line3D compare)
        {
            double A1 = 0, B1 = 0, C1 = 0, D1 = 0;
            CreatePlaneEquation(lines, ref A1, ref B1, ref C1, ref D1);

            Vector3D v1 = compare.Start;
            Vector3D v2 = compare.End;
            if ((v1.X * A1 + v1.Y * B1 + v1.Z * C1 + D1).AreEqual(0) && (v2.X * A1 + v2.Y * B1 + v2.Z * C1 + D1).AreEqual(0))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断两个多线段组成的面是否共面
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool IsCoplanarity(List<Line3D> lines1, List<Line3D> lines2)
        {

            if (IsParallel(lines1, lines2))
            {
                double A = 0, B = 0, C = 0, D = 0;
                CreatePlaneEquation(lines1, ref A, ref B, ref C, ref D);
                Vector3D v1 = lines2[0].Start;
                Vector3D v2 = lines2[lines2.Count - 1].End;
                //判断四个参数是否相等
                var distance1 = Math.Abs(A * v1.X + B * v1.Y + C * v1.Z + D) / Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2) + Math.Pow(C, 2));
                var distance2 = Math.Abs(A * v2.X + B * v2.Y + C * v2.Z + D) / Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2) + Math.Pow(C, 2));

                if (distance1.AreEqual(0) && distance2.AreEqual(0))
                {
                    return true;
                }
            }
            return false;

        }

        /// <summary>
        /// 判断一条线是否在多边形的内部
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static bool IsContainOtheRegion(List<Line3D> lines, Line3D compare)
        {
            //说明线和面共面
            if (IsCoplanarity(lines, compare))
            {
                //将平面投影到一个平面
                Vector3D yNormal = lines.GetNormal();

                //把法线定义为Z轴，任意取一条线为X轴
                Vector3D xNormal = lines[0].Direction.Normalize();

                //拷贝当前多线段
                List<Line3D> copyLines = lines.Copy();

                //获取拷贝线段
                Line3D copyCompare = compare.Copy();

                //进行坐标轴转换
                Vector3D zNormal = xNormal.Cross(yNormal).Normalize();
                //进行坐标转换
                CoordinateAlgorithm coordinateAlgorithm1 = new CoordinateAlgorithm(copyLines);
                //进行了坐标转换
                Matrix4 matrix = coordinateAlgorithm1.TransformXY();
                //对当前线段也进行坐标转换
                coordinateAlgorithm1.Transform(copyCompare, matrix);
                //转换为Line2D
                List<Line2D> line2ds = Line3ds2Line2ds(copyLines);
                //转化当前线段
                Line2D compareline2d = Line3D2Line2D(copyCompare);

                //判断返回结果
                if (compareline2d.IsInRegion(line2ds) || line2ds.Find(x => compareline2d.IsPartOf(x)) != null)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断后面的多边形是否在前面多边形的内部
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static bool IsContainOtheRegion(List<Line3D> lines, List<Line3D> compare)
        {
            //两个面共面
            if (IsParallel(lines, compare))
            {


                for (int i = 0; i < compare.Count; i++)
                {

                    if (IsContainOtheRegion(lines, compare[i]))
                    {
                        continue;
                    }
                    else
                    {

                        return false;
                    }
                }
                return true;
            }

            return false;
        }


        /// <summary>
        /// 在上下容差范围内，查找一个区域的水平投影，在另外一个区域的内部
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="compare"></param>
        /// <param name="toterance">正值代表向上容差，赋值代表向下容差</param>
        /// <returns></returns>
        public static bool IsContainOtheAllowanceRegion(List<Line3D> lines, List<Line3D> compare, double toterance)
        {
            //返回结果
            if (IsContainOtheAllowanceRegion(lines, compare))
            {
                if (Math.Abs(compare[0].Start.Z - lines[0].Start.Z).AreEqual(toterance))
                {

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 查找一个区域的水平投影，在另外一个区域的内部
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static bool IsContainOtheAllowanceRegion(List<Line3D> lines, List<Line3D> compare)
        {
            //第一步，将当前的所有的区域，投影到水平面
            List<Line3D> parent = new List<Line3D>();
            List<Line3D> children = new List<Line3D>();
            lines.ForEach(x =>
            {
                parent.Add(Line3D.Create(Vector3D.Create(x.Start.X, x.Start.Y, 0), Vector3D.Create(x.End.X, x.End.Y, 0)));
            });

            compare.ForEach(x =>
            {
                children.Add(Line3D.Create(Vector3D.Create(x.Start.X, x.Start.Y, 0), Vector3D.Create(x.End.X, x.End.Y, 0)));
            });
            //返回结果
            return IsContainOtheRegion(parent, children);
        }

        /// <summary>
        /// 查找一个区域的水平投影，在另外一个区域的内部
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static bool IsContainOtheAllowanceRegion(List<Line3D> lines, Line3D compare)
        {
            //第一步，将当前的所有的区域，投影到水平面
            List<Line3D> parent = new List<Line3D>();
            Line3D children = null;
            lines.ForEach(x =>
            {
                parent.Add(Line3D.Create(Vector3D.Create(x.Start.X, x.Start.Y, 0), Vector3D.Create(x.End.X, x.End.Y, 0)));
            });

            //获取小线段
            children = Line3D.Create(Vector3D.Create(compare.Start.X, compare.Start.Y, 0), Vector3D.Create(compare.End.X, compare.End.Y, 0));

            //返回结果
            return IsContainOtheRegion(parent, children);
        }
        /// <summary>
        /// 获取两个平行面之间的距离
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static double GetParallelRegionDistance(List<Line3D> lines, List<Line3D> compare)
        {

            //两个面共面
            if (IsParallel(lines, compare))
            {
                double A = 0, B = 0, C = 0, D = 0;
                CreatePlaneEquation(lines, ref A, ref B, ref C, ref D);

                //在面上任取一点
                Vector3D v = compare[0].Start;
                //点到面的垂直距离
                var distance = Math.Abs(A * v.X + B * v.Y + C * v.Z + D) / Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2) + Math.Pow(C, 2));

                return distance;
            }
            return -1;
        }
        /// <summary>
        /// 判断两个多边形是否有线段重合
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool IsCoincidenceWith(Line3D l1, Face f2)
        {

            foreach (Line3D l2 in f2.Edges)
            {

                if (l1.IsPartOf(l2) || l2.IsPartOf(l1) || l1.IsAlmostEqualTo(l2))
                {

                    return true;
                }

            }
            return false;

        }
        /// <summary>
        /// 判断当前墙线之间是否是阳角
        /// </summary>
        /// <param name="v"></param>
        /// <param name="sp"></param>
        /// <returns></returns>
        public static bool IsExternalCorner(Vector3D v, List<Line3D> spatialBoundary)
        {
            //线是逆时针方向的
            Line3D startLine = spatialBoundary.Find(x => x.End.IsAlmostEqualTo(v));
            Line3D endLine = spatialBoundary.Find(x => x.Start.IsAlmostEqualTo(v));

            //endLine逆时针到startLine的角度
            double angle = (endLine.Direction).AngleFrom(startLine.Direction);

            //假如在180度内，则说明是阳角
            if (angle < Math.PI)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取对称多边形中心点
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static Vector3D GetCentrality(List<Line3D> lines)
        {
            Vector3D center = null;
            List<double> cxs = new List<double>();
            List<double> cys = new List<double>();
            List<double> czs = new List<double>();
            lines.ForEach(x =>
            {

                cxs.Add(x.Start.X);
                cxs.Add(x.End.X);
                cys.Add(x.Start.Y);
                cys.Add(x.End.Y);
                czs.Add(x.Start.Z);
                czs.Add(x.End.Z);
            });
            center = new Vector3D(cxs.Average(), cys.Average(), czs.Average());
            return center;
        }

        /// <summary>
        /// 将二维的墙转换为自己定义的三维坐标系中
        /// </summary>
        /// <param name="line2D"></param>
        /// <param name="wallOrgin"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Line3D Line2D2Line3D(Line2D line2D, Vector3D wallOrgin, Vector3D dir)
        {
            return new Line3D(Point2D2Vector3D(line2D.Start, wallOrgin, dir), Point2D2Vector3D(line2D.End, wallOrgin, dir));
        }
        /// <summary>
        /// 将三维的墙转换为自己定义的二维坐标系中
        /// </summary>
        /// <param name="line3D"></param>
        /// <returns></returns>
        public static Line2D Line3D2Line2D(Line3D line3D)
        {
            return new Line2D(new Vector2D(line3D.Start.X, line3D.Start.Y), new Vector2D(line3D.End.X, line3D.End.Y));

        }


        /// <summary>
        /// 将三维图形转换为二维图形
        /// </summary>
        /// <param name="line3ds"></param>
        /// <returns></returns>
        public static List<Line2D> Line3ds2Line2ds(List<Line3D> line3ds)
        {

            List<Line2D> line2ds = new List<Line2D>();

            line3ds.ForEach(x =>
            {

                Line2D line2d = Line3D2Line2D(x);

                line2ds.Add(line2d);
            });
            return line2ds;
        }

        /// <summary>
        /// 将三维图形转换为二维图形
        /// </summary>
        /// <param name="line3ds"></param>
        /// <returns></returns>
        public static Polygon2D Line3ds2Polygon2D(List<Line3D> line3ds)
        {

            List<Line2D> line2ds = Line3ds2Line2ds(line3ds);

            return new Polygon2D(line2ds);
        }

        /// <summary>
        /// 将二维的墙转换为自己定义的三维坐标系中
        /// </summary>
        /// <param name="line"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Line3D Line2D2Line3D(Line2D line, double z)
        {
            return new Line3D(new Vector3D(line.Start.X, line.Start.Y, z), new Vector3D(line.End.X, line.End.Y, z));
        }
        /// <summary>
        /// 二维坐标点转换自定义的三维坐标点
        /// </summary>
        /// <param name="v"></param>
        /// <param name="wallOrgin"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static Vector3D Point2D2Vector3D(Vector2D v, Vector3D wallOrgin, Vector3D dir)
        {
            Vector3D NewVec = v.X * dir;
            NewVec.Z = NewVec.Z + v.Y;
            return wallOrgin + NewVec;
        }

        /// <summary>
        /// 获取任意多边形的编辑
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static double GetAreaofOutLines(List<Line3D> lines)
        {
            //找到最大封闭区域，也就是房间的区域
            List<Line3D> closeLines = FindClosedLines(lines, true, true);
            //对线段进行融合
            List<Line3D> mergeLines = MergeLines(closeLines);
            //计算任意多边形面积

            Polygon3D p3d = new Polygon3D(mergeLines);

            return p3d.Area;
        }


        /// <summary>
        /// 对指定的二维线段进行打断
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line2D> Decompose(List<Line2D> lines)
        {

            LinesDecomposeAlgorithm linesDecomposeAlgorithm = new LinesDecomposeAlgorithm();
            return linesDecomposeAlgorithm.Decompose(lines);
        }

        /// <summary>
        /// 对指定的三维线段进行打断
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line3D> Decompose(List<Line3D> lines)
        {

            LinesDecomposeAlgorithm linesDecomposeAlgorithm = new LinesDecomposeAlgorithm();
            return linesDecomposeAlgorithm.Decompose(lines);
        }

        ///// <summary>
        ///// 剔除存在没有共享端点的线，这个线有一个端点不和任何其他线连接（只考虑所有打断的线）
        ///// </summary>
        ///// <param name="lines"></param>
        ///// <returns></returns>
        //public static List<Line2D> Weed(List<Line2D> lines)
        //{
        //    WeedIndependentAlgorithm weedIndependentPoint = new WeedIndependentAlgorithm();
        //    return weedIndependentPoint.Weed(lines);
        //}
        ///// <summary>
        ///// 剔除存在没有共享端点的线，这个线有一个端点不和任何其他线连接（只考虑所有打断的线）
        ///// </summary>
        ///// <param name="lines"></param>
        ///// <returns></returns>
        //public static List<Line3D> Weed(List<Line3D> lines)
        //{
        //    WeedIndependentAlgorithm weedIndependentPoint = new WeedIndependentAlgorithm();
        //    return weedIndependentPoint.Weed(lines);
        //}

        /// <summary>
        /// 查找封闭区域
        /// </summary>
        /// <param name="searchLines"></param>
        /// <param name="isLargeRegion"></param>
        /// <param name="isDecompose"></param>
        /// <returns></returns>
        public static List<List<Line2D>> ClosedLookup(List<Line2D> searchLines, bool isLargeRegion, bool isDecompose = false)
        {
            ClosedLookupAlgorithm closedLookupAlgorithm = new ClosedLookupAlgorithm();
            return closedLookupAlgorithm.Lookup(searchLines, isLargeRegion, isDecompose);
        }

        public static List<List<Line3D>> ClosedLookup(List<Line3D> searchLines, bool isLargeRegion, bool isDecompose = false)
        {
            ClosedLookupAlgorithm closedLookupAlgorithm = new ClosedLookupAlgorithm();
            return closedLookupAlgorithm.Lookup(searchLines, isLargeRegion, isDecompose);
        }

    }
}
