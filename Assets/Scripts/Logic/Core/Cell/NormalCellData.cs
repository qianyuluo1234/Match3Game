using Match3Game.Config;

namespace Match3Game.Logic.Core.Cell
{
    public class NormalCellData : BaseCellData
    {
        public override ConfCellStateEnum state { get; } = ConfCellStateEnum.Normal;

        public NormalCellData(int rowIndex, int columnIndex) : base(rowIndex, columnIndex)
        {
        }

        public override void ChangeElementOnEnter()
        {

        }

        public override void ChangeElementOnStateRefresh()
        {
        }

        public override void ChangeElementOnExit()
        {
        }

        public override void OnCellStateChange()
        {
        }
    }
}