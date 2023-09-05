using System;
using GameTools;
using Match3Game.Logic.Core;
using Match3Game.UnityEx;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Match3Game.Logic.Element
{
    public abstract class BaseChess : IBaseElement
    {
        public IElementData data { get; private set; }
        public GameObject gameObject { get; }

        public BaseChess(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public virtual void Init(IElementData data)
        {
            this.data = data;
            InitGameObject();
        }
        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        public virtual void Reset()
        {
            this.data = null;
        }
        public virtual void Destroy()
        {
            Object.Destroy(gameObject);
        }
        
        protected abstract void InitGameObject();
        public Vector3 GetPositionOnLevel()
        {
            Vector2Int temp = Match3Utility.ArrayIndexConvertVector(data.rowIndex, data.columnIndex);
            return new Vector3(temp.x * 0.9f, temp.y * 0.9f);
        }
        
        public virtual void MoveToBack(Action completed = null)
        {
            Vector3 target = GetPositionOnLevel();
            Vector3 distance = target - gameObject.transform.localPosition;
            int tId = TimerManager.instance.AddTimerByMilliseconds(20, () =>
            {
                gameObject.transform.localPosition += distance * 0.1f;
            }, ()=>
            {
                gameObject.transform.localPosition = target;
                completed?.Invoke();
            },10);
            
            TimerManager.instance.Start(tId);
        }
        public virtual void MoveToTarget(Action completed = null)
        {
            //Debug.Log(data.rowIndex + "," + data.columnIndex);
            Vector3 target = GetPositionOnLevel();
            Vector3 distance = target - gameObject.transform.localPosition;

            //speed * 0.05f 由于计时器间隔为20ms,speed为 m/s，故做单位换算为此处 * 0.05
            bool inEnd = distance.sqrMagnitude <= 0.01f;
            Vector3 speed = inEnd ? Vector3.zero : distance.normalized * 4f * 0.05f;
            uint loop = inEnd
                ? 1
                : (uint) Mathf.CeilToInt(distance.magnitude / speed.magnitude);
            
            int tId = TimerManager.instance.AddTimerByMilliseconds(20,
                () => { gameObject.transform.localPosition += speed ; }, () =>
                {
                    gameObject.transform.localPosition = target;
#if UNITY_EDITOR
                    //仅编辑器下修改名称，实际项目中无意义
                    gameObject.name = $"{Match3Utility.ArrayIndexConvertVector(data.rowIndex, data.columnIndex)}";
#endif
                    completed?.Invoke();
                }, loop);

            TimerManager.instance.Start(tId);
        }
        

    }
}