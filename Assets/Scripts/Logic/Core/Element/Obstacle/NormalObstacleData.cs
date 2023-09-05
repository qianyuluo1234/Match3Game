using Match3Game.Config;

namespace Match3Game.Logic.Core.Element
{
    public class NormalObstacleData : IElementData
    {
        public int value { get; }
        public int rowIndex { get; private set; }
        public int columnIndex { get; private set; }
        public int fallValue { get; }
        public bool allowFall { get; }
        public bool allowBuildEliminationBlock { get; }
        public bool allowPlayerControl { get; } = false;
        public ConfElementsData confData { get; }
        
        public NormalObstacleData(int id)
        {
            this.value = id;
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;

            //todo 障碍物配置
            //confData = ConfigManager.instance.GetData<ConfElementsData>(ConfigNameEnum.Elements,id).Value;
        }


        public void SetFallValue(int targetValue)
        {
            throw new System.NotImplementedException();
        }

        public void SetIndex(int newRowIndex, int newColumnIndex)
        {
            this.rowIndex = newRowIndex;
            this.columnIndex = newColumnIndex;
        }

        public bool CanPlayerControl()
        {
            return allowPlayerControl;
        }

        public bool CanEliminate()
        {
            throw new System.NotImplementedException();
        }

        public bool CanFall()
        {
            throw new System.NotImplementedException();
        }

        public bool CanBuildEliminationBlock()
        {
            throw new System.NotImplementedException();
        }
    }
}