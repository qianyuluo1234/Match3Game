using System;
using System.Collections.Generic;
using Match3Game.Config;
using Match3Game.Logic.Core.Cell;
using Match3Game.Logic.Core.Element;
using Match3Game.Logic.Core.EliminatingBuilders;
using UnityEngine;

namespace Match3Game.Logic.Core
{
    // 按照通常的理解 二维数组中 行索引->y坐标 列索引->x坐标
    // 以Vector2Int表示时，为了符合习惯 new Vector2Int(columnIndex,rowIndex)
    public static class Match3Utility
    {
        private static Dictionary<int, Func<int, int, BaseCellData>> _cellFactoryDic;
        private static Dictionary<int, Func<int, IElementData>> _elementFactoryDic;
        private static Dictionary<int, IEliminateBlockBuilder> _eliminateBuilderDic;

        public static void Init()
        {
            IEliminateBlockBuilder eliminateBlockBuilder = new NormalEliminateBlockBuilder();
            Match3Utility.BindElementEliminateBuilder(10001, eliminateBlockBuilder);
            Match3Utility.BindElementEliminateBuilder(10002, eliminateBlockBuilder);
            Match3Utility.BindElementEliminateBuilder(10003, eliminateBlockBuilder);
            Match3Utility.BindElementEliminateBuilder(10004, eliminateBlockBuilder);
            Match3Utility.BindElementEliminateBuilder(10005, eliminateBlockBuilder);
            Match3Utility.BindElementEliminateBuilder(10006, eliminateBlockBuilder);
            Match3Utility.BindElementEliminateBuilder(10007, eliminateBlockBuilder);
            
            _cellFactoryDic = new Dictionary<int, Func<int, int, BaseCellData>>();
            _elementFactoryDic = new Dictionary<int, Func<int, IElementData>>()
            {
                {10001, (id) => new NormalChessData(id)},
                {10002, (id) => new NormalChessData(id)},
                {10003, (id) => new NormalChessData(id)},
                {10004, (id) => new NormalChessData(id)},
                {10005, (id) => new NormalChessData(id)},
                {10006, (id) => new NormalChessData(id)},
                {10007, (id) => new NormalChessData(id)},
            };
        }
        
        public static void BindElementEliminateBuilder(int id, IEliminateBlockBuilder eliminateBlockBuilder)
        {
            if (_eliminateBuilderDic == null)
            {
                _eliminateBuilderDic = new Dictionary<int, IEliminateBlockBuilder>();
            }

            if (!_eliminateBuilderDic.ContainsKey(id))
            {
                _eliminateBuilderDic.Add(id, null);
            }
            //此处只是示例。若之前存在，则直接覆盖。
            _eliminateBuilderDic[id] = eliminateBlockBuilder;
        }
        
        /// <summary>
        /// 全局交换预检测
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static List<PreCheckBlock> FindAllPreCheckBlocks(Match3MapData map)
        {
            List<PreCheckBlock> blocksList = new List<PreCheckBlock>();
            int row = map.row;
            int column = map.column;
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    IElementData elementData = map.GetElement(i, j);
                    int id = elementData.confData.id;
                    if (_eliminateBuilderDic[id].TryBuildPreCheckBlocks(i, j, map, out PreCheckBlock blocks))
                    {
                        blocksList.Add(blocks);
                    }
                }
            }

            return blocksList;
        }
        
        /// <summary>
        /// 全局检测所有的可消除块
        /// 从左到右，从小到大，依次进行(确定检测的两个方向)
        /// 水平检测并将相同元素水平标脏，这样水平连续的相同对象在水平方向仅需检测一次，垂直检测亦如水平检测一样，将相同元素垂直标脏
        /// </summary>
        /// <param name="map"></param>
        /// <returns>需要检测list.count是否 == 0</returns>
        public static List<EliminationBlock> FindAllEliminationBlocks(Match3MapData map)
        {
            List<EliminationBlock> blockList = new List<EliminationBlock>();
            int row = map.row;
            int column = map.column;
            bool[,] horizontalDirtyMap = new bool[row, column];
            bool[,] verticalDirtyMap = new bool[row, column];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    BaseCellData cellData = map.GetCell(i,j);
                    IElementData elementData = map.GetElement(i,j);
                    if (cellData.state == ConfCellStateEnum.Env)
                    {
                        continue;
                    }
                    
                    int id = elementData.confData.id;
                    if (_eliminateBuilderDic[id].TryBuildEliminationBlocks(i, j, map, horizontalDirtyMap,
                        verticalDirtyMap, out EliminationBlock blocks))
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
        /// <param name="rowIndex1"></param>
        /// <param name="columnIndex1"></param>
        /// <param name="rowIndex2"></param>
        /// <param name="columnIndex2"></param>
        /// <returns></returns>
        public static List<EliminationBlock> FindEliminationBlocksOnExchange(Match3MapData map, int rowIndex1,
            int columnIndex1, int rowIndex2, int columnIndex2)
        {
            List<EliminationBlock> blockList = new List<EliminationBlock>();
            IElementData elementData1 = map.GetElement(rowIndex1, columnIndex1);
            IElementData elementData2 = map.GetElement(rowIndex2, columnIndex2);

            if (elementData1 == null ||
                elementData2 == null ||
                elementData1 == elementData2 ||
                // map.GetCell(rowIndex1,columnIndex1).state == ConfCellStateEnum.Env ||
                // map.GetCell(rowIndex2,columnIndex2).state == ConfCellStateEnum.Env ||
                !elementData1.CanBuildEliminationBlock()||
                !elementData2.CanBuildEliminationBlock())
            {
                return blockList;
            }

            if (_eliminateBuilderDic[elementData1.confData.id]
                .TryBuildEliminationBlocksOnExchange(rowIndex1, columnIndex1, map, out EliminationBlock blocks1))
            {
                blockList.Add(blocks1);
            }

            if (_eliminateBuilderDic[elementData2.confData.id]
                .TryBuildEliminationBlocksOnExchange(rowIndex2, columnIndex2, map, out EliminationBlock blocks2))
            {
                blockList.Add(blocks2);
            }

            return blockList;
        }

        public static void ExecuteEliminate(int[,] map,List<EliminationBlock> blocks)
        {
            foreach (var block in blocks)
            {
                for (int i = 0; i < block.count; i++)
                {
                    VectorConvertArrayIndex(block[i], out int rowIndex, out int columnIndex);
                    map[rowIndex, columnIndex] = 0;
                }
                //todo onEliminateAction(int id,int count)/(EliminationBlock block)
            }
        }

        public static void OnEliminationBlockRemove(EliminationBlock block)
        {
            
        }

        public static BaseCellData CreateCellData(int id, int rowIndex, int column)
        {
            return _cellFactoryDic[id]?.Invoke(rowIndex, column);
        }

        public static IElementData CreateElementData(int id)
        {
            return _elementFactoryDic[id]?.Invoke(id);
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