using Tida.Geometry.Primitives;
using System;
using System.Collections.Generic;

namespace Tida.Geometry.External.Util
{
    /// <summary>
    /// 创建数据线段
    /// </summary>
    public class LinesModelCreator
    {
        /// <summary>
        /// 用于创建当前的数据线段
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static List<LineModel> Create(List<Line2D> lines)
        {


            //打断所有的线段
            List<Line2D> decomposeLines = lines.Decompose();

            //查找最外边缘
            List<Line2D> outerLines = GraphicAlgorithm.FindClosedLines(decomposeLines, true, false);

            if (outerLines == null)
            {

                throw new Exception("当前没有一个封闭区域");
            }
            for (int i = 0; i < outerLines.Count; i++)
            {

                Line2D line = decomposeLines.Find(x => x.IsAlmostEqualTo(outerLines[i]));
                decomposeLines.Remove(line);
            }
            //查找内墙
            List<Line2D> innerLines = new List<Line2D>(decomposeLines);

            //数据模型
            List<LineModel> lineModels = new List<LineModel>();
            //组建数据源
            outerLines.ForEach(x => {

                lineModels.Add(new LineModel(x, 1, LineType.outer));
            });

            //内装线添加
            innerLines.ForEach(x => {

                lineModels.Add(new LineModel(x, 2, LineType.inner));
            });

            return lineModels;
        }
    }
}
