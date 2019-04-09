using System;
using System.Collections.Generic;
using System.Linq;


// 分割算法描述，以分割长度6米，扩展系数0.5为例

// 首先找到超过6米的不可分割区间，将这些区间从整体区间链中剔除，得到分段的若干区间链
// 对每段区间连,进行递归分析策略
//     如果区间链长度不大于6米，则分割结束
//     如果区间链长度大于6米且不大于9米，则判断区间链的中点落入哪种区间
//         如果落入可分区间，则在该点进行分割，且分割结束
//         如果落入不可分区间，则判断该区间是否为区间链的第一区间
//             如果是第一区间，则在区间右端点处进行分割，且分割结束
//             如果不是第一区间，则在距离中点更近的区间端点处进行分割，且分割结束
//     如果区间链长度大于9米，则判断区间链的中点落入哪种区间
//         如果落入可分区间，则在该处将区间链分为两段，左右两段分别进行递归分析，并将各自的分段结果进行合并
//         如果落入不可分区间，则判断该区间是否为区间链的第一区间
//             如果是第一区间，则在区间右侧端点进行分割且分段，对右侧段的区间链进行递归分析
//             如果不是第一区间，则在距离中点近的端点处进行分割且分段，左右两段分别进行递归分析，并将各自的分段结果进行合并

//  两段区间链的结果进行合并的出发点是考虑到有可能左侧最后一段和右侧第一段都较小（长度和不大于6米）
namespace Tida.Geometry.External
{

    /// <summary>
    /// 对长墙进行打断算法
    /// </summary>
    internal class PartitionAlgorithm
    {
        public PartitionAlgorithm(double partitionLength, double expandCoef)
        {
            PartitionLength = partitionLength;
            ExpandCoef = expandCoef;
            ExpandLength = PartitionLength * (1 + ExpandCoef);
            SingleAlgorithm = new SinglePartitionAlgorithm(partitionLength, expandCoef);
        }

        // 必须保证分段区间是可分割和不可分割相间隔的
        // 有可能出现连续两个值相差很小甚至相等，出现在两个不可分割分段区间的相邻处
        public List<double> Partition(List<double> seperations, bool firstPartitionable)
        {
            Seperations = seperations;
            FirstPartitionable = firstPartitionable;
            Partitions = new List<double>();

            GetLargeInpartitionableIndexes();
            SplitSeperations();
            AddPartitionsFromLargeInpartitionables();
            AddPartitionsFromSeperationsList();
            SortPartitions();

            return Partitions;
        }

        private void GetLargeInpartitionableIndexes()
        {
            LargeInpartitionableIndexes = new List<int>();

            for (int i = 0; i < Seperations.Count; ++i)
            {
                if (IsInpartitionable(i) && IsLargeSeperation(i))
                    LargeInpartitionableIndexes.Add(i);
            }
        }

        private void SplitSeperations()
        {
            SeperationsList = new List<List<double>>();
            SeperationsStart = new List<double>();
            SeperationsFirstPartitionable = new List<bool>();

            if (LargeInpartitionableIndexes.Count == 0)
            {
                SeperationsList.Add(Seperations);
                SeperationsStart.Add(0);
                SeperationsFirstPartitionable.Add(FirstPartitionable);
            }
            else
            {
                int startIndex = 0;
                foreach (int index in LargeInpartitionableIndexes)
                {
                    if (index > startIndex)
                    {
                        List<double> list = Seperations.GetRange(startIndex, index - startIndex);

                        double start = GetSeperationStart(startIndex);
                        for (int i = 0; i < list.Count; ++i)
                        {
                            list[i] -= start;
                        }

                        SeperationsList.Add(list);
                        SeperationsStart.Add(start);
                        SeperationsFirstPartitionable.Add(!IsInpartitionable(startIndex));
                    }
                    startIndex = index + 1;
                }
            }
        }

        private void AddPartitionsFromLargeInpartitionables()
        {
            foreach (int index in LargeInpartitionableIndexes)
            {
                if (index != 0)
                {
                    Partitions.Add(GetSeperationStart(index));
                }

                if (index != LargeInpartitionableIndexes.Count - 1)
                {
                    Partitions.Add(Seperations[index]);
                }
            }
        }

        private void AddPartitionsFromSeperationsList()
        {
            for (int i = 0; i < SeperationsList.Count; ++i)
            {
                List<double> partitions = SingleAlgorithm.Partition(SeperationsList[i],
                    SeperationsFirstPartitionable[i]);

                for (int j = 0; j < partitions.Count; ++j)
                {
                    partitions[j] += SeperationsStart[i];
                }

                Partitions.AddRange(partitions);
            }
        }

        private void SortPartitions()
        {
            Partitions.Sort();
        }

