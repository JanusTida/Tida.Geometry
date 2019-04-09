using Tida.Geometry.Alternation;

namespace Tida.Geometry.Primitives
{
    /// <summary>
    /// 用于定义一个三维实体对象
    /// </summary>

    public abstract class Object3D
    {

        /// <summary>
        /// 当前的转换矩阵
        /// </summary>
        private TransformUtil transform =null;
        /// <summary>
        /// 当前三维物体的坐标转换类
        /// </summary>
       public TransformUtil Transform
        {
            set{
            
                this.transform=value;
                this.Vary();
            }
            get {
                return transform;
            }
        }
        /// <summary>
        /// 对当前三维物体进行坐标转化
        /// </summary>
        /// <param name="m4"></param>
        protected abstract void Vary();
    }
}
