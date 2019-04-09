using Tida.Geometry.Primitives;
using Tida.Geometry.External;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Tida.Geometry.External
{
    internal class ClosedLinesAlgorithm
    {


        /// <summary>
        /// 原始的线段
        /// </summary>
        public List<Line3D> SearchLines
        {

            private set;
            get;
        }

        /// <summary>
        /// 查找的区域是最大区域
        /// </summary>
        public bool IsLargeRegion
        {
            private set;
            get;
        }

        /// <summary>
        /// 查找的起点
        /// </summary>
        public Line3D OrginLine
        {
            private set;
            get;
        }

        /// <summary>
        /// 当前线段是否允许打断
        /// </summary>
        public bool IsDecompose
        {

            private set;
            get;
        }
        /// <summary>
        /// 当前线段的高度信息
        /// </summary>
        public double Elevation
        {

            private set;
            get;
        }
        /// <summary>
        /// 构造函数,此方法之能处理同一个平面的所有线段
        /// </summary>
        /// <param name="searchLines"></param>
        /// <param name="isLargeRegion"></param>
        public ClosedLinesAlgorithm(List<Line3D> searchLines, bool isLargeRegion, bool isDecompose = true, Line3D orginLine = null)
        {
            //要选择的选段
            this.SearchLines = searchLines;

            this.IsLargeRegion = isLargeRegion;

            this.IsDecompose = isDecompose;

            if (orginLine != null)
            {
                //起点位置
                this.OrginLine = orginLine;
                //起点高度
                this.Elevation = OrginLine.Start.Z;
            }
        }

        /// <summary>
        /// 开始查找
        /// </summary>
        /// <returns></returns>
        public List<Line3D> Find()
        {

            List<Line3D> result = new List<Line3D>();


            //将所有的线段投影到XYZ平面
            List<Line2D> line2ds = new List<Line2D>();
            //获取线段的标高信息
            this.Elevation = SearchLines.First().Start.Z;
            //首先投影所有线段
            SearchLines.ForEach(x =>
            {
                if (!x.Start.Z.AreEqual(this.Elevation) || !x.End.Z.AreEqual(this.Elevation))
                {
                    result = null;
                }
                Vector2D start = new Vector2D(x.Start.X, x.Start.Y);
                Vector2D end = new Vector2D(x.End.X, x.End.Y);
                Line2D line2d = Line2D.Create(start, end);
                line2ds.Add(line2d);

            });

     

            if (result == null)
            {
                //说明直线高度不一致
                throw new Exception("给的线段高度不一致");
            }


            List<Line2D> remainingLines = null;

            if (IsDecompose)
            {
                //对当前的线段进行打断
                remainingLines = line2ds.Decompose();
            }
            else
            {

                remainingLines = new List<Line2D>(line2ds);
            }


            //首先要移除独立的线，不和其他线供端点，或者只公用一个端点



            //获取最近点坐标
            Vector2D origin = null;

            //返回所有最靠近原点的店
            List<Line2D> huntLines = this.GetHuntWithOrigin(out origin, remainingLines);

            //所有的外框线
            List<Line2D> outerLines = new List<Line2D>();

            //获取起点的墙体
            this.FindOuterLinessWithAngle(remainingLines, huntLines, outerLines, origin);


            if (outerLines != null)
            {

                outerLines.ForEach(x =>
                {
                    Line3D line3d = new Line3D(Vector3D.Create(x.Start.X, x.Start.Y, Elevation), Vector3D.Create(x.End.X, x.End.Y, Elevation));
                    result.Add(line3d);

                });
            }

            return result;

        }
        #region 二维

        public List<Line2D> SearchLines2
        {

            private set;
            get;
        }

        /// <summary>
        /// 查找的起点
        /// </summary>
        public Line2D OrginLine2
        {
            private set;
            get;
        }


        public ClosedLinesAlgorithm(List<Line2D> searchLines, bool isLargeRegion, bool isDecompose = true, Line2D orginLine = null)
        {
            //要选择的选段
            this.SearchLines2 = searchLines;
            this.Elevation = 0;
            this.IsLargeRegion = isLargeRegion;
            this.IsDecompose = isDecompose;

            if (orginLine != null)
            {
                //起点位置
                this.OrginLine2 = orginLine;
            }
        }
        public List<Line2D> Find2()
        {
            List<Line2D> line2ds = SearchLines2;

            List<Line2D> result = new List<Line2D>();
            if (result == null)
            {
                //说明直线高度不一致
                throw new Exception("给的线段高度不一致");
            }


            List<Line2D> remainingLines = null;

            if (IsDecompose)
            {
                //对当前的线段进行打断
                remainingLines = line2ds.Decompose();
            }
            else
            {

                remainingLines = new List<Line2D>(line2ds);
            }


            //首先要移除独立的线，不和其他线供端点，或者只公用一个端点



            //获取最近点坐标
            Vector2D origin = null;

            //返回所有最靠近原点的店
            List<Line2D> huntLines = this.GetHuntWithOrigin(out origin, remainingLines);

            //所有的外框线
            List<Line2D> outerLines = new List<Line2D>();

            //获取起点的墙体
            this.FindOuterLinessWithAngle(remainingLines, huntLines, outerLines, origin);


            if (outerLines != null)
            {

                outerLines.ForEach(x =>
                {
                    Line2D line2d = new Line2D(Vector2D.Create(x.Start.X, x.Start.Y), Vector2D.Create(x.End.X, x.End.Y));
                    result.Add(line2d);

                });
            }

            return result;
        } 
        #endregion


        /// <summary>
        /// 获取夹角最大的墙体
        /// </summary>
        /// <param name="huntWalls"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        private void FindOuterLinessWithAngle(List<Line2D> remainingLines, List<Line2D> huntLines, List<Line2D> outerLines, Vector2D origin, Line2D lastLine = null)
        {

            List<Line2D> dicLines = new List<Line2D>();

            foreach (var item in huntLines)
            {
                Line2D tempLine = Line2D.Create(origin, OtherPointOfWall(item, origin));
                dicLines.Add(tempLine);
            }

            Line2D selectLine = null;

            if (lastLine == null)
            {

                //对key的角度进行降序排列，Key的Direction.Y越小，说明越水平
                selectLine = (from dic in dicLines
                              orderby Math.Asin(dic.Direction.Y)
                              select dic).FirstOrDefault();

            }
            else
            {
                if (IsLargeRegion)
                {
                              //对key值的内容参数进行降序排列
                    var d = from dic in dicLines orderby (-lastLine.Direction).AngleFrom(dic.Direction) descending select dic;
          
                    selectLine = d.FirstOrDefault();
                }
                else
                {
                    selectLine = (from dic in dicLines
                                  orderby (-lastLine.Direction).AngleFrom(dic.Direction) 
                                  select dic).FirstOrDefault();
                }

            }
            //找到的墙体是已经存在的墙体，说明已经形成闭合，退出当前函数
            if (!isExitLine(outerLines, selectLine))
            {

                Vector2D OtherPoint = this.OtherPointOfWall(selectLine, origin);
                List<Line2D> nhuntLiness = this.GetHuntLines(OtherPoint, remainingLines, selectLine);

                //没有形成闭合，但是已经找不到和当前点相关的墙体，说无法形成闭合
                if (nhuntLiness.Count > 0)
                {
                    outerLines.Add(selectLine);

                    this.FindOuterLinessWithAngle(remainingLines, nhuntLiness, outerLines, OtherPoint, selectLine);
                }
                else
                {
                    if (remainingLines.Count > 0)
                    {
                        this.RemoveExitLine(remainingLines, selectLine);
                        this.RemoveExitLine(huntLines, selectLine);

                        if (huntLines.Count > 0)
                        {
                            //重新从上一个节点开始查找
                            this.FindOuterLinessWithAngle(remainingLines, huntLines, outerLines, origin, lastLine);
                        }
                        else {

                            throw new Exception("当前无法闭合");
                        }
                    }
                    else {

                        throw new Exception("当前无法闭合");
                    }
                }

            }


        }

        /// <summary>
        /// 已知一条curve的一个端点，返回另一个端点
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="point1"></param>
        /// <returns></returns>
        private Vector2D OtherPointOfWall(Line2D line, Vector2D point)
        {
            if (point.IsAlmostEqualTo(line.Start))
                return line.End;
            else if (point.IsAlmostEqualTo(line.End))
                return line.Start;
            else
                return null;
        }

        /// <summary>
        /// 获取最靠近原点的墙体
        /// </summary>
        /// <returns></returns>
        private List<Line2D> GetHuntWithOrigin(out Vector2D origin, List<Line2D> decomposeLine2ds)
        {
            List<Line2D> huntWalls = null;
            if (OrginLine == null)
            {
                List<Vector2D> points = new List<Vector2D>();
                decomposeLine2ds.ForEach(x =>
                {
                    points.Add(x.Start);
                    points.Add(x.End);
                });
                //获取最小的点
                origin = points.Distinct(new Vector2DEqualityComparer()).OrderBy(a => a.X).ThenBy(c => c.Y).FirstOrDefault();
                //获取和最近点
                huntWalls = GetHuntLines(origin, decomposeLine2ds);
            }
            else
            {
                huntWalls = new List<Line2D>();
                Vector2D Orgin = new Vector2D(OrginLine.Start.X, OrginLine.Start.Y);
                origin = Orgin;
                Vector2D start = new Vector2D(OrginLine.Start.X, OrginLine.Start.Y);
                Vector2D end = new Vector2D(OrginLine.End.X, OrginLine.End.Y);
                Line2D line2d = Line2D.Create(start, end);
                huntWalls.Add(line2d);

            }


            return huntWalls;

        }

        /// <summary>
        /// 获取相撞的墙体信息,不能包含自己
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="remainingWalls"></param>
        /// <returns></returns>
        private List<Line2D> GetHuntLines(Vector2D origin, List<Line2D> remainingLines,Line2D self=null)
        {
            List<Line2D> closestWalls = new List<Line2D>();
            foreach (Line2D item in remainingLines)
            {
                if (self!=null&&item.IsAlmostEqualTo(self))
                {
                    continue;
                }

                Vector2D point1 = item.Start;
                Vector2D point2 = item.End;

                if (point1.IsAlmostEqualTo(origin) || point2.IsAlmostEqualTo(origin))
                {
                    closestWalls.Add(item);
                }
            }
            return closestWalls;

        }

        /// <summary>
        /// 判断当前集合是否已经存在当前线段
        /// </summary>
        /// <param name="?"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool isExitLine(List<Line2D> outerlines, Line2D line)
        {

            int index=-1;
            for (int i = 0; i < outerlines.Count; i++)
            {
                if (outerlines[i].IsAlmostEqualTo(line))
                {
                    //说明已经闭合
                    index = i;
                    break;
                }
            }

            //说明已经闭合
            if (index >= 0) {

                if (index > 0)
                {
                    //需要移除index之前的线段
                    outerlines.RemoveRange(0, index + 1);
                }
                return true;
            }
            return false;

        }


        //移除已经存在的线段
        private void RemoveExitLine(List<Line2D> outerlines, Line2D line)
        {

            foreach (Line2D oline in outerlines)
            {

                if (line.IsAlmostEqualTo(oline))
                {

                    outerlines.Remove(oline);
                    break;
                }
            }
        }



    }
}
