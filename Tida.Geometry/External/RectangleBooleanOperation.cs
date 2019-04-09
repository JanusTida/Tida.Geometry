using System.Collections.Generic;
using System.Linq;

using Tida.Geometry.Primitives;


namespace Tida.Geometry.External
{
    public class RectangleBooleanOperation
    {
        private InRegionAlgorithm inRegionAlgorithm;




        /// <summary>
        /// 
        /// </summary>
        /// <param name="osbOutLines">一个osb的轮廓线</param>
        /// <param name="openningOutLines">一个洞口的轮廓线</param>
        /// <param name="cutLocation">osb板被剪切掉的矩形区域的形心</param>
        /// <param name="cutlines">osb板被剪切掉的矩形区域</param>
        public void Operate(List<Line3D> osbOutLines, List<Line3D> openningOutLines, ref  Vector3D cutLocation, List<Line3D> cutlines)
        {
            inRegionAlgorithm = new InRegionAlgorithm();

            List<Vector3D> intersectPoints = GetIntersectPointWithoutVertex(osbOutLines, openningOutLines);
            List<Vector3D> pointInOSB = GetpointInOSB(osbOutLines, openningOutLines);

            //pointInOSB和intersectPoints都是需要的点，将其添加到集合中去
            List<Vector3D> needPoints = new List<Vector3D>();
            if (intersectPoints.Count > 0)
            {
                needPoints.AddRange(intersectPoints);
                if (pointInOSB.Count > 0)
                {
                    needPoints.AddRange(pointInOSB);
                }
                needPoints = needPoints.Distinct(new Vector3DEqualityComparer()).ToList();
            }
            else if (pointInOSB.Count > 0)//对于有很小的窗户，其位置在一层osb板之间
            {
                needPoints.AddRange(pointInOSB);

                needPoints = needPoints.Distinct(new Vector3DEqualityComparer()).ToList();
            }

            //说明洞口和osb板可能存在交集
            if (needPoints.Count >0)
            {
                //划分成多个小格子          
                List<List<Line3D>> cellList = GetCells(osbOutLines, needPoints);

                //找到在openningOutLines区域内的方格子
                cutlines.AddRange(GetCutRegion(cellList, openningOutLines));
                if (cutlines.Count >0)
                {
                    cutLocation = GetCentriod(cutlines);
                }
               
            }

        }

        private List<Line3D> GetCutRegion(List<List<Line3D>> cellList, List<Line3D> openningOutLines)
        {
            List<Line3D> resultLines = new List<Line3D>();
            foreach (List<Line3D> lines in cellList)
            {
                Vector3D centriod = GetCentriod(lines);
                int a = inRegionAlgorithm.Check(centriod, openningOutLines);
                if (a == 1)
                {
                    resultLines = lines;
                    break;
                }
            }
            return resultLines;
        }

        private Vector3D GetCentriod(List<Line3D> lines)
        {
            List<Vector3D> points = new List<Vector3D>();
            lines.ForEach(x =>
            {
                points.Add(x.Start);
                points.Add(x.End);
            });

            Vector3D point = new Vector3D();
            points.ForEach(x =>
            {
                point = point + x;
            });
            return point / points.Count;

        }
        /// <summary>
        ///根据点将osb划分成多个小方格子
        /// </summary>
        /// <param name="osbOutLines"></param>
        /// <param name="Points">划分小方格子的点</param>
        /// <returns></returns>
        private List<List<Line3D>> GetCells(List<Line3D> osbOutLines, List<Vector3D> Points)
        {
            //osb板的四个端点
            List<Vector3D> osbVertexs = new List<Vector3D>();
            osbOutLines.ForEach(x =>
            {
                osbVertexs.Add(x.Start);
                osbVertexs.Add(x.End);
            });
            osbVertexs = osbVertexs.Distinct(new Vector3DEqualityComparer()).ToList();

            //端点和内部的点全部添加到集合中
            List<Vector3D> allpoints = new List<Vector3D>();
            allpoints.AddRange(Points);
            allpoints.AddRange(osbVertexs);

            //求x或y轴上的刻度点,并去除重复的点. 从小到大排列
            List<double> XArray = new List<double>();
            List<double> YArray = new List<double>();
            allpoints.ForEach(x => { XArray.Add(x.X); YArray.Add(x.Y); });
            XArray = XArray.Distinct().OrderBy(a => a).ToList();
            YArray = YArray.Distinct().OrderBy(a => a).ToList();

            List<List<Line3D>> cellList = new List<List<Line3D>>();
            for (int i = 0; i < YArray.Count - 1; i++)
            {
                double miny = YArray[i];
                double maxy = YArray[i + 1];
                for (int j = 0; j < XArray.Count - 1; j++)
                {
                    double minx = XArray[j];
                    double maxx = XArray[j + 1];

                    List<Line3D> cellLines = CreateCell(minx, maxx, miny, maxy);
                    cellList.Add(cellLines);
                }
            }
            return cellList;
        }

