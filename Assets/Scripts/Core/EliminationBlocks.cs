using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Core
{
    public class EliminationBlocks
    {
        private List<Vector2Int> _posList;
        public int id { get; }
        public int count => _posList.Count;

        public Vector2Int this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new AggregateException("index must >= 0");
                }

                return _posList[index];
            }
        }

        public EliminationBlocks(int id, List<Vector2Int> posList)
        {
            if (posList == null || posList.Count < 3)
            {
                throw new AggregateException($"Unable to form elimination block");
            }

            this.id = id;
            _posList = posList;
        }

        public void Add(Vector2Int pos)
        {
            if (_posList.Contains(pos))
            {
                Debug.LogWarning($"this pos has exist! pos: {pos}");
                return;
            }

            _posList.Add(pos);
        }

        public bool Contains(Vector2Int pos)
        {
            return _posList.Contains(pos);
        }
        
    }
}