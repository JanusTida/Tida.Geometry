using System;
using System.Collections.Generic;
using Tida.Geometry.Primitives;

namespace Tida.Geometry.External
{
    /// <summary>
    /// 利用射线法判断一个点是否在某个多边形的内部，此处要求多边形为平面多边形，即如果多边形为空间，则要投影到xoy平面再来判断，
    /// 以判定点为端点做一条x正向的射向，如果交点为偶数则点在外部，如果交点个数为基数则点在区域内
    /// </summary>
    public class InRegionAlgorithm
    {
        /// <summary>
        /// -1表示点在边上，1表示在内部，2表示在外部
        /// </summary>
        /// <param name="vector3D"></param>
        /// <param name="outLines">投影到xoy平面后的楼板外轮廓</param>
        /// <returns></returns>
        public int Check(Vector3D vector3D, List<Line3D> outLines)
        {
            int count = 0;//交点的个数

            double x = vector3D.X;
            double y = vector3D.Y;
            foreach (Line3D line in outLines)
            {
                double x1 = line.Start.X;
                double y1 = line.Start.Y;
                double x2 = line.End.X;
                double y2 = line.End.Y;
                int nflag = IsIntersectAnt(x, y, x1, x2, y1, y2);//点与边的情况
                if (nflag < 0) return -1;//出现-1，表明点在边上
                count += nflag;
            }
            if (count % 2 == 1) return 1;//表明交点个数为基数
            return 2;//表明交点个数为偶数
        }

        /// <summary>
        /// 以p为端点向x正向引射线，考察射线与线段交点的情况
        /// 射线与线段的关系，如果相交则返回1，如果不想交则返回0，如果点在线段上则返回-1
        /// 下端点指 线段的另一端y大于该端点，
        /// </summary>
        /// <param name="x">考察点的x坐标</param>
        /// <param name="y">考察点的y坐标</param>
        /// <param name="x1">线段1端点x坐标</param>
        /// <param name="x2">线段2端点x坐标</param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        int IsIntersectAnt(double x, double y, double x1, double x2, double y1, double y2)
        {
            double minx = Math.Min(x1, x2);
            double maxx = Math.Max(x1, x2);
            double miny = Math.Min(y1, y2);
            double maxy = Math.Max(y1, y2);

            //如果是水平线段,且y与miny或maxy相同，则判断x的值,确定p是否在边上
            if (Math.Abs(maxy - miny) < 1e-6 && Math.Abs(y - miny) < 1e-6) return ((x > minx && x < maxx) || Math.Abs(x - minx) < 1e-6 || Math.Abs(x - maxx) < 1e-6) ? -1 : 0;
            //如果射线端点在线段的上部或者下部，右边，可快速知道两者没有交点
            else if (y > maxy || y < miny || x > maxx) return 0;

            //计算交点坐标
            double interPointX = x1 + (y - y1) * (x2 - x1) / (y2 - y1);//交点的x坐标
            if (Math.Abs(interPointX - x) < 1e-6) return -1;
            else if (interPointX < x) return 0;

            //如果穿过下端点，也认为没有交点
           // if (Math.Abs(y - miny) < 1e-6) return 0;
            //其他的都认为有交点
            return 1;

        }
    }

}
