using Match3Game.Config;

namespace Match3Game.Logic.Core
{
    public interface ICellData
    {
        int rowIndex { get; }
        int columnIndex { get; }

        //map 该位置没有任何物体，属于地图的一部分;normal 默认状态; ice 冰块;
        ConfCellStateEnum state { get; }

        //相邻格子发生消除时
        //void OnEliminateInAdjacentCell();
        
        void ChangeElementOnEnter();
        void ChangeElementOnStateRefresh();
        void ChangeElementOnExit();
    }
}