        private bool IsInpartitionable(int index)
        {
            bool isEvent = (index % 2 == 0);
            return (FirstPartitionable && !isEvent) || (!FirstPartitionable && isEvent);
        }

        private bool IsLargeSeperation(int index)
        {
            if (index == 0)
                return Seperations[index] > PartitionLength;
            else
                return (Seperations[index] - Seperations[index - 1]) > PartitionLength;
        }

        private double GetSeperationStart(int index)
        {
            return (index > 0) ? Seperations[index - 1] : 0;
        }

        private double PartitionLength { get; set; }

        private double ExpandCoef { get; set; }

        private double ExpandLength { get; set; }

        private SinglePartitionAlgorithm SingleAlgorithm { get; set; }

        private List<double> Seperations { get; set; }

        private bool FirstPartitionable { get; set; }

        private List<double> Partitions { get; set; }

        private List<int> LargeInpartitionableIndexes { get; set; }

        private List<List<double>> SeperationsList { get; set; }

        private List<double> SeperationsStart { get; set; }

        private List<bool> SeperationsFirstPartitionable { get; set; }

        private class SinglePartitionAlgorithm
        {
            public SinglePartitionAlgorithm(double partitionLength, double expandCoef)
            {
                PartitionLength = partitionLength;
                ExpandCoef = expandCoef;
                ExpandLength = PartitionLength * (1 + ExpandCoef);
            }

            public List<double> Partition(List<double> seperations, bool firstPartitionable)
            {
                List<double> partitions = null;

                DoPartition(seperations, firstPartitionable, out partitions);

                return partitions;
            }

            // 注意，这是一个递归调用的方法
            private bool DoPartition(List<double> seperations, bool firstPartitionable, out List<double> partitions)
            {
                partitions = new List<double>();

                double length = GetLength(seperations);

                if (!(length > PartitionLength))
                {
                    return true;
                }
                else if (!(length > ExpandLength))
                {
                    partitions.Add(DoExpandPartition(seperations, firstPartitionable));
                    return true;
                }
                else
                {
                    partitions.AddRange(DoSplitPartition(seperations, firstPartitionable));
                    return true;
                }
            }

            private double GetLength(List<double> seperations)
            {
                if (seperations.Count == 0)
                    return 0;
                else
                    return seperations.Last();
            }

            private double DoExpandPartition(List<double> seperations, bool firstPartitionable)
            {
                // 增广分割的策略是：
                // 判断中点落入哪个区间
                //     如果落入可分区间则直接在中点处分割
                //     如果落入不可分区间
                //         如果不可分区间为第一区间，则在区间右侧端点处分割
                //         否则在不可分区间距离中点更近的端点处分割
                double halfLength = GetLength(seperations) / 2.0;
                double leftOffset = 0;
                double rightOffset = 0;
                double partition = 0;

                int index = GetIndexAndOffset(seperations, halfLength, ref leftOffset, ref rightOffset);
                if (IsPartitionable(index, firstPartitionable))
                {
                    partition = halfLength;
                }
                else
                {
                    if (index == 0)
                    {
                        partition = seperations[0];
                    }
                    else
                    {
                        partition = halfLength + ((leftOffset < rightOffset) ? (-leftOffset) : rightOffset);
                    }
                }

                return partition;
            }

            private List<double> DoSplitPartition(List<double> seperations, bool firstPartitionable)
            {
                List<double> partitions = null;
                double halfLength = GetLength(seperations) / 2.0;
                double leftOffset = 0;
                double rightOffset = 0;

                int index = GetIndexAndOffset(seperations, halfLength, ref leftOffset, ref rightOffset);
                if (IsPartitionable(index, firstPartitionable))
                {
                    List<double> seperations1 = null;
                    List<double> seperations2 = null;
                    SplitToTwoSeperationsWithinPartitionable(seperations, index, halfLength,
                        out seperations1, out seperations2);

                    List<double> partitions1 = null;
                    List<double> partitions2 = null;
                    // 递归调用DoPartition
                    if (DoPartition(seperations1, firstPartitionable, out partitions1) &&
                        DoPartition(seperations2, true, out partitions2))
                    {
                        partitions = MergeTwoPartitions(partitions1, partitions2, seperations1, seperations2);
                    }
                }
                else
                {
                    if (index == 0)
                    {
                        partitions.Add(seperations[0]);
                        List<double> rightSeperations = SplitAtRightOfFirstInpartionable(seperations);
                        List<double> rightPartitions = null;
                        if (DoPartition(rightSeperations, true, out rightPartitions))
                        {
                            for (int i = 0; i < rightPartitions.Count; ++i)
                            {
                                rightPartitions[i] += seperations[0];
                            }
                            partitions.AddRange(rightPartitions);
                        }
                    }
                    else
                    {
                        if (leftOffset < rightOffset)
                        {
                            List<double> seperations1 = null;
                            List<double> seperations2 = null;
                            SplitAtLeftEndOfInpartitionable(seperations, index, out seperations1, out seperations2);

                            List<double> partitions1 = null;
                            List<double> partitions2 = null;
                            // 递归调用DoPartition
                            if (DoPartition(seperations1, firstPartitionable, out partitions1) &&
                                DoPartition(seperations2, false, out partitions2))
                            {
                                partitions = MergeTwoPartitions(partitions1, partitions2, seperations1, seperations2);
                            }
                        }
                        else
                        {
                            List<double> seperations1 = null;
                            List<double> seperations2 = null;
                            SplitAtRightEndOfInpartitionable(seperations, index, out seperations1, out seperations2);

                            List<double> partitions1 = null;
                            List<double> partitions2 = null;
                            // 递归调用DoPartition
                            if (DoPartition(seperations1, firstPartitionable, out partitions1) &&
                                DoPartition(seperations2, true, out partitions2))
                            {
                                partitions = MergeTwoPartitions(partitions1, partitions2, seperations1, seperations2);
                            }
                        }
                    }
                }

                if (null == partitions)
                {
                    throw new Exception("分裂成两条区间链然后分别进行分割的算法必须得到一个真实分割");
                }

                return partitions;
            }

