using Tida.Geometry.Primitives;
using System;

namespace Tida.Geometry.Alternation
{
    /// <summary>
    /// 空间转换矩阵
    /// </summary>

    public class Matrix4
    {
        // 矩阵数据缓冲区
        private int numColumns = 4;			// 矩阵列数
        private int numRows = 4;
        private double[] elements = null;

        //方便直接获取四阶矩阵中的元素
        private double m11
        {
            get { return GetElement(0, 0); }
            set { m11 = value; }
        }

        private double m12
        {
            get { return GetElement(0, 1); }
            set { m12 = value; }
        }

        private double m13
        {
            get { return GetElement(0, 2); }
            set { m13 = value; }
        }

        private double m14
        {
            get { return GetElement(0, 3); }
            set { m14 = value; }
        }

        private double m21
        {
            get { return GetElement(1, 0); }
            set { m21 = value; }
        }
        private double m22
        {
            get { return GetElement(1, 1); }
            set { m22 = value; }
        }
        private double m23
        {
            get { return GetElement(1, 2); }
            set { m23 = value; }
        }
        private double m24
        {
            get { return GetElement(1, 3); }
            set { m24 = value; }
        }

        private double m31
        {
            get { return GetElement(2, 0); }
            set { m31 = value; }
        }
        private double m32
        {
            get { return GetElement(2, 1); }
            set { m32 = value; }
        }

        private double m33
        {
            get { return GetElement(2, 2); }
            set { m33 = value; }
        }
        private double m34
        {
            get { return GetElement(2, 3); }
            set { m34 = value; }
        }

        private double m41
        {
            get { return GetElement(3, 0); }
            set { m41 = value; }
        }

        private double m42
        {
            get { return GetElement(3, 1); }
            set { m42 = value; }
        }
        private double m43
        {
            get { return GetElement(3, 2); }
            set { m43 = value; }
        }
        private double m44
        {
            get { return GetElement(3, 3); }
            set { m44 = value; }
        }
        /// <summary>
        /// 数值缓冲区
        /// </summary>

