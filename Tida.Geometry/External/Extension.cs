using Tida.Geometry.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tida.Geometry.External
{
    public static class Extension
    {

        #region 常量
        public const double SMALL_NUMBER = 1e-5;
        public const double BIG_NUMBER = 1e6;
        public const double INCH_MM = 304.8;
        public const double PI_DEG = 180;
        #endregion

        #region 实数相关
        /// <summary>
        /// 判断两个double类型是否接近相等
        /// </summary>
        /// <param name="source1"></param>
        /// <param name="source2"></param>
        /// <returns></returns>
        public static bool AreEqual(this double source1, double source2)
        {
            return Math.Abs(source1 - source2) < SMALL_NUMBER;
        }

        public static bool AreEqual(this double source1, double source2, double tolerance)
        {
            return Math.Abs(source1 - source2) < tolerance;
        }
        /// <summary>
        /// 去掉重复部分
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static List<double> Distinct(this List<double> args)
        {
            List<double> newList = new List<double> { args.First() };
            args.ForEach(x => { if (!newList.Any(y => y.AreEqual(x))) newList.Add(x); });
            return newList;
        }
        /// <summary>
        /// 毫米转换英尺
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double MMtoInch(double value)
        {
            return value / INCH_MM;
        }
        /// <summary>
        /// 英尺转换毫米
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double InchtoMM(double value)
        {
            return value * INCH_MM;
        }
        /// <summary>
        /// 毫米转换英尺
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3D MMtoInch(Vector3D v)
        {
            return new Vector3D(MMtoInch(v.X), MMtoInch(v.Y), MMtoInch(v.Z));
        }
        /// <summary>
        /// 英尺转换毫米
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3D InchtoMM(Vector3D v)
        {
            return new Vector3D(InchtoMM(v.X), InchtoMM(v.Y), InchtoMM(v.Z));
        }

        /// <summary>
        /// 角度转换为弧度
        /// </summary>
        /// <param name="angle"></param>s
        /// <returns></returns>
        public static double DegToRad(double deg)
        {
            return deg * (Math.PI / PI_DEG);
        }

        /// <summary>
        /// 弧度转换为角度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double RadToDeg(double rad)
        {
            return (PI_DEG / Math.PI) * rad;
        }
        #endregion

        #region Vector2D相关
        /// <summary>
        /// 按1e-6的容差对点的集合进行去重
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<Vector2D> DistinctPoint(this List<Vector2D> points)
        {
            var newPoints = new List<Vector2D> { points[0] };
            points.ForEach(x => { if (!newPoints.Any(y => y.IsAlmostEqualTo(x))) newPoints.Add(x); });
            return newPoints;
        }
        public static double GetLengthSquared(this Vector2D vector2D) => vector2D.X * vector2D.X + vector2D.Y * vector2D.Y;
        /// <summary>
        /// 当前点是否是直线的端点
        /// </summary>
        /// <param name="source"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsEndPoint(this Vector2D source, Line2D line)
        {
            if (source.IsAlmostEqualTo(line.Start) || source.IsAlmostEqualTo(line.End))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 将局部的向量转换至全局的向量;
        /// </summary>
        /// <param name="relativeSource">原局部向量</param>
        /// <param name="relativedToVector">原局部向量相对的向量在全局坐标系的表示</param>
        /// <exception cref="InvalidOperationException">相对向量长度为零</exception>
        /// <returns></returns>
        public static Vector2D RelativeToAbsoluteVector(this Vector2D relativeSource, Vector2D relativedToVector)
        {
            if (relativeSource == null)
            {
                throw new ArgumentNullException(nameof(relativeSource));
            }

            if (relativedToVector == null)
            {
                throw new ArgumentNullException(nameof(relativedToVector));
            }

            var lengthOfRelativedVector = relativedToVector.Modulus();

            if (lengthOfRelativedVector.AreEqual(0))
            {
                throw new InvalidOperationException($"The length of {nameof(relativedToVector)} can't be zero.");
            }

            return new Vector2D(
                   (relativeSource.X * relativedToVector.X - relativeSource.Y * relativedToVector.Y) / lengthOfRelativedVector,
                   (relativeSource.X * relativedToVector.Y + relativeSource.Y * relativedToVector.X) / lengthOfRelativedVector
            );
        }

        /// <summary>
        /// 将全局的向量转换至局部的向量;
        /// </summary>
        /// <param name="absoluteSource">原局部向量</param>
        /// <param name="relativedToVector">原局部向量相对的向量在全局坐标系的表示</param>
        /// <exception cref="InvalidOperationException">相对向量长度为零</exception>
        /// <returns></returns>
        public static Vector2D AbsoluteToRelativeVector(this Vector2D absoluteSource, Vector2D relativedToVector)
        {
            if (absoluteSource == null)
            {
                throw new ArgumentNullException(nameof(absoluteSource));
            }

            if (relativedToVector == null)
            {
                throw new ArgumentNullException(nameof(relativedToVector));
            }

            var squadOfRelativedVector = relativedToVector.X * relativedToVector.X + relativedToVector.Y * relativedToVector.Y;

            if (squadOfRelativedVector.AreEqual(0))
            {
                throw new InvalidOperationException($"The length of {nameof(relativedToVector)} can't be zero.");
            }

            return new Vector2D(
                (absoluteSource.X * relativedToVector.X + absoluteSource.Y * relativedToVector.Y) / squadOfRelativedVector,
                (absoluteSource.Y * relativedToVector.X - absoluteSource.X * relativedToVector.Y) / squadOfRelativedVector
            );
        }

        /// <summary>
        /// 判断是否不在region外部（在内部或边上）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        private static bool IsPossiblyInRegion(this Vector2D point, List<Line2D> region)
        {
            //先排序
            List<Line2D> copiedLines = new List<Line2D>(region);
            copiedLines = copiedLines.MakeCounterclockwise();

            int num = copiedLines.Count;
            double[] arrayX = new double[num];
            double[] arrayY = new double[num];
            Vector2D tempXY = new Vector2D(0, 0);
            for (int n = 0; n < copiedLines.Count(); n++)
            {
                tempXY = copiedLines[n].Start;
                arrayX[n] = tempXY.X;
                arrayY[n] = tempXY.Y;
            }
            double testx = point.X;
            double testy = point.Y;
            int i, j, crossings = 0;
            for (i = 0, j = num - 1; i < num; j = i++)
            {
                if (((arrayY[i] > testy) != (arrayY[j] > testy)) &&
                 (testx < (arrayX[j] - arrayX[i]) * (testy - arrayY[i]) / (arrayY[j] - arrayY[i]) + arrayX[i]))
                    crossings++;
            }
            return (crossings % 2 != 0);
        }

        /// <summary>
        /// 判断是否在Line内部，不包括端点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsInLine(this Vector2D point, Line2D line)
        {
            return point.IsOnLine(line) && !point.IsAlmostEqualTo(line.Start) && !point.IsAlmostEqualTo(line.End);
        }

        /// <summary>
        /// 判断一个点是否完全是region内部（不会在边上）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static bool IsInRegion(this Vector2D point, List<Line2D> region)
        {
            return point.IsPossiblyInRegion(region) && !point.IsOnRegionEdge(region);
        }

        /// <summary>
        /// 判断是否在region的边上  region.Any(item => point.IsOnLine(item));
        /// </summary>
        /// <param name="point"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static bool IsOnRegionEdge(this Vector2D point, List<Line2D> region)
        {
            return region.Any(point.IsOnLine);
        }

        /// <summary>
        /// 判断是否在line上，包括端点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsOnLine(this Vector2D point, Line2D line)
        {
            return line.Distance(point) < SMALL_NUMBER;
        }
        public static bool IsOnTwoLine(this Vector2D point, Line2D line1, Line2D line2)
        {
            return point.IsOnLine(line1) && point.IsOnLine(line2);
        }
        public static bool IsParallelWith(this Vector2D point, Vector2D source)
        {
            return (Math.Abs(point.AngleWith(source)) < SMALL_NUMBER
                || Math.Abs(point.AngleWith(source) - Math.PI) < SMALL_NUMBER);
        }

        public static bool IsVerticalWith(this Vector2D v1, Vector2D v2)
        {
            return Math.Abs(v1.AngleTo(v2) - Math.PI / 2) < 1e-6;
        }



        /// <summary>
        /// 点投影到直线上的点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Vector2D ProjectOn(this Vector2D point, Line2D line)
        {
            //算法说明：先过this点做垂直于curve的平面，得到其方程，
            //求出该平面与curve的交点即为投影点
            Vector2D s = line.Start;
            Vector2D d = line.Direction;
            double t = ((point.X - s.X) * d.X + (point.Y - s.Y) * d.Y) / (d.X * d.X + d.Y * d.Y);
            return new Vector2D(s.X + d.X * t, s.Y + d.Y * t);
        }
        /// <summary>
        /// 点到直线的垂直距离 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static double VerticalDistanceTo(this Vector2D point, Line2D line)
        {
            Vector2D projectPoint = point.ProjectOn(line);
            return point.Distance(projectPoint);
        }

        /// <summary>
        /// 点到直线的最近距离，非垂直距离
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static double Distance(this Vector2D point, Line2D line)
        {
            return line.Distance(point);
        }


        /// <summary>
        /// 移动指定向量的位置;
        /// </summary>
        /// <param name="x">移动的横长</param>
        /// <param name="y">移动的纵长</param>
        public static void Move(this Vector2D vector2D, double x, double y)
        {

            if (vector2D == null)
            {
                throw new ArgumentNullException(nameof(vector2D));
            }

            vector2D.X += x;
            vector2D.Y += y;
        }


        /// <summary>
        /// 获取向量根据指定的偏移移动后的向量;
        /// </summary>
        /// <returns></returns>
        public static Vector2D CreateByOffset(this Vector2D vector2D, double x, double y)
        {

            if (vector2D == null)
            {
                throw new ArgumentNullException(nameof(vector2D));
            }

            return new Vector2D(vector2D.X + x, vector2D.Y + y);
        }

        /// <summary>
        /// 移动;
        /// </summary>
        /// <param name="vector"></param>
        public static void Move(this Vector2D vector2D, Vector2D vector)
        {

            if (vector2D == null)
            {
                throw new ArgumentNullException(nameof(vector2D));
            }

            if (vector == null)
            {
                throw new ArgumentNullException(nameof(vector));
            }

            vector2D.X += vector.X;
            vector2D.Y += vector.Y;
        }


        #endregion

        #region Vector3D相关

        /// <summary>
        /// 按1e-6的容差对点的集合进行去重
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<Vector3D> DistinctPoint(this List<Vector3D> points)
        {
            var newPoints = new List<Vector3D> { points[0] };
            points.ForEach(x => { if (!newPoints.Any(y => y.IsAlmostEqualTo(x))) newPoints.Add(x); });
            return newPoints;
        }

        /// <summary>
        /// 计算一个点是否是一个直线的端点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="curve"></param>
        /// <returns></returns>
        public static bool IsEndPoint(this Vector3D point, Line3D curve)
        {
            return point.IsAlmostEqualTo(curve.Start, Extension.SMALL_NUMBER) |
                   point.IsAlmostEqualTo(curve.End, Extension.SMALL_NUMBER);
        }

        /// <summary>
        /// 比较两个点在默认误差范围(1e-6)内是否相同
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualTo(this Vector3D point1, Vector3D point2)
        {
            return point1.IsAlmostEqualTo(point2, SMALL_NUMBER);
        }

        /// <summary>
        /// 比较两个点在误差范围内是否相同
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public static bool IsAlmostEqualTo(this Vector3D point1, Vector3D point2, double tolerance)
        {
            return point1.Distance(point2) < tolerance;
        }

        /// <summary>
        /// 向量的夹角，值域为[0,π]
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double AngleTo(this Vector3D v1, Vector3D v2)
        {
            if ((v1.Modulus() * v2.Modulus()).AreEqual(0)) return 0;
            var cosA = v1.Dot(v2) / (v1.Modulus() * v2.Modulus());
            if (cosA > 1)
                cosA = 1;
            if (cosA < -1)
                cosA = -1;
            return Math.Acos(cosA);
        }

        /// <summary>
        /// 向量所在直线的夹角，值域为[0,π/2]
        /// </summary>
        /// <param name="v"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static double AngleWith(this Vector3D v, Vector3D vector)
        {
            var angle = v.AngleTo(vector);
            return angle < Math.PI / 2 ? angle : Math.PI - angle;
        }

        /// <summary>
        ///向量source逆时针到旋转到终点向量的角度（均为平行于水平面的向量），值域为[0 ,2π)（平行于水平面的向量）
        /// </summary>
        /// <param name="v"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double AngleFrom(this Vector3D v, Vector3D source)
        {
            return v.AngleFrom(source, Vector3D.BasisZ);
        }


        /// <summary>
        /// 设定一个参考平面的法向量，向量source逆时针旋转到终点向量的角度，值域为[0 ,2π)
        /// </summary>
        /// <param name="v"></param>
        /// <param name="source"></param>
        /// <param name="refNormal">参考平面的法向量，与视线看过去的方向相反</param>
        /// <returns></returns>
        public static double AngleFrom(this Vector3D v, Vector3D source, Vector3D refNormal)
        {
            var angle = v.AngleTo(source);
            if (angle.AreEqual(0))
                return 0;
            if (v.Cross(source).AngleTo(refNormal) < Math.PI / 2)
                return 2 * Math.PI - angle;
            return angle;
        }

        /// <summary>
        /// 判断是否在line3D上，包括端点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line3D"></param>
        /// <returns></returns>
        public static bool IsOnLine(this Vector3D point, Line3D line3D, double tolerance = SMALL_NUMBER)
        {
            var t = line3D.ClosestParameter(point);
            if (t >= -SMALL_NUMBER && t <= 1 + SMALL_NUMBER)
                return line3D.Evaluate(t).Distance(point) < tolerance;
            return false;
            //return point.Distance(line3D).AreEqual(0);
        }

        /// <summary>
        /// 判断线段是否经过一个点
        /// </summary>
        /// <param name="line"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool IsPassPoint(this Line3D line, Vector3D v)
        {
            Vector3D dir1 = (line.Start - v).Normalize();
            Vector3D dir2 = (line.End - v).Normalize();

            if (dir1.IsAlmostEqualTo(dir2) || dir1.IsAlmostEqualTo(-dir2))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断是否在line3D上，不包括端点
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line3D"></param>
        /// <returns></returns>
        public static bool IsInLine(this Vector3D point, Line3D line3D)
        {
            return point.IsOnLine(line3D) && !point.IsAlmostEqualTo(line3D.Start) && !point.IsAlmostEqualTo(line3D.End);
        }
        /// <summary>
        /// 是否同时在两条线上
        /// </summary>
        /// <param name="point"></param>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool IsOnTwoLine(this Vector3D point, Line3D line1, Line3D line2)
        {
            return point.IsOnLine(line1) && point.IsOnLine(line2);
        }


        /// <summary>
        /// 点到线段的最近距离(不一定是垂直距离)
        /// </summary>
        /// <param name="line3D"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double Distance(this Vector3D source, Line3D line3D)
        {
            double space = 0;
            double a, b, c;
            a = line3D.Start.Distance(line3D.End);
            b = line3D.Start.Distance(source);
            c = line3D.End.Distance(source);
            if (c < Extension.SMALL_NUMBER || b < Extension.SMALL_NUMBER)
            {
                return space;
            }
            if (a < Extension.SMALL_NUMBER)
            {
                space = b;
                return space;
            }
            if (c * c >= a * a + b * b)
            {
                space = b;
                return space;
            }
            if (b * b >= a * a + c * c)
            {
                space = c;
                return space;
            }
            Vector3D v1 = new Vector3D(source.X, source.Y, source.Z);
            Line3D l1 = Line3D.Create(new Vector3D(line3D.Start.X, line3D.Start.Y, line3D.Start.Z), new Vector3D(line3D.End.X, line3D.End.Y, line3D.End.Z));
            space = Line3D.Create(v1, v1.ProjectOn(l1)).Length;
            return space;
        }

        /// <summary>
        /// 点到面的最近距离，即为点到面区域内最近的距离
        /// </summary>
        /// <param name="source"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public static double Distance(this Vector3D source, Face face)
        {
            double distance = double.MaxValue;
            Vector3D projectedPoint = source.ProjectOn(face);

            List<Line2D> projectedEdges = new List<Line2D>();
            face.Edges.ForEach(x =>
            {
                projectedEdges.Add(x.Line3D2Line2D());
            });

            Vector2D point = projectedPoint.Vector3D2Vector2D();
            bool result = point.IsPossiblyInRegion(projectedEdges);

#if DEBUG
            InRegionAlgorithm algorithm = new InRegionAlgorithm();
            int result2 = algorithm.Check(projectedPoint, face.Edges);
            if (result2 == -1 || result2 == 1)
            {
                double distance2 = source.Distance(projectedPoint);
            }
#endif

            if (result)
            {
                distance = source.Distance(projectedPoint);
            }
            else
            {
                foreach (var edge in face.Edges)
                {
                    double tempDistance = source.Distance(edge);
                    if (tempDistance < distance)
                        distance = tempDistance;
                }
            }
            return distance;
        }

        /// <summary>
        /// 判断点是否在区域内（包括点是否在边上）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static bool IsPointOnRegion(this Vector3D point, List<Line3D> region)
        {
            if (point == null)
                throw new ArgumentException("参数值为空。", nameof(point));
            if (region == null || region.Count == 0)
                throw new ArgumentException("参数值为空。", nameof(region));

            //三维坐标转换为局部三维坐标

            //获得多边形的法向量
            Vector3D normal = region.GetNormalInConvexPoint();
            Vector3D orgin = region.First().Start;
            Vector3D basisX = region.First().Direction;
            Vector3D basisZ = normal;
            Vector3D basisY = basisZ.Cross(basisX).Normalize();
            var matrix = Alternation.TransformUtil.GetMatrix(orgin, basisX, basisY);

            //坐标转换，全局三维转局部二维
            region = Alternation.TransformUtil.TransformLines(matrix, region);


            //三维线段转二维线段
            List<Line2D> lines = new List<Line2D>();
            region.ForEach(x => lines.Add(x.Line3D2Line2D()));

            //三维点转二维点
            point = Alternation.TransformUtil.TransformPoint(matrix, point);
            Vector2D point2D = point.Vector3D2Vector2D();

            bool result = point2D.IsPossiblyInRegion(lines) || point2D.IsOnRegionEdge(lines);
            return result;
        }


        /// <summary>
        /// 获取两点之间的水平距离
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static double HorizontalDistanceOfPoints(this Vector3D start, Vector3D end)
        {
            Vector3D v1 = new Vector3D(start.X, start.Y, 0);
            Vector3D v2 = new Vector3D(end.X, end.Y, 0);
            return (v1 - v2).Modulus();
        }
        ///// <summary>
        ///// 给在同一直线上的点，按照坐标从小到大排序，并按照X、Y、Z的优先级依次排列
        ///// </summary>
        ///// <param name="sourcePoints"></param>
        ///// <returns></returns>
        //public static List<Vector3D> OrderByXYZ(this List<Vector3D> sourcePoints)
        //{
        //    Vector3D direction = null;
        //    if (sourcePoints.Count >= 2)
        //    {
        //        for (int i = 1; i < sourcePoints.Count; i++)
        //        {
        //            direction = (sourcePoints[i] - sourcePoints[0]).Normalize();
        //            if (!direction.IsAlmostEqualTo(Vector3D.Zero))
        //                break;
        //        }
        //    }
        //    if (direction != null && !direction.IsAlmostEqualTo(Vector3D.Zero))
        //    {
        //        if (direction.Dot(Vector3D.BasisX).AreEqual(0))
        //        {
        //            //对Y值的排序有问题
        //            if (direction.IsAlmostEqualTo(Vector3D.BasisY) || direction.IsAlmostEqualTo(-Vector3D.BasisY))
        //                sourcePoints = sourcePoints.OrderBy(x => x.Y).ToList();
        //            else
        //                sourcePoints = sourcePoints.OrderBy(x => x.Z).ToList();
        //        }
        //        else
        //        {
        //            sourcePoints = sourcePoints.OrderBy(x => x.X).ToList();
        //        }
        //    }
        //    return sourcePoints;
        //}

        /// <summary>
        /// 给同一直线上的点，按照坐标从小到大排序，并按照X、Y、Z的优先级依次排列（修改版本）
        /// 如果是不在同一直线，但是同一平面，获得的direction不准确（可以考虑用法线判断）
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <returns></returns>
        public static List<Vector3D> OrderByXYZ(this List<Vector3D> sourcePoints)
        {
            Vector3D direction = null;
            if (sourcePoints.Count >= 2)
            {
                for (int i = 1; i < sourcePoints.Count; i++)
                {
                    direction = (sourcePoints[i] - sourcePoints[0]).Normalize();
                    if (!direction.IsAlmostEqualTo(Vector3D.Zero))
                        break;
                }
            }
            if (direction != null && !direction.IsAlmostEqualTo(Vector3D.Zero))
            {
                if (direction.Dot(Vector3D.BasisX).AreEqual(0))
                {
                    if (direction.Dot(Vector3D.BasisY).AreEqual(0))
                        sourcePoints = sourcePoints.OrderBy(x => x.Z).ToList();
                    else
                        sourcePoints = sourcePoints.OrderBy(x => x.Y).ToList();
                }
                else
                {
                    sourcePoints = sourcePoints.OrderBy(x => x.X).ToList();
                }
            }
            return sourcePoints;
        }

        /// <summary>
        /// 给同一条直线的点，按照线的方向排序
        /// </summary>
        /// <param name="sourcePoints"></param>
        /// <param name="lineDirection"></param>
        /// <returns></returns>
        public static List<Vector3D> OrderByDirection(this List<Vector3D> sourcePoints, Vector3D lineDirection)
        {
            sourcePoints = sourcePoints.OrderByXYZ();
            if (sourcePoints.Count >= 2)
            {
                Vector3D direction = (sourcePoints.Last() - sourcePoints.First()).Normalize();
                if (direction.IsAlmostEqualTo(-lineDirection))
                    sourcePoints.Reverse();
            }
            return sourcePoints;
        }
        /// <summary>
        /// 返回点投影到直线上的点
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="line3D"></param>
        /// <returns></returns>
        public static Vector3D ProjectOn(this Vector3D v1, Line3D line3D)
        {
            //算法说明：先过v1点做垂直于line3D的平面，得到其方程，
            //求出该平面与line3D的交点即为投影点
            var s = line3D.Start;
            var d = line3D.Direction;
            var t = ((v1.X - s.X) * d.X + (v1.Y - s.Y) * d.Y + (v1.Z - s.Z) * d.Z) / (d.X * d.X + d.Y * d.Y + d.Z * d.Z);
            return new Vector3D(s.X + d.X * t, s.Y + d.Y * t, s.Z + d.Z * t);
        }




        /// <summary>
        /// 点在面上
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Vector3D ProjectOn(this Vector3D v1, Face face)
        {
            Vector3D point = face.Edges.First().Start;
            Line3D sourceLine = new Line3D(v1, v1 + face.Normal * 10);
            Vector3D projectedPoint = point.ProjectOn(sourceLine);
            return projectedPoint;
        }

        /// <summary>
        /// 点投影在面上
        /// </summary>
        /// <param name="v1">要投影的点</param>
        /// <param name="origin">投影平面的原点</param>
        /// <param name="normal">投影平面的法向量</param>
        /// <returns></returns>
        public static Vector3D ProjectOn(this Vector3D v1, Vector3D origin, Vector3D normal)
        {
            Line3D sourceLine = new Line3D(v1, v1 + normal * 10);
            Vector3D projectedPoint = origin.ProjectOn(sourceLine);
            return projectedPoint;
        }

        /// <summary>
        /// 线在面上
        /// </summary>
        /// <param name="line"></param>
        /// <param name="face"></param>
        /// <returns></returns>
        public static Line3D ProjectOn(this Line3D line, Face face)
        {
            Vector3D start = line.Start.ProjectOn(face);
            Vector3D end = line.End.ProjectOn(face);
            return new Line3D(start, end);
        }

        /// <summary>
        /// 线段投影在面上
        /// </summary>
        /// <param name="line">要投影的线段</param>
        /// <param name="origin">投影平面的原点</param>
        /// <param name="normal">投影平面的法向量</param>
        /// <returns></returns>
        public static Line3D ProjectOn(this Line3D line, Vector3D origin, Vector3D normal)
        {
            Vector3D start = line.Start.ProjectOn(origin, normal);
            Vector3D end = line.End.ProjectOn(origin, normal);
            return new Line3D(start, end);
        }


        /// <summary>
        /// 将点影到xoy屏幕
        /// </summary>
        /// <param name="line">需要投影的线</param>
        /// <returns></returns>
        public static Vector3D ProjectOnXoY(this Vector3D point)
        {
            return new Vector3D(point.X, point.Y, 0);
        }

        /// <summary>
        /// 判断两个向量是否平行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsParallel(this Vector3D source, Vector3D target)
        {
            if (source.Normalize().IsAlmostEqualTo(target.Normalize()) || source.Normalize().IsAlmostEqualTo(-target.Normalize()))
                return true;
            return false;
        }

        /// <summary>
        /// 判断两个条线段是否平行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsParallel(this Line3D source, Line3D target)
        {
            Vector3D sourceDir = source.Direction;
            Vector3D targetDir = target.Direction;
            return sourceDir.IsParallel(targetDir);
        }

        #endregion

        #region Line2D相关
        /// <summary>
        /// 过一个点沿direction做直线（长度为1e7）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Line2D CreateLine(this Vector2D point, Vector2D direction)
        {
            Vector2D p0 = point.MoveTo(1e7, -direction);
            Vector2D p1 = point.MoveTo(1e7, direction);
            return Line2D.Create(p0, p1);
        }
        ///// <summary>
        ///// 线段相互打断，分解成最小单元
        ///// </summary>
        ///// <param name="lines"></param>
        ///// <returns></returns>
        //public static List<Line2D> Decompose(this List<Line2D> lines)
        //{
        //    List<Line2D> newLines = new List<Line2D>(lines);
        //    bool b = false;
        //    int i = 0;
        //    while (!b)
        //    {
        //        int num = 0;
        //        Line2D c1 = newLines[i];
        //        for (int j = newLines.Count - 1; j > i; j--)
        //        {
        //            Line2D c2 = newLines[j];
        //            Vector2D point = c1.Intersect(c2);
        //            if (point != null)
        //            {
        //                if (!point.IsEndPoint(c1))
        //                {
        //                    num++;
        //                    newLines.Remove(c1);
        //                    newLines.Add(Line2D.Create(c1.Start, point));
        //                    newLines.Add(Line2D.Create(point, c1.End));
        //                }
        //                if (!point.IsEndPoint(c2))
        //                {
        //                    num++;
        //                    newLines.Remove(c2);
        //                    newLines.Add(Line2D.Create(c2.Start, point));
        //                    newLines.Add(Line2D.Create(point, c2.End));
        //                }
        //            }
        //            if (num != 0)
        //                break;
        //        }
        //        if (num == 0)
        //        {
        //            if (i == newLines.Count - 1)
        //                b = true;
        //            i++;
        //        }
        //        else
        //            i = 0;
        //    }//以上步骤是把多片墙体相交分成单独一段
        //    return newLines;
        //}

        /// <summary>
        /// 线段相互打断，分解成最小单元
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line2D> Decompose(this List<Line2D> lines)
        {
            List<Line2D> newLines = new List<Line2D>(lines);
            bool b = false;
            int i = 0;
            while (!b)
            {
                int num = 0;
                Line2D c1 = newLines[i];
                for (int j = newLines.Count - 1; j > i; j--)
                {
                    Line2D c2 = newLines[j];
                    Vector2D point = c1.Intersect(c2);
                    if (point != null)
                    {
                        if (!point.IsEndPoint(c1))
                        {
                            num++;
                            newLines.Remove(c1);
                            newLines.Add(Line2D.Create(c1.Start, point));
                            newLines.Add(Line2D.Create(point, c1.End));
                        }
                        if (!point.IsEndPoint(c2))
                        {
                            num++;
                            newLines.Remove(c2);
                            newLines.Add(Line2D.Create(c2.Start, point));
                            newLines.Add(Line2D.Create(point, c2.End));
                        }
                    }
                    if (num != 0)
                        break;
                }
                if (num == 0)
                {
                    if (i == newLines.Count - 1)
                        b = true;
                    i++;
                }
                else
                    i = 0;
            }//以上步骤是把多片墙体相交分成单独一段
            return newLines;
        }

        public static List<Line2D> Distinct(this List<Line2D> lines)
        {
            if (lines.Count == 0)
                return lines;
            var newlines = new List<Line2D> { lines[0] };
            lines.ForEach(x => { if (!newlines.Any(y => y.IsAlmostEqualTo(x))) newlines.Add(x); });
            return newlines;
        }

        /// <summary>
        /// 返回直线被点分割后的线段
        /// </summary>
        /// <param name="line2D"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<Line2D> DivideViaPoints(this Line2D line2D, List<Vector2D> points)
        {
            List<Line2D> dividedCurves = new List<Line2D>();
            if (points == null || points.Count == 0 || points.Any(x => !x.IsOnLine(line2D)))
            {
                dividedCurves.Add(line2D);
            }
            else
            {
                List<Vector2D> allPoints = new List<Vector2D> { line2D.Start, line2D.End };
                allPoints.AddRange(points);
                allPoints = allPoints.DistinctPoint();
                allPoints = allPoints.OrderBy(x => x.Distance(line2D.Start)).ToList();
                for (int i = 0; i < allPoints.Count - 1; i++)
                {
                    dividedCurves.Add(Line2D.Create(allPoints[i], allPoints[i + 1]));
                }
            }
            return dividedCurves;
        }
        /// <summary>
        /// 偏移值若为正向外偏移，若为负向内偏移。
        /// </summary>
        /// <param name="startExtend"></param>
        /// <param name="endExtend"></param>
        /// <returns></returns>
        public static Line2D Extend(this Line2D line2D, double startExtend, double endExtend)
        {
            Vector2D newStart = new Vector2D(0, 0);
            Vector2D newEnd = new Vector2D(0, 0);
            if (startExtend > 0)
            {
                newStart = line2D.Start - line2D.Direction * startExtend;
            }
            else
            {
                newStart = line2D.Start + line2D.Direction * Math.Abs(startExtend);
            }
            if (endExtend > 0)
            {
                newEnd = line2D.End + line2D.Direction * endExtend;
            }
            else
            {
                newEnd = line2D.End - line2D.Direction * Math.Abs(endExtend);
            }
            return Line2D.Create(newStart, newEnd);
        }

        /// <summary>
        /// 从逆时针转成顺时针或相反
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line2D> Flip(this List<Line2D> lines)
        {
            List<Line2D> newLines = new List<Line2D> { Line2D.Create(lines[0].End, lines[0].Start) };
            for (int i = lines.Count - 1; i > 0; i--)
            {
                newLines.Add(Line2D.Create(lines[i].End, lines[i].Start));
            }
            return newLines;
        }

        /// <summary>
        /// 获得直线的端点，无重复
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static Vector2D[] GetEndPoints(this IEnumerable<Line2D> lines)
        {
            List<Vector2D> newPoints = new List<Vector2D>();
            foreach (var item in lines)
            {
                newPoints.Add(item.Start);
                newPoints.Add(item.End);
            }
            return newPoints.DistinctPoint().ToArray();
        }

        public static List<Line2D> GetOutline(this List<Line2D> lines, List<Line2D> outLines)
        {
            List<Line2D> removeLines = new List<Line2D>();
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < outLines.Count; j++)
                {
                    GraphicAlgorithm.TwoParallelLines2D two = new GraphicAlgorithm.TwoParallelLines2D(lines[i], outLines[j]);
                    if (two.Relationship == GraphicAlgorithm.TwoParallelLinesRelationship.线段1全在线段2内 ||
                        two.Relationship == GraphicAlgorithm.TwoParallelLinesRelationship.完全相同)
                    {
                        removeLines.Add(lines[i]);
                    }
                }
            }
            return removeLines;
        }


        /// <summary>
        /// 判断是否与line共线
        /// </summary>
        /// <param name="source"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsCollinearWith(this Line2D source, Line2D line)
        {
            if (!source.IsParallelWith(line))
                return false;
            Vector2D point = source.Start.ProjectOn(line);
            return point.Distance(source.Start) < SMALL_NUMBER;
        }

        //判断一个线是否在一个区域之内
        public static bool IsInRegion(this Line2D line, List<Line2D> region)
        {
            //是否具有相交点
            bool isIntersect = region.Any(x => x.Intersect(line) != null);
            //开始点是否在区域内
            bool isStartInRegion = line.Start.IsInRegion(region);
            //结束点是否在区域内
            bool isEndInRegion = line.End.IsInRegion(region);
            //是否开始点在边缘线上
            bool isStartOnRegionEdge = line.Start.IsOnRegionEdge(region);
            //是否结束点在边缘线上
            bool isEndOnRegionEdge = line.End.IsOnRegionEdge(region);
            //终点再区域内
            bool isMiddeInregion = line.MiddlePoint.IsInRegion(region);
            //是否和边缘线叠加
            bool isSuperposition = line.IsSuperpositionWithRegionEdge(region);
            int intersectN = 0;
            List<Vector2D> intersect = new List<Vector2D>();
            //判断相交的线段
            region.ForEach(x =>
            {
                Vector2D v1 = x.Intersect(line);
                if (v1 != null && intersect.Find(y => y.IsAlmostEqualTo(v1)) == null)
                {
                    intersect.Add(v1);
                }
            });
            intersectN = intersect.Count;
            //都在区域内，没有交点，则在区域内
            if (isStartInRegion && isEndInRegion && !isIntersect)
                return true;
            //都在线上，有两个交点，且不重叠，则在区域内
            if (isStartOnRegionEdge && isEndOnRegionEdge && !isSuperposition && isMiddeInregion)
                return true;
            //有一个在区域内，一个在线上，且不重叠，有一个交点
            if (isStartInRegion && isEndOnRegionEdge && !isSuperposition && intersectN == 1)
                return true;
            if (isStartOnRegionEdge && isEndInRegion && !isSuperposition && intersectN == 1)
                return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool IsCounterclockwise(this List<Line2D> lines)
        {
            List<Line2D> newLines = lines.OrderBy(x => x.Start.X).ToList();
            Vector2D convexPoint = newLines.First().Start;
            Vector2D dir1 = (from line in lines where line.End.IsAlmostEqualTo(convexPoint) select line.Direction).FirstOrDefault();
            Vector2D dir2 = (from line in lines where line.Start.IsAlmostEqualTo(convexPoint) select line.Direction).FirstOrDefault();
            if (dir1 == null || dir2 == null)
                throw new ArgumentException("输入的参数不是首尾相接的闭合线段", nameof(lines));
            return dir1 != null && dir1.Cross(dir2) > SMALL_NUMBER;
        }

        /// <summary>
        /// 完全在区域边内部或跟边相同
        /// </summary>
        /// <param name="line2D"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static bool IsOnRegionEdge(this Line2D line2D, List<Line2D> region)
        {
            bool b = false;
            foreach (var item in region)
            {
                GraphicAlgorithm.TwoParallelLines2D lines = new GraphicAlgorithm.TwoParallelLines2D(line2D, item);
                if (lines.Relationship == GraphicAlgorithm.TwoParallelLinesRelationship.线段1全在线段2内 ||
                    lines.Relationship == GraphicAlgorithm.TwoParallelLinesRelationship.完全相同)
                {
                    b = true;
                    break;
                }
            }
            return b;
        }
        /// <summary>
        /// 一部分在region内
        /// </summary>
        /// <param name="line2D"></param>
        /// <param name="region"></param>
        /// <param name="intersections"></param>
        /// <returns>传出与region的交点</returns>
        private static bool IsPartInRegion(this Line2D line2D, List<Line2D> region, out List<Vector2D> intersections)
        {
            intersections = new List<Vector2D>();
            bool isIntersect = region.Any(x => line2D.Intersect(x) != null);
            bool isInRegion = line2D.IsInRegion(region);
            bool isPartOnedge = line2D.IsPartOnRegionEdge(region);
            bool HasPointOutside = !(line2D.Start.IsInRegion(region) || line2D.Start.IsOnRegionEdge(region)) ||
               !(line2D.End.IsInRegion(region) || line2D.End.IsOnRegionEdge(region));
            if (isIntersect && isInRegion && HasPointOutside)
            {
                intersections.AddRange(region.Select(line2D.Intersect).Where(point => point != null));
                return true;
            }
            if (isIntersect && isPartOnedge)
            {
                foreach (var item in region)
                {
                    if (item.IsParallelWith(line2D))
                        continue;
                    if (item.Intersect(line2D) != null)
                    {
                        intersections.Add(item.Intersect(line2D));
                    }
                    else
                    {
                        if (item.Distance(line2D.Start).AreEqual(0))
                            intersections.Add(line2D.Start);
                        else if (item.Distance(line2D.End).AreEqual(0))
                            intersections.Add(line2D.End);
                        else if (line2D.Distance(item.Start).AreEqual(0))
                            intersections.Add(item.Start.ProjectOn(line2D));
                        else if (line2D.Distance(item.End).AreEqual(0))
                            intersections.Add(item.End.ProjectOn(line2D));
                    }
                }
                // intersections.AddRange(region.Select(line2D.Intersect).Where(point => point != null));
                return true;
            }

            return false;
        }

        /// <summary>
        /// 一部分在区域边界上
        /// </summary>
        /// <param name="line2D"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public static bool IsPartOnRegionEdge(this Line2D line2D, List<Line2D> region)
        {
            bool isOn = false;
            foreach (var item in region)
            {
                GraphicAlgorithm.TwoParallelLines2D lines = new GraphicAlgorithm.TwoParallelLines2D(line2D, item);
                if (lines.Relationship == GraphicAlgorithm.TwoParallelLinesRelationship.两条线段部分搭接
                    || lines.Relationship == GraphicAlgorithm.TwoParallelLinesRelationship.线段2全在线段1内)
                {
                    isOn = true;
                    break;
                }
            }
            return isOn;
        }
        /// <summary>
        /// 一部分在region内或边上，一部分在region外，不包含只有一个点在边界上的情况
        /// </summary>
        /// <param name="line2D"></param>
        /// <param name="region"></param>
        /// <param name="innerLine"></param>
        /// <returns>传出region内部的线段</returns>
        public static bool IsPartInRegion(this Line2D line2D, List<Line2D> region, out List<Line2D> innerLine)
        {
            innerLine = new List<Line2D>();
            bool isPartIn = false;
            bool b = line2D.IsPartInRegion(region, out List<Vector2D> intersections);
            if (b)
            {
                List<Line2D> cutedLines = line2D.DivideViaPoints(intersections);
                foreach (var item in cutedLines)
                {
                    if (item.IsInRegion(region))
                    {
                        innerLine.Add(item);
                        isPartIn = true;
                    }
                }
            }
            return isPartIn;
        }

        /// <summary>
        /// 一部分在region内或边上，一部分在region外，不包含只有一个点在边界上的情况
        /// </summary>
        /// <param name="line2D"></param>
        /// <param name="region"></param>
        /// <param name="innerLine"></param>
        /// <returns>传出在区域边界的部分</returns>
        public static bool IsPartOnRegionEdge(this Line2D line2D, List<Line2D> region, out List<Line2D> innerLine)
        {
            innerLine = new List<Line2D>();
            bool isPartOn = false;
            bool b = line2D.IsPartInRegion(region, out List<Vector2D> intersections);
            if (b)
            {
                List<Line2D> cutedLines = line2D.DivideViaPoints(intersections);
                foreach (var item in cutedLines)
                {
                    if (!item.IsInRegion(region))
                    {
                        innerLine.Add(item);
                        isPartOn = true;
                    }
                }
            }
            return isPartOn;
        }


        public static bool IsPartInRegion(this Line2D line2D, List<Line2D> region)
        {
            bool isPartIn = false;
            bool b = line2D.IsPartInRegion(region, out List<Vector2D> intersections);
            if (b)
            {
                List<Line2D> cutedLines = line2D.DivideViaPoints(intersections);
                foreach (var item in cutedLines)
                {
                    if (item.IsInRegion(region))
                    {
                        isPartIn = true;
                    }
                }
            }
            return isPartIn;
        }

        /// <summary>
        /// 是否和当前边缘线叠加
        /// </summary>
        /// <param name="line"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        private static bool IsSuperpositionWithRegionEdge(this Line2D line, List<Line2D> region)
        {
            foreach (var item in region)
            {
                GraphicAlgorithm.TwoParallelLines2D two = new GraphicAlgorithm.TwoParallelLines2D(line, item);
                GraphicAlgorithm.TwoParallelLinesRelationship s = two.Relationship;
                if (s == GraphicAlgorithm.TwoParallelLinesRelationship.两条线段部分搭接 ||
                    s == GraphicAlgorithm.TwoParallelLinesRelationship.完全相同 ||
                    s == GraphicAlgorithm.TwoParallelLinesRelationship.线段2全在线段1内 ||
                    s == GraphicAlgorithm.TwoParallelLinesRelationship.线段1全在线段2内)

                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否与line平行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsParallelWith(this Line2D source, Line2D line)
        {
            Vector2D d1 = source.Direction;
            Vector2D d2 = line.Direction;
            double angle = d1.AngleTo(d2);
            return Math.Abs(angle) < SMALL_NUMBER || Math.Abs(angle - Math.PI) < SMALL_NUMBER;
        }

        /// <summary>
        /// 一个线属于例外一个线的一部分，//line和source调换位置，让它符合命名，调用它的代码也做了相应修改，2017-6-9-ByJohnny
        /// </summary>
        /// <param name="source"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsPartOf(this Line2D line, Line2D source)
        {
            if (line.Start.IsOnLine(source) && line.End.IsOnLine(source))
            {

                return true;
            }
            return false;
        }
        public static List<Line2D> MakeCounterclockwise(this List<Line2D> lines)
        {
            GraphicAlgorithm.MergeLine(lines);
            List<Line2D> sortedLines = new List<Line2D> {
                //添加所有BaseLine为线的起点
                lines.First()
            };
            Vector2D endPoint = lines.First().End;
            lines.RemoveAt(0);
            GraphicAlgorithm.HuntCurveByStartPoint(lines, endPoint, sortedLines);
            if (!sortedLines.IsCounterclockwise())
                sortedLines = sortedLines.Flip();
            return sortedLines;
        }

        /// <summary>
        /// 两线段的差集
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static List<Line2D> Minus(this Line2D line1, Line2D line2)
        {
            List<Line2D> minusLines = new List<Line2D>();
            GraphicAlgorithm.TwoParallelLines2D two = new GraphicAlgorithm.TwoParallelLines2D(line1, line2);
            GraphicAlgorithm.TwoParallelLinesRelationship re = two.Relationship;
            if (re == GraphicAlgorithm.TwoParallelLinesRelationship.完全相同 ||
                re == GraphicAlgorithm.TwoParallelLinesRelationship.不共线 ||
                re == GraphicAlgorithm.TwoParallelLinesRelationship.不平行 ||
                re == GraphicAlgorithm.TwoParallelLinesRelationship.两条线段端点搭接 ||
                re == GraphicAlgorithm.TwoParallelLinesRelationship.平行不共线 ||
                re == GraphicAlgorithm.TwoParallelLinesRelationship.线段1全在线段2内
                )
                return null;
            else if (re == GraphicAlgorithm.TwoParallelLinesRelationship.线段2全在线段1内)
            {
                if (Math.Abs(line1.Direction.AngleTo(line2.Direction)) < SMALL_NUMBER)
                {
                    Line2D lineTemp1 = Line2D.Create(line1.Start, line2.Start);
                    Line2D lineTemp2 = Line2D.Create(line2.End, line1.End);
                    if (lineTemp1.Length > SMALL_NUMBER)
                        minusLines.Add(lineTemp1);
                    if (lineTemp2.Length > SMALL_NUMBER)
                        minusLines.Add(lineTemp2);
                }
                else
                {
                    Line2D line22 = line2.CreateReversed();
                    Line2D lineTemp1 = Line2D.Create(line1.Start, line22.Start);
                    Line2D lineTemp2 = Line2D.Create(line22.End, line1.End);
                    if (lineTemp1.Length > SMALL_NUMBER)
                        minusLines.Add(lineTemp1);
                    if (lineTemp2.Length > SMALL_NUMBER)
                        minusLines.Add(lineTemp2);
                }
            }
            else if (re == GraphicAlgorithm.TwoParallelLinesRelationship.两条线段部分搭接)
            {
                if (Math.Abs(line1.Direction.AngleTo(line2.Direction)) < SMALL_NUMBER)
                {
                    if (line2.Start.IsOnLine(line1))
                        minusLines.Add(Line2D.Create(line1.Start, line2.Start));
                    else
                        minusLines.Add(Line2D.Create(line1.End, line2.End));
                }
                else
                {
                    if (line2.Start.IsOnLine(line1))
                        minusLines.Add(Line2D.Create(line1.Start, line2.End));
                    else
                        minusLines.Add(Line2D.Create(line1.End, line2.Start));
                }
            }
            return minusLines;
        }

        /// <summary>
        /// 拷贝一个线
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Line2D Copy(this Line2D line)
        {
            Line2D copy = Line2D.Create(Vector2D.Create(line.Start.X, line.Start.Y), Vector2D.Create(line.End.X, line.End.Y));
            return copy;
        }
        /// <summary>
        /// 拷贝所有的线段
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line2D> Copy(this List<Line2D> lines)
        {

            List<Line2D> copys = new List<Line2D>();

            lines.ForEach(x =>
            {

                copys.Add(x.Copy());
            });
            return copys;
        }


        /// <summary>
        /// 按顺序排序
        /// </summary>
        /// <param name="originalLines"></param>
        /// <returns></returns>
        public static List<Line2D> SortLinesContinuously(this List<Line2D> originalLines)
        {
            List<Line3D> line3Ds = new List<Line3D>();
            originalLines.ForEach(x => line3Ds.Add(x.Line2D2Line3D()));

            line3Ds = line3Ds.SortLinesContinuously();

            List<Line2D> targetLine2ds = new List<Line2D>();
            line3Ds.ForEach(x => targetLine2ds.Add(x.Line3D2Line2D()));
            return targetLine2ds;
        }
        #endregion

        #region Ellipse2D相关
        /// <summary>
        /// 求解椭圆与线段交点;
        /// </summary>
        /// <param name="ellipse2D"></param>
        /// <param name="line2D"></param>
        /// <param name="pinIncluded">是否包括端点</param>
        /// <returns></returns>
        public static Vector2D[] IntersectWithLine(this Ellipse2D ellipse2D, Line2D line2D, bool pinIncluded = false)
        {

            if (ellipse2D == null)
            {
                throw new ArgumentNullException(nameof(ellipse2D));
            }

            if (line2D == null)
            {
                throw new ArgumentNullException(nameof(line2D));
            }

            var points = IntersectWithStraightLine(ellipse2D, line2D);

            if (pinIncluded)
            {
                return points?.Where(p => p.IsOnLine(line2D))?.ToArray() ?? null;
            }
            else
            {
                return points?.Where(p => p.IsInLine(line2D))?.ToArray() ?? null;
            }
        }

        /// <summary>
        /// 求解椭圆与直线交点;
        /// </summary>
        /// <param name="ellipse2D"></param>
        /// <param name="line2D"></param>
        /// <returns></returns>
        public static Vector2D[] IntersectWithStraightLine(this Ellipse2D ellipse2D, Line2D line2D)
        {

            if (ellipse2D == null)
            {
                throw new ArgumentNullException(nameof(ellipse2D));
            }

            if (line2D == null)
            {
                throw new ArgumentNullException(nameof(line2D));
            }

            //若椭圆的横/纵半径为零,不能相交;
            if (ellipse2D.RadiusX == 0 || ellipse2D.RadiusY == 0)
            {
                return null;
            }

            //若线段的重点与起点重合,则不能相交;
            if ((line2D.Start.IsAlmostEqualTo(line2D.End)))
            {
                return null;
            }

            ///以下注释中:
            ///其中a=<see cref="Ellipse2D.RadiusX"/>,b = <see cref="Ellipse2D.RadiusY"/>
            ///x_c = <see cref="Ellipse2D.Center.X"/>,y_c = <see cref="Ellipse2D.Center.Y"/>

            var a = ellipse2D.RadiusX;
            var b = ellipse2D.RadiusY;
            var x_c = ellipse2D.Center.X;
            var y_c = ellipse2D.Center.Y;


            //若线段的起始点与终止点的横坐标相等,则该直线的斜率不存在,将做特殊处理;
            if (line2D.Start.X == line2D.End.X)
            {
                //可用x = m表示该直线;
                var m = line2D.Start.X;

                ///经计算总结,y = Math.Sqrt(a ^ 2 - (m - x_c)^2) * b / a + y_c;
                //查看a^2 - (m - x_c)^2是否小于零;
                var numberToBeSqrt = a * a - (m - x_c) * (m - x_c);
                if (numberToBeSqrt < 0)
                {
                    return null;
                }
                //若为零,则直线与椭圆相切,y = y_c;
                else if (numberToBeSqrt == 0)
                {
                    return new Vector2D[] {
                        new Vector2D(m,y_c)
                    };
                }
                ///否则相交,y = y_c±(Math.Sqrt(numberToBeSqrt) * b / a);
                else
                {
                    var addOrReduceNum = Math.Sqrt(numberToBeSqrt) * b / a;
                    return new Vector2D[] {
                        new Vector2D(m,y_c + addOrReduceNum),
                        new Vector2D(m,y_c - addOrReduceNum)
                    };
                }
            }
            //若斜率存在,则直接代入方程计算;
            else
            {
                //使用斜率式y = kx + m;代入求解;
                var k = (line2D.Start.Y - line2D.End.Y) / (line2D.Start.X - line2D.End.X);
                var m = line2D.Start.Y - k * line2D.Start.X;

                //经计算总结('$'代表平方,x_c):;
                //x = -((m-y_c)ka$ - x_cb$) ±  sqrt(((m-y_c)ka$ - x_cb$)$ - (b$ + a$k$)(b$(x_c$ - a$) + a$(m-y_c)$))) /  (b$ + a$k$);
                var num1 = ((m - y_c) * k * a * a - x_c * b * b);
                var num2 = m - y_c;
                var numberToBeSqrt = num1 * num1 -
                    (b * b + a * a * k * k) * (b * b * (x_c * x_c - a * a) + a * a * num2 * num2);

                if (numberToBeSqrt < 0)
                {
                    return null;
                }
                else
                {
                    var negB = -((m - y_c) * k * a * a - x_c * b * b);
                    var a_2 = (b * b + a * a * k * k);

                    if (numberToBeSqrt == 0)
                    {
                        var x = negB / a_2;
                        return new Vector2D[]{
                            new Vector2D(x,k * x + m)
                        };
                    }
                    else
                    {
                        var sqrtedNumber = Math.Sqrt(numberToBeSqrt);

                        var x1 = (negB + sqrtedNumber) / a_2;
                        var x2 = (negB - sqrtedNumber) / a_2;

                        return new Vector2D[]{
                            new Vector2D(x1,k * x1 + m),
                            new Vector2D(x2,k * x2 + m)
                        };
                    }
                }

            }

        }

        /// <summary>
        /// 判断某个位置是否在椭圆上;
        /// </summary>
        /// <param name="ellipse2D"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool IsOnEllipse(this Vector2D point, Ellipse2D ellipse2D)
        {

            if (ellipse2D == null)
            {
                throw new ArgumentNullException(nameof(ellipse2D));
            }

            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            return ellipse2D.RangeOf(point) == RangeFlag.OnEdge;
        }

        /// <summary>
        /// 判断某个位置是否在椭圆内;
        /// </summary>
        /// <param name="point"></param>
        /// <param name="ellipse2D"></param>
        /// <returns></returns>
        public static bool IsInEllipse(this Vector2D point, Ellipse2D ellipse2D)
        {
            if (ellipse2D == null)
            {
                throw new ArgumentNullException(nameof(ellipse2D));
            }


            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            return ellipse2D.RangeOf(point) == RangeFlag.Inside;
        }

        /// <summary>
        /// 表示在点与封闭图形的包含情况;
        /// </summary>
        enum RangeFlag
        {
            //内部;
            Inside,
            //处于边界;
            OnEdge,
            //在外部;
            Outsize
        }

        /// <summary>
        /// 判断点与椭圆的包含情况,内部使用;
        /// </summary>
        /// <param name="point"></param>
        /// <param name="ellipse2D"></param>
        /// <returns></returns>
        private static RangeFlag RangeOf(this Ellipse2D ellipse2D, Vector2D point)
        {
            if (ellipse2D == null)
            {
                throw new ArgumentNullException(nameof(ellipse2D));
            }


            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            var a = ellipse2D.RadiusX;
            var b = ellipse2D.RadiusY;
            var x = point.X - ellipse2D.Center.X;
            var y = point.Y - ellipse2D.Center.Y;

            var val = (a * a * y * y + b * b * x * x);
            var constraince = a * a * b * b;
            if (val.AreEqual(constraince))
            {
                return RangeFlag.OnEdge;
            }
            return val < constraince ? RangeFlag.Inside : RangeFlag.Outsize;
        }

        #endregion

        #region Line3D相关

        /// <summary>
        /// 求两条直线（有端点的直线）之间的垂直距离
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static double VerticalDistanceTo(this Line3D line1, Line3D line2)
        {
            if (line1.IsParallelWith(line2))
            {
                var p = line1.Start.ProjectOn(line2);
                return line1.Start.Distance(p);
            }
            var intersect = line1.IntersectStraightLine(line2);
            if (intersect == null)//肯定为异面直线
            {
                Vector3D normal = line1.Direction.Cross(line2.Direction);
                return Math.Abs((line1.Start - line2.Start).Dot(normal)) / normal.Modulus();
            }
            throw new ArgumentException("两直线相交，无法求得之间的距离");
        }

        /// <summary>
        /// 求点与直线的垂直距离
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public static double VerticalDistanceTo(this Line3D line1, Vector3D p)
        {
            var point = p.ProjectOn(line1);

            return p.Distance(point);
        }




        /// <summary>
        /// 偏移值若为正向外偏移，若为负向内偏移。
        /// </summary>
        /// <param name="line3D"></param>
        /// <param name="startExtend"></param>
        /// <param name="endExtend"></param>
        /// <returns></returns>
        public static Line3D Extend(this Line3D line3D, double startExtend, double endExtend)
        {
            Vector3D newStart;
            Vector3D newEnd;
            if (startExtend >= 0)
            {
                newStart = line3D.Start - line3D.Direction * startExtend;
            }
            else
            {
                newStart = line3D.Start + line3D.Direction * Math.Abs(startExtend);
            }
            if (endExtend >= 0)
            {
                newEnd = line3D.End + line3D.Direction * endExtend;
            }
            else
            {
                newEnd = line3D.End - line3D.Direction * Math.Abs(endExtend);
            }
            return Line3D.Create(newStart, newEnd);
        }


        /// <summary>
        /// 当前线是否是当前线的一部分
        /// </summary>
        /// <param name="source"></param>
        /// <param name="curve2"></param>
        /// <returns></returns>
        public static bool IsPartOf(this Line3D source, Line3D curve2)
        {
            Vector3D startPoint1 = source.Start;
            Vector3D endPoint1 = source.End;
            if (startPoint1.IsOnLine(curve2) && endPoint1.IsOnLine(curve2))
                return true;
            return false;
        }

        /// <summary>
        /// 是否重合
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CoincidesWith(this Line3D source, Line3D line3D)
        {
            return source.IsPartOf(line3D) && line3D.IsPartOf(source);
        }

        public static List<Line3D> Flip(this List<Line3D> lines)
        {
            if (lines == null || lines.Count == 0)
                return new List<Line3D>();
            List<Line3D> newLines = new List<Line3D> { Line3D.Create(lines[0].End, lines[0].Start) };
            for (int i = lines.Count - 1; i > 0; i--)
            {
                newLines.Add(Line3D.Create(lines[i].End, lines[i].Start));
            }
            return newLines;
        }

        public static bool IsEndPoint(this Line3D curve, Vector3D point)
        {
            return point.IsAlmostEqualTo(curve.Start, Extension.SMALL_NUMBER) |
                   point.IsAlmostEqualTo(curve.End, Extension.SMALL_NUMBER);
        }

        /// <summary>
        /// 创建一条新line3d，端点相反
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Line3D CreateReverse(this Line3D line)
        {
            Vector3D start = line.Start;
            Vector3D end = line.End;
            return new Line3D(new Vector3D(end.X, end.Y, end.Z), new Vector3D(start.X, start.Y, start.Z));
        }

        /// <summary>
        /// 点到空间直线的垂足
        /// </summary>
        /// <param name="line3D"></param>
        /// <param name="point">三维空间中的某点</param>
        /// <returns>返回该直线上空间点的垂足</returns>
        public static Vector3D GetPedal(this Line3D line3D, Vector3D point)
        {
            // pedal=point+((point-p0) X unitVector) X unitVectorNe)
            // P-空间中的任意一点；p0-直线上的任意一点；unitVector-直线的单位向量；pedal-垂足
            //该函数和projecton为同一功能
            Vector3D pedal = null;
            Vector3D p0 = line3D.Start;//直线上的任意一点，此时选择直线的起点
            Vector3D unitVector = line3D.Direction.Normalize();
            pedal = point + ((point - p0).Cross(unitVector).Cross(unitVector));
            return pedal;
        }

        /// <summary>
        /// 判断当前线段是否是水平的
        /// </summary>
        /// <param name="line3D"></param>
        /// <returns></returns>
        public static bool IsHorizontal(this Line3D line3D)
        {
            return line3D.End.Z.AreEqual(line3D.Start.Z);
        }

        /// <summary>
        /// 判断是否与line平行
        /// </summary>
        /// <param name="source"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsParallelWith(this Line3D source, Line3D line)
        {
            Vector3D d1 = source.Direction;
            Vector3D d2 = line.Direction;
            double angle = d1.AngleTo(d2);
            return Math.Abs(angle) < SMALL_NUMBER || Math.Abs(angle - Math.PI) < SMALL_NUMBER;
        }

        /// <summary>
        /// 判断是否与line共线
        /// </summary>
        /// <param name="source"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static bool IsCollinearWith(this Line3D source, Line3D line)
        {
            if (!source.IsParallelWith(line))
                return false;
            Vector3D point = source.Start.ProjectOn(line);
            return point.Distance(source.Start) < SMALL_NUMBER;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lines">原始线段集合</param>
        /// <param name="normal">参考平面的方向</param>
        /// <returns></returns>
        public static bool IsCounterclockwise(this List<Line3D> lines, Vector3D normal)
        {
            List<Vector3D> points = new List<Vector3D>();
            lines.ForEach(x => points.Add(x.Start));

            //找到阳角点
            points = OrderByXYZ(points);
            Vector3D convexPoint = points.First();//凸点
            Vector3D dir1 = null;
            lines.ForEach(x =>
            {
                if (x.End.IsAlmostEqualTo(convexPoint))
                    dir1 = x.Direction;
            });
            Vector3D dir2 = lines.Find(x => x.Start.IsAlmostEqualTo(convexPoint)).Direction;
            if (dir1 == null || dir2 == null)
                throw new ArgumentException("输入的参数不是首尾相接的闭合线段", nameof(lines));

            return dir1.Cross(dir2).Normalize().AngleTo(normal) < Math.PI / 2;
        }

        public static bool IsVertical(this Line3D line3D)
        {
            return line3D.End.X.AreEqual(line3D.Start.X) && line3D.End.Y.AreEqual(line3D.Start.Y);
        }

        public static bool IsIncline(this Line3D line3D)
        {
            return !line3D.IsVertical() && !line3D.IsHorizontal();
        }
        /// <summary>
        /// 将线投影到xoy屏幕
        /// </summary>
        /// <param name="line">需要投影的线</param>
        /// <returns></returns>
        public static Line3D ProjectOnXoY(this Line3D line)
        {
            Vector3D startPoint = new Vector3D(line.Start.X, line.Start.Y, 0);
            Vector3D endPoint = new Vector3D(line.End.X, line.End.Y, 0);
            return new Line3D(startPoint, endPoint);
        }
        /// <summary>
        /// Z坐标截断，变成二维坐标
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Line2D TrimZ(this Line3D line)
        {
            Vector2D startPoint = new Vector2D(line.Start.X, line.Start.Y);
            Vector2D endPoint = new Vector2D(line.End.X, line.End.Y);
            return new Line2D(startPoint, endPoint);
        }
        /// <summary>
        /// 将线投影到xoy屏幕
        /// </summary>
        /// <param name="lines">需要投影的线</param>
        /// <returns></returns>
        public static List<Line3D> ProjectOnXoY(this List<Line3D> lines)
        {
            return lines.Select(ProjectOnXoY).ToList();
        }

        /// <summary>
        /// 对线集合逆时针排序
        /// </summary>
        /// <param name="originalLines"></param>
        /// <param name="normal">参考平面的法线，与看的视线方向相反</param>
        /// <returns></returns>
        public static List<Line3D> SortLinesByCounterClockwise(this List<Line3D> originalLines, Vector3D normal)
        {
            List<Line3D> sortedLines = originalLines.SortLinesContinuously();
            if (!sortedLines.IsCounterclockwise(normal))
                sortedLines = sortedLines.Flip();
            return sortedLines;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalLines"></param>
        /// <returns></returns>
        public static List<Line3D> SortLinesContinuously(this List<Line3D> originalLines)
        {
            List<Line3D> copiedOriginalLines = new List<Line3D>(originalLines);
            List<Line3D> sortedLines = new List<Line3D> {
                //添加所有BaseLine为线的起点
                copiedOriginalLines.First()
            };
            Vector3D endPoint = copiedOriginalLines.First().End;
            copiedOriginalLines.RemoveAt(0);
            GraphicAlgorithm.HuntCurveByStartPoint(copiedOriginalLines, endPoint, sortedLines);

            return sortedLines;
        }
        /// <summary>
        /// 线段是否连续，首尾相接
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool IsContinuous(this List<Line3D> lines)
        {
            bool b = lines.Last().End.IsAlmostEqualTo(lines.First().Start);
            if (b)
            {
                for (int i = 0; i < lines.Count - 1; i++)
                {
                    if (!lines[i].End.IsAlmostEqualTo(lines[i + 1].Start))
                    {
                        b = false;
                        break;
                    }
                }
            }
            return b;
        }
        /// <summary>
        /// 以第i个元素为起点，重新排序
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static void ReSort(this List<Line3D> lines, int i)
        {
            var newLines = new List<Line3D>(lines);
            lines.Clear();
            for (int j = i; j < newLines.Count; j++)
            {
                lines.Add(newLines[j]);
            }
            for (int j = 0; j < i; j++)
            {
                lines.Add(newLines[j]);
            }
        }

        /// <summary>
        /// 当前多线段中，是否包含指定的线段集合中一条线段
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="compare"></param>
        /// <returns></returns>
        public static Line3D Line3DsContainOneOfLine3Ds(this List<Line3D> lines, List<Line3D> compare)
        {

            foreach (Line3D x in lines)
            {
                Line3D line = compare.Find(y => y.IsAlmostEqualTo(x));

                if (line != null)
                {
                    return line;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取多边形原点
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        public static Vector3D GetOrigin(this List<Line3D> edges)
        {
            Vector3D origin = null;
            //原点
            Vector3D temp = new Vector3D(0, 0, 0);
            foreach (Line3D line in edges)
            {
                temp += line.Start;
                temp += line.End;
            }
            if (edges.Count > 0)
                origin = temp / (edges.Count * 2);
            return origin;
        }


        /// <summary>
        /// 获取规则多边形的中心点
        /// </summary>
        /// <param name="edges"></param>
        /// <returns></returns>
        public static Vector3D GetCenter(this List<Line3D> edges)
        {

            double x = 0, y = 0, z = 0;
            edges.ForEach(p =>
            {

                x += p.Start.X;
                x += p.End.X;
                y += p.Start.Y;
                y += p.End.Y;
                z += p.Start.Z;
                z += p.End.Z;
            });
            int count = edges.Count * 2;
            return new Vector3D(x / count, y / count, z / count);
        }
        /// <summary>
        /// 获取当前多边形的法向量
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static Vector3D GetNormal(this List<Line3D> lines)
        {
            Vector3D refNomal = null;
            for (int i = 0; i < lines.Count - 1; i++)
            {
                Vector3D tempNormal = lines[i].Direction.Cross(lines[i + 1].Direction).Normalize();
                if (!tempNormal.IsAlmostEqualTo(Vector3D.Zero))
                {
                    refNomal = tempNormal;
                    break;
                }
            }
            return refNomal;
        }


        /// <summary>
        /// 获得多边形在阳角点的法向量
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static Vector3D GetNormalInConvexPoint(this List<Line3D> lines)
        {
            lines = lines.SortLinesContinuously();
            List<Vector3D> points = new List<Vector3D>();
            lines.ForEach(x => points.Add(x.Start));

            //找到阳角点
            points = OrderByXYZ(points);
            Vector3D convexPoint = points.First();//凸点
            Vector3D dir1 = null; //有问题         
            lines.ForEach(x =>
            {
                if (x.End.IsAlmostEqualTo(convexPoint))
                    dir1 = x.Direction;
            });
            Vector3D dir2 = lines.Find(x => x.Start.IsAlmostEqualTo(convexPoint)).Direction;
            if (dir1 == null || dir2 == null)
                throw new ArgumentException("输入的参数不是首尾相接的闭合线段", nameof(lines));
            return dir1.Cross(dir2).Normalize();
        }

        /// <summary>
        /// 拷贝一个多边形线段
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<Line3D> Copy(this List<Line3D> lines)
        {

            List<Line3D> mergeLines = new List<Line3D>();
            //产生不相关点
            lines.ForEach(x =>
            {
                mergeLines.Add(Line3D.Create(Vector3D.Create(x.Start.X, x.Start.Y, x.Start.Z), Vector3D.Create(x.End.X, x.End.Y, x.End.Z)));
            });
            return mergeLines;
        }

        /// <summary>
        /// 拷贝一个多边形线段
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static Line3D Copy(this Line3D line)
        {

            Line3D mergeLine = null;
            //产生不相关点
            mergeLine = Line3D.Create(Vector3D.Create(line.Start.X, line.Start.Y, line.Start.Z), Vector3D.Create(line.End.X, line.End.Y, line.End.Z));

            return mergeLine;
        }


        /// <summary>
        /// 将所有的线进行偏移得到新的线段集合
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static List<Line3D> Offset(this List<Line3D> lines, double offset, Vector3D direction)
        {

            List<Line3D> offsetLines = new List<Line3D>();

            //对所有的线段进行偏移
            lines.ForEach(x =>
            {
                offsetLines.Add(x.Offset(offset, direction));
            });
            //返回偏移线段
            return offsetLines;
        }



        #endregion

        #region 面相关

        //获取一个面的法线方向
        public static Vector3D PNormal(this Face f)
        {

            List<Line3D> edges = new List<Line3D>(f.Edges);
            //任意取两个共点的线
            Line3D L1 = f.Edges[0];
            edges.Remove(L1);

            Line3D L2 = edges.Find(x => x.Start.IsAlmostEqualTo(L1.End) || x.End.IsAlmostEqualTo(L1.End));

            Vector3D v1 = L1.Direction;
            Vector3D v2 = L2.Direction;
            Vector3D v3 = v1.Cross(v2);
            //只取向下的向量
            if (v3.Z > 0)
            {

                return -v3;
            }
            return v3;
        }

        /// <summary>
        /// 向指定方向偏移一点距离
        /// </summary>
        /// <param name="f"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Face Offset(this Face f, double offset, Vector3D direction)
        {
            List<Line3D> edges = new List<Line3D>();
            f.Edges.ForEach(x =>
            {
                edges.Add(x.Offset(offset, direction));
            });
            Face nf = Face.Create(edges);
            return nf;
        }

        /// <summary>
        /// 面投影到某个平面上
        /// </summary>
        /// <param name="face">要投影的面</param>
        /// <param name="origin">投影平面的原点</param>
        /// <param name="normal">投影平面的法向量</param>
        /// <returns></returns>
        public static Face ProjectOn(this Face face, Vector3D origin, Vector3D normal)
        {
            List<Line3D> edges = new List<Line3D>();
            //面和投影平面垂直
            if (face.Normal.Dot(normal).AreEqual(0))
                return null;
            else
            {
                face.Edges.ForEach(x =>
                {
                    edges.Add(x.ProjectOn(origin, normal));

                });
                Face nf = Face.Create(edges);
                return nf;
            }
        }
        #endregion



        #region 公共方法
        /// <summary>
        /// 三维线段转换为二维线段，是将三维线段投影到XOY平面，转换为二维线段
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Line2D Line3D2Line2D(this Line3D source)
        {
            Vector2D start = Vector3D2Vector2D(source.Start);
            Vector2D end = Vector3D2Vector2D(source.End);
            return new Line2D(start, end);
        }

        /// <summary>
        /// 三维点转换为二维点，是将三维点投影到XOY平面，转换为二维点
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>

        private static Vector2D Vector3D2Vector2D(this Vector3D source)
        {
            return new Vector2D(source.X, source.Y);
        }


        /// <summary>
        /// 二维线段转换为三维线段
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Line3D Line2D2Line3D(this Line2D source)
        {
            Vector3D start = Vector2D2Vector3D(source.Start);
            Vector3D end = Vector2D2Vector3D(source.End);
            return new Line3D(start, end);
        }

        /// <summary>
        /// 二维点转换为三维点
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>

        private static Vector3D Vector2D2Vector3D(this Vector2D source)
        {
            return new Vector3D(source.X, source.Y, 0);
        }


        /// <summary>
        /// 线段集合的扩张和收缩，向外扩张获得线段集合为1个，向内收缩获得的集合为多个或者null
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="expandDistance">正为向外扩张，负则向内收缩</param>
        /// <returns></returns>
        public static List<List<Line3D>> Expansion(this List<Line3D> lines, double expandDistance)
        {
            if (lines == null || lines.Count == 0)
                throw new ArgumentException("输入的参数值为空", nameof(lines));

            lines = lines.SortLinesContinuously();
            if (expandDistance == 0)
                return new List<List<Line3D>> { lines };

            //获得多边形的法向量
            Vector3D normal = lines.GetNormalInConvexPoint();

            Vector3D orgin = lines.First().Start;
            Vector3D basisX = lines.First().Direction;
            Vector3D basisZ = normal;
            Vector3D basisY = basisZ.Cross(basisX).Normalize();
            var matrix = Alternation.TransformUtil.GetMatrix(orgin, basisX, basisY);
            var inverseMatrix = matrix.GetInverse();

            //坐标转换，全局三维转局部二维
            lines = Alternation.TransformUtil.TransformLines(matrix, lines);

            //三维转二维
            List<Line2D> line2ds = new List<Line2D>();
            lines.ForEach(x => line2ds.Add(x.Line3D2Line2D()));

            //获得扩展和缩进后的线段集合
            var expandedLine2ds = Expansion(line2ds, expandDistance);
            if (expandedLine2ds == null)
                return null;
            else
            {
                List<List<Line3D>> expandedLine3ds = new List<List<Line3D>>();
                expandedLine2ds.ForEach(x =>
                {
                    //二维转三维
                    List<Line3D> tempLines = new List<Line3D>();
                    x.ForEach(y => tempLines.Add(y.Line2D2Line3D()));

                    //局部三维转全局三维
                    var trasformedLines = Alternation.TransformUtil.TransformLines(inverseMatrix, tempLines);
                    expandedLine3ds.Add(trasformedLines);
                });

                return expandedLine3ds;
            }


            //List<Line3D> offsetLines = new List<Line3D>();
            //lines.ForEach(line =>
            //{
            //    Vector3D offsetDirection = line.Direction.Cross(normal).Normalize();

            //    Line3D newLine = line.Offset(expandDistance, offsetDirection);
            //    offsetLines.Add(newLine);
            //});


            //List<Line3D> expandedLines = offsetLines.GetNewOutLines2();
            //List<List<Line3D>> targetLines = new List<List<Line3D>>();
            //if (expandDistance >= 0)
            //{
            //    //查找最大区域
            //    targetLines = GraphicAlgorithm.ClosedLookup(expandedLines, true, true);
            //}
            //else
            //{
            //    //查找最小区域
            //    targetLines = GraphicAlgorithm.ClosedLookup(expandedLines, false, true);
            //}
            //return expandedLines;
        }

        /// <summary>
        /// 线段集合的扩张和收缩
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="expandDistance">正为向外扩张，负则向内收缩</param>
        /// <returns></returns>
        public static List<List<Line2D>> Expansion(this List<Line2D> lines, double expandDistance)
        {
            lines = lines.SortLinesContinuously();

            if (!CheckLinesValidation(lines))
                throw new Exception("连续的两条线段共线，线段围成的闭合区域不能扩张或收缩。");

            if (expandDistance == 0)
                return new List<List<Line2D>> { lines };

            if (!lines.IsCounterclockwise())
                lines = lines.Flip();

            List<Line2D> offsetLines = new List<Line2D>();
            lines.ForEach(line =>
            {
                //尽量减少浮点运算，以免造成精度误差
                Vector3D dir = line.Direction.Vector2D2Vector3D();
                Vector3D offsetDirection = dir.Cross(Vector3D.BasisZ);
                Vector2D proDir = offsetDirection.Vector3D2Vector2D().Normalize();

                var newLine = line.CreateOffset(expandDistance * proDir);
                offsetLines.Add(newLine);
            });

            List<Line2D> expandedLines = offsetLines.GetNewOutLines2();
            List<List<Line2D>> targetLines = new List<List<Line2D>>();
            if (expandDistance >= 0)
            {
                //查找最大区域
                targetLines = GraphicAlgorithm.ClosedLookup(expandedLines, true, true);
            }
            else
            {
                //查找最小区域
                targetLines = GraphicAlgorithm.ClosedLookup(expandedLines, false, true);
                DeleteLinesOutOfRegion(lines, targetLines, expandDistance);
            }

            if (targetLines.Count == 0)
                targetLines = null;
            else
            {
                //线段融合
                var mergedLines = new List<List<Line2D>>();
                targetLines.ForEach(x =>
                {
                    var tempLines = GraphicAlgorithm.MergeLines(x);
                    mergedLines.Add(tempLines);
                });
                return mergedLines;
            }

            return targetLines;
        }

        /// <summary>
        /// 检查线段的合法性，判断连续的两条线段不能共线
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static bool CheckLinesValidation(List<Line2D> lines)
        {
            if (lines == null || lines.Count == 0)
                throw new ArgumentException("参数值为空", nameof(lines));

            for (int i = 0; i < lines.Count - 1; i++)
            {
                var line = lines[i];
                var nextLine = lines[i + 1];
                if (line.Direction.IsParallelWith(nextLine.Direction))
                    return false;
            }

            if (lines.First().Direction.IsParallelWith(lines.Last().Direction))
                return false;

            return true;
        }


        /// <summary>
        /// 删除区域外的线
        /// </summary>
        /// <param name="originalLines"></param>
        /// <param name="sourceLines"></param>
        /// <param name="offsetDistance"></param>
        private static void DeleteLinesOutOfRegion(List<Line2D> originalLines, List<List<Line2D>> sourceLines, double offsetDistance)
        {
            if (sourceLines == null)
                return;

            for (int i = 0; i < sourceLines.Count; i++)
            {
                var tempLines = sourceLines[i];

                bool isOutRegion = false;
                foreach (var line in tempLines)
                {
                    if (!line.Start.IsInRegion(originalLines))
                    {
                        isOutRegion = true;
                        break;
                    }
                }

                if (!isOutRegion)
                {
                    foreach (var line in tempLines)
                    {
                        foreach (var oLine in originalLines)
                        {
                            var distance = oLine.Distance(line.Start);
                            if (distance < Math.Abs(offsetDistance) - Extension.SMALL_NUMBER)
                            {
                                isOutRegion = true;
                                break;
                            }
                        }
                        if (isOutRegion)
                            break;
                    }
                }
                if (isOutRegion)
                {
                    sourceLines.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// 找到封闭区域
        /// </summary>
        /// <param name="originalLines"></param>
        /// <param name="expandedLines"></param>
        /// <param name="expandDistance"></param>
        /// <param name="isExpanded"></param>
        /// <returns></returns>
        public static List<List<Line2D>> FindClosedArea(this List<Line2D> expandedLines, List<Line2D> originalLines, double expandDistance, bool isExpanded = true)
        {
            List<List<Line2D>> targetLines = new List<List<Line2D>>();
            if (isExpanded)
            {
                //查找最大区域
                targetLines = GraphicAlgorithm.ClosedLookup(expandedLines, true, true);
            }
            else
            {
                //查找最小区域
                targetLines = GraphicAlgorithm.ClosedLookup(expandedLines, false, true);
                DeleteLinesOutOfRegion(originalLines, targetLines, expandDistance);
            }

            if (targetLines.Count == 0)
                targetLines = null;
            else
            {
                //线段融合
                var mergedLines = new List<List<Line2D>>();
                targetLines.ForEach(x =>
                {

                    var tempLines = GraphicAlgorithm.MergeLines(x);
                    mergedLines.Add(tempLines);
                });
                return mergedLines;
            }
            return targetLines;
        }


        /// <summary>
        /// 从偏移的线获得新的轮廓线，轮廓线按照原顺序排列
        /// </summary>
        /// <param name="lines">按序排列的线，并且没有共端点</param>
        /// <returns></returns>
        private static List<Line2D> GetNewOutLines2(this List<Line2D> lines)
        {
            if (lines == null || lines.Count == 0)
                throw new ArgumentException("参数值为空", nameof(lines));

            List<Vector2D> intersectPoints = new List<Vector2D>();

            var firstLine = new Line2D(lines.First().Start, lines.First().End);
            lines.Add(firstLine);

            for (int i = 0; i < lines.Count - 1; i++)
            {
                Line2D line1 = lines[i];
                Line2D line2 = lines[i + 1];

                var point = line1.IntersectStraightLine(line2);
                if (point != null)
                    intersectPoints.Add(point);
            }

            List<Line2D> newLines = new List<Line2D> {
                new Line2D(intersectPoints.Last(), intersectPoints.First())
            };

            for (int i = 0; i < intersectPoints.Count - 1; i++)
            {
                newLines.Add(new Line2D(intersectPoints[i], intersectPoints[i + 1]));
            }
            return newLines;
        }


        /// <summary>
        /// 从偏移的线获得新的轮廓线
        /// </summary>
        /// <param name="lines">按序排列的线，并且没有共端点</param>
        /// <returns></returns>
        public static List<Line3D> GetNewOutLines(this List<Line3D> lines)
        {
            //递归算法，排除偏移后与偏移之前方向不一致的线，并重新求交点，获取新的线
            List<Vector3D> intersectPoints = new List<Vector3D>();

            Line3D firstLine = new Line3D(lines.First().Start, lines.First().End);
            lines.Add(firstLine);


            for (int i = 0; i < lines.Count - 1; i++)
            {
                Line3D line1 = Line3D.CreateUnbound(lines[i].Start, lines[i].Direction);
                Line3D line2 = Line3D.CreateUnbound(lines[i + 1].Start, lines[i + 1].Direction);

                Vector3D point = line1.IntersectStraightLine(line2);
                if (point != null)
                    intersectPoints.Add(point);
            }

            List<Line3D> newLines = new List<Line3D> {
                new Line3D(intersectPoints.Last(), intersectPoints.First())
            };

            for (int i = 0; i < intersectPoints.Count - 1; i++)
            {
                newLines.Add(new Line3D(intersectPoints[i], intersectPoints[i + 1]));
            }


            bool isResult = true;
            for (int i = 0; i < lines.Count - 1; i++)
            {
                if (!lines[i].Direction.IsAlmostEqualTo(newLines[i].Direction))
                {
                    isResult = false;
                    newLines.RemoveAt(i);
                    break;
                }
            }

            if (!isResult)
            {
                return GetNewOutLines(newLines);
            }

            else
                return newLines;
        }

        #endregion
    }
}
