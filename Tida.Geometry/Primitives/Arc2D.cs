using System;
using System.Collections.Generic;
using System.Text;

namespace Tida.Geometry.Primitives {
    /// <summary>
    /// 几何对象-二维圆弧;
    /// </summary>
    public class Arc2D : Freezable {
        /// <summary>
        /// 指定圆弧的中心以构造一个圆弧;
        /// </summary>
        /// <param name="center"></param>
        public Arc2D(Vector2D center)
        {
            this.Center = center;
        }

        /// <summary>
        /// 默认构造方法;
        /// </summary>
        public Arc2D()
        {

        }

        private Vector2D _center;
        /// <summary>
        /// 圆弧中心;
        /// </summary>
        public Vector2D Center {
            get => _center;
            set => SetFreezableProperty(ref _center, value);
        }

        private double _radius;
        /// <summary>
        /// 圆弧的半径;
        /// </summary>
        public double Radius {
            get => _radius;
            set => SetFreezableProperty(ref _radius, value);
        }

        private double _startAngle;
        /// <summary>
        /// 起始角度(弧度);
        /// </summary>
        public double StartAngle {
            get => _startAngle;
            set => SetFreezableProperty(ref _startAngle,value);
        }

        private double _angle;
        /// <summary>
        /// 角度(逆时针计算);
        /// </summary>
        public double EndAngle {
            get => _angle;
            set => SetFreezableProperty(ref _angle, value);
        }


    }
}
