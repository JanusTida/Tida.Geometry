using Tida.Geometry.Primitives;
using System.Collections.Generic;
using Tida.Geometry.External;

namespace Tida.Geometry.Alternation
{
    /// <summary>
    /// 当前类的主要用途：将空间中任意一个面，转换到XY平面上
    /// </summary>
    public class CoordinateAlgorithm
    {
        /// <summary>
        /// 要转换的三维线集合
        /// </summary>
        private List<Line3D> line3ds
        {
            get;
            set;
        }

        /// <summary>
        /// 当前的本地坐标
        /// </summary>
        private Vector3D localPosition
        {
            get;
            set;
        }


        /// <summary>
        /// 当前的转换矩阵
        /// </summary>
        public Matrix4 CurrentMatrix4
        {
            get;
            set;
        }

        /// <summary>
        /// 当前的逆矩阵
        /// </summary>
        public Matrix4 InversetMatrix4
        {
            get
            {
                Matrix4 inversetMatrix4 = CurrentMatrix4.Clone();
                return inversetMatrix4.GetInverse();
            }
            set { }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CoordinateAlgorithm(List<Line3D> lines)
        {
            this.line3ds = lines;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public CoordinateAlgorithm()
        {
        }


        /// <summary>
        /// 构造函数，初始化当前平面
        /// </summary>
        /// <param name="face"></param>
        public CoordinateAlgorithm(Face face)
        {
            this.line3ds = face.Edges;
        }

        /// <summary>
        /// 将当前平面转化XY平面上，返回转换矩阵
        /// </summary>
        /// <returns></returns>
        public Matrix4 TransformXY()
        {

            //首先获取当前面的法线设置为Z轴
            Vector3D axisZ = this.line3ds.GetNormal().Normalize();
            //任意取一条线的方向为X轴相仿
            Vector3D axisX = this.line3ds[0].Direction.Normalize();
            //取得Y轴的向量
            Vector3D axisY = axisX.Cross(axisZ).Normalize();
            //原点
            localPosition = Vector3D.Zero;
            //返回转换后的逆矩阵
            return this.Transform(localPosition, axisX, axisY, axisZ);

        }

        /// <summary>
        ///  将当前平面转化XY平面上，返回转换矩阵
        /// </summary>
        /// <param name="origin">指定原点</param>
        /// <returns></returns>
        public Matrix4 TransformXY(Vector3D origin)
        {
            this.localPosition = origin;
            //首先获取当前面的法线设置为Z轴
            Vector3D axisZ = this.line3ds.GetNormal().Normalize();
            //任意取一条线的方向为X轴相仿
            Vector3D axisX = this.line3ds[0].Direction.Normalize();
            //取得Y轴的向量
            Vector3D axisY = axisX.Cross(axisZ).Normalize();
            //返回转换后的逆矩阵
            return this.Transform(localPosition, axisX, axisY, axisZ);

        }


        /// <summary>
        /// 转换到任意坐标系,返回转换矩阵
        /// </summary>
        /// <param name="origin">表示三维转换的原点</param>
        /// <param name="localAxisX">要转换的X轴向量</param>
        /// <param name="localaxisY">要转换的Y轴向量</param>
        /// <param name="localaxisZ">要转换的Z轴向量</param>
        /// <returns></returns>
        public Matrix4 Transform(Vector3D origin, Vector3D xAxis, Vector3D yAxis, Vector3D zAxis = null)
        {

            Vector3D xDir = xAxis.Normalize();
            Vector3D yDir = yAxis.Normalize();
            Vector3D zDir = null;
            if (zAxis == null)
                zDir = (xDir.Cross(yDir)).Normalize();
            else
                zDir = zAxis.Normalize();

            //旋转矩阵
            double[] rotDB = new double[] 
            { xDir.X, xDir.Y, xDir.Z, 0,
                yDir.X, yDir.Y, yDir.Z, 0,
                zDir.X, zDir.Y, zDir.Z,
                0, 0, 0, 0, 1 };
            Matrix4 mRot = new Matrix4(4, 4, rotDB);

            //平移矩阵
            double[] posDB = new double[]
          {    1, 0, 0, -origin.X,
                0, 1, 0, -origin.Y,
                0, 0, 1, -origin.Z,
                0, 0, 0, 1 };

            Matrix4 mPos = new Matrix4(4, 4, posDB);

            CurrentMatrix4= mPos * mRot;
            
            //给三维物体重新设定矩形信息
            if (this.line3ds != null)
            {
                this.line3ds.ForEach(x =>
                {
                    x.Transform = new TransformUtil(CurrentMatrix4);
                });
            }

            return CurrentMatrix4;
        }

        /// <summary>
        /// 通过指定的矩阵进行转换
        /// </summary>
        /// <param name="m4">指定的矩阵</param>
        public Matrix4 Transform(Matrix4 m4)
        {
            CurrentMatrix4 = m4;
            //给三维物体重新设定矩形信息
            this.line3ds.ForEach(x =>
            {
                x.Transform = new TransformUtil(CurrentMatrix4);
            });

            return CurrentMatrix4;
        }


        /// <summary>
        /// 通过指定的矩阵进行转换
        /// </summary>
        /// <param name="m4">指定的矩阵</param>
        public Matrix4 Transform(List<Line3D> lines, Matrix4 m4)
        {
            //指定当前的线集合
            this.line3ds = lines;

            CurrentMatrix4 = m4;
            //给三维物体重新设定矩形信息
            this.line3ds.ForEach(x =>
            {
                x.Transform = new TransformUtil(CurrentMatrix4);
            });

            return CurrentMatrix4;
        }



        /// <summary>
        /// 坐标系转换
        /// </summary>
        /// <param name="transMatrix">转换矩阵</param>
        /// <param name="line3Ds">待转换的线</param>
        public static void TransformLines(Matrix4 transMatrix, List<Line3D> line3Ds)
        {
            line3Ds.ForEach(x =>
            {
                TransformLine(transMatrix, x);
            });
        }

        public static void TransformLine(Matrix4 transMatrix, Line3D line)
        {
            if (transMatrix == null || line == null)
                return;
            Vector3D start = line.Start;
            Vector3D end = line.End;

            //开始转换
            Vector3D newStart = transMatrix * start;
            Vector3D newEnd = transMatrix * end;

            line.Start = newStart;
            line.End = newEnd;
        }

        /// <summary>
        /// 转换向量
        /// </summary>
        /// <param name="transMatrix"></param>
        /// <param name="direction">方向向量</param>
        public static Vector3D TransformDirection(Matrix4 transMatrix, Vector3D direction)
        {
            if (transMatrix == null || direction == null)
                return null;

            Vector3D start = Vector3D.Zero;
            Vector3D end = direction;

            //开始转换
            Vector3D newStart = transMatrix * start;
            Vector3D newEnd = transMatrix * end;

            Vector3D newDirection = (newEnd - newStart).Normalize();
            return newDirection;
        }

        /// <summary>
        /// 通过指定的矩阵进行转换
        /// </summary>
        /// <param name="m4">指定的矩阵</param>
        public Matrix4 Transform(Line3D line, Matrix4 m4)
        {
            //指定当前的线集合
            this.line3ds = new List<Line3D>();
            line3ds.Add(line);
            CurrentMatrix4 = m4;
            //给三维物体重新设定矩形信息
            this.line3ds.ForEach(x =>
            {
                x.Transform = new TransformUtil(CurrentMatrix4);
            });

            return CurrentMatrix4;
        }

        /// <summary>
        /// 反向转换当前矩阵信息
        /// </summary>
        public void UnTransform()
        {
            //给三维物体重新设定矩形信息
            this.line3ds.ForEach(x =>
            {
                x.Transform = new TransformUtil(this.InversetMatrix4);
            });
        }

    }
}
