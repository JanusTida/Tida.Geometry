using Tida.Geometry.Alternation;
using Tida.Geometry.External;
using System;
using System.Collections.Generic;


namespace Tida.Geometry.Primitives
{

    /// <summary>
    /// 一个空间三维点
    /// </summary>
    public class Vector3D:Freezable
    {
        /// <summary>
        /// 构造函数，初始化一个三维向量
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vector3D(double x, double y, double z)
        {
            this._x = x;
            this._y = y;
            this._z = z;
        }

        /// <summary>
        /// 构造函数，初始化一个三维向量
        /// </summary>
        /// <param name="source"></param>
        public Vector3D(Vector3D source)
        {
            this._x = source.X;
            this._y = source.Y;
            this._z = source.Z;
        }
        /// <summary>
        /// 新建一个零向量
        /// </summary>
        public Vector3D()
        {
            this._x = 0;
            this._y = 0;
            this._z = 0;
        }

        /// <summary>
        /// 构造器，创建一个三维向量
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3D Create(double x, double y, double z)
        {
            return new Vector3D(x, y, z);
        }


        private double _x;
        /// <summary>
        /// 向量的X
        /// </summary>
        /// 
        
        public double X
        {
            get => _x;
            set => SetFreezableProperty(ref _x, value);
        }

        private double _y;
        /// <summary>
        /// 向量的Y
        /// </summary>
        
        public double Y
        {
            get => _y;
            set => SetFreezableProperty(ref _y, value);
        }

        private double _z;
        /// <summary>
        /// 向量的Z
        /// </summary>
        
        public double Z
        {
            get => _z;
            set => SetFreezableProperty(ref _z, value);
        }

        private static Vector3D _zero;
        /// <summary>
        /// 原点坐标或者零向量
        /// </summary>
        public static Vector3D Zero => GetFrozenVector3D(ref _zero,0, 0, 0);

        private static Vector3D _basisX;
        /// <summary>
        /// X轴
        /// </summary>
        public static Vector3D BasisX => GetFrozenVector3D(ref _basisX, 1, 0, 0);

        private static Vector3D _basisY;
        /// <summary>
        /// Y轴
        /// </summary>
        public static Vector3D BasisY => GetFrozenVector3D(ref _basisY, 0, 1, 0);

        private static Vector3D _basisZ;
        /// <summary>
        /// Z轴
        /// </summary>
        public static Vector3D BasisZ => GetFrozenVector3D(ref _basisZ, 0, 0, 1);

        /// <summary>
        /// 返回冻结的向量;
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static Vector3D GetFrozenVector3D(ref Vector3D vectorInstance, double x, double y,double z) {
            if (vectorInstance == null) {
                vectorInstance = new Vector3D(x, y,z);
                vectorInstance.Freeze();
            }

            return vectorInstance;
        }

        /// <summary>
        /// 将向量转成用于坐标转换的列矩阵{x,y,z,1}
        /// </summary>
        /// <returns></returns>
        public Matrix ToMatrix()
        {
            return new Matrix(4, 1, new[] { _x, _y, _z, 1 });
        }
        ///<summary>
        /// 向量的模
        /// </summary>
        /// <returns></returns>
        public double Modulus()
        {
            return Math.Sqrt(SquaredLength);
        }


        public double SquaredLength => Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2);

        /// <summary>
        /// 归一化
        /// </summary>
        /// <returns></returns>
        public Vector3D Normalize()
        {
            double mod = Modulus();
            if (mod < Extension.SMALL_NUMBER)
                return new Vector3D(0, 0, 0);
            return new Vector3D(X / mod, Y / mod, Z / mod);
        }

        /// <summary>
        /// 向量到向量之间的距离
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public double Distance(Vector3D source)
        {
            return Math.Sqrt(Math.Pow(X - source.X, 2) + Math.Pow(Y - source.Y, 2) + Math.Pow(Z - source.Z, 2));
        }
        /// <summary>
        /// 向量的点积
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public double Dot(Vector3D source)
        {
            return (X * source.X + Y * source.Y + Z * source.Z);
        }

