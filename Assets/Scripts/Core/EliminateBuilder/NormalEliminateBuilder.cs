using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Core.EliminationBuilder
{
    public class NormalEliminateBuilder : IEliminateBuilder
    {
        public bool TryBuildEliminationBlocks(int rowIndex, int columnIndex, int[,] map, bool[,] horizontalDirtyMap,
            bool[,] verticalDirtyMap, out EliminationBlocks blocks)
        {
            int id = map[rowIndex, columnIndex];
            List<Vector2Int> tempList = null;
            if (!horizontalDirtyMap[rowIndex, columnIndex])
            {
                int count = 1;
                bool result = CheckRightEliminationBlocks(rowIndex, columnIndex, map, ref count);

                //将水平检测过的对象标脏，表示在本次检测中，以下对象无需再进行水平检测
                if (result)
                {
                    tempList = new List<Vector2Int>();
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
                bool result = CheckUpEliminationBlocks(rowIndex, columnIndex, map, ref count);
                if (result && tempList == null)
                {
                    tempList = new List<Vector2Int>();
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
                blocks = null;
                return false;
            }
            
            blocks = new EliminationBlocks(id, tempList);
            FillEliminationBlocks(blocks, map, horizontalDirtyMap, verticalDirtyMap);
            return true;
        }

        public bool TryBuildEliminationBlocksOnExchange(int rowIndex, int columnIndex, int[,] map, out EliminationBlocks blocks)
        {
            int countLeft = 1;
            int countRight = 1;
            int countUp= 1;
            int countDown = 1;
            List<Vector2Int> tempList = null;
            if (CheckLeftEliminationBlocks(rowIndex, columnIndex, map, ref countLeft))
            {
                tempList = new List<Vector2Int>();
                for (int i = 0; i < countLeft; i++)
                {
                    tempList.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex - i));
                }
            }

            if (CheckRightEliminationBlocks(rowIndex, columnIndex, map, ref countRight))
            {
                if (tempList == null)
                {
                    tempList = new List<Vector2Int>();
                }
                for (int i = 0; i < countLeft; i++)
                {
                    tempList.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex + i));
                }
            }

            if (CheckUpEliminationBlocks(rowIndex, columnIndex, map, ref countUp))
            {
                if (tempList == null)
                {
                    tempList = new List<Vector2Int>();
                }
                for (int i = 0; i < countLeft; i++)
                {
                    tempList.Add(Match3Utility.ArrayIndexConvertVector(rowIndex + i, columnIndex));
                }
            }

            if (CheckDownEliminationBlocks(rowIndex, columnIndex, map, ref countDown))
            {
                if (tempList == null)
                {
                    tempList = new List<Vector2Int>();
                }
                for (int i = 0; i < countLeft; i++)
                {
                    tempList.Add(Match3Utility.ArrayIndexConvertVector(rowIndex - i, columnIndex));
                }
            }

            if (tempList == null)
            {
                blocks = null;
                return false;
            }
            int id = map[rowIndex, columnIndex];
            blocks = new EliminationBlocks(id, tempList);
            return true;
        }

        
        private static void FillEliminationBlocks(EliminationBlocks blocks, int[,] map, bool[,] horizontalDirtyMap,
            bool[,] verticalDirtyMap)
        {
            int index = 1;
            while (true)
            {
                if (index >= blocks.count)
                {
                    break;
                }

                Match3Utility.VectorConvertArrayIndex(blocks[index],out int rowIndex, out int columnIndex);

                if (!horizontalDirtyMap[rowIndex, columnIndex])
                {
                    int count = 1;
                    bool result = CheckRightEliminationBlocks(rowIndex, columnIndex, map, ref count);
                    for (int k = 1; k < count; k++)
                    {
                        horizontalDirtyMap[rowIndex, columnIndex + k] = true;
                        if (result)
                        {
                            blocks.Add(Match3Utility.ArrayIndexConvertVector(rowIndex, columnIndex + k));
                        }
                    }
                }

                if (!verticalDirtyMap[rowIndex, columnIndex])
                {
                    int count = 1;
                    bool result = CheckUpEliminationBlocks(rowIndex, columnIndex, map, ref count);
                    for (int k = 1; k < count; k++)
                    {
                        verticalDirtyMap[rowIndex + k, columnIndex] = true;
                        if (result)
                        {
                            blocks.Add(Match3Utility.ArrayIndexConvertVector(rowIndex + k, columnIndex));
                        }
                    }
                }

                index++;
            }
        }
        
        private static bool CheckLeftEliminationBlocks(int rowIndex, int columnIndex, int[,] map,
            ref int count)
        {
            if (columnIndex - count <= 0)
            {
                return count >= 3;
            }

            if (map[rowIndex, columnIndex - count] == map[rowIndex, columnIndex])
            {
                count++;
                return CheckRightEliminationBlocks(rowIndex, columnIndex, map, ref count);
            }

            return count >= 3;
        }
        private static bool CheckRightEliminationBlocks(int rowIndex, int columnIndex, int[,] map,
            ref int count)
        {
            if (columnIndex + count >= map.GetLength(1))
            {
                return count >= 3;
            }

            if (map[rowIndex, columnIndex + count] == map[rowIndex, columnIndex])
            {
                count++;
                return CheckRightEliminationBlocks(rowIndex, columnIndex, map, ref count);
            }

            return count >= 3;
        }
        private static bool CheckUpEliminationBlocks(int rowIndex, int columnIndex, int[,] map,
            ref int count)
        {
            if (rowIndex + count >= map.GetLength(0))
            {
                return count >= 3;
            }

            if (map[rowIndex + count, columnIndex] == map[rowIndex, columnIndex])
            {
                count++;
                return CheckUpEliminationBlocks(rowIndex, columnIndex, map, ref count);
            }

            return count >= 3;
        }
        private static bool CheckDownEliminationBlocks(int rowIndex, int columnIndex, int[,] map,
            ref int count)
        {
            if (rowIndex - count <= 0)
            {
                return count >= 3;
            }

            if (map[rowIndex - count, columnIndex] == map[rowIndex, columnIndex])
            {
                count++;
                return CheckUpEliminationBlocks(rowIndex, columnIndex, map, ref count);
            }

            return count >= 3;
        }
    }
}