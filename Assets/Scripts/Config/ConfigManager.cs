using System.Collections.Generic;

namespace Match3Game.Config
{
    public class ConfigManager
    {
        private static ConfigManager _instance;
        private static  object _lock = new object();

        public static ConfigManager instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigManager();
                        }
                    }
                }

                return _instance;
            }
        }


        private Dictionary<int, IConfDataTable> _configInfoDic;
        private ConfigManager()
        {
            
        }

        public void Init()
        {
            ConfMap map = new ConfMap();
            ConfElements elements = new ConfElements();
            
            _configInfoDic = new Dictionary<int, IConfDataTable>();
            _configInfoDic.Add((int) ConfigNameEnum.Map, map);
            _configInfoDic.Add((int) ConfigNameEnum.Elements, elements);
            
            map.Parse();
            elements.Parse();
        }
        
        public T? GetData<T>(ConfigNameEnum tableName,int id) where T:struct
        {
            int tableId = (int)tableName;
            if (_configInfoDic.ContainsKey(tableId))
            {
                IConfDataTable<T> table  = _configInfoDic[tableId] as IConfDataTable<T>;
                return table?.GetData(id);
            }
            return null;
        }

        public bool TryGetData<T>(ConfigNameEnum tableName, int id,out T t) where T : struct
        {
            int tableId = (int)tableName;
            if (_configInfoDic.ContainsKey(tableId))
            {
                IConfDataTable<T> table  = _configInfoDic[tableId] as IConfDataTable<T>;
                var temp = table?.GetData(id);
                t = (T) temp.Value;
                return temp != null;
            }
            t = default;
            return false;
        }
    }
}