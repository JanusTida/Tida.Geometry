using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tida.Geometry.Primitives {
    /// <summary>
    /// 矩形重构版;相对<see cref="Rectangle2D"/>,本类使用的信息更少;
    /// </summary>
    public class Rectangle2D2 {
        

        public Rectangle2D2(Line2D middleLine2D,double width) {
            MiddleLine2D = middleLine2D;
            Width = width;
        }

        /// <summary>
        /// 创建一个新的空矩形;
        /// </summary>
        /// <returns></returns>
        public static Rectangle2D2 CreateEmpty() {
            return new Rectangle2D2(Line2D.Zero, double.NaN);
        }

        /// <summary>
        /// 矩形的两条中线之一;该值不可为空;
        /// </summary>
        private Line2D _middleLine2D;
        public Line2D MiddleLine2D {
            get {
                return _middleLine2D;
            }
            set {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }
                
                _middleLine2D = value;
            }
        }

        /// <summary>
        /// 与<see cref="MiddleLine2D"/>垂直的一对边的长度;
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 获取所有顶点;延迟返回;
        /// </summary>
        public IEnumerable<Vector2D> GetVertexes() {
            if (MiddleLine2D == null) {
                yield break;
            }

            //获取中线的单位向量,后求其法向量乘以宽度一半,即另一中线的向量一般;
            var dir = MiddleLine2D.Direction;
            if (dir == null) {
                yield break;
            }

            //乘以宽度;
            var verticalVector = new Vector2D(-dir.Y, dir.X) * Width / 2;
            

            yield return MiddleLine2D.Start + verticalVector;
            yield return MiddleLine2D.Start - verticalVector;
            yield return MiddleLine2D.End - verticalVector;
            yield return MiddleLine2D.End + verticalVector;
        }
        
        /// <summary>
        /// 获取所有边;延迟返回;
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Line2D> GetLines() {
            if (MiddleLine2D == null) {
                yield break;
            }

            var vertexes = GetVertexes();
            if(vertexes == null) {
                yield break;
            }
            
            //记录第一个顶点;
            Vector2D firstVertex = null;
            //记录轮询中最后一个顶点;
            Vector2D lastVertex = null;

            //轮询返回0-1,1-2,2-3边;
            foreach (var vertex in vertexes) {
                if (firstVertex == null) {
                    firstVertex = vertex;
                }

                if(lastVertex != null) {
                    yield return new Line2D(lastVertex, vertex);
                }
                lastVertex = vertex;
            }

            //返回3-0边;
            if(lastVertex != firstVertex) {
                yield return new Line2D(lastVertex, firstVertex);
            }

        }

        /// <summary>
        /// 中心;
        /// </summary>
        public Vector2D Center {
            get {
                if(MiddleLine2D == null) {
                    return null;
                }
                return MiddleLine2D.MiddlePoint;
            }
        }

        public static Rectangle2D2 operator + (Rectangle2D2 rectangle2D2,Vector2D offset) {
            return new Rectangle2D2(rectangle2D2.MiddleLine2D.CreateOffset(offset), rectangle2D2.Width);
        }
    }
}
