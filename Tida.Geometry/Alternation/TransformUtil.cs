using System.Collections.Generic;
using Tida.Geometry.External;
using Tida.Geometry.Primitives;

namespace Tida.Geometry.Alternation
{
    public class TransformUtil
    {
        /// <summary>
        /// 在世界坐标系中的位置
        /// </summary>

        public Vector3D Position
        {
            set;
            get;
        }

        /// <summary>
        /// 在转换坐标系中的位置
        /// </summary>

        public Vector3D LocalPosition
        {
            set;
            get;

        }

        /// <summary>
        /// 当前投影中心点的位置
        /// </summary>

        public Vector3D PerspectiveProjection
        {

            set;
            get;
        }


        /// <summary>
        /// 构造函数，初始化当前的矩阵信息
        /// </summary>

        public Matrix4 Matrix
        {
            set;
            get;
        }



        /// <summary>
        /// 构造函数，初始化一个矩阵信息
        /// </summary>
        public TransformUtil()
        {
            this.Matrix = Matrix4.Create();
        }

        /// <summary>
        /// 使用指定矩阵，生成一个Transform
        /// </summary>
        /// <param name="matrix"></param>
        public TransformUtil(Matrix4 matrix)
        {
            this.Matrix = matrix;
        }




        /// <summary>
        /// 在矩阵中移动
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Translate(double x, double y, double z)
        {
            this.Matrix.Translation(x, y, z);
        }
        /// <summary>
        /// X轴的移动
        /// </summary>
        /// <param name="x"></param>
        public void TranslateX(double x)
        {
            this.Matrix.Translation(x, 0, 0);
        }
        /// <summary>
        /// Y轴的移动
        /// </summary>
        /// <param name="y"></param>
        public void TranslateY(double y)
        {

            this.Matrix.Translation(0, y, 0);
        }
        /// <summary>
        /// Z轴的移动
        /// </summary>
        /// <param name="z"></param>
        public void TranslateZ(double z)
        {
            this.Matrix.Translation(0, 0, z);
        }
        /// <summary>
        /// 当前图形的缩放
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Scale(double x, double y, double z)
        {

            this.Matrix.Scale(x, y, z);
        }
        /// <summary>
        /// X轴的缩放
        /// </summary>
        /// <param name="x"></param>
        public void ScaleX(double x)
        {
            this.Matrix.Scale(x, 0, 0);
        }
        /// <summary>
        /// Y轴的缩放
        /// </summary>
        /// <param name="y"></param>
        public void ScaleY(double y)
        {
            this.Matrix.Scale(0, y, 0);
        }
        /// <summary>
        /// Z轴的缩放
        /// </summary>
        /// <param name="z"></param>
        public void ScaleZ(double z)
        {
            this.Matrix.Scale(0, 0, z);
        }
        /// <summary>
        /// 坐标轴的转动
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Rotate(double angle, double x, double y, double z)
        {

            this.Matrix.Rotate(angle, x, y, z);
        }
        /// <summary>
        /// 在X轴上的转动
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="x"></param>
        public void RotateX(double angle, double x)
        {

            this.Matrix.Rotate(angle, x, 0, 0);
        }
        /// <summary>
        /// 在Y轴上的转动
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="y"></param>
        public void RotateY(double angle, double y)
        {
            this.Matrix.Rotate(angle, 0, y, 0);
        }
        /// <summary>
        /// 在Z轴上的转动
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="z"></param>
        public void RotateZ(double angle, double z)
        {
            this.Matrix.Rotate(angle, 0, 0, z);
        }

        /// <summary>
        /// 图形的切错
        /// </summary>
        /// <param name="x_angle"></param>
        /// <param name="y_angle"></param>
        public void Skew(double x_angle, double y_angle)
        {


        }
        /// <summary>
        /// X轴方向的切错
        /// </summary>
        /// <param name="x_angle"></param>
        public void SkewX(double x_angle) { }

        /// <summary>
        /// Y轴方向的切错
        /// </summary>
        /// <param name="y_angle"></param>
        public void SkewY(double y_angle) { }


