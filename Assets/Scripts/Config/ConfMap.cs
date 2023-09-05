using System.Collections.Generic;
using UnityEngine;

namespace Match3Game.Config
{
    public class ConfMap : IConfDataTable<ConfMapData>
    {
        private Dictionary<int, ConfMapData> _table = new Dictionary<int, ConfMapData>();

        public void Parse()
        {
            AddVirtualData(this);
        }

        public ConfMapData? GetData(int id)
        {
            if (_table.ContainsKey(id))
            {
                return _table[id];
            }

            return null;
        }
        
                
        static void AddVirtualData(ConfMap map)
        {
            map._table.Add(1, new ConfMapData() {id = 1, row = 8, column = 5, types = new[] {10001, 10002, 10003, 10004, 10005} , preSetCellDic = null});
        }
    }
}