        public double[] Elements
        {
            get
            {
                return elements;
            }
            set
            {
                elements = value;
            }
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private Matrix4()
        {
            elements = new double[16];
        }
        /// <summary>
        /// 初始化一个矩阵
        /// </summary>
        /// <param name="src"></param>
        public Matrix4(double[] src)
        {

            if (src != null && src.Length == 16)
            {
                elements = src;
            }
            else
            {
                elements = new double[]{
                    1,0,0,0,
                    0,1,0,0,
                    0,0,1,0,
                    0,0,0,1
                };
            }
        }


        public Matrix4(int nRows, int nCols, double[] value)
        {
            numRows = nRows;
            numColumns = nCols;
            Init(numRows, numColumns);
            SetData(value);
        }

        /// <summary>
        /// 用已有矩阵设置当前矩阵
        /// </summary>
        /// <param name="src"></param>
        public Matrix4(Matrix4 src)
        {
            if (src != null)
            {
                var s = src.elements;
                elements = new double[16];
                for (int i = 0; i < 16; ++i)
                {
                    elements[i] = s[i];
                }
            }
        }

        /// <summary>
        /// 设置矩阵各元素的值
        /// </summary>
        /// <param name="value">一维数组，长度为numColumns*numRows，存储矩阵各元素的值</param>
        public void SetData(double[] value)
        {
            elements = (double[])value.Clone();
        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="nRows">指定的矩阵行数</param>
        /// <param name="nCols">指定的矩阵列数</param>
        /// <returns>成功返回true, 否则返回false</returns>
        public bool Init(int nRows, int nCols)
        {
            numRows = nRows;
            numColumns = nCols;
            int nSize = nCols * nRows;
            if (nSize < 0)//
                return false;

            // 分配内存
            elements = new double[nSize];//矩阵中元素的个数为数组的长度

            return true;
        }

        #region 先列后行的给矩阵elements赋值，不同于前面先行后列
        ///// <summary>
        ///// 设定当前矩阵的信息
        ///// </summary>
        ///// <param name="n11"></param>
        ///// <param name="n12"></param>
        ///// <param name="n13"></param>
        ///// <param name="n14"></param>
        ///// <param name="n21"></param>
        ///// <param name="n22"></param>
        ///// <param name="n23"></param>
        ///// <param name="n24"></param>
        ///// <param name="n31"></param>
        ///// <param name="n32"></param>
        ///// <param name="n33"></param>
        ///// <param name="n34"></param>
        ///// <param name="n41"></param>
        ///// <param name="n42"></param>
        ///// <param name="n43"></param>
        ///// <param name="n44"></param>
        private void Set(
         double n11, double n12, double n13, double n14,
         double n21, double n22, double n23, double n24,
         double n31, double n32, double n33, double n34,
         double n41, double n42, double n43, double n44
         )
        {
            var te = this.elements;
            te[0] = n11; te[4] = n12; te[8] = n13; te[12] = n14;
            te[1] = n21; te[5] = n22; te[9] = n23; te[13] = n24;
            te[2] = n31; te[6] = n32; te[10] = n33; te[14] = n34;
            te[3] = n41; te[7] = n42; te[11] = n43; te[15] = n44;
        }
        ///// <summary>
        ///// 设置当前矩阵的信息,数字代表行列
        ///// </summary>
        ///// <param name="n11"></param>
        ///// <param name="n12"></param>
        ///// <param name="n13"></param>
        ///// <param name="n14"></param>
        ///// <param name="n21"></param>
        ///// <param name="n22"></param>
        ///// <param name="n23"></param>
        ///// <param name="n24"></param>
        ///// <param name="n31"></param>
        ///// <param name="n32"></param>
        ///// <param name="n33"></param>
        ///// <param name="n34"></param>
        ///// <param name="n41"></param>
        ///// <param name="n42"></param>
        ///// <param name="n43"></param>
        ///// <param name="n44"></param>
        public static Matrix4 Create(
            double n11, double n12, double n13, double n14,
            double n21, double n22, double n23, double n24,
            double n31, double n32, double n33, double n34,
            double n41, double n42, double n43, double n44
            )
        {
            Matrix4 m4 = new Matrix4();
            m4.Set(n11, n12, n13, n14,
             n21, n22, n23, n24,
             n31, n32, n33, n34,
             n41, n42, n43, n44);
            return m4;
        }
        #endregion

        /// <summary>
        /// 创建一个默认的矩阵
        /// </summary>
        /// <returns></returns>
        public static Matrix4 Create()
        {

            Matrix4 m4 = new Matrix4();
            m4.SetIndentity();
            return m4;
        }
        /// <summary>
        /// 重定义矩阵
        /// </summary>
        public Matrix4 SetIndentity()
        {
            var e = this.elements;
            //for (int i = 0; i < 4; i++) {
            //    for (int j = 0; j < 4; j++) {
            //        e[i * 4 + j] = j == i ? 1 : 0;
            //    }
            //}

            e[0] = 1; e[4] = 0; e[8] = 0; e[12] = 0;
            e[1] = 0; e[5] = 1; e[9] = 0; e[13] = 0;
            e[2] = 0; e[6] = 0; e[10] = 1; e[14] = 0;
            e[3] = 0; e[7] = 0; e[11] = 0; e[15] = 1;
            return this;
        }

        public static Matrix4 identity
        {
            get
            {
                double[] unit = new double[] {
                    1.0, 0.0, 0.0, 0.0,
                    0.0, 1.0, 0.0, 0.0,
                    0.0, 0.0, 1.0, 0.0,
                    0.0, 0.0, 0.0, 1.0 };

                return new Matrix4(4, 4, unit);

            }
        }

        /// <summary>
        /// 索引器: 访问矩阵元素 
        /// </summary>
        /// <param name="row">元素的行</param>
        /// <param name="col">元素的列</param>
        /// <returns></returns>
        public double this[int row, int col]
        {
            get
            {
                return elements[col + row * 4];
            }
            set
            {
                elements[col + row * 4] = value;
            }
        }







        public double GetElement(int nRow, int nCol)
        {
            return elements[nCol + nRow * numColumns];
        }
        /// <summary>
        /// 拷贝一个新实例
        /// </summary>
        /// <returns></returns>
        public Matrix4 Clone()
        {
            return new Matrix4(this);
        }

        /// <summary>
        /// 从其他矩阵拷贝
        /// </summary>
        /// <param name="m4"></param>
        /// <returns></returns>
        public Matrix4 Copy(Matrix4 m4)
        {
            this.elements = m4.elements;
            return this;
        }

        /// <summary>
        /// 拷贝矩阵的平移位置信息
        /// </summary>
        /// <param name="m4"></param>
        /// <returns></returns>
        public Matrix4 CopyPosition(Matrix4 m4)
        {

            var te = this.elements;
            var me = m4.elements;

            te[12] = me[12];
            te[13] = me[13];
            te[14] = me[14];

            return this;

        }


        ///// <summary>
        ///// 设置当前矩阵的基本信息
        ///// </summary>
        ///// <param name="xAxis"></param>
        ///// <param name="yAxis"></param>
        ///// <param name="zAxis"></param>
        //public void SetBasis(Vector3D xAxis, Vector3D yAxis, Vector3D zAxis)
        //{
        //    this.Set(
        //        xAxis.X, yAxis.X, zAxis.X, 0,
        //        xAxis.Y, yAxis.Y, zAxis.Y, 0,
        //        xAxis.Z, yAxis.Z, zAxis.Z, 0,
        //        0, 0, 0, 1
        //    );
        //}



        /// <summary>
        /// 实现矩阵的加法
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix4 Add(Matrix4 other)
        {
            var e = this.elements;
            var a = this.elements;
            var b = other.elements;

            // 矩阵加法
            for (int i = 0; i < 16; i++)
            {
                e[i] = a[i] + b[i];
            }

            return this;
        }

        /// <summary>
        /// 实现矩阵的减法
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix4 Subtract(Matrix4 other)
        {
            var e = this.elements;
            var a = this.elements;
            var b = other.elements;
            // 矩阵加法
            for (int i = 0; i < 16; i++)
            {
                e[i] = a[i] - b[i];
            }
            return this;
        }


        /// <summary>
        /// 实现矩阵的数乘
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Matrix4 Multiply(double value)
        {
            var e = this.elements;
            // 进行数乘
            for (int i = 0; i < 16; i++)
            {
                e[i] = e[i] * value;
            }

            return this;
        }


        /// <summary>
        /// 矩阵的相乘连接
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Matrix4 Concat(Matrix4 other)
        {
            var te = this.elements;
            var ae = this.elements;
            var be = other.elements;


            double a11 = ae[0], a12 = ae[4], a13 = ae[8], a14 = ae[12];
            double a21 = ae[1], a22 = ae[5], a23 = ae[9], a24 = ae[13];
            double a31 = ae[2], a32 = ae[6], a33 = ae[10], a34 = ae[14];
            double a41 = ae[3], a42 = ae[7], a43 = ae[11], a44 = ae[15];

            double b11 = be[0], b12 = be[4], b13 = be[8], b14 = be[12];
            double b21 = be[1], b22 = be[5], b23 = be[9], b24 = be[13];
            double b31 = be[2], b32 = be[6], b33 = be[10], b34 = be[14];
            double b41 = be[3], b42 = be[7], b43 = be[11], b44 = be[15];

            te[0] = a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41;
            te[4] = a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42;
            te[8] = a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43;
            te[12] = a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44;

            te[1] = a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41;
            te[5] = a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42;
            te[9] = a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43;
            te[13] = a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44;

            te[2] = a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41;
            te[6] = a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42;
            te[10] = a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43;
            te[14] = a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44;

            te[3] = a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41;
            te[7] = a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42;
            te[11] = a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43;
            te[15] = a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44;

            return this;
        }
        /// <summary>
        /// 矩阵的转置
        /// </summary>
        /// <returns></returns>
        public Matrix4 Transpose()
        {
            double[] e;
            double t;

            e = this.elements;

            t = e[1]; e[1] = e[4]; e[4] = t;
            t = e[2]; e[2] = e[8]; e[8] = t;
            t = e[3]; e[3] = e[12]; e[12] = t;
            t = e[6]; e[6] = e[9]; e[9] = t;
            t = e[7]; e[7] = e[13]; e[13] = t;
            t = e[11]; e[11] = e[14]; e[14] = t;

            return this;
        }



        /// <summary>
        /// 矩阵的逆矩阵
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        private Matrix4 SetInverseOf(Matrix4 other)
        {
            int i;
            double[] s, d, inv;
            double det;

            s = other.elements;
            d = this.elements;
            inv = new double[16];

            inv[0] = s[5] * s[10] * s[15] - s[5] * s[11] * s[14] - s[9] * s[6] * s[15]
                      + s[9] * s[7] * s[14] + s[13] * s[6] * s[11] - s[13] * s[7] * s[10];
            inv[4] = -s[4] * s[10] * s[15] + s[4] * s[11] * s[14] + s[8] * s[6] * s[15]
                      - s[8] * s[7] * s[14] - s[12] * s[6] * s[11] + s[12] * s[7] * s[10];
            inv[8] = s[4] * s[9] * s[15] - s[4] * s[11] * s[13] - s[8] * s[5] * s[15]
                      + s[8] * s[7] * s[13] + s[12] * s[5] * s[11] - s[12] * s[7] * s[9];
            inv[12] = -s[4] * s[9] * s[14] + s[4] * s[10] * s[13] + s[8] * s[5] * s[14]
                      - s[8] * s[6] * s[13] - s[12] * s[5] * s[10] + s[12] * s[6] * s[9];

            inv[1] = -s[1] * s[10] * s[15] + s[1] * s[11] * s[14] + s[9] * s[2] * s[15]
                      - s[9] * s[3] * s[14] - s[13] * s[2] * s[11] + s[13] * s[3] * s[10];
            inv[5] = s[0] * s[10] * s[15] - s[0] * s[11] * s[14] - s[8] * s[2] * s[15]
                      + s[8] * s[3] * s[14] + s[12] * s[2] * s[11] - s[12] * s[3] * s[10];
            inv[9] = -s[0] * s[9] * s[15] + s[0] * s[11] * s[13] + s[8] * s[1] * s[15]
                      - s[8] * s[3] * s[13] - s[12] * s[1] * s[11] + s[12] * s[3] * s[9];
            inv[13] = s[0] * s[9] * s[14] - s[0] * s[10] * s[13] - s[8] * s[1] * s[14]
                      + s[8] * s[2] * s[13] + s[12] * s[1] * s[10] - s[12] * s[2] * s[9];

            inv[2] = s[1] * s[6] * s[15] - s[1] * s[7] * s[14] - s[5] * s[2] * s[15]
                      + s[5] * s[3] * s[14] + s[13] * s[2] * s[7] - s[13] * s[3] * s[6];
            inv[6] = -s[0] * s[6] * s[15] + s[0] * s[7] * s[14] + s[4] * s[2] * s[15]
                      - s[4] * s[3] * s[14] - s[12] * s[2] * s[7] + s[12] * s[3] * s[6];
            inv[10] = s[0] * s[5] * s[15] - s[0] * s[7] * s[13] - s[4] * s[1] * s[15]
                      + s[4] * s[3] * s[13] + s[12] * s[1] * s[7] - s[12] * s[3] * s[5];
            inv[14] = -s[0] * s[5] * s[14] + s[0] * s[6] * s[13] + s[4] * s[1] * s[14]
                      - s[4] * s[2] * s[13] - s[12] * s[1] * s[6] + s[12] * s[2] * s[5];

            inv[3] = -s[1] * s[6] * s[11] + s[1] * s[7] * s[10] + s[5] * s[2] * s[11]
                      - s[5] * s[3] * s[10] - s[9] * s[2] * s[7] + s[9] * s[3] * s[6];
            inv[7] = s[0] * s[6] * s[11] - s[0] * s[7] * s[10] - s[4] * s[2] * s[11]
                      + s[4] * s[3] * s[10] + s[8] * s[2] * s[7] - s[8] * s[3] * s[6];
            inv[11] = -s[0] * s[5] * s[11] + s[0] * s[7] * s[9] + s[4] * s[1] * s[11]
                      - s[4] * s[3] * s[9] - s[8] * s[1] * s[7] + s[8] * s[3] * s[5];
            inv[15] = s[0] * s[5] * s[10] - s[0] * s[6] * s[9] - s[4] * s[1] * s[10]
                      + s[4] * s[2] * s[9] + s[8] * s[1] * s[6] - s[8] * s[2] * s[5];

            det = s[0] * inv[0] + s[1] * inv[4] + s[2] * inv[8] + s[3] * inv[12];
            if (det == 0)
            {
                return this;
            }

            det = 1 / det;
            for (i = 0; i < 16; i++)
            {
                d[i] = inv[i] * det;
            }

            return this;
        }

        /// <summary>
        /// 计算逆矩阵
        /// </summary>
        /// <returns></returns>
        public void InvertSelf()
        {
            this.SetInverseOf(this);
        }

        /// <summary>
        /// 计算逆矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetInverse()
        {
            Matrix4 matrix = new Matrix4(this);
            return matrix.SetInverseOf(matrix);
        }

        //public Matrix4 Inverse
        //{
        //    get
        //    {
        //        double d = this.Determinant;
        //        if (d == 0.0)
        //            return Matrix4.identity;

        //        Matrix4 M = new Matrix4();
        //        M.m11 = (m23 * m34 * m42 - m24 * m33 * m42 + m24 * m32 * m43 -
        //                 m22 * m34 * m43 - m23 * m32 * m44 + m22 * m33 * m44) / d;
        //        M.m12 = (m14 * m33 * m42 - m13 * m34 * m42 - m14 * m32 * m43 +
        //                 m12 * m34 * m43 + m13 * m32 * m44 - m12 * m33 * m44) / d;
        //        M.m13 = (m13 * m24 * m42 - m14 * m23 * m42 + m14 * m22 * m43 -
        //                 m12 * m24 * m43 - m13 * m22 * m44 + m12 * m23 * m44) / d;
        //        M.m14 = (m14 * m23 * m32 - m13 * m24 * m32 - m14 * m22 * m33 +
        //                 m12 * m24 * m33 + m13 * m22 * m34 - m12 * m23 * m34) / d;
        //        M.m21 = (m24 * m33 * m41 - m23 * m34 * m41 - m24 * m31 * m43 +
        //                 m21 * m34 * m43 + m23 * m31 * m44 - m21 * m33 * m44) / d;
        //        M.m22 = (m13 * m34 * m41 - m14 * m33 * m41 + m14 * m31 * m43 -
        //                 m11 * m34 * m43 - m13 * m31 * m44 + m11 * m33 * m44) / d;
        //        M.m23 = (m14 * m23 * m41 - m13 * m24 * m41 - m14 * m21 * m43 +
        //                 m11 * m24 * m43 + m13 * m21 * m44 - m11 * m23 * m44) / d;
        //        M.m24 = (m13 * m24 * m31 - m14 * m23 * m31 + m14 * m21 * m33 -
        //                 m11 * m24 * m33 - m13 * m21 * m34 + m11 * m23 * m34) / d;
        //        M.m31 = (m22 * m34 * m41 - m24 * m32 * m41 + m24 * m31 * m42 -
        //                 m21 * m34 * m42 - m22 * m31 * m44 + m21 * m32 * m44) / d;
        //        M.m32 = (m14 * m32 * m41 - m12 * m34 * m41 - m14 * m31 * m42 +
        //                 m11 * m34 * m42 + m12 * m31 * m44 - m11 * m32 * m44) / d;
        //        M.m33 = (m12 * m24 * m41 - m14 * m22 * m41 + m14 * m21 * m42 -
        //                 m11 * m24 * m42 - m12 * m21 * m44 + m11 * m22 * m44) / d;
        //        M.m34 = (m14 * m22 * m31 - m12 * m24 * m31 - m14 * m21 * m32 +
        //                 m11 * m24 * m32 + m12 * m21 * m34 - m11 * m22 * m34) / d;
        //        M.m41 = (m23 * m32 * m41 - m22 * m33 * m41 - m23 * m31 * m42 +
        //                 m21 * m33 * m42 + m22 * m31 * m43 - m21 * m32 * m43) / d;
        //        M.m42 = (m12 * m33 * m41 - m13 * m32 * m41 + m13 * m31 * m42 -
        //                 m11 * m33 * m42 - m12 * m31 * m43 + m11 * m32 * m43) / d;
        //        M.m43 = (m13 * m22 * m41 - m12 * m23 * m41 - m13 * m21 * m42 +
        //                 m11 * m23 * m42 + m12 * m21 * m43 - m11 * m22 * m43) / d;
        //        M.m44 = (m12 * m23 * m31 - m13 * m22 * m31 + m13 * m21 * m32 -
        //                 m11 * m23 * m32 - m12 * m21 * m33 + m11 * m22 * m33) / d;
        //        return M;
        //    }
        //}


        public double Determinant
        {
            get
            {
                double a0 = m11 * m22 - m12 * m21;
                double a1 = m11 * m23 - m13 * m21;
                double a2 = m11 * m24 - m14 * m21;
                double a3 = m12 * m23 - m13 * m22;
                double a4 = m12 * m24 - m14 * m22;
                double a5 = m13 * m24 - m14 * m23;
                double b0 = m31 * m42 - m32 * m41;
                double b1 = m31 * m43 - m33 * m41;
                double b2 = m31 * m44 - m34 * m41;
                double b3 = m32 * m43 - m33 * m42;
                double b4 = m32 * m44 - m34 * m42;
                double b5 = m33 * m44 - m34 * m43;

                return a0 * b5 - a1 * b4 + a2 * b3 + a3 * b2 - a4 * b1 + a5 * b0;
            }
        }
        /**设置正交矩阵*/
        public Matrix4 SetOrtho(double left, double right, double bottom, double top, double near, double far)
        {
            double[] e;
            double rw, rh, rd;
            if (left == right || bottom == top || near == far)
            {
                throw new Exception();
            }

            rw = 1 / (right - left);
            rh = 1 / (top - bottom);
            rd = 1 / (far - near);

            e = this.elements;

            e[0] = 2 * rw;
            e[1] = 0;
            e[2] = 0;
            e[3] = 0;

            e[4] = 0;
            e[5] = 2 * rh;
            e[6] = 0;
            e[7] = 0;

            e[8] = 0;
            e[9] = 0;
            e[10] = -2 * rd;
            e[11] = 0;

            e[12] = -(right + left) * rw;
            e[13] = -(top + bottom) * rh;
            e[14] = -(far + near) * rd;
            e[15] = 1;

            return this;
        }

        /**设置正交矩阵*/
        public Matrix4 Ortho(double left, double right, double bottom, double top, double near, double far)
        {
            return this.Concat(new Matrix4().SetOrtho(left, right, bottom, top, near, far));
        }


        /**设置剪切矩阵*/
        public Matrix4 SetFrustum(double left, double right, double bottom, double top, double near, double far)
        {
            double[] e;
            double rw, rh, rd;

            if (left == right || top == bottom || near == far)
            {
                throw new Exception();
            }
            if (near <= 0)
            {
                throw new Exception();
            }
            if (far <= 0)
            {
                throw new Exception();
            }

            rw = 1 / (right - left);
            rh = 1 / (top - bottom);
            rd = 1 / (far - near);

            e = this.elements;

            e[0] = 2 * near * rw;
            e[1] = 0;
            e[2] = 0;
            e[3] = 0;

            e[4] = 0;
            e[5] = 2 * near * rh;
            e[6] = 0;
            e[7] = 0;

            e[8] = (right + left) * rw;
            e[9] = (top + bottom) * rh;
            e[10] = -(far + near) * rd;
            e[11] = -1;

            e[12] = 0;
            e[13] = 0;
            e[14] = -2 * near * far * rd;
            e[15] = 0;

            return this;
        }

        /// <summary>
        /// 设置剪切矩阵
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="top"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        /// <returns></returns>

        public Matrix4 Frustum(double left, double right, double bottom, double top, double near, double far)
        {
            return this.Concat(new Matrix4().SetFrustum(left, right, bottom, top, near, far));
        }

        /**设置投影矩阵*/
        public Matrix4 SetPerspective(double fovy, double aspect, double near, double far)
        {
            double[] e;
            double rd, s, ct;

            if (near == far || aspect == 0)
            {
                throw new Exception();
            }
            if (near <= 0)
            {
                throw new Exception();
            }
            if (far <= 0)
            {
                throw new Exception();
            }

            fovy = Math.PI * fovy / 180 / 2;
            s = Math.Sin(fovy);
            if (s == 0)
            {
                throw new Exception();
            }

            rd = 1 / (far - near);
            ct = Math.Cos(fovy) / s;

            e = this.elements;

            e[0] = ct / aspect;
            e[1] = 0;
            e[2] = 0;
            e[3] = 0;

            e[4] = 0;
            e[5] = ct;
            e[6] = 0;
            e[7] = 0;

            e[8] = 0;
            e[9] = 0;
            e[10] = -(far + near) * rd;
            e[11] = -1;

            e[12] = 0;
            e[13] = 0;
            e[14] = -2 * near * far * rd;
            e[15] = 0;

            return this;
        }

        /**进行矩阵变换*/
        public Matrix4 Perspective(double fovy, double aspect, double near, double far)
        {
            return this.Concat(new Matrix4().SetPerspective(fovy, aspect, near, far));
        }

        /**进行矩阵的缩放*/
        public Matrix4 SetScale(double x, double y, double z)
        {
            var e = this.elements;
            e[0] = x; e[4] = 0; e[8] = 0; e[12] = 0;
            e[1] = 0; e[5] = y; e[9] = 0; e[13] = 0;
            e[2] = 0; e[6] = 0; e[10] = z; e[14] = 0;
            e[3] = 0; e[7] = 0; e[11] = 0; e[15] = 1;
            return this;
        }

        /**矩阵的缩放*/
        public Matrix4 Scale(double x, double y, double z)
        {
            var e = this.elements;
            e[0] *= x; e[4] *= y; e[8] *= z;
            e[1] *= x; e[5] *= y; e[9] *= z;
            e[2] *= x; e[6] *= y; e[10] *= z;
            e[3] *= x; e[7] *= y; e[11] *= z;
            return this;
        }

        /**矩阵的移动*/
        public Matrix4 SetTranslation(double x, double y, double z)
        {
            var e = this.elements;
            e[0] = 1; e[4] = 0; e[8] = 0; e[12] = x;
            e[1] = 0; e[5] = 1; e[9] = 0; e[13] = y;
            e[2] = 0; e[6] = 0; e[10] = 1; e[14] = z;
            e[3] = 0; e[7] = 0; e[11] = 0; e[15] = 1;
            return this;
        }

        /**矩阵的移动*/
        public Matrix4 Translation(double x, double y, double z)
        {
            var e = this.elements;
            e[12] += e[0] * x + e[4] * y + e[8] * z;
            e[13] += e[1] * x + e[5] * y + e[9] * z;
            e[14] += e[2] * x + e[6] * y + e[10] * z;
            e[15] += e[3] * x + e[7] * y + e[11] * z;
            return this;
        }

        /**矩阵的角度转动*/
        public Matrix4 SetRotate(double angle, double x, double y, double z)
        {
            double[] e;
            double s, c, len, rlen, nc, xy, yz, zx, xs, ys, zs;

            angle = Math.PI * angle / 180;
            e = this.elements;

            s = Math.Sin(angle);
            c = Math.Cos(angle);

            if (0 != x && 0 == y && 0 == z)
            {
                if (x < 0)
                {
                    s = -s;
                }
                e[0] = 1; e[4] = 0; e[8] = 0; e[12] = 0;
                e[1] = 0; e[5] = c; e[9] = -s; e[13] = 0;
                e[2] = 0; e[6] = s; e[10] = c; e[14] = 0;
                e[3] = 0; e[7] = 0; e[11] = 0; e[15] = 1;
            }
            else if (0 == x && 0 != y && 0 == z)
            {
                if (y < 0)
                {
                    s = -s;
                }
                e[0] = c; e[4] = 0; e[8] = s; e[12] = 0;
                e[1] = 0; e[5] = 1; e[9] = 0; e[13] = 0;
                e[2] = -s; e[6] = 0; e[10] = c; e[14] = 0;
                e[3] = 0; e[7] = 0; e[11] = 0; e[15] = 1;
            }
            else if (0 == x && 0 == y && 0 != z)
            {
                if (z < 0)
                {
                    s = -s;
                }
                e[0] = c; e[4] = -s; e[8] = 0; e[12] = 0;
                e[1] = s; e[5] = c; e[9] = 0; e[13] = 0;
                e[2] = 0; e[6] = 0; e[10] = 1; e[14] = 0;
                e[3] = 0; e[7] = 0; e[11] = 0; e[15] = 1;
            }
            else
            {
                len = Math.Sqrt(x * x + y * y + z * z);
                if (len != 1)
                {
                    rlen = 1 / len;
                    x *= rlen;
                    y *= rlen;
                    z *= rlen;
                }
                nc = 1 - c;
                xy = x * y;
                yz = y * z;
                zx = z * x;
                xs = x * s;
                ys = y * s;
                zs = z * s;

                e[0] = x * x * nc + c;
                e[1] = xy * nc + zs;
                e[2] = zx * nc - ys;
                e[3] = 0;

                e[4] = xy * nc - zs;
                e[5] = y * y * nc + c;
                e[6] = yz * nc + xs;
                e[7] = 0;

                e[8] = zx * nc + ys;
                e[9] = yz * nc - xs;
                e[10] = z * z * nc + c;
                e[11] = 0;

                e[12] = 0;
                e[13] = 0;
                e[14] = 0;
                e[15] = 1;
            }

            return this;
        }

        /**矩阵的转动*/
        public Matrix4 Rotate(double angle, double x, double y, double z)
        {
            return this.Concat(new Matrix4().SetRotate(angle, x, y, z));
        }

        /**查看物体*/
        public Matrix4 SetLookAt(double eyeX, double eyeY, double eyeZ, double centerX, double centerY, double centerZ, double upX, double upY, double upZ)
        {
            double[] e;
            double fx, fy, fz, rlf, sx, sy, sz, rls, ux, uy, uz;

            fx = centerX - eyeX;
            fy = centerY - eyeY;
            fz = centerZ - eyeZ;

            rlf = 1 / Math.Sqrt(fx * fx + fy * fy + fz * fz);
            fx *= rlf;
            fy *= rlf;
            fz *= rlf;

            sx = fy * upZ - fz * upY;
            sy = fz * upX - fx * upZ;
            sz = fx * upY - fy * upX;

            rls = 1 / Math.Sqrt(sx * sx + sy * sy + sz * sz);
            sx *= rls;
            sy *= rls;
            sz *= rls;

            ux = sy * fz - sz * fy;
            uy = sz * fx - sx * fz;
            uz = sx * fy - sy * fx;


            e = this.elements;
            e[0] = sx;
            e[1] = ux;
            e[2] = -fx;
            e[3] = 0;

            e[4] = sy;
            e[5] = uy;
            e[6] = -fy;
            e[7] = 0;

            e[8] = sz;
            e[9] = uz;
            e[10] = -fz;
            e[11] = 0;

            e[12] = 0;
            e[13] = 0;
            e[14] = 0;
            e[15] = 1;

            return this.Translation(-eyeX, -eyeY, -eyeZ);
        }

        /**查看物体*/
        public Matrix4 LookAt(double eyeX, double eyeY, double eyeZ, double centerX, double centerY, double centerZ, double upX, double upY, double upZ)
        {
            return this.Concat(new Matrix4().SetLookAt(eyeX, eyeY, eyeZ, centerX, centerY, centerZ, upX, upY, upZ));
        }




        /// <summary>
        /// 获取矩阵的最大缩放值
        /// </summary>
        /// <returns></returns>
        public double GetMaxScaleOnAxis()
        {
            var te = this.elements;

            var scaleXSq = te[0] * te[0] + te[1] * te[1] + te[2] * te[2];
            var scaleYSq = te[4] * te[4] + te[5] * te[5] + te[6] * te[6];
            var scaleZSq = te[8] * te[8] + te[9] * te[9] + te[10] * te[10];

            return Math.Sqrt(Math.Max(Math.Max(scaleXSq, scaleYSq), scaleZSq));

        }


        /// <summary>
        /// 设置当前正交矩阵
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="near"></param>
        /// <param name="far"></param>
        public Matrix4 SetOrthographic(double left, double right, double top, double bottom, double near, double far)
        {

            var te = this.elements;
            var w = right - left;
            var h = top - bottom;
            var p = far - near;

            var x = (right + left) / w;
            var y = (top + bottom) / h;
            var z = (far + near) / p;

            te[0] = 2 / w; te[4] = 0; te[8] = 0; te[12] = -x;
            te[1] = 0; te[5] = 2 / h; te[9] = 0; te[13] = -y;
            te[2] = 0; te[6] = 0; te[10] = -2 / p; te[14] = -z;
            te[3] = 0; te[7] = 0; te[11] = 0; te[15] = 1;

            return this;
        }

        /** 实现矩阵的乘法*/
        public Matrix4 Multiply(Matrix4 other)
        {
            return this.Concat(other);
        }

        /**
         * 重载 + 运算符
         * 
         * @return Matrix对象
         */
        public static Matrix4 operator +(Matrix4 m1, Matrix4 m2)
        {
            return m1.Add(m2);
        }

        /**
         * 重载 - 运算符
         * 
         * @return Matrix对象
         */
        public static Matrix4 operator -(Matrix4 m1, Matrix4 m2)
        {
            return m1.Subtract(m2);
        }

        /**
         * 重载 * 运算符
         * 
         * @return Matrix对象
         */
        public static Matrix4 operator *(Matrix4 m1, Matrix4 m2)
        {
            return m1.Multiply(m2);
        }

        /// <summary>
        /// 点的坐标转换
        /// </summary>
        /// <param name="m"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3D operator *(Matrix4 m, Vector3D v)
        {
            return new Vector3D(
                m.m11 * v.X + m.m12 * v.Y + m.m13 * v.Z + m.m14,
                m.m21 * v.X + m.m22 * v.Y + m.m23 * v.Z + m.m24,
                m.m31 * v.X + m.m32 * v.Y + m.m33 * v.Z + m.m34);
        }

        /// <summary>
        /// 向量的坐标转换
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Vector3D TransformVector(Vector3D v)
        {
            return this * v - this * Vector3D.Zero;
        }
        /**
         * 重载 double[] 运算符
         * 
         * @return double[]对象
         */
        public static implicit operator double[] (Matrix4 m)
        {
            return m.elements;

        }
        /// <summary>
        /// 判断两个矩阵是否相等
        /// </summary>
        /// <param name="m4"></param>
        /// <returns></returns>
        public bool IsAlmostEqualTo(Matrix4 m4)
        {

            var te = this.elements;
            var me = m4.elements;

            for (var i = 0; i < 16; i++)
            {

                if (te[i] != me[i]) return false;

            }
            return true;

        }

    }
}
