using System;
using System.Collections.Generic;
using System.Linq;
using Match3Game.Config;
using Match3Game.Logic.Core.Cell;
using Match3Game.Logic.Core.Element;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match3Game.Logic.Core
{
    public class Match3MapData
    {
        public int row { get; }
        public int column { get; }
        private ConfMapData confData { get; }

        private BaseCellData[,] _cells;
        private IElementData[,] _elements;

        private Vector2Int[] _rebuildPoints;
        public Dictionary<int, List<IElementData>> columnDataDic { get; private set; }
        public Match3MapData(int id)
        {
            confData = ConfigManager.instance.GetData<ConfMapData>(ConfigNameEnum.Map, id).Value;
            this.row = confData.row;
            this.column = confData.column;
        }

        public void Create()
        {
            _cells = new BaseCellData[row, column];
            _elements = new IElementData[row, column];
            //create pre cell
            if (confData.preSetCellDic != null)
            {
                foreach (var kvPair in confData.preSetCellDic)
                {
                    foreach (var pos in kvPair.Value)
                    {
                        int rowIndex = (int) pos.x;
                        int columnIndex = (int) pos.y;
                        _cells[rowIndex, columnIndex] =
                            Match3Utility.CreateCellData(kvPair.Key, rowIndex,
                                columnIndex); 
                    }
                }
            }

            //create normal cell and element

            int length = confData.types.Length;
            Dictionary<int, int> value2IdDic = new Dictionary<int, int>();
            for (int i = 0; i < length; i++)
            {
                int id = confData.types[i];
                ConfElementsData data = ConfigManager.instance.GetData<ConfElementsData>(ConfigNameEnum.Elements, id).Value;
                value2IdDic.Add(data.value, id);
            }
            
            List<int> temp = new List<int>(length);
            for (int i = 0; i < row; i++)
            {
                bool checkVertical = i >= 2;

                for (int j = 0; j < column; j++)
                {
                    if (_cells[i, j] == null)
                    {
                        _cells[i, j] = new NormalCellData(i, j);
                    }
                    
                    //cellstatehandler
                    if (_cells[i, j].state == ConfCellStateEnum.Env)
                    {
                        continue;
                    }
                    
                    int exclude1 = 0;
                    int exclude2 = 0;
                    if (checkVertical)
                    {
                        exclude1 = _elements[i - 2, j].value;
                    }

                    if (j >= 2)
                    {
                        exclude2 = _elements[i, j - 2].value;
                    }

                    temp.Clear();
                    temp.AddRange(value2IdDic.Keys);
                    temp.Remove(exclude1);
                    temp.Remove(exclude2);
                    int value = temp[Random.Range(0, temp.Count)];
                    _elements[i, j] = Match3Utility.CreateElementData(value2IdDic[value]);
                    _elements[i, j].SetIndex(i, j);
                }
            }
        }
        
        public void ExChangeElement(IElementData source, IElementData target)
        {
            int tempRowIndex = target.rowIndex;
            int tempColumnIndex = target.columnIndex;
            //修改数组中位置
            _elements[source.rowIndex, source.columnIndex] = target;
            _elements[tempRowIndex, tempColumnIndex] = source;
            //修改数据索引
            target.SetIndex(source.rowIndex, source.columnIndex);
            source.SetIndex(tempRowIndex, tempColumnIndex);
        }
        public void ExecuteEliminate(List<EliminationBlock> blocks)
        {
            List<Vector2Int> rebuildList = new List<Vector2Int>();
            foreach (var block in blocks)
            {
                for (int i = 0; i < block.count; i++)
                {
                    Match3Utility.VectorConvertArrayIndex( block[i],out int rowIndex, out int columnIndex);
                    _elements[rowIndex, columnIndex] = null;
                    if (!rebuildList.Contains(block[i]))
                    {
                        rebuildList.Add(block[i]);
                    }
                }
            }

            _rebuildPoints = rebuildList.ToArray();
        }

        public void RebuildData()
        {
            columnDataDic = new Dictionary<int, List<IElementData>>();
            foreach (var pos in _rebuildPoints)
            {
                int index = Random.Range(0, confData.types.Length);
                IElementData elementData = Match3Utility.CreateElementData(confData.types[index]);
                Match3Utility.VectorConvertArrayIndex(pos,out int _,out int columnIndex);
                if (!columnDataDic.ContainsKey(columnIndex))
                {
                    columnDataDic.Add(columnIndex,new List<IElementData>());
                }
                //columnDataDic[columnIndex].Count 不是固定值，注意下方调用add方法.故为length+0,length+1...
                elementData.SetIndex(_elements.GetLength(0) + columnDataDic[columnIndex].Count, columnIndex);
                columnDataDic[columnIndex].Add(elementData);
            }
            foreach (var kvPair in columnDataDic)
            {
                kvPair.Value.Sort((data1, data2) => data1.rowIndex - data2.rowIndex);
            }
        }
        
        public void RebuildIndex()
        {
            //按列
            Dictionary<int, List<Vector2Int>> columnPointsDic = new Dictionary<int, List<Vector2Int>>();
            foreach (var pos in _rebuildPoints)
            {
                Match3Utility.VectorConvertArrayIndex(pos,out int _,out int columnIndex);
                if (!columnPointsDic.ContainsKey(columnIndex))
                {
                    columnPointsDic.Add(columnIndex,new List<Vector2Int>());
                }
                columnPointsDic[columnIndex].Add(pos);
            }
            foreach (var kvPair in columnPointsDic)
            {
                kvPair.Value.Sort((pos1, pos2) => pos1.y - pos2.y);
            }
            
            for (int i = 0; i < _elements.GetLength(1); i++)
            {
                if (!columnPointsDic.ContainsKey(i))
                {
                    continue;
                }
                
                Match3Utility.VectorConvertArrayIndex(columnPointsDic[i].First(), out int rowIndex, out int _);
                for (int j = rowIndex; j < _elements.GetLength(0); j++)
                {
                    if (_cells[j, i].state == ConfCellStateEnum.Env || _elements[j, i] != null)
                    {
                        continue;
                    }

                    for (int k = j + 1; k < _elements.GetLength(0) + columnDataDic[i].Count; k++)
                    {
                        if (k < _elements.GetLength(0))
                        {
                            if (_elements[k, i] == null)
                            {
                                continue;
                            }
                            if (!_elements[k, i].CanFall())
                            {
                                break;
                            }
                            _elements[j, i] = _elements[k, i];
                            //_elements[j, i].SetFallValue(k - j);
                            _elements[j, i].SetIndex(j, i);
                            _elements[k, i] = null;
                        }
                        else
                        {
                            int index = k - _elements.GetLength(0);
                            if (columnDataDic[i][index] == null)
                            {
                                continue;
                            }
                            _elements[j, i] = columnDataDic[i][index];
                            //_elements[j, i].SetFallValue(k - j);
                            _elements[j, i].SetIndex(j, i);
                            columnDataDic[i][index] = null;
                        }
                        break;
                    }
                }
            }
            
            columnDataDic = null;
            _rebuildPoints = null;
        }


        public bool TryGetNeighborCell(BaseCellData cellData, Vector2Int dir, out BaseCellData neighbor)
        {
            Match3Utility.VectorConvertArrayIndex(dir, out int rowIndex, out int columnIndex);
            if (cellData.rowIndex + rowIndex >= 0 && cellData.columnIndex + columnIndex >= 0
                && cellData.rowIndex + rowIndex < _cells.GetLength(0) 
                && cellData.columnIndex + columnIndex < _cells.GetLength(1))
            {
                neighbor = _cells[cellData.rowIndex + rowIndex, cellData.columnIndex + columnIndex];
                return true;
            }

            neighbor = null;
            return false;
        }

        public bool TryGetNeighborElement(IElementData elementData, Vector2Int dir, out IElementData neighbor)
        {
            Match3Utility.VectorConvertArrayIndex(dir, out int rowIndex, out int columnIndex);
            if (elementData.rowIndex + rowIndex >= 0 && elementData.columnIndex + columnIndex >= 0
                                                     && elementData.rowIndex + rowIndex < _cells.GetLength(0) 
                                                     && elementData.columnIndex + columnIndex < _cells.GetLength(1))
            {
                neighbor = _elements[elementData.rowIndex + rowIndex, elementData.columnIndex + columnIndex];
                return true;
            }

            neighbor = null;
            return false;
        }

        public bool TryGetNeighborChess(IElementData elementData, Vector2Int dir, out IElementData neighbor)
        {
            Match3Utility.VectorConvertArrayIndex(dir, out int rowIndex, out int columnIndex);
            if (elementData.rowIndex + rowIndex >= 0 && elementData.columnIndex + columnIndex >= 0
                                                     && elementData.rowIndex + rowIndex < _cells.GetLength(0) 
                                                     && elementData.columnIndex + columnIndex < _cells.GetLength(1))
            {
                neighbor = _elements[elementData.rowIndex + rowIndex, elementData.columnIndex + columnIndex];
                return neighbor.allowBuildEliminationBlock;
            }
            neighbor = null;
            return false;
        }
        
        public bool IsNeighbor(IElementData element1, IElementData element2)
        {
            if (element1 == null || element2 == null)
            {
                return false;
            }

            return (element1.rowIndex == element2.rowIndex && (element1.columnIndex + 1 == element2.columnIndex || element1.columnIndex - 1 == element2.columnIndex)) 
                   || (element1.columnIndex  == element2.columnIndex && (element1.rowIndex + 1 == element2.rowIndex || element1.rowIndex - 1 == element2.rowIndex));
        }
 
        public bool IsNeighbor(ICellData cell1, ICellData cell2)
        {
            if (cell1 == null || cell2 == null)
            {
                return false;
            }

            return (cell1.rowIndex == cell2.rowIndex && (cell1.columnIndex + 1 == cell2.columnIndex || cell1.columnIndex - 1 == cell2.columnIndex)) 
                   || (cell1.columnIndex  == cell2.columnIndex && (cell1.rowIndex + 1 == cell2.rowIndex || cell1.rowIndex - 1 == cell2.rowIndex));
        }
        
        public BaseCellData GetCell(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || columnIndex < 0 || rowIndex >= _cells.GetLength(0) || columnIndex >= _cells.GetLength(1))
            {
                throw new IndexOutOfRangeException();
            }

            return _cells[rowIndex, columnIndex];
        }
        public IElementData GetElement(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || columnIndex < 0 || rowIndex >= _elements.GetLength(0) || columnIndex >= _elements.GetLength(1))
            {
                throw new IndexOutOfRangeException();
            }

            return _elements[rowIndex, columnIndex];
        }
        
    }
}