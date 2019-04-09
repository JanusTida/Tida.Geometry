using System;
using System.Collections.Generic;
using Tida.Geometry.External;

namespace Tida.Geometry.Primitives
{

    /// <summary>
    /// 初始化当前一个直线
    /// </summary>

    public class Line3D : Object3D
    {

        /// <summary>
        /// 构造函数创建
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Line3D(Vector3D start, Vector3D end)
        {
            Start = start;
            End = end;
        }

        private Vector3D start;

        public Vector3D Start
        {
            set { start = value; }
            get { return start; }
        }

        private Vector3D end;

        public Vector3D End
        {
            set { end = value; }
            get { return end; }
        }

        private Vector3D origin;
        /// <summary>
        /// 返回当前的原点坐标
        /// </summary>

        public Vector3D Origin
        {
            get
            {
                return origin;
            }
            set { origin = value; }
        }

        private Line3D()
        { }
        /// <summary>
        /// 通过两个点创建一条直线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Line3D Create(Vector3D start, Vector3D end)
        {
            if (start == null && end == null)
                throw new ArgumentNullException(nameof(start));
            else if (start == null)
                throw new ArgumentNullException(nameof(start));
            else if (end == null)
                throw new ArgumentNullException(nameof(end));

            return new Line3D(start, end);
        }

        /// <summary>
        /// 通过起点，方向和长度确定一个直线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static Line3D Create(Vector3D start, Vector3D direction, double step)
        {
            Vector3D nV = start + step * direction.Normalize();
            return new Line3D(start, nV);
        }

