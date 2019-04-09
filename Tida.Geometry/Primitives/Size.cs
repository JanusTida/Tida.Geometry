namespace Tida.Geometry.Primitives
{
    /// <summary>
    /// 用于定义一个尺寸信息类
    /// </summary>

    public class Size
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Size()
        {
            Width = 0;
            Height =0;
        }
        /// <summary>
        /// 构造函数，初始化一个尺寸
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        private Size(double w, double h)
        {
            Width = w;
            Height = h;
        }

        /// <summary>
        /// 创建一个尺寸
        /// </summary>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static Size Create(double w, double h)
        {
            return new Size(w, h);
        }

        /// <summary>
        /// 尺寸的宽度
        /// </summary>
        
       public double Width { get; set; }
        /// <summary>
        /// 尺寸的高度
        /// </summary>
         
       public double Height { get; set; }
    }
}
