using Tida.Geometry.Primitives;
using Tida.Geometry.External;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tida.Geometry.External
{
    public class FloorDividingAlgorithm
    {
        //楼板外侧轮廓
        private Polygon2D FloorRegion { get; set; }

        public FloorDividingAlgorithm(List<Line2D> floorOutLine)
        {
            FloorRegion = new Polygon2D(floorOutLine);
        }

        #region 生成小矩形
        public List<Rectangle2D> GetMinimunRectangle(List<Line2D> lines)
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
            return GetRectangle(recPoints);
        }
        private List<Rectangle2D> GetRectangle(List<Vector2D> points)
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

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="allLines">         //allLines是连续的线</param>
        public List<List<Line3D>> FindAllFloorOutLines(List<Line3D> allLines)
        {
            //  List<Line3D> resultDecomposedLinesTT = DecomposeLines(allLines);

            List<Line3D> resultDecomposedLines = new List<Line3D>();
            DecomposeCurves(allLines, resultDecomposedLines);


            //查找外框线
            List<Line3D> sortOutLines = GraphicAlgorithm.FindClosedLines(allLines, true, false);

            List<Line3D> decomposedOutlines = new List<Line3D>();
            foreach (var line in resultDecomposedLines)
            {
                if (IsLineContained(line, sortOutLines))
                    decomposedOutlines.Add(line);
            }

            decomposedOutlines = decomposedOutlines.SortLinesByCounterClockwise(Vector3D.BasisZ);


            List<List<Line3D>> curveArrarys = new List<List<Line3D>>();
            List<Line3D> usedLines = new List<Line3D>();
            FindLineArrarys(resultDecomposedLines, decomposedOutlines, usedLines, curveArrarys);

            return curveArrarys;
        }

        /// <summary>
        /// 将Curve用交点打断，分解成最小的单元
        /// </summary>
        /// <param name="locationLines"></param>
        /// <returns>返回分解后的curve</returns>
        public void DecomposeCurves(List<Line3D> regionEdges, List<Line3D> decomposLines)
        {

            foreach (Line3D pw1 in regionEdges)
            {
                //获取一条线
                List<Vector3D> intersects = new List<Vector3D>();
                //和所有的线相交计算，获取交点的位置
                foreach (Line3D pw2 in regionEdges)
                {
                    Vector3D intersect = null;
                    intersect = pw1.Intersect(pw2);
                    if (intersect != null)
                    {
                        if (!IsAtCurveEnd(intersect, pw1))
                        {
                            intersects.Add(intersect);
                        }

                    }
                }
                if (intersects.Count > 0)
                {
                    //获取所有的交点，对交点进行排序
                    intersects.Add(pw1.Start);
                    intersects.Add(pw1.End);
                    intersects = intersects.OrderBy(t => t.Distance(pw1.Start)).ToList();
                    intersects = intersects.Distinct().ToList();
                    for (int i = 1; i < intersects.Count; i++)
                    {
                        Line3D baseLine = Line3D.Create(intersects[i - 1], intersects[i]);

                        decomposLines.Add(baseLine);
                    }
                }
                else
                {
                    //不和任何线中心相交，则添加自己
                    decomposLines.Add(pw1);
                }
            }
        }


        /// <summary>
        /// 判断点xyz是否为curve的端点
        /// </summary>
        /// <param name="xyz"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        private bool IsAtCurveEnd(Vector3D v, Line3D line)
        {
            if (v.IsAlmostEqualTo(line.Start) || v.IsAlmostEqualTo(line.End))
                return true;
            else
                return false;
        }

        private List<Line3D> DecomposeLines(List<Line3D> allLines)
        {
            //获得打断后的楼板线
            List<Line3D> resultDecomposedLines = new List<Line3D>();
            //将所有的线段投影到XYZ平面
            List<Line2D> line2ds = new List<Line2D>();
            //获取线段的标高信息
            double Elevation = allLines.First().Start.Z;

            //首先投影所有线段
            allLines.ForEach(x =>
            {
                if (!x.Start.Z.AreEqual(Elevation) || !x.End.Z.AreEqual(Elevation))
                {
                    resultDecomposedLines = null;
                }
                Vector2D start = new Vector2D(x.Start.X, x.Start.Y);
                Vector2D end = new Vector2D(x.End.X, x.End.Y);
                Line2D line2d = Line2D.Create(start, end);
                line2ds.Add(line2d);

            });

            if (resultDecomposedLines == null)
            {
                //说明直线高度不一致
                throw new Exception("给的线段高度不一致");
            }

            //对当前的线段进行打断
            List<Line2D> remainingLines = line2ds.Decompose();

            remainingLines.ForEach(x =>
            {
                Line3D line3d = new Line3D(Vector3D.Create(x.Start.X, x.Start.Y, Elevation), Vector3D.Create(x.End.X, x.End.Y, Elevation));
                resultDecomposedLines.Add(line3d);
            });

            return resultDecomposedLines;
        }

        private void FindLineArrarys(List<Line3D> resultDecomposedLines, List<Line3D> sortOutLines, List<Line3D> usedLines, List<List<Line3D>> curveArrarys)
        {

            //首先要剔除那些线的两端和其他线不是相连的线段

            //查找一个任意起点
            Line3D startLine = null;
            if (sortOutLines.Count > 0)
            {
                startLine = sortOutLines[0];
            }

            //需要移除的墙体
            List<Line3D> remainingLines = new List<Line3D>();
            remainingLines.AddRange(resultDecomposedLines);



            //查找最小封闭区域
            List<Line3D> Lines = GraphicAlgorithm.FindClosedLines(resultDecomposedLines, false, false, startLine);
            curveArrarys.Add(Lines);

            foreach (var line in Lines)
            {
                //包含在外框线中
                bool isContained = IsLineContained(line, sortOutLines);
                //包含在内框线中，并且已经被使用过
                bool isUsed = IsLineContained(line, usedLines);

                if (isContained || isUsed)
                {
                    Line3D targetLine = FindtargetLine(line, remainingLines);
                    if (targetLine != null)
                        remainingLines.Remove(targetLine);
                }
                else
                {
                    usedLines.Add(line);
                }

                #region MyRegion
                //if (isContained)
                //{
                //    Line3D targetLine = FindtargetLine(line, remainingLines);
                //    if (targetLine != null)
                //        remainingLines.Remove(targetLine);
                //}
                ////添加到使用的线中
                //else
                //{

                //    if (isUsed)
                //    {
                //        Line3D targetLine = FindtargetLine(line, remainingLines);
                //        if (targetLine != null)
                //            remainingLines.Remove(targetLine);
                //    }
                //} 
                #endregion
            }

            if (remainingLines.Count > 0)
            {
                FindLineArrarys(remainingLines, sortOutLines, usedLines, curveArrarys);
            }
            else
                return;
        }

        private bool IsLineContained(Line3D sourceLine, List<Line3D> lines)
        {

            foreach (var line in lines)
            {
                //if (line.IsAlmostEqualTo(sourceLine))
                //    return true;
                if (sourceLine.Start.IsOnLine(line) && sourceLine.End.IsOnLine(line))
                    return true;
            }
            return false;
        }

        private Line3D FindtargetLine(Line3D sourceLine, List<Line3D> lines)
        {

            foreach (var line in lines)
            {
                if (line.IsAlmostEqualTo(sourceLine))
                    return line;
            }
            return null;
        }

    }
}
