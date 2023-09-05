using Match3Game.Config;

namespace Match3Game.Logic.Core.Element
{
    public class BoomChessData : IElementData
    {
        public int value { get; }
        public int rowIndex { get; private set; }
        public int columnIndex { get; private set; }
        public int fallValue { get; private set; }
        public bool allowFall { get; set; } = true;
        public bool allowBuildEliminationBlock { get; set; } = true;
        public bool allowPlayerControl { get; set; } = true;
        public ConfElementsData confData { get; }

        public void SetFallValue(int targetValue)
        {
            fallValue = targetValue;
        }

        public void SetIndex(int newRowIndex, int newColumnIndex)
        {
            rowIndex = newColumnIndex;
            columnIndex = newColumnIndex;
        }

        public bool CanPlayerControl()
        {
            throw new System.NotImplementedException();
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

        public BoomChessData(int id)
        {
            confData = ConfigManager.instance.GetData<ConfElementsData>(ConfigNameEnum.Elements,id).Value;
            this.value = confData.value;

        }
    }
}