        public static Matrix4 ViewMatrix(Vector3D pos, Vector3D xAxis, Vector3D yAxis)
        {
            Vector3D xDir = xAxis.Normalize();
            Vector3D yDir = yAxis.Normalize();
            Vector3D zDir = (xDir.Cross(yDir)).Normalize();

            //旋转矩阵
            double[] rotDB = new double[]
            { xDir.X, xDir.Y, xDir.Z, 0,
                yDir.X, yDir.Y, yDir.Z, 0,
                zDir.X, zDir.Y, zDir.Z,
                0, 0, 0, 0, 1 };
            Matrix4 mRot = new Matrix4(4, 4, rotDB);

            //平移矩阵
            double[] posDB = new double[]
          {    1, 0, 0, -pos.X,
                0, 1, 0, -pos.Y,
                0, 0, 1, -pos.Z,
                0, 0, 0, 1 };

            Matrix4 mPos = new Matrix4(4, 4, posDB);

            return mPos * mRot;

        }

        /// <summary>
        /// 转换向量
        /// </summary>
        /// <param name="transMatrix"></param>
        /// <param name="direction">方向向量</param>
        public static Vector3D TransformDirection(Matrix4 transMatrix, Vector3D direction)
        {
            if (transMatrix == null)
                throw new System.ArgumentException("参数值为空。", nameof(transMatrix));
            if (direction == null)
                throw new System.ArgumentException("参数值为空。", nameof(direction));

            Vector3D start = Vector3D.Zero;
            Vector3D end = direction;

            //开始转换
            Vector3D newStart = transMatrix * start;
            Vector3D newEnd = transMatrix * end;

            Vector3D newDirection = (newEnd - newStart).Normalize();
            return newDirection;
        }

        /// <summary>
        /// 转换三维点坐标
        /// </summary>
        /// <param name="transMatrix"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static Vector3D TransformPoint(Matrix4 transMatrix, Vector3D point)
        {
            return transMatrix * point;
        }

        /// <summary>
        /// 批量转换线段集合的坐标
        /// </summary>
        /// <param name="transMatrix">转换矩阵</param>
        /// <param name="line3Ds">待转换的线</param>
        public static List<Line3D> TransformLines(Matrix4 transMatrix, List<Line3D> line3Ds)
        {
            if (transMatrix == null)
                throw new System.ArgumentException("参数值为空。", nameof(transMatrix));
            if (line3Ds == null || line3Ds.Count == 0)
                throw new System.ArgumentException("参数值为空。", nameof(line3Ds));

            List<Line3D> newLines = new List<Line3D>();
            line3Ds.ForEach(x =>
            {
                newLines.Add(TransformLine(transMatrix, x));
            });

            return newLines;
        }

        /// <summary>
        /// 转换三维线段坐标
        /// </summary>
        /// <param name="transMatrix"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static Line3D TransformLine(Matrix4 transMatrix, Line3D line)
        {
            if (transMatrix == null)
                throw new System.ArgumentException("参数值为空。", nameof(transMatrix));
            if (line == null)
                throw new System.ArgumentException("参数值为空。", nameof(line));

            Vector3D start = line.Start;
            Vector3D end = line.End;

            //开始转换
            Vector3D newStart = transMatrix * start;
            Vector3D newEnd = transMatrix * end;
            return new Line3D(newStart, newEnd);
        }


        /// <summary>
        /// 将三维坐标，直接平移为平面二维坐标
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2D Projection(Vector3D v)
        {
            return new Vector2D(v.X, v.Y);
        }
        /// <summary>
        /// 将三维坐标，直接投影为到其他平面
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector3D Transform(Vector3D v, Matrix4 m)
        {
            Vector3D nv = m * v;
            return nv;
        }


        /// <summary>
        /// 直接将二维坐标转换到指定的空间坐标
        /// </summary>
        /// <param name="v"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3D Projection(Vector2D v, double z)
        {
            return new Vector3D(v.X, v.Y, z);
        }
        /// <summary>
        /// 将三维线直接投影为二维线段
        /// </summary>
        /// <param name="line3d"></param>
        /// <returns></returns>
        public static Line2D Projection(Line3D line3d)
        {
            Line2D line2d = Line2D.Create(Projection(line3d.Start), Projection(line3d.End));
            return line2d;
        }


