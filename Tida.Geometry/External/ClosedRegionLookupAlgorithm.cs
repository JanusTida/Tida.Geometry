using Tida.Geometry.Alternation;
using Tida.Geometry.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tida.Geometry.External
{
    public class ClosedRegionLookupAlgorithm
    {
        /// <summary>
        /// 查找一个最大或者最小的封闭区域
        /// </summary>
        /// <returns></returns>
        public List<List<Line3D>> Lookup(List<Line3D> searchLines, bool isLargeRegion, bool isDecompose = false)
        {
            if (searchLines.Count > 3)
            {

                //获取当前面的转换举证
                Matrix4 m = TransformUtil.GetMatrix(searchLines);

                //将当前坐标转换到平面坐标
                List<Line3D> tf = TransformUtil.Transform(searchLines, m);

                //计算投影
                List<Line2D> pf = TransformUtil.Projection(tf);

                //获取到结果
                List<List<Line2D>> result = this.Lookup(pf, isLargeRegion, isDecompose);

                //获取结果
                List<List<Line3D>> line3ds = new List<List<Line3D>>();

                //获取逆矩阵
                var im = TransformUtil.GetInversetMatrix(m);

                //对结果进行转换
                if (result != null && result.Count > 0)
                {
                    result.ForEach(x =>
                    {
                        List<Line3D> tp = TransformUtil.Projection(x, 0);
                        List<Line3D> tm = TransformUtil.Transform(tp, im);
                        line3ds.Add(tm);
                    });


                }

                return line3ds;
            }
            return null;
        }


        /// <summary>
        /// 查找最大和最小封闭区域
        /// </summary>
        /// <param name="searchLines"></param>
        /// <param name="isLargeRegion"></param>
        /// <param name="isDecompose"></param>
        /// <returns></returns>
        public List<List<Line2D>> Lookup(List<Line2D> searchLines, bool isLargeRegion, bool isDecompose = false)
        {
            //最少有三条线，不然无法组成封闭区域
            if (searchLines.Count < 3)
            {
                return null;
            }
            List<List<Line2D>> closets = new List<List<Line2D>>();

            //需要处理的所有线
            List<Line2D> readyLines = new List<Line2D>(searchLines);

            //对线进行打断
            if (isDecompose)
            {
                readyLines = readyLines.Decompose();//GraphicAlgorithm.Decompose(readyLines);
            }

            //剔除有一个点没有连接点的点
            readyLines = GraphicAlgorithm.Weed(readyLines);

            if (readyLines == null || readyLines.Count == 0)
            {

                return null;
            }
            List<Line2D> Large = null;
            //查找最大的多边形
            FindMaxOne(readyLines, ref Large);

            if (isLargeRegion)
            {

                if (Large != null)
                {

                    closets.Add(Large);
                }
                return closets;
            }
            else
            {

                if (Large != null)
                {
                    FindMins(readyLines, Large, closets);
                }

                return closets;
            }

        }

        /// <summary>
        /// 查找最小的封闭区域
        /// </summary>
        /// <param name="readyLines"></param>
        /// <param name="larges"></param>
        /// <param name="closets"></param>
        private void FindMins(List<Line2D> findLines, List<Line2D> larges, List<List<Line2D>> closets)
        {

            //查找一个小区域
            List<Line2D> small = FindOne(findLines, false);


            if (small != null)
            {
                closets.Add(small);

                small.ForEach(x =>
                {
                    //查找当前线中是否有最大
                    List<Line2D> largeExit = larges.FindAll(y => y.IsAlmostEqualTo(x));

                    if (largeExit.Count > 0)
                    {
                        //外线只属于一个封闭空间
                        largeExit.ForEach(l =>
                        {
                            Line2D mt = findLines.Find(m => m.IsAlmostEqualTo(l));

                            findLines.Remove(mt);
                            larges.Remove(l);
                        });

                    }
                    else
                    {
                        larges.Add(x);
                    }
                });
                if (findLines.Count > 0)
                {
                    FindMins(findLines, larges, closets);
                }


            }
        }


        /// <summary>
        /// 查找一个最大的封闭区域,不管图形形状，除弧形以外
        /// </summary>
        /// <returns></returns>
        private void FindMaxOne(List<Line2D> searchLines, ref List<Line2D> larges)
        {
            if (larges == null)
            {
                larges = new List<Line2D>();
            }

            List<Line2D> findLines = new List<Line2D>(searchLines);

            //查找一个最大的矩形
            List<Line2D> findone = FindOne(findLines, true);

            if (findone != null)
            {
                larges = findone;
            }
            else
            {
                larges = null;
            }

        }

        /// <summary>
        /// 查找一个封闭区域
        /// </summary>
        /// <param name="searchLines"></param>
        /// <param name="isLargeRegion"></param>
        /// <param name="startL"></param>
        /// <returns></returns>
        private List<Line2D> FindOne(List<Line2D> ureadyLines, bool isLargeRegion)
        {
            List<Line2D> readyLines = new List<Line2D>(ureadyLines);
            //判断是否需要打断

            //开始点
            Vector2D originP = null;
            //查找一个离原点最近的点，和当前的线，则从这个线开始查找闭合区间
            List<Line2D> huntLines = this.FindBegin(readyLines, out originP);
            //需要查找的线
            List<Line2D> resultLines = new List<Line2D>();
            //获取起点的墙体
            this.FindClosestLines(readyLines, huntLines, isLargeRegion, ref resultLines, originP);

            //返回结果
            return resultLines;
        }

        /// <summary>
        /// 获取一个开始点，得到起始点
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="decomposeLine2ds"></param>
        /// <returns></returns>
        private List<Line2D> FindBegin(List<Line2D> lines, out Vector2D origin)
        {
            List<Vector2D> points = new List<Vector2D>();
            lines.ForEach(x =>
            {
                points.Add(x.Start);
                points.Add(x.End);
            });
            //获取最小的点
            origin = points.Distinct(new Tida.Geometry.Primitives.Vector2DEqualityComparer()).OrderBy(a => a.X).ThenBy(c => c.Y).FirstOrDefault();

            //查找和当前点相关的线
            List<Line2D> rexlines = FindHuntLines(origin, lines, null);

            return rexlines;
        }


        /// <summary>
        /// 获取相撞的墙体信息,不能包含自己
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="remainingWalls"></param>
        /// <returns></returns>
        private List<Line2D> FindHuntLines(Vector2D origin, List<Line2D> lines, Line2D self)
        {
            List<Line2D> laxlines = new List<Line2D>();
            foreach (Line2D item in lines)
            {
                //查找的线不能是自己
                if (self != null && item.IsAlmostEqualTo(self))
                {
                    continue;
                }

                Vector2D point1 = item.Start;
                Vector2D point2 = item.End;

                if (point1.IsAlmostEqualTo(origin) || point2.IsAlmostEqualTo(origin))
                {
                    laxlines.Add(item);
                }
            }
            return laxlines;

        }

        /// <summary>
        /// 查找一个封闭的区域
        /// </summary>
        private void FindClosestLines(List<Line2D> readyLines, List<Line2D> huntLines, bool IsLargeRegion, ref List<Line2D> resultLines, Vector2D startP, Line2D nextLine = null)
        {


            List<Line2D> rpointlines = new List<Line2D>();

            //获取当前关联的所有其他线
            foreach (var item in huntLines)
            {
                Line2D matrixLine = Line2D.Create(startP, OtherPointOfLine(item, startP));
                rpointlines.Add(matrixLine);
            }

            Line2D searchline = nextLine;
            //假如需要查找的线不存在，则查询对应的线段
            if (nextLine == null)
            {
                // 查找一个最水平的线，那么这个线为从左开始，切方向向右的线段对key的角度进行降序排列，Key的Direction.Y越小，说明越水平
                searchline = (from sl in rpointlines orderby Math.Asin(sl.Direction.Y) select sl).FirstOrDefault();
            }
            else
            {
                //对key值的内容参数进行降序排列
                if (IsLargeRegion)
                {
                    searchline = (from dic in rpointlines orderby (-searchline.Direction).AngleFrom(dic.Direction) descending select dic).FirstOrDefault();
                }
                else
                {
                    searchline = (from dic in rpointlines orderby (-searchline.Direction).AngleFrom(dic.Direction) select dic).FirstOrDefault();
                }

            }


            //查找当前线中，是否已经存在，假如存在，则说明已经闭合，直接退出当前的递归操作
            if (!HasLookin(resultLines, searchline))
            {
                //闭合后，则查找线的另外一个点
                Vector2D OtherPoint = this.OtherPointOfLine(searchline, startP);

                //在当前的集合中，再查找相关联的所有的线
                List<Line2D> nhuntLiness = this.GetHuntLines(OtherPoint, readyLines, searchline);

                //没有形成闭合，但是已经找不到和当前点相关的墙体，说无法形成闭合
                if (nhuntLiness.Count > 0)
                {
                    //说明还可以继续查找
                    resultLines.Add(searchline);

                    //继续进行查找
                    this.FindClosestLines(readyLines, nhuntLiness, IsLargeRegion, ref resultLines, OtherPoint, searchline);
                }
                else
                {
                    //假如线还没有查找完成，则当前线已经没有下一个线了，说明当前线是无效的线
                    if (readyLines.Count > 0)
                    {
                        //移除这个线，并且从其他的线中，重新查找闭合的线
                        this.RemoveLine(readyLines, searchline);
                        this.RemoveLine(huntLines, searchline);

                        if (huntLines.Count > 0)
                        {
                            //移除一个捕获线，从当前结束点重新开始查找
                            this.FindClosestLines(readyLines, huntLines, IsLargeRegion, ref resultLines, startP, nextLine);
                        }
                        else
                        {

                            resultLines = null;
                            return;
                        }
                    }
                    else
                    {

                        resultLines = null;
                        return;

                    }
                }

            }
        }

        /// <summary>
        /// 判断当前集合是否已经存在当前线段
        /// </summary>
        /// <param name="?"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private bool HasLookin(List<Line2D> lines, Line2D line)
        {

            int index = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IsAlmostEqualTo(line))
                {
                    //说明已经闭合
                    index = i;
                    break;
                }
            }

            //说明已经闭合
            if (index >= 0)
            {
                return true;

            }
            return false;

        }

        /// <summary>
        /// 查找封闭区域
        /// </summary>
        /// <param name="searchLines"></param>
        /// <param name="isLargeRegion"></param>
        /// <param name="startL"></param>
        /// <returns></returns>
        public List<Polygon2D> Lookup(Polygon2D polygon2D, bool isLargeRegion, Line2D startL = null)
        {
            return null;
        }

        /// <summary>
        /// 查找一个封闭区域
        /// </summary>
        /// <param name="polygon3D"></param>
        /// <param name="isLargeRegion"></param>
        /// <param name="startL"></param>
        /// <returns></returns>
        public List<Polygon3D> Lookup(Polygon3D polygon3D, bool isLargeRegion, Line2D startL = null)
        {
            return null;
        }


        /// <summary>
        /// 已知一条curve的一个端点，返回另一个端点
        /// </summary>
        /// <param name="curve"></param>
        /// <param name="point1"></param>
        /// <returns></returns>
        private Vector2D OtherPointOfLine(Line2D line, Vector2D point)
        {
            if (point.IsAlmostEqualTo(line.Start))
                return line.End;
            else if (point.IsAlmostEqualTo(line.End))
                return line.Start;
            else
                return null;
        }


        /// <summary>
        /// 获取相撞的墙体信息,不能包含自己
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="remainingWalls"></param>
        /// <returns></returns>
        private List<Line2D> GetHuntLines(Vector2D origin, List<Line2D> remainingLines, Line2D self = null)
        {
            List<Line2D> closestWalls = new List<Line2D>();
            foreach (Line2D item in remainingLines)
            {
                if (self != null && item.IsAlmostEqualTo(self))
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


        //移除已经存在的线段
        private void RemoveLine(List<Line2D> outerlines, Line2D line)
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


