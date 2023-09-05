using Match3Game.Config;

namespace Match3Game.Logic.Core.Element
{
    public class NormalChessData : IElementData
    {
        public bool allowBuildEliminationBlock { get; } = true;
        public bool allowPlayerControl { get; } = true;
        public int fallValue { get; private set; }
        public bool allowFall { get; } = true;

        public int value { get; }
        public int rowIndex { get; private set; }
        public int columnIndex { get; private set; }
        public ConfElementsData confData { get; }

        public NormalChessData(int id)
        {
            confData = ConfigManager.instance.GetData<ConfElementsData>(ConfigNameEnum.Elements, id).Value;
            this.value = confData.value;
        }

        public void SetFallValue(int targetValue)
        {
            fallValue = targetValue;
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
            return true;
        }

        public bool CanFall()
        {
            return allowFall;
        }

        public bool CanBuildEliminationBlock()
        {
            return allowBuildEliminationBlock;
        }
    }
}