        /// <summary>
        /// 将三维线直接投影为二维线段
        /// </summary>
        /// <param name="line3d"></param>
        /// <returns></returns>
        public static List<Line2D> Projection(List<Line3D> line3ds)
        {
            List<Line2D> line2ds = new List<Line2D>();
            line3ds.ForEach(x =>
            {
                line2ds.Add(Line2D.Create(Projection(x.Start), Projection(x.End)));
            });
            return line2ds;
        }

        /// <summary>
        /// 将三维还原到二维平面中
        /// </summary>
        /// <param name="line2ds"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static List<Line3D> Projection(List<Line2D> line2ds, double z)
        {
            List<Line3D> line3ds = new List<Line3D>();
            line2ds.ForEach(x =>
            {
                line3ds.Add(Line3D.Create(Projection(x.Start, z), Projection(x.End, z)));
            });
            return line3ds;
        }
        /// <summary>
        /// 进行投影转换操作
        /// </summary>
        /// <param name="line3D"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Line3D Transform(Line3D line3D, Matrix4 m)
        {
            Line3D nline3D = Line3D.Create(m * line3D.Start, m * line3D.End);
            return nline3D;
        }


        /// <summary>
        /// 坐标系转换
        /// </summary>
        /// <param name="transMatrix">转换矩阵</param>
        /// <param name="line3Ds">待转换的线</param>
        public static List<Line3D> Transform(List<Line3D> line3ds, Matrix4 m)
        {
            List<Line3D> nline3ds = new List<Line3D>();
            line3ds.ForEach(x =>
            {
                nline3ds.Add(Transform(x, m));
            });
            return nline3ds;
        }

        /// <summary>
        /// 将二维线段，向上偏移到三维空间
        /// </summary>
        /// <param name="line2d"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Line3D Projection(Line2D line2d, double z)
        {
            Line3D line3d = Line3D.Create(Projection(line2d.Start, z), Projection(line2d.End, z));
            return line3d;
        }

        /// <summary>
        /// 获取一个点的镜像点
        /// </summary>
        /// <param name="v"></param>
        /// <param name="l"></param>
        /// <returns></returns>
        public static Vector2D Mirror(Vector2D v, Line2D mirror)
        {
            Vector2D p = v.ProjectOn(mirror);
            Line2D le = Line2D.Create(v, p);
            Vector2D re = p.Offset(le.Direction * le.Length);
            return re;
        }

        /// <summary>
        /// 通过一个点，和点上三个对应的三个互相垂直的线，获取一个转换矩阵
        /// </summary>
        /// <returns></returns>
        public static Matrix4 GetMatrix(Vector3D pos, Vector3D xAxis, Vector3D yAxis)
        {
            Vector3D xDir = xAxis.Normalize();
            Vector3D yDir = yAxis.Normalize();
            Vector3D zDir = (xDir.Cross(yAxis)).Normalize();

            //旋转矩阵
            double[] rotDB = new double[]
            { xDir.X, xDir.Y, xDir.Z, 0,
                yDir.X, yDir.Y, yDir.Z, 0,
                zDir.X, zDir.Y, zDir.Z,
                0, 0, 0, 0, 1 };
            Matrix4 mRot = new Matrix4(4, 4, rotDB);

            //平移矩阵
            double[] posDB = new double[]
          {    1, 0, 0, -pos.X,
                0, 1, 0, -pos.Y,
                0, 0, 1, -pos.Z,
                0, 0, 0, 1 };

            Matrix4 mPos = new Matrix4(4, 4, posDB);

            return mPos * mRot;

        }

        /// <summary>
        /// 通过直线获取转换矩阵
        /// </summary>
        /// <param name="searchLines"></param>
        /// <returns></returns>
        public static Matrix4 GetMatrix(List<Line3D> line3ds)
        {
            var origin = line3ds[0].Start;
            //先获取这个面的法线
            Vector3D axisZ = line3ds.GetNormal().Normalize();
            //把当前点所在的直线作为X轴
            Vector3D axisX = line3ds[0].Direction.Normalize();
            //计算出Y轴的向量
            Vector3D axisY = axisX.Cross(axisZ).Normalize();
            //返回转换后的逆矩阵
            return GetMatrix(origin, axisX, axisY);
        }


        /// <summary>
        /// 获取一个矩阵的逆矩阵
        /// </summary>
        public static Matrix4 GetInversetMatrix(Matrix4 m)
        {
            Matrix4 cm = m.Clone();
            return cm.GetInverse();
        }
    }



}