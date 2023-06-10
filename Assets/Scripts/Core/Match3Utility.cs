using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3Game.Core
{
    // 按照通常的理解 二维数组中 行索引->y坐标 列索引->x坐标
    // 以Vector2Int表示时，为了符合习惯 new Vector2Int(columnIndex,rowIndex)
    public static class Match3Utility
    {
        private static Dictionary<int, IEliminateBuilder> _eliminateBuilderDic;

        public static void BindElementEliminateBuilder<T>(int id, IEliminateBuilder eliminateBuilder)
        {
            if (_eliminateBuilderDic == null)
            {
                _eliminateBuilderDic = new Dictionary<int, IEliminateBuilder>();
            }

            if (!_eliminateBuilderDic.ContainsKey(id))
            {
                _eliminateBuilderDic.Add(id, null);
            }
            //此处只是示例。若之前存在，则直接覆盖。实际项目中
            _eliminateBuilderDic[id] = eliminateBuilder;
        }
        public static void CreateMap(Match3Data match3Data)
        {
            int row = match3Data.row;
            int column = match3Data.column;
            int length = match3Data.types.Length;
            match3Data.map = new int[row, column];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    match3Data.map[i, j] = match3Data.types[Random.Range(0, length)];
                }
            }
        }

        //public static bool

        // 三消全局检测
        // 从左到右，从小到大，依次进行(确定检测的两个方向)
        // 水平检测并将相同元素水平标脏，这样水平连续的相同对象在水平方向仅需检测一次，
        // 垂直检测亦如水平检测一样，将相同元素垂直标脏
        /// <summary>
        /// 三消全局检测
        /// </summary>
        /// <param name="map"></param>
        /// <returns>需要 检测list.count > 0</returns>
        public static List<EliminationBlocks> FindAllEliminationBlocks(int[,] map)
        {
            List<EliminationBlocks> blockList = new List<EliminationBlocks>();
            int row = map.GetLength(0);
            int column = map.GetLength(1);
            bool[,] horizontalDirtyMap = new bool[row, column];
            bool[,] verticalDirtyMap = new bool[row, column];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    int id = map[i, j];
                    if (_eliminateBuilderDic[id].TryBuildEliminationBlocks(i, j, map, horizontalDirtyMap,
                        verticalDirtyMap, out EliminationBlocks blocks))
                    {
                        blockList.Add(blocks);
                    }
                }
            }
            return blockList;
        }

        /// <summary>
        /// 交换检测
        /// </summary>
        /// <param name="map"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static List<EliminationBlocks> FindEliminationBlocksOnExchange(int[,] map, Vector2Int pos1, Vector2Int pos2)
        {
            List<EliminationBlocks> blockList = new List<EliminationBlocks>();
            VectorConvertArrayIndex(pos1, out int rowIndex1, out int columnIndex1);
            VectorConvertArrayIndex(pos2, out int rowIndex2, out int columnIndex2);
            //水平交换
            if (rowIndex1 == rowIndex2)
            {
                
            }
            //垂直交换
            else if(columnIndex1 == columnIndex2)
            {
                
            }
            else
            {
                //todo
            }

            return blockList;
        }
        
        private static bool CheckLeft(int x, int y, int[,] map)
        {
            if (y <= 0)
            {
                return false;
            }

            return map[x, y - 1] == map[x, y];
        }

        private static bool CheckRight(int x, int y, int[,] map)
        {
            if (y >= map.GetLength(1) - 1)
            {
                return false;
            }

            return map[x, y + 1] == map[x, y];
        }

        private static bool CheckUp(int x, int y, int[,] map)
        {
            if (x >= map.GetLength(0) - 1)
            {
                return false;
            }

            return map[x + 1, y] == map[x, y];
        }

        private static bool CheckDown(int x, int y, int[,] map)
        {
            if (x <= 0)
            {
                return false;
            }

            return map[x - 1, y] == map[x, y];
        }

        public static Vector2Int ArrayIndexConvertVector(int rowIndex, int columnIndex)
        {
            return new Vector2Int(columnIndex, rowIndex);
        }

        public static void VectorConvertArrayIndex(Vector2Int pos,out int rowIndex,out int columnIndex)
        {
            rowIndex = pos.y;
            columnIndex = pos.x;
        }
    }
}