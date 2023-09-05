using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Config
{
    public struct ConfMapData
    {
        public int id;
        public int row;
        public int column;
        public int[] types;
        public Dictionary<int, Vector2[]> preSetCellDic;
    }
}