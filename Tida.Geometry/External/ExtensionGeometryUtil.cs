using System.Collections.Generic;
using System.Linq;
using Tida.Geometry.Primitives;

namespace Tida.Geometry.External
{
    public class ExtensionGeometryUtil
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <param name="tolerance">判断的误差值</param>
        /// <returns></returns>
        public static Line3D GetTwoLineCommonPart(Line3D line1, Line3D line2, double tolerance = Extension.SMALL_NUMBER)
        {
            Line3D targetLine = null;
            if (line1.Direction.IsAlmostEqualTo(line2.Direction)
                || line1.Direction.IsAlmostEqualTo(-line2.Direction))
            {
                List<Vector3D> points = new List<Vector3D>();
                if (line1.Start.IsOnLine(line2,tolerance) && !points.Contains(line1.Start, new Vector3DEqualityComparer()))
                    points.Add(line1.Start);
                if (line1.End.IsOnLine(line2, tolerance) && !points.Contains(line1.End, new Vector3DEqualityComparer()))
                    points.Add(line1.End);
                if (line2.Start.IsOnLine(line1, tolerance) && !points.Contains(line2.Start, new Vector3DEqualityComparer()))
                    points.Add(line2.Start);
                if (line2.End.IsOnLine(line1, tolerance) && !points.Contains(line2.End, new Vector3DEqualityComparer()))
                    points.Add(line2.End);

                if (points.Count >= 2)
                {
                    points = points.OrderByXYZ();
                    targetLine = new Line3D(points.First(), points.Last());
                }
            }
            return targetLine;

        }


        /// <summary>
        /// 线投影到某个Z值高度的平面
        /// </summary>
        /// <param name="sourceLine"></param>
        /// <param name="zValue"></param>
        /// <returns></returns>
        public static Line3D ProjectOnZ(Line3D sourceLine, double zValue)
        {

            Vector3D start = sourceLine.Start;
            Vector3D end = sourceLine.End;
            start = new Vector3D(start.X, start.Y, zValue);
            end = new Vector3D(end.X, end.Y, zValue);

            Line3D projectedLine = new Line3D(start, end);
            return projectedLine;
        }



    }
}
