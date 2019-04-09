using System;
using Tida.Geometry.External;
using System.Collections.Generic;

namespace Tida.Geometry.Primitives
{

    public class Line2D : Freezable
    {
        private Vector2D _start;
        /// <summary>
        /// 起点
        /// </summary>
        
        public Vector2D Start {
            get => _start;
            set => SetFreezableProperty(ref _start, value);
        }


        private Vector2D _end;
        /// <summary>
        /// 终点
        /// </summary>

        public Vector2D End {
            get => _end;
            set => SetFreezableProperty(ref _end, value);
        }

        /// <summary>
        /// 构造函数，初始化一个直线
        /// </summary>
        public Line2D()
        { }

        private static Line2D _zero;
        public static Line2D Zero {
            get {
                if (_zero == null) {
                    _zero = new Line2D(Vector2D.Zero, Vector2D.Zero);
                    _zero.Freeze();
                }
                return _zero;
            }
        }


        /// <summary>
        /// 通过两点初始化一个直线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public Line2D(Vector2D start, Vector2D end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// 通过两点创建一个直线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Line2D Create(Vector2D start, Vector2D end)
        {
            return new Line2D(start, end);
        }
        /// <summary>
        /// 由起点、方向和长度来构造一条直线
        /// </summary>
        /// <param name="start"></param>
        /// <param name="direction"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static Line2D Create(Vector2D start, Vector2D direction, double length)
        {
            Vector2D nV = start + length * direction;
            return new Line2D(start, nV);
        }

        /// <summary>
        /// 单位方向向量
        /// </summary>

        public Vector2D Direction
        {
            get
            {
                if (End != null && Start != null)
                    return (End - Start).Normalize();
                return null;
            }

        }
        /// <summary>
        /// 长度
        /// </summary>

        public double Length
        {
            get
            {
                return Math.Sqrt(Math.Pow((End.X - Start.X), 2) + Math.Pow((End.Y - Start.Y), 2));
            }
        }

        /// <summary>
        /// 以向量direction来偏移
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Line2D CreateOffset(Vector2D direction)
        {
            Vector2D p1 = Start.Offset(direction);
            Vector2D p2 = End.Offset(direction);
            return Create(p1, p2);
        }
            
        /// <summary>
        /// 创建一个相反方向的线段
        /// </summary>
        /// <returns></returns>
        public Line2D CreateReversed()
        {
            return Create(End, Start);
        }


        public double ClosestParameter(Vector2D testPoint)
        {
            var v = End - Start;
            var ls = v.GetLengthSquared();
            var v1 = testPoint - Start;
            var v2 = testPoint - End;
            var result = 0.0d;
            if (ls > 0)
            {
                if (v2.GetLengthSquared() <= v2.GetLengthSquared())
                    result = v1 * v / ls;
                else result = 1 + v2 * v / ls;
            }
            return result;
        }

        /// <summary>
        /// 点到线段的最近距离，非垂直距离
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public double Distance(Vector2D source)
        {
            double space = 0;
            var a = Length;
            var b = source.Distance(Start);
            var c = source.Distance(End);
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
            Vector2D v1 = new Vector2D(source.X, source.Y);
            Line2D l1 = Line2D.Create(new Vector2D(this.Start.X, this.Start.Y), new Vector2D(this.End.X, this.End.Y));
            space = Line2D.Create(v1, v1.ProjectOn(l1)).Length;
            return space;
        }

        /// <summary>
        /// 直线上某点的坐标
        /// </summary>
        /// <param name="parameter">比例</param>
        /// <returns></returns>
        public Vector2D Evaluate(double parameter)
        {
            return (End - Start) * parameter + Start;
        }
        /// <summary>
        /// 中点
        /// </summary>

        public Vector2D MiddlePoint
        {
            get { return (Start + End) / 2; }
        }

        private bool IsEndPoint(Line2D line1, Vector2D source)
        {
            if (source.IsAlmostEqualTo(line1.Start) || source.IsAlmostEqualTo(line1.End))
            {
                return true;
            }
            return false;
        }

        public Vector2D IntersectFast(Line2D line, bool isSegement = true, double espilon = Extension.SMALL_NUMBER)
        {
            espilon = espilon < 0 ? Extension.SMALL_NUMBER : espilon;
            double rxs = (End - Start).Cross(line.End - line.Start);
            if (Math.Abs(rxs) < espilon) return null;
            double r = (line.Start - Start).Cross(line.End - line.Start) / rxs;
            var point = Evaluate(r);
            if (!isSegement) return point;
            var t = ClosestParameter(point);
            var u = line.ClosestParameter(point);
            var isOnline = t >= -espilon && t <= 1 + espilon && u >= -espilon && u <= 1 + espilon;
            if (isOnline) return point;
            return null;
        }

