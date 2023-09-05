using Match3Game.Config;

namespace Match3Game.Logic.Core.Cell
{
    public class EnvCellData : BaseCellData
    {
        public override ConfCellStateEnum state => ConfCellStateEnum.Env;

        public EnvCellData(int rowIndex, int columnIndex) : base(rowIndex, columnIndex)
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