namespace Match3Game.Logic.Controller
{
    public interface IChessSelectEvent
    {
        void OnSelect();
        void OnDeselect();
    }
}