        public Vector2D Intersect(Line2D line)
        {
            return IntersectFast(line);
        }

        /// <summary>
        /// 两条线段的相交点
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Vector2D IntersectSlow(Line2D target)
        {
            Vector2D a = Start;
            Vector2D b = End;
            Vector2D c = target.Start;
            Vector2D d = target.End;
            if (Math.Abs((b.Y - a.Y) * (c.X - d.X) - (b.X - a.X) * (c.Y - d.Y)) < 1e-6)//Extension.SMALL_NUMBER
            {
                if (a.IsOnTwoLine(this, target) && IsEndPoint(target, a) && !IsEndPoint(this, a))
                {
                    return a;
                }
                if (b.IsOnTwoLine(this, target) && IsEndPoint(target, b) && !IsEndPoint(this, b))
                {
                    return b;
                }
                return null;
            }
            double pX = ((b.X - a.X) * (c.X - d.X) * (c.Y - a.Y) - c.X * (b.X - a.X) * (c.Y - d.Y) + a.X * (b.Y - a.Y) * (c.X - d.X)) / ((b.Y - a.Y) * (c.X - d.X) - (b.X - a.X) * (c.Y - d.Y));
            double pY = ((b.Y - a.Y) * (c.Y - d.Y) * (c.X - a.X) - c.Y * (b.Y - a.Y) * (c.X - d.X) + a.Y * (b.X - a.X) * (c.Y - d.Y)) / ((b.X - a.X) * (c.Y - d.Y) - (b.Y - a.Y) * (c.X - d.X));
            if ((pX - a.X) * (pX - b.X) <= 1e-6 && (pX - c.X) * (pX - d.X) <= 1e-6 &&
                (pY - a.Y) * (pY - b.Y) <= 1e-6 && (pY - c.Y) * (pY - d.Y) <= 1e-6)
            {
                return new Vector2D(pX, pY);
            }
            return null;
        }


        /// <summary>
        /// 判断两个直线的相交关系
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Vector2D IntersectStraightLineSlow(Line2D target)
        {
            Vector2D a = this.Start;
            Vector2D b = this.End;
            Vector2D c = target.Start;
            Vector2D d = target.End;
            //叉积等0.说明两条线平行
            if (Math.Abs((b.Y - a.Y) * (c.X - d.X) - (b.X - a.X) * (c.Y - d.Y)) < Extension.SMALL_NUMBER)
            {
                return null;
            }
            else
            {
                double pX = ((b.X - a.X) * (c.X - d.X) * (c.Y - a.Y) - c.X * (b.X - a.X) * (c.Y - d.Y) + a.X * (b.Y - a.Y) * (c.X - d.X)) / ((b.Y - a.Y) * (c.X - d.X) - (b.X - a.X) * (c.Y - d.Y));
                double pY = ((b.Y - a.Y) * (c.Y - d.Y) * (c.X - a.X) - c.Y * (b.Y - a.Y) * (c.X - d.X) + a.Y * (b.X - a.X) * (c.Y - d.Y)) / ((b.X - a.X) * (c.Y - d.Y) - (b.Y - a.Y) * (c.X - d.X));
                return new Vector2D(pX, pY);
            }
        }

        public Vector2D IntersectStraightLine(Line2D target)
        {
            return IntersectFast(target, false);
        }

        /// <summary>
        /// 比较两个线段是否相同
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsAlmostEqualTo(Line2D other)
        {
            return IsAlmostEqualTo(other, Extension.SMALL_NUMBER);
        }

        /// <summary>
        /// 比较两个线段是否相同
        /// </summary>
        /// <param name="other"></param>
        /// <param name="toterance">容差范围</param>
        /// <returns></returns>
        public bool IsAlmostEqualTo(Line2D other, double toterance)
        {
            return (Start.IsAlmostEqualTo(other.Start, toterance) && End.IsAlmostEqualTo(other.End, toterance)) || (Start.IsAlmostEqualTo(other.End, toterance) && End.IsAlmostEqualTo(other.Start, toterance));
        }

        public override string ToString()
        {
            return string.Format("{0},{1}->{2},{3} | L={4}", Start.X, Start.Y, End.X, End.Y, Length);
        }

        public static Line2D operator +(Line2D line2D, Vector2D offset)
        {
            return line2D.CreateOffset(offset);
        }

        public static Line2D operator -(Line2D line2D, Vector2D offset)
        {
            return line2D.CreateOffset(-offset);
        }
    }

    public class Line2DEqualityComparer : IEqualityComparer<Line2D>
    {
        public bool Equals(Line2D l1, Line2D l2)
        {
            return l1.IsAlmostEqualTo(l2);
        }
        public int GetHashCode(Line2D obj)
        {
            return 0;
        }
    }
}