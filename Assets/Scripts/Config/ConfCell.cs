using System.Collections.Generic;

namespace Match3Game.Config
{
    public class ConfCell :IConfDataTable<ConfCellData>
    {
        private Dictionary<int, ConfCellData> _table = new Dictionary<int, ConfCellData>();

        public void Parse()
        {
            //AddVirtualData(this);
        }

        public ConfCellData? GetData(int id)
        {
            if (_table.ContainsKey(id))
            {
                return _table[id];
            }
            return null;
        }
    }
}