using Tida.Geometry.Alternation;
using Tida.Geometry.Primitives;
using System;


namespace Tida.Geometry.External
{
    public class GlobalToLocalCoordinateAlgorithm
    {
        private Vector3D localOrigin;
        private Vector3D axisX;
        private Vector3D axisY;
        /// <summary>
        /// 全局坐标系转换成局部坐标系
        /// </summary>
        /// <param name="localOrigin">局部坐标系中的原点</param>
        /// <param name="axisX">局部坐标系中的x轴，是单位向量</param>
        /// <param name="axisY">局部坐标系中的y轴，是单位向量</param>
        public GlobalToLocalCoordinateAlgorithm(Vector3D localOrigin, Vector3D axisX, Vector3D axisY)
        {
            this.axisX = axisX;
            this.axisY = axisY;
            this.localOrigin = localOrigin;
        }


        /// <summary>
        /// 旧坐标系转换到新坐标系的转换经过了三个步骤：原点平移，绕z轴旋转某个角度，绕x旋转某个角度；
        /// 如果旧坐标系到新坐标系不是这样变换来的，那当然不能使用这个方法；
        /// 平移矩阵，绕z旋转矩阵,绕x旋转矩阵，请参照计算机图形学相关知识
        /// </summary>
        /// <returns>返回旧坐标系转换到新坐标系的转换矩阵</returns>
        public Matrix GetOZXTransformMatrix()
        {
            //绕Vector3D.BasisZ轴旋转从Vector3D.BasisX到axisX，并计算出新坐标系的方向
            double angle1 = GetRotationAngel(Vector3D.BasisX, axisX, Vector3D.BasisZ);
            Vector3D newAxisx1 = new Vector3D(axisX.X, axisX.Y, axisX.Z);
            Vector3D newAxisy1 = Vector3D.BasisZ.Cross(newAxisx1);
            Vector3D newAxisz1 = Vector3D.BasisZ;

            //绕新坐标的newAxisx1轴从newAxisy1到axisY， 并计算出新坐标系的方向   
            double angle2 = GetRotationAngel(newAxisy1, axisY, newAxisx1);
            Vector3D newAxisx2 = new Vector3D(newAxisx1.X, newAxisx1.Y, newAxisx1.Z);
            Vector3D newAxisy2 = axisY;
            Vector3D newAxisz2 = newAxisx2.Cross(newAxisy2);

            //平移矩阵
            double[] value1 = new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -localOrigin.X, -localOrigin.Y, -localOrigin.Z, 1 };
            Matrix matrix1 = new Matrix(4, 4, value1);

            //绕z轴旋转 某个角度
            double[] value2 = new double[] { Math.Cos(angle1), Math.Sin(angle1), 0, 0, -Math.Sin(angle1), Math.Cos(angle1), 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
            Matrix matrix2 = new Matrix(4, 4, value2);

            //绕x轴旋转 某个角度
            double[] value3 = new double[] { 1, 0, 0, 0, 0, Math.Cos(angle2), Math.Sin(angle2), 0, 0, -Math.Sin(angle2), Math.Cos(angle2), 0, 0, 0, 0, 1 };
            Matrix matrix3 = new Matrix(4, 4, value3);

            Matrix tMatrix = matrix1 * matrix2 * matrix3;
            return tMatrix;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="positive">正方向</param>
        /// <returns></returns>
        private  double GetRotationAngel(Vector3D source, Vector3D target, Vector3D positive)
        {
            double angel = source.AngleTo(target);
            Vector3D thirdAxis = source.Cross(target).Normalize();
            if (thirdAxis.IsAlmostEqualTo(positive)) //如果是逆时针旋转得到则角度返号
            {
                angel = -angel;
            }
            return angel;
        }

        public Vector3D Transform(Vector3D source, Matrix transMatrix)
        {
            Matrix resultMatrix1 = new Matrix(1, 4);
            double[] sourceValue = new double[] { source.X, source.Y, source.Z, 1 };
            Matrix initialMatrix1 = new Matrix(1, 4, sourceValue);
            resultMatrix1 = initialMatrix1 * transMatrix;
            Vector3D targetPoint = new Vector3D(resultMatrix1.GetElement(0, 0), resultMatrix1.GetElement(0, 1), resultMatrix1.GetElement(0, 2));
            return targetPoint;
        }

        public Vector2D TransformTo2D(Vector3D source)
        {
            Matrix transMatrix = GetOZXTransformMatrix();
            Vector3D vector3D = Transform(source, transMatrix);
            return new Vector2D(vector3D.X,vector3D.Y);
        }

        /// <summary>
        /// 与Transform是相反的过程
        /// </summary>
        /// <returns>返回一个转换矩阵</returns>
        public Matrix GetRETransformMatrix()
        {
            //由局部到整体的坐标转换，就是取angel1，angel2，（x,y,z）的相反数，然后矩阵反着想成m3*m2*m1
            //中间求解newaxis1，newaxis2并不是逆向转换每一步得到的新坐标系，仅复制transform的内容，但是angel1，angel2，的值是正确的

            //绕Vector3D.BasisZ轴旋转从Vector3D.BasisX到axisX，并计算出新坐标系的方向
            double angle1 = GetRotationAngel(Vector3D.BasisX, axisX, Vector3D.BasisZ);
            Vector3D newAxisx1 = new Vector3D(axisX.X, axisX.Y, axisX.Z);
            Vector3D newAxisy1 = Vector3D.BasisZ.Cross(newAxisx1);
            Vector3D newAxisz1 = Vector3D.BasisZ;
            angle1 = -angle1;

            //绕新坐标的newAxisx1轴从newAxisy1到axisY， 并计算出新坐标系的方向   
            double angle2 = GetRotationAngel(newAxisy1, axisY, newAxisx1);
            Vector3D newAxisx2 = new Vector3D(newAxisx1.X, newAxisx1.Y, newAxisx1.Z);
            Vector3D newAxisy2 = axisY;
            Vector3D newAxisz2 = newAxisx2.Cross(newAxisy2);
            angle2 = -angle2;

            //平移矩阵
            localOrigin = -localOrigin;
            double[] value1 = new double[] { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, -localOrigin.X, -localOrigin.Y, -localOrigin.Z, 1 };
            Matrix matrix1 = new Matrix(4, 4, value1);

            //绕z轴旋转 某个角度
            double[] value2 = new double[] { Math.Cos(angle1), Math.Sin(angle1), 0, 0, -Math.Sin(angle1), Math.Cos(angle1), 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };
            Matrix matrix2 = new Matrix(4, 4, value2);

            //绕x轴旋转 某个角度
            double[] value3 = new double[] { 1, 0, 0, 0, 0, Math.Cos(angle2), Math.Sin(angle2), 0, 0, -Math.Sin(angle2), Math.Cos(angle2), 0, 0, 0, 0, 1 };
            Matrix matrix3 = new Matrix(4, 4, value3);

            Matrix tMatrix = matrix3 * matrix2 * matrix1;
            return tMatrix;
        }
    }
}
