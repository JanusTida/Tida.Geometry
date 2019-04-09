using Tida.Geometry.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tida.Geometry.External.Util {
    /// <summary>
    /// 判定处理;
    /// </summary>
    public static class HitUtil {
        /// <summary>
        /// 获得平面内两线段的交点;
        /// </summary>
        /// <param name="line0"></param>
        /// <param name="lin11"></param>
        /// <param name="extend0">是否无限延长线段0</param>
        /// <param name="extend1">是否无限延长线段1</param>
        /// <returns></returns>
        public static Vector2D LinesIntersect(
            Line2D line0, Line2D lin11,
            bool extend0 = false,bool extend1 = false) {

            if(line0 == null) {
                throw new ArgumentNullException(nameof(line0));
            }
            if(lin11 == null) {
                throw new ArgumentNullException(nameof(lin11));
            }


            double x1 = line0.Start.X;
            double x2 = line0.End.X;
            double x3 = lin11.Start.X;
            double x4 = lin11.End.X;
            double y1 = line0.Start.Y;
            double y2 = line0.End.Y;
            double y3 = lin11.Start.Y;
            double y4 = lin11.End.Y;

            double denominator = ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            if (denominator == 0) // lines are parallel
                return null;
            double numerator_ua = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3));
            double numerator_ub = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3));
            double ua = numerator_ua / denominator;
            double ub = numerator_ub / denominator;
            // if a line is not extended then ua (or ub) must be between 0 and 1
            if (extend0 == false) {
                if (ua < 0 || ua > 1)
                    return null;
            }
            if (extend1 == false) {
                if (ub < 0 || ub > 1)
                    return null;
            }
            Vector2D retPoint = null;
            if (extend0 || extend1) // no need to chck range of ua and ub if check is one on lines 
            {
                retPoint = new Vector2D(x1 + ua * (x2 - x1), y1 + ua * (y2 - y1));
            }
            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1) {
                retPoint = new Vector2D(x1 + ua * (x2 - x1), y1 + ua * (y2 - y1));
            }
            return retPoint;
        }

        /// <summary>
        /// 获得平面内线段与矩形的交点;
        /// </summary>
        /// <param name="lp1"></param>
        /// <param name="lp2"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static IEnumerable<Vector2D> LineIntersectWithRect(Line2D line, Rectangle2D rect) {
            if(line == null) {
                throw new ArgumentNullException(nameof(line));
            }

            if(rect == null) {
                throw new ArgumentNullException(nameof(rect));
            }

            var intersectPoints = new List<Vector2D>();

            //线段分别与四条边比较;
            var leftIntersect = LinesIntersect(line, rect.Left);
            var bottomIntersect = LinesIntersect(line, rect.Bottom);
            var rightIntersect = LinesIntersect(line, rect.Right);
            var topIntersect = LinesIntersect(line, rect.Top);

            //去重;
            Func<Vector2D, Vector2D, Vector2D> disctinct = (point0, point1) => {
                if (point0 != null && point1 != null) {
                    if (point1.X == point1.X && point1.Y == point1.Y) {
                        intersectPoints.Add(point1);
                    }
                }
                return null;
            };


            return null;
            
            
        }

        /// <summary>
        /// 检查某个坐标是否处于某个矩形内部;
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool Contains(this Rectangle2D2 rect,Vector2D point) {
            if(rect == null) {
                throw new ArgumentNullException(nameof(rect));
            }

            if(point == null) {
                throw new ArgumentNullException(nameof(point));
            }

            return point.IsInRegion(rect.GetLines().ToList());
        }
    }
}
