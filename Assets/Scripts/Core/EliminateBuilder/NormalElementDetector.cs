using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Match3Game.Core.EliminationBuilder
{
    public class NormalElementDetector : IElementDetector
    {
        public bool TryBuildPreExchangeBlocks(int rowIndex, int columnIndex, int[,] map, out PreCheckBlock block)
        {
            int startId = map[rowIndex, columnIndex];
            Vector2Int start = Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex);
            
            //水平交换预测
            if (columnIndex + 1 < map.GetLength(1) && startId != map[rowIndex, columnIndex + 1])
            {
                Vector2Int target = Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex + 1);
                HashSet<Vector2Int> tempList = new HashSet<Vector2Int>();
                int targetId = map[rowIndex, columnIndex + 1];
                int countRight = 1;
                if (CheckRightEliminationBlocks(rowIndex, columnIndex + 1, startId, map, ref countRight))
                {
                    for (int i = 0; i < countRight; i++)
                    {
                        tempList.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex + 1 + i));
                    }
                }

                if (CheckVerticalEliminationBlocks(rowIndex, columnIndex + 1, startId, map, out Vector2Int[] posArray1))
                {
                    tempList.UnionWith(posArray1);
                }
                int countLeft = 1;
                if (CheckLeftEliminationBlocks(rowIndex, columnIndex, targetId, map, ref countLeft))
                {
                    for (int i = 0; i < countRight; i++)
                    {
                        tempList.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex  - i));
                    }
                }
                
                if (CheckVerticalEliminationBlocks(rowIndex, columnIndex, targetId, map, out Vector2Int[] posArray2))
                {
                    tempList.UnionWith(posArray2);
                }

                if (tempList.Count >= 3)
                {
                    block = new PreCheckBlock(start, target, tempList.ToArray());
                    return true;
                }
            }
            //垂直交换预测
            else if (rowIndex + 1 < map.GetLength(0) && startId != map[rowIndex + 1, columnIndex])
            {
                Vector2Int target = Match3Utility.ArrayIndexConvertVector(rowIndex + 1, columnIndex);
                HashSet<Vector2Int> tempList = new HashSet<Vector2Int>();
                int targetId = map[rowIndex + 1, columnIndex];
                int countUp = 1;
                if (CheckUpEliminationBlocks(rowIndex + 1, columnIndex, startId, map, ref countUp))
                {
                    for (int i = 0; i < countUp; i++)
                    {
                        tempList.Add(Match3Utility.ArrayIndexConvertVector(rowIndex + 1 + i, columnIndex));
                    }
                }

                if (CheckHorizontalEliminationBlocks(rowIndex + 1, columnIndex, startId, map, out Vector2Int[] posArray1))
                {
                    tempList.UnionWith(posArray1);
                }
                int countDown = 1;
                if (CheckDownEliminationBlocks(rowIndex, columnIndex, targetId, map, ref countDown))
                {
                    for (int i = 0; i < countUp; i++)
                    {
                        tempList.Add(Match3Utility.ArrayIndexConvertVector(rowIndex - i, columnIndex));
                    }
                }
                
                if (CheckHorizontalEliminationBlocks(rowIndex, columnIndex, targetId, map, out Vector2Int[] posArray2))
                {
                    tempList.UnionWith(posArray2);
                }

                if (tempList.Count >= 3)
                {
                    block = new PreCheckBlock(start,target, tempList.ToArray());
                    return true;
                }
            }

            block = null;
            return false;
        }

        public bool TryBuildEliminationBlocks(int rowIndex, int columnIndex, int[,] map, bool[,] horizontalDirtyMap,
            bool[,] verticalDirtyMap, out EliminationBlock block)
        {
            int id = map[rowIndex, columnIndex];
            HashSet<Vector2Int> tempList = null;
            if (!horizontalDirtyMap[rowIndex, columnIndex])
            {
                int count = 1;
                bool result = CheckRightEliminationBlocks(rowIndex, columnIndex, id, map, ref count);

                //将水平检测过的对象标脏，表示在本次检测中，以下对象无需再进行水平检测
                if (result)
                {
                    tempList = new HashSet<Vector2Int>();
                }

                for (int k = 0; k < count; k++)
                {
                    horizontalDirtyMap[rowIndex, columnIndex + k] = true;
                    if (result)
                    {
                        tempList.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex + k));
                    }
                }
            }

            if (!verticalDirtyMap[rowIndex, columnIndex])
            {
                int count = 1;
                bool result = CheckUpEliminationBlocks(rowIndex, columnIndex, id, map, ref count);
                if (result && tempList == null)
                {
                    tempList = new HashSet<Vector2Int>();
                }

                for (int k = 0; k < count; k++)
                {
                    verticalDirtyMap[rowIndex + k, columnIndex] = true;

                    Vector2Int pos = Match3Utility.ArrayIndexConvertVector(rowIndex + k, columnIndex);

                    if (result && !tempList.Contains(pos))
                    {
                        tempList.Add(pos);
                    }
                }
            }

            if (tempList == null)
            {
                block = null;
                return false;
            }

            block = new EliminationBlock(id, tempList.ToList());
            FillEliminationBlocks(block, map, horizontalDirtyMap, verticalDirtyMap);
            return true;
        }

        public bool TryBuildEliminationBlocksOnExchange(int rowIndex, int columnIndex, int[,] map,
            out EliminationBlock block)
        {
            HashSet<Vector2Int> tempList = null;

            if (CheckHorizontalEliminationBlocks(rowIndex, columnIndex, map[rowIndex, columnIndex], map,
                out Vector2Int[] posArray1))
            {
                tempList = new HashSet<Vector2Int>();
                tempList.UnionWith(posArray1);
            }

            if (CheckVerticalEliminationBlocks(rowIndex, columnIndex, map[rowIndex, columnIndex], map,
                out Vector2Int[] posArray2))
            {
                if (tempList == null)
                {
                    tempList = new HashSet<Vector2Int>();
                }

                tempList.UnionWith(posArray2);
            }

            if (tempList == null)
            {
                block = null;
                return false;
            }

            int id = map[rowIndex, columnIndex];
            block = new EliminationBlock(id, tempList.ToList());
            return true;
        }

        private static void FillEliminationBlocks(EliminationBlock block, int[,] map, bool[,] horizontalDirtyMap,
            bool[,] verticalDirtyMap)
        {
            int index = 1;
            while (true)
            {
                if (index >= block.count)
                {
                    break;
                }

                Match3Utility.VectorConvertArrayIndex(block[index], out int rowIndex, out int columnIndex);

                if (!horizontalDirtyMap[rowIndex, columnIndex])
                {
                    int count = 1;
                    bool result = CheckRightEliminationBlocks(rowIndex, columnIndex, block.id, map, ref count);
                    for (int k = 1; k < count; k++)
                    {
                        horizontalDirtyMap[rowIndex, columnIndex + k] = true;
                        if (result)
                        {
                            block.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex + k));
                        }
                    }
                }

                if (!verticalDirtyMap[rowIndex, columnIndex])
                {
                    int count = 1;
                    bool result = CheckUpEliminationBlocks(rowIndex, columnIndex, block.id, map, ref count);
                    for (int k = 1; k < count; k++)
                    {
                        verticalDirtyMap[rowIndex + k, columnIndex] = true;
                        if (result)
                        {
                            block.Add(Match3Utility.ArrayIndexConvertVector(rowIndex + k, columnIndex));
                        }
                    }
                }

                index++;
            }
        }

        private static bool CheckLeftEliminationBlocks(int rowIndex, int columnIndex, int id, int[,] map,
            ref int count)
        {
            if (columnIndex - count <= 0)
            {
                return count >= 3;
            }

            if (map[rowIndex, columnIndex - count] == id)
            {
                count++;
                return CheckLeftEliminationBlocks(rowIndex, columnIndex, id, map, ref count);
            }

            return count >= 3;
        }

        private static bool CheckRightEliminationBlocks(int rowIndex, int columnIndex, int id, int[,] map,
            ref int count)
        {
            if (columnIndex + count >= map.GetLength(1))
            {
                return count >= 3;
            }

            if (map[rowIndex, columnIndex + count] == id)
            {
                count++;
                return CheckRightEliminationBlocks(rowIndex, columnIndex, id, map, ref count);
            }

            return count >= 3;
        }

        private static bool CheckUpEliminationBlocks(int rowIndex, int columnIndex, int id, int[,] map,
            ref int count)
        {
            if (rowIndex + count >= map.GetLength(0))
            {
                return count >= 3;
            }

            if (map[rowIndex + count, columnIndex] == id)
            {
                count++;
                return CheckUpEliminationBlocks(rowIndex, columnIndex, id, map, ref count);
            }

            return count >= 3;
        }

        private static bool CheckDownEliminationBlocks(int rowIndex, int columnIndex, int id, int[,] map,
            ref int count)
        {
            if (rowIndex - count <= 0)
            {
                return count >= 3;
            }

            if (map[rowIndex - count, columnIndex] == id)
            {
                count++;
                return CheckUpEliminationBlocks(rowIndex, columnIndex, id, map, ref count);
            }

            return count >= 3;
        }

        private static bool CheckHorizontalEliminationBlocks(int rowIndex, int columnIndex, int id, int[,] map,
            out Vector2Int[] posArray)
        {
            int count = 1;
            CheckLeftEliminationBlocks(rowIndex, columnIndex, id, map, ref count);
            int left = count - 1;
            count = 1;
            CheckRightEliminationBlocks(rowIndex, columnIndex, id, map, ref count);
            int right = count - 1;
            count = left + count;
            if (count < 3)
            {
                posArray = null;
                return false;
            }

            List<Vector2Int> temp = new List<Vector2Int>(count);
            temp.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex));
            for (int i = 1; i < left + 1; i++)
            {
                temp.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex - i));
            }

            for (int i = 1; i < right + 1; i++)
            {
                temp.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex + i));

            }

            posArray = temp.ToArray();
            return true;
        }

        private static bool CheckVerticalEliminationBlocks(int rowIndex, int columnIndex, int id, int[,] map,
            out Vector2Int[] posArray)
        {
            int count = 1;
            CheckDownEliminationBlocks(rowIndex, columnIndex, id, map, ref count);
            int down = count - 1;
            count = 1;
            CheckUpEliminationBlocks(rowIndex, columnIndex, id, map, ref count);
            int up = count - 1;
            count = down + count;
            if (count < 3)
            {
                posArray = null;
                return false;
            }

            List<Vector2Int> temp = new List<Vector2Int>();
            temp.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex));
            for (int i = 1; i < down + 1; i++)
            {
                temp.Add(Match3Utility.ArrayIndexConvertVector(rowIndex - i, columnIndex));
            }

            for (int i = 1; i < up + 1; i++)
            {
                temp.Add(Match3Utility.ArrayIndexConvertVector(rowIndex + i, columnIndex));

            }

            posArray = temp.ToArray();
            return true;
        }
    }
}