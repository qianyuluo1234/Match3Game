using Match3Game.Event;
using Match3Game.Logic.Core;
using Match3Game.Logic.Element;
using Match3Game.Manager.Level;
using Match3Game.UnityEx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Match3Game.Logic.Controller
{
    public class NormalChessController
    {
        public bool enable { get; set; } = true;
        
        private bool _isHorizontal = false;
        private bool _isDrag = false;
        private NormalChess _dragChess;
        private BaseChess[] _selectedBothSides = new BaseChess[2];


        private void OnClick(BaseEventData baseEventData)
        {
            if (!enable || _isDrag)
            {
                return;
            }

            PointerEventData eventData = baseEventData as PointerEventData;
            NormalChess chess =
                LevelManager.instance.currentGameMap.FindChessByGameObject(eventData.pointerClick) as NormalChess;
            if (chess == null || !chess.data.allowPlayerControl)
            {
                return;
            }

            IBaseElement selected = ControllerManager.instance.selected;
            if (selected != null && 
                LevelManager.instance.currentGameMap.data.IsNeighbor(selected.data, chess.data))
            {
                ExChangeElement(chess);
            }
            else
            {
                SetClickSelectElement(chess);
            }
        }
        private void OnBeginDrag(BaseEventData baseEventData)
        {
            if (!enable)
            {
                return;
            }

            PointerEventData eventData = baseEventData as PointerEventData;
            _dragChess = LevelManager.instance.currentGameMap.FindChessByGameObject(eventData.pointerClick) as NormalChess;
            if (_dragChess == null || !_dragChess.data.allowPlayerControl)
            {
                return;
            }
            _isDrag = true;

            _isHorizontal = Mathf.Abs(eventData.delta.x) >= Mathf.Abs(eventData.delta.y);
            
            IElementData[] _selectedBothSideDatas = new IElementData[2];
            if (_isHorizontal)
            {
                LevelManager.instance.currentGameMap.data.TryGetNeighborChess(_dragChess.data, Vector2Int.left,
                    out _selectedBothSideDatas[0]);
                LevelManager.instance.currentGameMap.data.TryGetNeighborChess(_dragChess.data, Vector2Int.right,
                    out _selectedBothSideDatas[1]);
            }
            else
            {
                LevelManager.instance.currentGameMap.data.TryGetNeighborChess(_dragChess.data, Vector2Int.down,
                    out _selectedBothSideDatas[0]);
                LevelManager.instance.currentGameMap.data.TryGetNeighborChess(_dragChess.data, Vector2Int.up,
                    out _selectedBothSideDatas[1]);
            }

            for (int i = 0; i < _selectedBothSideDatas.Length; i++)
            {
                if (_selectedBothSideDatas[i] == null || !_selectedBothSideDatas[i].allowPlayerControl)
                {
                    _selectedBothSideDatas[i] = _dragChess.data;
                }
                _selectedBothSides[i] = LevelManager.instance.currentGameMap.GetElement(_selectedBothSideDatas[i].rowIndex,
                    _selectedBothSideDatas[i].columnIndex) as BaseChess;
            }

            SetMoveSelectChess(_dragChess);
        }
        private void OnDrag(BaseEventData baseEventData)
        {
            Vector2 delta = (baseEventData as PointerEventData).delta;

            if (Mathf.Abs(delta.x) < 0.1f && Mathf.Abs(delta.y) < 0.1f)
            {
                return;
            }
            
            if (_isHorizontal)
            {
                if (Mathf.Abs(delta.x) > 100)
                {
                    delta.y = delta.y / delta.x * 100;
                    delta.x = delta.x > 0 ? 100 : -100;
                }

                float min = _selectedBothSides[0].GetPositionOnLevel().x;
                float max = _selectedBothSides[1].GetPositionOnLevel().x;

                Vector3 pos = _dragChess.gameObject.transform.localPosition;
                pos.x = Mathf.Clamp(pos.x + delta.x * 1f * Time.deltaTime, min, max);
                _dragChess.gameObject.transform.localPosition = pos;
            }
            else
            {
                if (Mathf.Abs(delta.y) > 100)
                {
                    delta.x = delta.x / delta.y * 100;
                    delta.y = delta.y > 0 ? 100 : -100;
                }

                float min = _selectedBothSides[0].GetPositionOnLevel().y;
                float max = _selectedBothSides[1].GetPositionOnLevel().y;

                Vector3 pos = _dragChess.gameObject.transform.localPosition;
                pos.y = Mathf.Clamp(pos.y + delta.y * 1f * Time.deltaTime, min, max);
                _dragChess.gameObject.transform.localPosition = pos;
            }
        }
        private void OnEndDrag(BaseEventData baseEventData)
        {
            var localPosition = _dragChess.gameObject.transform.localPosition;
            Vector3 distance1 = localPosition - _selectedBothSides[0].GetPositionOnLevel();
            Vector3 distance2 = localPosition - _selectedBothSides[1].GetPositionOnLevel();
            Vector3 distanceSelf = localPosition - _dragChess.GetPositionOnLevel();
            BaseChess target = null;
            if (distance1.sqrMagnitude < distance2.sqrMagnitude)
            {
                if (_selectedBothSides[0] != _dragChess && distance1.sqrMagnitude < distanceSelf.sqrMagnitude)
                {
                    target = _selectedBothSides[0];
                }
            }
            else
            {
                if (_selectedBothSides[1] != _dragChess && distance2.sqrMagnitude < distanceSelf.sqrMagnitude)
                {
                    target = _selectedBothSides[1];
                }
            }

            if (target != null && target.data.value != _dragChess.data.value)
            {
                ExChangeElement(target);
            }
            else
            {
                _dragChess.MoveToBack();
            }

            _selectedBothSides[0] = null;
            _selectedBothSides[1] = null;
            _dragChess = null;
            _isDrag = false;
        }
        private void ExChangeElement(BaseChess targetChess)
        {
            IBaseElement selected = ControllerManager.instance.selected;
            if (targetChess == null || !(selected != null && selected.data.allowBuildEliminationBlock))
            {
                return;
            }

            if (targetChess.data.value != selected.data.value)
            {
                LevelManager.instance.currentGameMap.ExChangeChess(selected as BaseChess, targetChess);
                EventManager.instance.TriggerEvent((int) EEventId.OnElementExchange, (BaseChess)selected, targetChess);
            }

            (selected as IChessSelectEvent)?.OnDeselect();
            ControllerManager.instance.SetSelected(null);
        }
        private void SetClickSelectElement(NormalChess chess)
        {
            if (chess == null)
            {
                return;
            }
            IBaseElement selected = ControllerManager.instance.selected;

            if (selected == chess)
            {
                chess.OnDeselect();
                ControllerManager.instance.SetSelected(null);
                return;
            }

            SetSelectChessInternal(chess);
        }
        private void SetMoveSelectChess(NormalChess chess)
        {
            ControllerManager.instance.SetSelected(null);

            if (chess == null || ControllerManager.instance.selected == chess)
            {
                return;
            }

            SetSelectChessInternal(chess);
        }
        private void SetSelectChessInternal(NormalChess chess)
        {
            IBaseElement selected = ControllerManager.instance.selected;
            (selected as IChessSelectEvent)?.OnDeselect();
            ControllerManager.instance.SetSelected(chess);
            chess.OnSelect();
        }
        public void AddListener(NormalChess chess)
        {
            EventTrigger eventTrigger = chess.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = chess.gameObject.AddComponent<EventTrigger>();
            }
            eventTrigger.AddListener(OnClick, EventTriggerType.PointerClick);
            eventTrigger.AddListener(OnBeginDrag, EventTriggerType.BeginDrag);
            eventTrigger.AddListener(OnDrag, EventTriggerType.Drag);
            eventTrigger.AddListener(OnEndDrag, EventTriggerType.EndDrag);
        }
    }
}