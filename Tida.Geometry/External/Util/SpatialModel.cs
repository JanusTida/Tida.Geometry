using System.Collections.Generic;

namespace Tida.Geometry.External.Util
{
    /// <summary>
    /// 代表一个封闭空间
    /// </summary>
    public class SpatialModel
    {
        /// <summary>
        /// 一个多边形封闭区域的所有线型
        /// </summary>
      public  List<LineModel> LineModels
        {
            get;
            set;
        }
        /// <summary>
        /// 当前封闭区域的Id信息
        /// </summary>
        public int Id {

            get;
            set;
        }

        public int NumberName {
            get;
            set;
        }
    }
}
