using System.Collections.Generic;

namespace Match3Game.Config
{
    public class ConfElements : IConfDataTable<ConfElementsData>
    {
        private Dictionary<int, ConfElementsData> _table = new Dictionary<int, ConfElementsData>();

        public void Parse()
        {
            AddVirtualData(this);
        }

        public ConfElementsData? GetData(int id)
        {
            if (_table.ContainsKey(id))
            {
                return _table[id];
            }
            return null;
        }

        static void AddVirtualData(ConfElements elements)
        {
            elements._table.Add(10001,
                new ConfElementsData() {id = 10001, value = 1, name = "狐狸", icon = "001_0"});
            elements._table.Add(10002,
                new ConfElementsData() {id = 10002, value = 2, name = "熊", icon = "002_0"});
            elements._table.Add(10003,
                new ConfElementsData() {id = 10003, value = 3, name = "鸡", icon = "003_0"});
            elements._table.Add(10004,
                new ConfElementsData() {id = 10004, value = 4, name = "兔子", icon = "004_0"});
            elements._table.Add(10005,
                new ConfElementsData() {id = 10005, value = 5, name = "考拉", icon = "005_0"});
            elements._table.Add(10006,
                new ConfElementsData() {id = 10006, value = 6, name = "猫", icon = "006_0"});
            elements._table.Add(10007,
                new ConfElementsData() {id = 10007, value = 7, name = "熊猫", icon = "007_0"});
        }
    }
}