using UnityEngine.EventSystems;

namespace Match3Game.Logic.Controller
{
    public interface IChessDragEvent
    {
        void OnBeginDrag(BaseEventData baseEventData);
        void OnDrag(BaseEventData baseEventData);
        void OnEndDrag(BaseEventData baseEventData);
    }
}