        /// <summary>
        /// 创建一个无端点的直线
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Line3D CreateUnbound(Vector3D origin, Vector3D direction)
        {
            Line3D line = new Line3D();
            line.direction = direction.Normalize();
            line.Origin = origin;
            line.Start = null;
            line.End = null;
            return line;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testPoint"></param>
        /// <returns></returns>
        public double ClosestParameter(Vector3D testPoint)
        {
            double rc = 0;
            var dir = End - Start;
            if (dir.SquaredLength > 0)
            {
                if (testPoint.Distance(Start) <= testPoint.Distance(End))
                    rc = (testPoint - Start).Dot(dir) / dir.SquaredLength;
                else
                    rc = 1 + (testPoint - End).Dot(dir) / dir.SquaredLength;
            }
            return rc;
        }

        /// <summary>
        /// 当前直线的方向信息
        /// </summary>
        private Vector3D direction;
        /// <summary>
        /// 直线单位方向
        /// </summary>

        public Vector3D Direction
        {
            get
            {
                if (End != null && Start != null)
                    direction = (End - Start).Normalize();
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        /// <summary>
        /// 获取当前线的长度
        /// </summary>

        public double Length
        {
            get
            {
                if (Start != null && End != null)
                    return Math.Sqrt(Math.Pow((End.X - Start.X), 2) + Math.Pow((End.Y - Start.Y), 2) + Math.Pow((End.Z - Start.Z), 2));
                throw new ArgumentNullException("该直线无端点");
            }
            set
            {

            }
        }

        /// <summary>
        /// 获取之间指定比例的坐标
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Vector3D Evaluate(double parameter)
        {
            return (End - Start) * parameter + Start;
        }

        /// <summary>
        /// 线段和线段相交
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public Vector3D IntersectSlow(Line3D curve)
        {
            double x0 = Start.X, y0 = Start.Y, z0 = Start.Z,
                        x1 = curve.Start.X, y1 = curve.Start.Y, z1 = curve.Start.Z,
                        l0 = Direction.X, m0 = Direction.Y, n0 = Direction.Z,
                        l1 = curve.Direction.X, m1 = curve.Direction.Y, n1 = curve.Direction.Z,
                        deltaLM = l0 * m1 - m0 * l1, deltaMN = m0 * n1 - n0 * m1, deltaLN = l0 * n1 - n0 * l1,
                        t0, t1;

            if (deltaLM.AreEqual(0) && deltaLN.AreEqual(0) && deltaMN.AreEqual(0))
            {
                if (Start.IsOnTwoLine(this, curve) && !this.CoincidesWith(curve))
                {
                    return Start;
                }
                if (End.IsOnTwoLine(this, curve) && !this.CoincidesWith(curve))
                {
                    return End;
                }
                else
                {
                    return null;
                }
            }
            if (!deltaLM.AreEqual(0))
            {
                t0 = (m1 * x1 - l1 * y1 - m1 * x0 + l1 * y0) / deltaLM;
                if (!l1.AreEqual(0))
                    t1 = (x0 + l0 * t0 - x1) / l1;
                else
                    t1 = (y0 + m0 * t0 - y1) / m1;
                double zTry1 = z0 + n0 * t0, zTry2 = z1 + n1 * t1;
                Vector3D intersection = new Vector3D(x0 + l0 * t0, y0 + m0 * t0, z0 + n0 * t0);
                if (zTry1.AreEqual(zTry2) && intersection.IsOnTwoLine(this, curve))
                    return intersection;
                else
                    return null;
            }
            else if (!deltaLN.AreEqual(0))
            {
                t0 = (n1 * x1 - l1 * z1 - n1 * x0 + l1 * z0) / deltaLN;
                if (!l1.AreEqual(0))
                    t1 = (x0 + l0 * t0 - x1) / l1;
                else
                    t1 = (z0 + n0 * t0 - z1) / n1;
                double yTry1 = y0 + m0 * t0, yTry2 = y1 + m1 * t1;
                Vector3D intersection = new Vector3D(x0 + l0 * t0, y0 + m0 * t0, z0 + n0 * t0);
                if (yTry1.AreEqual(yTry2) && intersection.IsOnTwoLine(this, curve))
                    return intersection;
                else
                    return null;
            }

            else
            {
                t0 = (n1 * y1 - m1 * z1 - n1 * y0 + m1 * z0) / deltaMN;
                if (!m1.AreEqual(0))
                    t1 = (y0 + m0 * t0 - y1) / m1;
                else
                    t1 = (z0 + n0 * t0 - z1) / n1;
                double xTry1 = x0 + l0 * t0, xTry2 = x1 + l1 * t1;
                Vector3D intersection = new Vector3D(x0 + l0 * t0, y0 + m0 * t0, z0 + n0 * t0);
                if (xTry1.AreEqual(xTry2) && intersection.IsOnTwoLine(this, curve))
                    return intersection;
                else
                    return null;
            }
        }

        /// <summary>
        /// 求线段的相交点
        /// </summary>
        /// <param name="curve">直线</param>
        /// <param name="isLineSegment">是否为线段</param>
        /// <param name="epsilon">误差值</param>
        /// <returns></returns>
        public Vector3D Intersect(Line3D curve, bool isLineSegment = true, double epsilon = Extension.SMALL_NUMBER)
        {
            var a = 0d;
            var b = 0d;
            epsilon = epsilon < 0 ? Extension.SMALL_NUMBER : epsilon;
            var intersect = LineLine(this, curve, out a, out b, epsilon);
            if (!intersect) return null;
            var isOnLine = a >= -epsilon && a <= 1 + epsilon && b >= -epsilon && b <= 1 + epsilon;
            if (isLineSegment && !isOnLine) return null;
            var p1 = Evaluate(a);
            var p2 = curve.Evaluate(b);
            var dis = p1.Distance(p2);
            if (dis < epsilon) return p1;
            return null;
        }

        private bool LineLine(Line3D lineA, Line3D lineB, out double a, out double b, double epsilon = Extension.SMALL_NUMBER)
        {
            a = b = 0;
            if (lineA.Length < epsilon) return false;
            if (lineB.Length < epsilon) return false;
            var p13 = lineA.Start - lineB.Start;
            var p43 = lineB.End - lineB.Start;
            var p21 = lineA.End - lineA.Start;
            var d1343 = p13.Dot(p43);
            var d4321 = p43.Dot(p21);
            var d1321 = p13.Dot(p21);
            var d4343 = p43.Dot(p43);
            var d2121 = p21.Dot(p21);
            var denom = d2121 * d4343 - d4321 * d4321;
            if (Math.Abs(denom) < epsilon) return false;
            double number = d1343 * d4321 - d1321 * d4343;
            a = number / denom;
            b = (d1343 + d4321 * a) / d4343;
            return true;
        }
        /// <summary>
        /// 线段与线段通过直线与直线的相交方式返回交点
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public Vector3D IntersectLine(Line3D line)
        {
            var line1 = CreateUnbound(this.start, Direction);//首先将线段变成射线，方便用IntersectStraightLine这个函数 
            var line2 = CreateUnbound(line.start, line.Direction);
            return line1.IntersectStraightLine(line2);
        }
        /// <summary>
        /// 射直线和射直线相交
        /// </summary>
        /// <param name="curve"></param>
        /// <returns></returns>
        public Vector3D IntersectStraightLine(Line3D curve)
        {
            double x0 = Origin.X, y0 = Origin.Y, z0 = Origin.Z,
                        x1 = curve.Origin.X, y1 = curve.Origin.Y, z1 = curve.Origin.Z,
                        l0 = Direction.X, m0 = Direction.Y, n0 = Direction.Z,
                        l1 = curve.Direction.X, m1 = curve.Direction.Y, n1 = curve.Direction.Z,
                        deltaLM = l0 * m1 - m0 * l1, deltaMN = m0 * n1 - n0 * m1, deltaLN = l0 * n1 - n0 * l1,
                        t0, t1;

            if (deltaLM.AreEqual(0) && deltaLN.AreEqual(0) && deltaMN.AreEqual(0))
            {
                return null;
            }
            if (!deltaLM.AreEqual(0))
            {
                t0 = (m1 * x1 - l1 * y1 - m1 * x0 + l1 * y0) / deltaLM;
                if (!l1.AreEqual(0))
                    t1 = (x0 + l0 * t0 - x1) / l1;
                else
                    t1 = (y0 + m0 * t0 - y1) / m1;
                double zTry1 = z0 + n0 * t0, zTry2 = z1 + n1 * t1;
                Vector3D intersection = new Vector3D(x0 + l0 * t0, y0 + m0 * t0, z0 + n0 * t0);
                return intersection;
            }
            else if (!deltaLN.AreEqual(0))
            {
                t0 = (n1 * x1 - l1 * z1 - n1 * x0 + l1 * z0) / deltaLN;
                if (!l1.AreEqual(0))
                    t1 = (x0 + l0 * t0 - x1) / l1;
                else
                    t1 = (z0 + n0 * t0 - z1) / n1;
                double yTry1 = y0 + m0 * t0, yTry2 = y1 + m1 * t1;
                Vector3D intersection = new Vector3D(x0 + l0 * t0, y0 + m0 * t0, z0 + n0 * t0);
                return intersection;
            }

            else
            {
                t0 = (n1 * y1 - m1 * z1 - n1 * y0 + m1 * z0) / deltaMN;
                if (!m1.AreEqual(0))
                    t1 = (y0 + m0 * t0 - y1) / m1;
                else
                    t1 = (z0 + n0 * t0 - z1) / n1;
                double xTry1 = x0 + l0 * t0, xTry2 = x1 + l1 * t1;
                Vector3D intersection = new Vector3D(x0 + l0 * t0, y0 + m0 * t0, z0 + n0 * t0);
                return intersection;
            }
        }

        public Vector3D IntersectStraightLineFast(Line3D curve)
        {
            var line1 = Create(Origin, Origin + Direction);
            var line2 = Create(curve.Origin, curve.Origin + curve.direction);
            return line1.Intersect(line2, true);
        }

        /// <summary>
        /// 直线和线段相交
        /// </summary>
        /// <param name="limitedCurve">线段</param>
        /// <returns></returns>
        public Vector3D IntersectStraightLine2(Line3D limitedCurve)
        {
            double x0 = Origin.X, y0 = Origin.Y, z0 = Origin.Z,
                        x1 = limitedCurve.Start.X, y1 = limitedCurve.Start.Y, z1 = limitedCurve.Start.Z,
                        l0 = Direction.X, m0 = Direction.Y, n0 = Direction.Z,
                        l1 = limitedCurve.Direction.X, m1 = limitedCurve.Direction.Y, n1 = limitedCurve.Direction.Z,
                        deltaLM = l0 * m1 - m0 * l1, deltaMN = m0 * n1 - n0 * m1, deltaLN = l0 * n1 - n0 * l1,
                        t0, t1;

            if (deltaLM.AreEqual(0) && deltaLN.AreEqual(0) && deltaMN.AreEqual(0))
            {
                return null;
            }
            if (!deltaLM.AreEqual(0))
            {
                t0 = (m1 * x1 - l1 * y1 - m1 * x0 + l1 * y0) / deltaLM;
                if (!l1.AreEqual(0))
                    t1 = (x0 + l0 * t0 - x1) / l1;
                else
                    t1 = (y0 + m0 * t0 - y1) / m1;
                double zTry1 = z0 + n0 * t0, zTry2 = z1 + n1 * t1;
                Vector3D intersection = new Vector3D(x0 + l0 * t0, y0 + m0 * t0, z0 + n0 * t0);
                if (zTry1.AreEqual(zTry2) && intersection.IsOnLine(limitedCurve))
                    return intersection;
                else
                    return null;
            }
            else if (!deltaLN.AreEqual(0))
            {
                t0 = (n1 * x1 - l1 * z1 - n1 * x0 + l1 * z0) / deltaLN;
                if (!l1.AreEqual(0))
                    t1 = (x0 + l0 * t0 - x1) / l1;
                else
                    t1 = (z0 + n0 * t0 - z1) / n1;
                double yTry1 = y0 + m0 * t0, yTry2 = y1 + m1 * t1;
                Vector3D intersection = new Vector3D(x0 + l0 * t0, y0 + m0 * t0, z0 + n0 * t0);
                if (yTry1.AreEqual(yTry2) && intersection.IsOnLine(limitedCurve))
                    return intersection;
                else
                    return null;
            }
            else
            {
                t0 = (n1 * y1 - m1 * z1 - n1 * y0 + m1 * z0) / deltaMN;
                if (!m1.AreEqual(0))
                    t1 = (y0 + m0 * t0 - y1) / m1;
                else
                    t1 = (z0 + n0 * t0 - z1) / n1;
                double xTry1 = x0 + l0 * t0, xTry2 = x1 + l1 * t1;
                Vector3D intersection = new Vector3D(x0 + l0 * t0, y0 + m0 * t0, z0 + n0 * t0);
                if (xTry1.AreEqual(xTry2) && intersection.IsOnLine(limitedCurve))
                    return intersection;
                else
                    return null;
            }
        }
        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <returns></returns>
        public Line3D Clone()
        {
            Vector3D s = new Vector3D(Start.X, Start.Y, Start.Z);
            Vector3D e = new Vector3D(End.X, End.Y, End.Z);
            return new Line3D(s, e);
        }

        public bool IsAlmostEqualTo(Line3D other, double toterance)
        {
            return (Start.IsAlmostEqualTo(other.Start, toterance) && End.IsAlmostEqualTo(other.End, toterance)) || (Start.IsAlmostEqualTo(other.End, toterance) && End.IsAlmostEqualTo(other.Start, toterance));
        }

        public bool IsAlmostEqualTo(Line3D other)
        {
            return IsAlmostEqualTo(other, Extension.SMALL_NUMBER);
        }

        public Line3D Offset(double offset, Vector3D vdirection)
        {
            Vector3D startPoint = Start.Offset(offset, vdirection);
            Vector3D endPoint = End.Offset(offset, vdirection);
            return new Line3D(startPoint, endPoint);
        }
        /// <summary>
        /// 将自身平移
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="vdirection"></param>
        public void OffsetSelf(double offset, Vector3D vdirection)
        {
            Start.OffsetSelf(offset, vdirection);
            End.OffsetSelf(offset, vdirection);
        }
        /// <summary>
        /// <see cref="Line3D"/> 的另一端点
        /// </summary>
        /// <param name="currentEnd"></param>
        /// <returns></returns>
        public Vector3D OtherEnd(Vector3D currentEnd)
        {
            if (currentEnd.IsAlmostEqualTo(Start))
                return End;
            return Start;
        }

        /// <summary>
        /// 对坐标转换进行重写
        /// </summary>
        protected override void Vary()
        {
            if (Transform != null)
            {
                this.start = this.start.ApplyMatrix4(this.Transform.Matrix);
                this.end = this.end.ApplyMatrix4(this.Transform.Matrix);
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}->{3},{4},{5} | L={6}", Start.X, Start.Y, Start.Z, End.X, End.Y, End.Z, Length);
        }


    }
    public class Line3DEqualityComparer : IEqualityComparer<Line3D>
    {
        public bool Equals(Line3D l1, Line3D l2)
        {
            return l1.IsAlmostEqualTo(l2);
        }
        public int GetHashCode(Line3D obj)
        {
            return 0;
        }
    }


}
