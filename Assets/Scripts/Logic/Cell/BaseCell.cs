using Match3Game.Config;
using Match3Game.Logic.Core;
using Match3Game.Logic.Core.Cell;
using Match3Game.Manager.Level;
using UnityEditor;
using UnityEngine;

namespace Match3Game.Logic.Cell
{
    public class BaseCell
    {
        public BaseCellData data { get; }
        public GameObject gameObject { get; }
        
        public bool enable
        {
            get
            {
                if (data.state == ConfCellStateEnum.Env)
                {
                    return false;
                }
                
                return gameObject != null && gameObject.activeSelf;
            }
        }
        
        public BaseCell(BaseCellData cellData)
        {
            data = cellData;
            
            if (cellData.state == ConfCellStateEnum.Env)
            {
                return;
            }
            
            GameObject cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Arts/Prefabs/Cell.prefab");
            gameObject = Object.Instantiate(cellPrefab, LevelManager.instance.cellBottomLayer);
            gameObject.transform.localPosition = GetPositionOnLevel();
        }
        
        private Vector3 GetPositionOnLevel()
        {
            Vector2Int temp = Match3Utility.ArrayIndexConvertVector(data.rowIndex, data.columnIndex);
            return new Vector3(temp.x * 0.9f, temp.y * 0.9f);
        }
    }
}