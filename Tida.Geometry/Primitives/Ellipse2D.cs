using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tida.Geometry.Primitives {
    /// <summary>
    /// 平面椭圆或圆;
    /// </summary>
    public class Ellipse2D:ICloneable<Ellipse2D> {
        public Ellipse2D() {

        }
        public Ellipse2D(Vector2D center,double radiusX,double radiusY) {
            this.Center = center;
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        /// 判断某点是否在椭圆内部;
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Contains(Vector2D position) {
            if(position == null) {
                throw new ArgumentNullException(nameof(position));
            }

            if (Center == null) {
                return false;
            }
            
            if(RadiusX == 0 || RadiusY == 0) {
                return false;
            }

            //点与椭圆的包含关系通过(b^2 * x^2 +  a^2 * y^2)是否大于a^2 * b^2判断;
            var x = (position.X - Center.X);
            var y = (position.Y - Center.Y);
            var a2 = RadiusX * RadiusX;
            var b2 = RadiusY * RadiusY;

            return (x * x * b2) + (y * y * a2) <= a2 * b2;
        }

        public Ellipse2D Clone() => new Ellipse2D(Center, RadiusX, RadiusY);

        /// <summary>
        /// 延横轴方向的半径;
        /// </summary>
        public double RadiusX { get; set; }

        /// <summary>
        /// 延纵轴方向的半径
        /// </summary>
        public double RadiusY { get; set; }

        /// <summary>
        /// 圆心所在位置;
        /// </summary>
        public Vector2D Center { get; set; }

        /// <summary>
        /// 获取上顶点;
        /// </summary>
        /// <returns></returns>
        public Vector2D GetTopPoint() {
            if(Center == null) {
                return null;
            }
            return new Vector2D(Center.X, Center.Y + RadiusY);
        }

        /// <summary>
        /// 获取下顶点;
        /// </summary>
        /// <returns></returns>
        public Vector2D GetBottomPoint() {
            if (Center == null) {
                return null;
            }
            return new Vector2D(Center.X, Center.Y - RadiusY);
        }

        /// <summary>
        /// 获取左顶点;
        /// </summary>
        /// <returns></returns>
        public Vector2D GetLeftPoint() {
            if (Center == null) {
                return null;
            }
            return new Vector2D(Center.X - RadiusX, Center.Y);
        }

        /// <summary>
        /// 获取右顶点;
        /// </summary>
        /// <returns></returns>
        public Vector2D GetRightPoint() {
            if (Center == null) {
                return null;
            }
            return new Vector2D(Center.X + RadiusX, Center.Y);
        }

        public static Ellipse2D operator +(Ellipse2D ellipse2D,Vector2D offset) {
            return new Ellipse2D(ellipse2D.Center + offset, ellipse2D.RadiusX, ellipse2D.RadiusY);
        }


    }
}
