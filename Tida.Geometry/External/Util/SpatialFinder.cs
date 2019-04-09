using Tida.Geometry.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tida.Geometry.External.Util
{
    /// <summary>
    /// 用于查找当前的封闭区域
    /// </summary>
    public class SpatialFinder
    {

        private int tick = 0;


        /// <summary>
        /// 查找所有的封闭区域，封闭区域的lineModel的位置是按逆时针排序，但线不是首尾相接的
        /// </summary>
        /// <param name="linemodels"></param>
        /// <returns></returns>
        public List<SpatialModel> Find(List<LineModel> lineModels)
        {

            //获取房间内部的线段
            List<LineModel> separateLine = this.GetSeparateInteriorLines(lineModels);

            ///当前所有的封闭区域模型
            List<SpatialModel> spatialModels = new List<SpatialModel>();

            List<LineModel> outModels = new List<LineModel>();
            //找到所有的外边缘线
            lineModels.ForEach(x =>
            {

                if (x.LineType==LineType.outer)
                {

                    outModels.Add(x);
                }
            });

            //查找所有的内部房间区域
            this.FindSpatialFromLinesModel(lineModels, separateLine, spatialModels, outModels);

            for (int i = 0; i < spatialModels.Count; i++) {
                spatialModels[i].NumberName = i;
            }
            return spatialModels;

        }



        /// <summary>
        /// 获取当前墙体，有一端没有和任何墙相交，属于分割墙体
        /// </summary>
        /// <param name="walls"></param>
        private List<LineModel> GetSeparateInteriorLines(List<LineModel> interiorLines)
        {
            List<LineModel> remove = new List<LineModel>();

            interiorLines.ForEach(x =>
            {

                bool isOne = false;
                bool istwo = false;
                for (int i = 0; i < interiorLines.Count; i++)
                {

                    if (x == interiorLines[i])
                    {

                        continue;
                    }
                    if (x.Line.Start.IsAlmostEqualTo(interiorLines[i].Line.Start) || x.Line.Start.IsAlmostEqualTo(interiorLines[i].Line.End))
                    {
                        isOne = true;

                        continue;
                    }

                    if (x.Line.End.IsAlmostEqualTo(interiorLines[i].Line.Start) || x.Line.End.IsAlmostEqualTo(interiorLines[i].Line.End))
                    {
                        istwo = true;
                        continue;

                    }
                }

                if (!isOne || !istwo)
                {
                    x.LineType= LineType.Separate;
                    remove.Add(x);
                }

            });

            remove.ForEach(x =>
            {
                interiorLines.Remove(x);
            });

            return remove;
        }


        /// <summary>
        /// 查找最小区域
        /// </summary>
        /// <param name="interiorWalls"></param>
        /// <param name="separateInterior"></param>
        /// <param name="spatials"></param>
        /// <param name="sortOutLines"></param>
        private void FindSpatialFromLinesModel(List<LineModel> interiorLines, List<LineModel> separateLines, List<SpatialModel> spatialModels, List<LineModel> outModels)
        {

            //查找一个任意起点
            Line2D startLine = null;
            //从外边缘线开发查找
            if (outModels.Count > 0)
            {
                startLine = outModels[0].Line;
            }

            //查找内装墙体
            if (startLine == null)
            {
                if (tick >= interiorLines.Count)
                {
                    return;
                }
                startLine = interiorLines.OrderByDescending(x => x.UseAge).ElementAtOrDefault(tick).Line;
                tick++;
            }


            //需要处理的墙体
            List<Line2D> remainingLines = new List<Line2D>();

            //记录需要查找的线
            interiorLines.ForEach(x =>
            {
                remainingLines.Add(x.Line);
            });



            //查找和这个墙相关的封闭区域
            List<LineModel> spatialLines = new List<LineModel>();


            List<Line2D> Lines = null;

            try
            {
                //查找最小封闭区域
                Lines = GraphicAlgorithm.FindClosedLines(remainingLines, false, false, startLine);
            }
            catch (Exception ex)
            {

                string message = ex.Message;
            }

            //假如出现查找不到的情况，会继续查找
            if (Lines == null) {

                if (remainingLines.Count > 0) {
                    FindSpatialFromLinesModel(interiorLines, separateLines, spatialModels, outModels);
                }

            }
            else {
                tick = 0;
                //假如出现了一条线还当前多边形内部，则说明当前多边形不合法，需要重新查询
                bool reSearch = false;
                //循环所有线段
                foreach (Line2D x in remainingLines) {
                    //假如成立
                    if (x.IsInRegion(Lines)) {
                        //说明内部有线，则需要重新查找
                        reSearch = true;
                        break;
                    }
                }

                if (reSearch) {
                    FindSpatialFromLinesModel(interiorLines, separateLines, spatialModels, outModels);

                    return;
                }

                //需要移除的线
                List<LineModel> removeInteriorLines = new List<LineModel>();

                //查找所有细分的墙体
                foreach (Line2D line in Lines) {

                    LineModel iw = interiorLines.Find(x => x.Line.IsAlmostEqualTo(line));
                    if (iw != null) {

                        iw.UseAge -= 1;

                        spatialLines.Add(iw);

                        if (iw.UseAge == 0) {
                            removeInteriorLines.Add(iw);

                        }
                    }
                }


                //移除已经使用完成的墙体
                removeInteriorLines.ForEach(x => {
                    if (x.LineType == LineType.outer) {
                        outModels.Remove(outModels.Find(u => u.Line.IsAlmostEqualTo(x.Line)));
                    }
                    interiorLines.Remove(x);
                });


                List<Line2D> boundary = null;

                //合并相关的内墙线
                List<LineModel> mergeInteriorLines = this.MergeSpatialLines(spatialLines, separateLines, out boundary);


                //声明封闭区域
                SpatialModel spatial = new SpatialModel();

                //合并完成后，查找轮廓线
                if (boundary.Count != Lines.Count) {
                    Lines = GraphicAlgorithm.FindClosedLines(boundary, false, false, startLine);
                }

                List<Line2D> mergeLines = GraphicAlgorithm.MergeLines(new List<Line2D>(Lines));


                spatial.LineModels = mergeInteriorLines;

                //当前底部的区域信息

                //添加一个封闭区域
                spatialModels.Add(spatial);


                if (interiorLines.Count > 0)
                {
                    FindSpatialFromLinesModel(interiorLines, separateLines, spatialModels, outModels);
                }

            }



        }


        /// <summary>
        /// 合并相关墙体
        /// </summary>
        /// <param name="spatialWalls"></param>
        private List<LineModel> MergeSpatialLines(List<LineModel> spatialLines, List<LineModel> separateInterior, out List<Line2D> boundary)
        {

            List<LineModel> interiorLines = new List<LineModel>();

            List<Line2D> Line3ds = new List<Line2D>();

            foreach (LineModel v in spatialLines)
            {
                //获取当前所有的线段
                Line3ds.Add(v.Line);
            }
            //排除不需要合并的点
            List<Vector2D> withoutpoint = new List<Vector2D>();
            separateInterior.ForEach(x =>
            {

                withoutpoint.Add(x.Line.Start);
                withoutpoint.Add(x.Line.End);

            });
            //合并所有的线段
            List<Line2D> mergeLines = GraphicAlgorithm.MergeLinesWithoutpoints(Line3ds, withoutpoint);

            boundary = mergeLines;

            //循环所有的合并后的线段
            for (int i = 0; i < mergeLines.Count; i++)
            {
                //键值信息
                List<LineModel> kvps = new List<LineModel>();

                foreach (LineModel v in spatialLines)
                {              
                    if (v.Line.IsPartOf(mergeLines[i]))
                    {
                        kvps.Add(v);
                    }
                }
                kvps.ForEach(x => { spatialLines.Remove(x); });

                List<LineModel> mergeW = this.CreateMergeInteriorLine(kvps, mergeLines[i]);
                interiorLines.AddRange(mergeW);
            }


            return interiorLines;


        }


        private List<LineModel> CreateMergeInteriorLine(List<LineModel> kvps, Line2D line)
        {

            List<LineModel> interiorLines = new List<LineModel>();
            if (kvps.Count == 1)
            {

                interiorLines.Add(kvps[0]);
            }
            else
            {
                LineModel lineModel = new LineModel(line, kvps[0].UseAge,kvps[0].LineType);
                interiorLines.Add(lineModel);
            }
            return interiorLines;
        }
    }
}