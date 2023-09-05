using System;
using System.Collections.Generic;
using GameTools;
using Match3Game.Config;
using Match3Game.Event;
using Match3Game.Logic.Cell;
using Match3Game.Logic.Core;
using Match3Game.Logic.Core.Cell;
using Match3Game.Logic.Element;
using UnityEngine;

namespace Match3Game.Logic.Map
{
    public class GameMap
    {
        public Match3MapData data { get; }
        private BaseCell[,] _cells;
        private IBaseElement[,] _elements;
        
        private ChessBuilder _chessBuilder;

        private List<IBaseElement> _rebuildElements;
        public GameMap(int confDataId)
        {
            data = new Match3MapData(confDataId);
            _chessBuilder = new ChessBuilder(10);
        }

        public void Create()
        {
            data.Create();
            
            _cells = new BaseCell[data.row, data.column];
            _elements = new IBaseElement[data.row,data.column];
            for (int i = 0; i < data.row; i++)
            {
                for (int j = 0; j < data.column; j++)
                {
                    BaseCellData cellData = data.GetCell(i, j);
                    _cells[i, j] = new BaseCell(cellData);
                    if (cellData.state == ConfCellStateEnum.Env)
                    {
                        continue;
                    }
                    
                    IElementData elementData = data.GetElement(i, j);
                    
                    if (elementData.allowBuildEliminationBlock)
                    {
                        _elements[i,j] = _chessBuilder.Build(elementData);
                        _elements[i, j].Init(data.GetElement(i, j));
                    }
                    else
                    {
                        //创建障碍物
                    }
                }
            }
        }
        
        public void ExChangeChess(BaseChess source, BaseChess target)
        {
            //交换数据
            data.ExChangeElement(source.data, target.data);
            
            //更新对象位置
            _elements[source.data.rowIndex, source.data.columnIndex] = source;
            _elements[target.data.rowIndex, target.data.columnIndex] = target;

            EventManager.instance.TriggerEvent((int) EEventId.OnElementHandleBegin);
            source.MoveToTarget();
            target.MoveToTarget(() => EventManager.instance.TriggerEvent((int) EEventId.OnElementHandleEnd));
        }

        public void ExecuteEliminate(List<EliminationBlock> blocks)
        {
            data.ExecuteEliminate(blocks);

            foreach (var block in blocks)
            {
                for (int i = 0; i < block.count; i++)
                {
                    Match3Utility.VectorConvertArrayIndex( block[i],out int rowIndex, out int columnIndex);
                    BaseChess chess = (BaseChess) _elements[rowIndex, columnIndex];
                    if (chess != null)
                    {
                        _chessBuilder.Retrieve(chess);
                    }
                    _elements[rowIndex, columnIndex] = null;
                }
            }

            EventManager.instance.TriggerEvent((int) EEventId.OnEliminateFinish);
        }

        public void Rebuild()
        {
            //1.生成与缺失数量同等数量的随机棋子进入预备位置
            //2.移动缺失位置对应列项的所有棋子至指定位置
            data.RebuildData();
            _rebuildElements = new List<IBaseElement>();
            foreach (var list in data.columnDataDic.Values)
            {
                foreach (var chessData in list)
                {
                    BaseChess chess = _chessBuilder.Build(chessData);
                    _rebuildElements.Add(chess);
                    chess.Init(chessData);
                }
            }
            data.RebuildIndex();
            int lastFallRowIndex = 0;
            int lastFallcolumnIndex = 0;
            int tempFallValue = 0;
            for (int i = 0; i < data.row; i++)
            {
                for (int j = 0; j < data.column; j++)
                {
                    BaseCell cell = _cells[i, j];
                    if (cell.data.state == ConfCellStateEnum.Env)
                    {
                        continue;
                    }
                    
                    IBaseElement element = _elements[i, j];
                    if (element == null ||element.data.rowIndex == i && element.data.columnIndex == j)
                    {
                        continue;
                    }

                    if (tempFallValue <= element.data.fallValue)
                    {
                        lastFallRowIndex = i;
                        lastFallcolumnIndex = j;
                        tempFallValue = element.data.fallValue;
                    }
                    if (element is BaseChess)
                    {
                        _elements[element.data.rowIndex, element.data.columnIndex] = element;
                        _elements[i, j] = null;
                        //(element as BaseChess).MoveToTarget();
                    }
                }
            }

            foreach (var rebuildElement in _rebuildElements)
            {
                _elements[rebuildElement.data.rowIndex, rebuildElement.data.columnIndex] = rebuildElement;
                //(rebuildElement as BaseChess).MoveToTarget();
            }

            for (int i = 0; i < data.row; i++)
            {
                for (int j = 0; j < data.column; j++)
                {
                    IBaseElement element = _elements[i, j];
                    if (element.gameObject.transform.localPosition == (element as BaseChess).GetPositionOnLevel())
                    {
                        continue;
                    }
                    Action completed = null;
                    if (element.data.rowIndex == lastFallRowIndex && element.data.columnIndex == lastFallcolumnIndex)
                    {
                        completed = () =>
                        {
                            //test 等待0.5s后在通知
                            int tid = TimerManager.instance.AddTimerBySeconds(0.5f, () =>
                            {
                                EventManager.instance.TriggerEvent((int) EEventId.OnElementFallEnd);
                            });
                            TimerManager.instance.Start(tid);
                        };
                    }
                    (element as BaseChess)?.MoveToTarget(completed);
                }
            }
        }
        
        public BaseCell GetCell(int rowIndex,int columnIndex)
        {
            return _cells[rowIndex, columnIndex];
        }

        public IBaseElement GetElement(int rowIndex,int columnIndex)
        {
            return _elements[rowIndex, columnIndex];
        }

        public BaseChess FindChessByGameObject(GameObject gameObject)
        {
            foreach (var element in _elements)
            {
                if (element.gameObject == gameObject)
                {
                    return element as BaseChess;
                }
            }
            return null;
        }
    }
}