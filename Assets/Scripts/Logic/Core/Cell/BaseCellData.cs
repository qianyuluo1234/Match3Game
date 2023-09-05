using Match3Game.Config;

namespace Match3Game.Logic.Core.Cell
{
    public abstract class BaseCellData : ICellData
    {
        public int rowIndex { get; }
        public int columnIndex { get; }
        public abstract ConfCellStateEnum state { get; }
        public BaseCellData(int rowIndex, int columnIndex)
        {
            this.rowIndex = rowIndex;
            this.columnIndex = columnIndex;
        }
        public abstract void ChangeElementOnEnter();

        public abstract void ChangeElementOnStateRefresh();

        public abstract void ChangeElementOnExit();

        public abstract void OnCellStateChange();
    }
}