        private List<Line3D> CreateCell(double minx, double maxx, double miny, double maxy)
        {
            List<Line3D> cellLines = new List<Line3D>();
            //构成cell的四个顶点
            Vector3D point1 = new Vector3D(minx, miny, 0);
            Vector3D point2 = new Vector3D(maxx, miny, 0);
            Vector3D point3 = new Vector3D(maxx, maxy, 0);
            Vector3D point4 = new Vector3D(minx, maxy, 0);

            List<Vector3D> vertex = new List<Vector3D>() { point1, point2, point3, point4, point1 };
            for (int i = 0; i < vertex.Count - 1; i++)
            {
                Line3D line = Line3D.Create(vertex[i], vertex[i + 1]);
                cellLines.Add(line);
            }
            return cellLines;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="osbOutLines"></param>
        /// <param name="openningOutLines"></param>
        /// <returns></returns>
        public  List<Vector3D> GetIntersectPointWithoutVertex(List<Line3D> osbOutLines, List<Line3D> openningOutLines)
        {
            List<Vector3D> intersectPoints = new List<Vector3D>();
            osbOutLines.ForEach(x =>
            {
                openningOutLines.ForEach(y =>
                {
                    Vector3D point = x.Intersect(y);
                    if (point != null)
                    {
                        intersectPoints.Add(point);
                    }
                });
            });

            //排除端点是交点的情况
            osbOutLines.ForEach(x =>
            {
                for (int i = 0; i < intersectPoints.Count; i++)
                {
                    if (x.Start.IsAlmostEqualTo(intersectPoints[i]) || x.End.IsAlmostEqualTo(intersectPoints[i]))
                    {
                        intersectPoints.RemoveAt(i);
                        i--;
                    }
                }

            });

            return intersectPoints;
        }
        /// <summary>
        /// 获得在osb板区域内的洞口的端点
        /// </summary>
        /// <param name="osbOutLines"></param>
        /// <param name="openningOutLines"></param>
        /// <returns>返回在osb轮廓线边上和内部的点</returns>
        private List<Vector3D> GetpointInOSB(List<Line3D> osbOutLines, List<Line3D> openningOutLines)
        {

            List<Vector3D> inOSBPoints = new List<Vector3D>();

            //获得洞口的四个端点，并去除重复
            List<Vector3D> openningVertexs = new List<Vector3D>();
            openningOutLines.ForEach(x =>
            {
                openningVertexs.Add(x.Start);
                openningVertexs.Add(x.End);
            });
            openningVertexs = openningVertexs.Distinct(new Vector3DEqualityComparer()).ToList();

            //判断点是否在osboutlines的区域内
            openningVertexs.ForEach(x =>
            {
                int a = inRegionAlgorithm.Check(x, osbOutLines);
                if (a == 1)
                {
                    inOSBPoints.Add(x);
                }
            });

            return inOSBPoints;
        }
    }
}