        /// <summary>
        /// 向量的叉积
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public Vector3D Cross(Vector3D source)
        {
            return new Vector3D(Y * source.Z - Z * source.Y, Z * source.X - X * source.Z, X * source.Y - Y * source.X);
        }
        /// <summary>
        /// 沿direction方向对其偏移offset的距离
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public Vector3D Offset(double offset, Vector3D direction)
        {
            Vector3D point = this + direction.Normalize() * offset;
            return new Vector3D(point._x, point._y, point._z);
        }
        /// <summary>
        /// 平移自身
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="direction"></param>
        public void OffsetSelf(double offset, Vector3D direction)
        {
            this.X = (this + direction.Normalize() * offset).X;
            this.Y = (this + direction.Normalize() * offset).Y;
            this.Z = (this + direction.Normalize() * offset).Z;

        }
        /// <summary>
        /// 按向量vector进行偏移
        /// </summary>
        /// <param name="vector">偏移向量</param>
        /// <returns></returns>
        public Vector3D Offset(Vector3D vector)
        {
            Vector3D point = this + vector;
            return point;

        }

        /// <summary>
        /// 向量的减
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3D operator -(Vector3D v)
        {
            return new Vector3D(-v.X, -v.Y, -v.Z);
        }
        /// <summary>
        /// 向量的减
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        /// <summary>
        /// 向量的加
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v2.X + v1.X, v2.Y + v1.Y, v2.Z + v1.Z);
        }
        /// <summary>
        /// 向量的乘
        /// </summary>
        /// <param name="v"></param>
        /// <param name="k"></param>
        /// <returns></returns>
        public static Vector3D operator *(Vector3D v, double k)
        {
            return new Vector3D(v.X * k, v.Y * k, v.Z * k);
        }
        /// <summary>
        /// 向量的乘
        /// </summary>
        /// <param name="k"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3D operator *(double k, Vector3D v)
        {
            return new Vector3D(v.X * k, v.Y * k, v.Z * k);
        }

        /// <summary>
        /// 向量的除
        /// </summary>
        /// <param name="v"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3D operator /(Vector3D v, double value)
        {
            if (Math.Abs(value) < Extension.SMALL_NUMBER)
                throw new DivideByZeroException();
            return new Vector3D(v.X / value, v.Y / value, v.Z / value);
        }

        /// <summary>
        /// 和矩阵相乘
        /// </summary>
        /// <param name="m4"></param>
        /// <returns></returns>
        public Vector3D ApplyMatrix4(Matrix4 m4)
        {

            return m4 * this;
      
            //double x = this.X, y = this.Y, z = this.Z;

            //var e = m4.Elements;

            //double nx = e[0] * x + e[4] * y + e[8] * z + e[12];
            //double ny = e[1] * x + e[5] * y + e[9] * z + e[13];
            //double nz = e[2] * x + e[6] * y + e[10] * z + e[14];

            //return new Vector3D(nx, ny, nz);
        }
        /// <summary>
        /// 去掉一个坐标值，变成二维坐标。默认去掉z坐标 
        /// </summary>
        /// <param name="cutOff"></param>
        /// <returns></returns>
        public Vector2D ToVector2D(string cutOff = "z")
        {
            if (cutOff == "z")
                return new Vector2D(X, Y);
            if (cutOff == "x")
                return new Vector2D(Y, Z);
            return new Vector2D(X, Z);
        }
        public override string ToString()
        {
            return string.Format("{0},{1},{2}", X, Y,Z);
        }
    }

    /// <summary>
    /// 用于比较两个Vector3D是否相同的位置
    /// </summary>
    public class Vector3DEqualityComparer : IEqualityComparer<Vector3D>
    {
        public bool Equals(Vector3D v1, Vector3D v2)
        {
            return v1.IsAlmostEqualTo(v2);
        }
        public int GetHashCode(Vector3D obj)
        {
            return 0;
        }

    }

    /// <summary>
    /// 用于比较两个Vector3D的x是否相同的位置
    /// </summary>
    public class Vector3DXEqualityComparer : IEqualityComparer<Vector3D>
    {
        public bool Equals(Vector3D v1, Vector3D v2)
        {
            return Math.Abs(v1.X - v2.X) < 1e-6;
        }
        public int GetHashCode(Vector3D obj)
        {
            return 0;
        }

    }
}
