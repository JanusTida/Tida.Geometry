using Tida.Geometry.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tida.Geometry.External
{
    /// <summary>
    /// 线段打断算法
    /// </summary>
    public class LinesDecomposeAlgorithm
    {


        /// <summary>
        /// 将二维线段打断成最小的线单元
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public List<Line2D> Decompose(List<Line2D> lines)
        {
            //获取原始的线集合
            List<Line2D> newLines = new List<Line2D>(lines);

            //获取线和分割点的字典表
            Dictionary<Line2D, List<Vector2D>> lineDIC = new Dictionary<Line2D, List<Vector2D>>();

            //循环所有的线的相交关系，得到字典表
            for (int i = 0; i < newLines.Count; i++)
            {

                for (int j = i; j < newLines.Count; j++)
                {

                    Vector2D IntersectP = newLines[i].Intersect(newLines[j]);

                    if (IntersectP != null)
                    {

                        if (!IntersectP.IsEndPoint(newLines[i]))
                        {
                            if (!lineDIC.Keys.Contains(newLines[i]))
                            {

                                lineDIC.Add(newLines[i], new List<Vector2D>() { IntersectP });
                            }
                            else
                            {
                                lineDIC[newLines[i]].Add(IntersectP);
                            }
                        }
                        if (!IntersectP.IsEndPoint(newLines[j]))
                        {
                            if (!lineDIC.Keys.Contains(newLines[j]))
                            {

                                lineDIC.Add(newLines[j], new List<Vector2D>() { IntersectP });
                            }
                            else
                            {
                                lineDIC[newLines[j]].Add(IntersectP);
                            }
                        }

                    }
                }
            }
            //变化所有线
            foreach (KeyValuePair<Line2D, List<Vector2D>> vetsInLine in lineDIC)
            {
                //移除原来线段
                newLines.Remove(vetsInLine.Key);

                vetsInLine.Value.Add(vetsInLine.Key.Start);
                vetsInLine.Value.Add(vetsInLine.Key.End);
                ///去掉重复的点
                var vets = vetsInLine.Value.Distinct(new Vector2DEqualityComparer());


                //通过比较距离
                var orderVets = vets.OrderBy(x => x.Distance(vetsInLine.Key.Start));


                var count = orderVets.Count();
                for (int i = 0; i < count - 1; i++)
                {
                    newLines.Add(Line2D.Create(orderVets.ElementAt(i), orderVets.ElementAt(i + 1)));
                }
            }

            return newLines;
        }

        /// <summary>
        /// 将三维线段打断成最小的单元
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public List<Line3D> Decompose(List<Line3D> lines)
        {
            //获取原始的线集合
            List<Line3D> newLines = new List<Line3D>(lines);

            //获取线和分割点的字典表
            Dictionary<Line3D, List<Vector3D>> lineDIC = new Dictionary<Line3D, List<Vector3D>>();

            //循环所有的线的相交关系，得到字典表
            for (int i = 0; i < newLines.Count; i++)
            {

                for (int j = i; j < newLines.Count; j++)
                {

                    Vector3D IntersectP = newLines[i].Intersect(newLines[j]);

                    if (IntersectP != null)
                    {

                        if (!IntersectP.IsEndPoint(newLines[i]))
                        {
                            if (lineDIC[newLines[i]] == null)
                            {

                                lineDIC.Add(newLines[i], new List<Vector3D>() { IntersectP });
                            }
                            else
                            {
                                lineDIC[newLines[i]].Add(IntersectP);
                            }
                        }
                        if (!IntersectP.IsEndPoint(newLines[j]))
                        {
                            if (lineDIC[newLines[j]] == null)
                            {

                                lineDIC.Add(newLines[j], new List<Vector3D>() { IntersectP });
                            }
                            else
                            {
                                lineDIC[newLines[j]].Add(IntersectP);
                            }
                        }

                    }
                }
            }
            //变化所有线
            foreach (KeyValuePair<Line3D, List<Vector3D>> vetsInLine in lineDIC)
            {
                //移除原来线段
                newLines.Remove(vetsInLine.Key);

                vetsInLine.Value.Add(vetsInLine.Key.Start);
                vetsInLine.Value.Add(vetsInLine.Key.End);
                ///去掉重复的点
                var vets = vetsInLine.Value.Distinct(new Vector3DEqualityComparer());


                //通过比较距离
                var orderVets = vets.OrderBy(x => x.Distance(vetsInLine.Key.Start));


                var count = orderVets.Count();
                for (int i = 0; i < count - 1; i++)
                {
                    newLines.Add(Line3D.Create(orderVets.ElementAt(i), orderVets.ElementAt(i + 1)));
                }
            }
            return newLines;
        }
    }
}
