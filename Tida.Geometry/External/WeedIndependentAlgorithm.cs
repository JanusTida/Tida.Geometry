using Tida.Geometry.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tida.Geometry.External
{
    /// <summary>
    /// 清除没有任何连接的点
    /// </summary>
    public class WeedIndependentAlgorithm
    {

        /// <summary>
        /// 剔除端点无效的点
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
       

        public List<Line2D> Weed(List<Line2D> ilines) {

           var  lines = GraphicAlgorithm.Decompose(ilines);
            if (lines != null)
            {
                List<Line2D> rets = new List<Line2D>();
                for (int i = 0; i < lines.Count; i++) {


                    var start = lines[i].Start;
                    var end = lines[i].End;

                    var one = lines.FindAll(x => x.Start.IsAlmostEqualTo(start) || x.End.IsAlmostEqualTo(start));
                    var two = lines.FindAll(x => x.Start.IsAlmostEqualTo(end) || x.End.IsAlmostEqualTo(end));

                    if (one.Count == 1 || two.Count == 1)
                    {
                        //添加一个需要移除的图形元素
                        rets.Add(lines[i]);
                    }
                }

                if (rets.Count > 0)
                {
                    rets.ForEach(x =>
                    {
                        lines.Remove(x);
                    });

                    Weed(lines);
                }

                return lines;
            }
            return null;

        }



        /// <summary>
        /// 获取无法形成封闭区域的线
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public List<Line2D> WeedLess(List<Line2D> ilines)
        {
            var lines = ilines.Decompose();//GraphicAlgorithm.Decompose(ilines);

            //var lines = GraphicAlgorithm.Decompose(ilines);

            List<Line2D> mweed = new List<Line2D>(lines);
            //去掉了无端点线段
            List<Line2D> weeds = this.Weed(mweed);
            var weedLess = lines.FindAll(x => !weeds.Contains(x, new Line2DEqualityComparer()));
            return weedLess;
        }


        /// <summary>
        /// 剔除无端点的线段
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public List<Line3D> Weed(List<Line3D> lines)
        {

            if (lines != null)
            {
                List<Line3D> rets = new List<Line3D>();
                for (int i = 0; i < lines.Count; i++)
                {

                    var start = lines[i].Start;
                    var end = lines[i].End;

                    var one = lines.Find(x => x.Start.IsAlmostEqualTo(start) || x.End.IsAlmostEqualTo(start));
                    var two = lines.Find(x => x.Start.IsAlmostEqualTo(end) || x.End.IsAlmostEqualTo(end));

                    if (one == null || two == null)
                    {
                        //添加一个需要移除的图形元素
                        rets.Add(lines[i]);
                    }
                }

                if (rets.Count > 0)
                {
                    rets.ForEach(x =>
                    {
                        lines.Remove(x);
                    });

                    Weed(lines);
                }

                return lines;
            }
            return null;

        }




        /// <summary>
        /// 剔除端点无效的点
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public List<Line3D> WeedLess(List<Line3D> lines)
        {
            List<Line3D> mweed = new List<Line3D>(lines);
            //去掉了无端点线段
            List<Line3D> weeds = this.Weed(mweed);

            var weedLess = lines.FindAll(x => weeds.Contains(x, new Line3DEqualityComparer()));

            return weedLess;
        }
    }
}
