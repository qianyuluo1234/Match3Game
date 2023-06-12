using System;
using UnityEngine;

namespace Match3Game.Core
{
    public class PreCheckBlock
    {
        private Vector2Int[] _eliminateArray;
        public Vector2Int startPos { get; }
        public Vector2Int targetPos { get; }
        public int count => _eliminateArray.Length;
        
        public Vector2Int this[int index]
        {
            get
            {
                if (index < 0)
                {
                    throw new AggregateException("index must >= 0");
                }

                return _eliminateArray[index];
            }
        }
        public PreCheckBlock(Vector2Int startPos,Vector2Int targetPos,Vector2Int[] eliminateArray)
        {
            this.startPos = startPos;
            this.targetPos = targetPos;
            this._eliminateArray = eliminateArray;
        }
    }
}