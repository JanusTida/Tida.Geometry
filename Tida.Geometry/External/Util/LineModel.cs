using Tida.Geometry.Primitives;

namespace Tida.Geometry.External.Util
{
    /// <summary>
    /// 数据模型
    /// </summary>
    public class LineModel
    {

        public LineModel(Line2D l, int useage, LineType type) {

            this.Line = l;
            this.UseAge = useage;
            this.LineType = type;
        }
        /// <summary>
        /// 线的信息
        /// </summary>
        public Line2D Line
        {

            get; set;
        }
        /// <summary>
        /// 使用次数
        /// </summary>
        public int UseAge
        {

            set; get;
        }

        public LineType LineType { set; get; }
    }

    public enum LineType {

        outer=0,
        inner=1,
        Separate=2,
        Assist=3
    }
}
