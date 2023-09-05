using System;
using Match3Game.Logic.Controller;
using Match3Game.Logic.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Match3Game.Logic.Element
{
    public class NormalChess : BaseChess, IChessSelectEvent//, IChessDragEvent
    {
        public NormalChess(GameObject gameObject) : base(gameObject)
        {
            ControllerManager.instance.AddListener(this);
        }

        protected override void InitGameObject()
        {
            gameObject.transform.localPosition = GetPositionOnLevel();
            Object[] elementAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Arts/Sprites/Element.png");
            gameObject.GetComponent<SpriteRenderer>().sprite =
                Array.Find(elementAssets, (_) => _.name == data.confData.icon) as Sprite;

#if UNITY_EDITOR
            gameObject.name = $"{Match3Utility.ArrayIndexConvertVector(data.rowIndex, data.columnIndex)}";
#endif
        }
        
        public void OnSelect()
        {
            Object[] elementAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Arts/Sprites/Element.png");
            gameObject.GetComponent<SpriteRenderer>().sprite =
                Array.Find(elementAssets,
                    (_) => _.name == $"00{data.value}_3") as Sprite;
        }
        public void OnDeselect()
        {
            Object[] elementAssets = AssetDatabase.LoadAllAssetsAtPath("Assets/Arts/Sprites/Element.png");
            gameObject.GetComponent<SpriteRenderer>().sprite =
                Array.Find(elementAssets, (_) => _.name == data.confData.icon) as Sprite;
        }

        
    }
}