            private int GetIndexAndOffset(List<double> seperations, double length, ref double leftOffset, ref double rightOffset)
            {
                double left = 0;
                int i = 0;

                foreach (double seperation in seperations)
                {
                    if (length < seperation)
                    {
                        leftOffset = length - left;
                        rightOffset = seperation - length;
                        break;
                    }
                    left = seperation;
                    ++i;
                }

                return i;
            }

            private bool IsPartitionable(int index, bool firstPartitionable)
            {
                bool isEvent = (index % 2 == 0);
                return (firstPartitionable && isEvent) || (!firstPartitionable && !isEvent);
            }

            private void SplitToTwoSeperationsWithinPartitionable(List<double> seperations, int index, double position,
                out List<double> seperations1, out List<double> seperations2)
            {
                seperations1 = new List<double>();
                seperations2 = new List<double>();

                for (int i = 0; i < index - 1; ++i)
                {
                    seperations1.Add(seperations[i]);
                }
                seperations1.Add(position);

                for (int i = index; i < seperations.Count; ++i)
                {
                    seperations2.Add(seperations[i] - position);
                }
            }

            private List<double> SplitAtRightOfFirstInpartionable(List<double> seperations)
            {
                List<double> rightSeperations = new List<double>();
                for (int i = 1; i < seperations.Count; ++i)
                {
                    rightSeperations.Add(seperations[i] - seperations[0]);
                }
                return rightSeperations;
            }

            private void SplitAtLeftEndOfInpartitionable(List<double> seperations, int index,
                out List<double> seperations1, out List<double> seperations2)
            {
                seperations1 = new List<double>();
                seperations2 = new List<double>();

                for (int i = 0; i < index; ++i)
                {
                    seperations1.Add(seperations[i]);
                }

                for (int i = index; i < seperations.Count; ++i)
                {
                    seperations2.Add(seperations[i] - seperations[index - 1]);
                }
            }

            private void SplitAtRightEndOfInpartitionable(List<double> seperations, int index,
                out List<double> seperations1, out List<double> seperations2)
            {
                seperations1 = new List<double>();
                seperations2 = new List<double>();

                for (int i = 0; i <= index; ++i)
                {
                    seperations1.Add(seperations[i]);
                }

                for (int i = index + 1; i < seperations.Count; ++i)
                {
                    seperations2.Add(seperations[i] - seperations[index]);
                }
            }

            private List<double> MergeTwoPartitions(List<double> partitions1, List<double> partitions2,
                List<double> seperations1, List<double> seperations2)
            {
                List<double> partitions = new List<double>();

                double lastPartition1 = 0;
                for (int i = 0; i < partitions1.Count; ++i)
                {
                    lastPartition1 = partitions1[i];
                    partitions.Add(lastPartition1);
                }

                double lastInterval1 = (partitions1.Count == 0) ? seperations1.Last() : (seperations1.Last() - lastPartition1);
                double firstInterval2 = (partitions2.Count == 0) ? seperations2.Last() : partitions2.First();
                if (lastInterval1 + firstInterval2 > PartitionLength)
                {
                    partitions.Add(seperations1.Last());
                }

                for (int i = 0; i < partitions2.Count; ++i)
                {
                    partitions.Add(seperations1.Last() + partitions2[i]);
                }

                return partitions;
            }

            private double PartitionLength { get; set; }

            private double ExpandCoef { get; set; }

            private double ExpandLength { get; set; }
        }
    }
}
