using System.Collections.Generic;
using Match3Game.Logic.Core;
using Match3Game.Manager.Level;
using UnityEditor;
using UnityEngine;

namespace Match3Game.Logic.Element
{
    public class ChessBuilder
    {
        private List<BaseChess> _activeList;
        private List<BaseChess> _inActiveList;

        public int idleCount => _inActiveList.Count;
        public int maxIdleCount { get; }

        private GameObject _prefab;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="maxIdleCount">最大闲置数，超出数量的闲置对象将直接移除</param>
        public ChessBuilder(int maxIdleCount)
        {
            _activeList = new List<BaseChess>();
            _inActiveList = new List<BaseChess>();
            this.maxIdleCount = maxIdleCount;
            _prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Arts/Prefabs/Chess.prefab");
        }
        
        public BaseChess Build(IElementData data)
        {
            BaseChess chess;
            if (Load(data, out chess))
            {
                chess.Show();
                _activeList.Add(chess);
                _inActiveList.Remove(chess);
            }
            else
            {
                chess = Create(data);
                _activeList.Add(chess);
            }
            //chess.Init(data);
            return chess;
        }

        public bool Retrieve(BaseChess chess)
        {
            if (!_activeList.Contains(chess))
            {
                return false;
            }
            _activeList.Remove(chess);
            if (_inActiveList.Count < maxIdleCount)
            {
                _inActiveList.Add(chess);
                chess.Hide();
                chess.Reset();
            }
            else
            {
                chess.Destroy();
            }
            
            return true;
        }
        
        public void Clear()
        {
            foreach (var chess in _activeList)
            {
                chess.Destroy();
            }

            foreach (var chess in _inActiveList)
            {
                chess.Destroy();
            }
            
            _activeList.Clear();
            _inActiveList.Clear();
            _activeList = null;
            _inActiveList = null;
        }

        private BaseChess Create(IElementData data)
        {
            GameObject gameObject = Object.Instantiate(_prefab, LevelManager.instance.elementLayer);
            //todo 根据data 判断要创建的实际类型

            BaseChess chess = new NormalChess(gameObject);
            //chess.Init(data);
            return chess;
        }

        private bool Load(IElementData data,out BaseChess chess)
        {
            //todo 根据data 判断要创建的实际类型
            chess = _inActiveList.Find(_ => _.GetType() == typeof(NormalChess));
            return chess != null;
        }
        
    }
}