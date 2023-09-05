using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Match3Game.Event;
using Match3Game.Logic.Controller;
using Match3Game.Logic.Core;
using Match3Game.Logic.Element;
using Match3Game.Logic.Map;
using UnityEngine;

namespace Match3Game.Manager.Level
{
    public class LevelManager
    {
        private static LevelManager _instance;
        private static object _lock = new object();

        public static LevelManager instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LevelManager();
                        }
                    }
                }

                return _instance;
            }
        }

        public GameObject levelScene { get; private set; }
        public Transform elementLayer { get; private set; }
        public Transform cellBottomLayer { get; private set; }
        public Transform cellTopLayer { get; private set; }

        public GameMap currentGameMap { get; private set; }

        public NormalChessController controller { get; private set; }


        private LevelManager()
        {
        }

        public void Init()
        {
            levelScene = GameObject.Find("LevelScene");
            elementLayer = levelScene.transform.Find("ElementLayer");
            cellBottomLayer = levelScene.transform.Find("CellBottomLayer");
            cellTopLayer = levelScene.transform.Find("CellTopLayer");
        }

        public void GameLoad(int levelId)
        {
            //加载地图,创建格子;创建元素;下落填充
            currentGameMap = new GameMap(levelId);
            this.controller = new NormalChessController();
            BindingEvent();
        }

        public void GameStart()
        {
            currentGameMap.Create();
        }


        public void Pause()
        {

        }

        public void Finish()
        {
            UnbindingEvent();
        }

        public void Success()
        {

        }

        public void Fail()
        {

        }

        public void Exit()
        {

        }


        async private void OnElementExChange(BaseChess source, BaseChess target)
        {
            List<EliminationBlock> blocks = await Task.Run(
                () => Match3Utility.FindEliminationBlocksOnExchange(
                    currentGameMap.data,
                    source.data.rowIndex, source.data.columnIndex,
                    target.data.rowIndex, target.data.columnIndex
                ));
            bool result = await AwaitElementHandleEnd();

            if (result)
            {
                if (blocks != null && blocks.Count > 0)
                {
                    currentGameMap.ExecuteEliminate(blocks);
                }
                else
                {
                    currentGameMap.ExChangeChess(source, target);
                }
            }
        }

        private void OnEliminateFinish()
        {
            currentGameMap.Rebuild();
        }

        async private void OnElementFallEnd()
        {
            List<EliminationBlock> blocks =
                await Task.Run(() => Match3Utility.FindAllEliminationBlocks(currentGameMap.data));
            if (blocks != null && blocks.Count > 0)
            {
                currentGameMap.ExecuteEliminate(blocks);
            }
        }

        private Task<bool> AwaitElementHandleEnd()
        {
            TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>();

            void OnElementHandleEnd()
            {
                completionSource.SetResult(true);
                EventManager.instance.RemoveListener((int) EEventId.OnElementHandleEnd, OnElementHandleEnd);
            }

            EventManager.instance.AddListener((int) EEventId.OnElementHandleEnd, OnElementHandleEnd);

            return completionSource.Task;
        }

        private void BindingEvent()
        {
            EventManager.instance.AddListener<BaseChess, BaseChess>((int) EEventId.OnElementExchange,
                OnElementExChange);
            EventManager.instance.AddListener((int) EEventId.OnEliminateFinish, OnEliminateFinish);
            EventManager.instance.AddListener((int) EEventId.OnElementFallEnd, OnElementFallEnd);
        }

        private void UnbindingEvent()
        {
            EventManager.instance.RemoveListener<BaseChess, BaseChess>((int) EEventId.OnElementExchange,
                OnElementExChange);
            EventManager.instance.RemoveListener((int) EEventId.OnEliminateFinish, OnEliminateFinish);
            EventManager.instance.RemoveListener((int) EEventId.OnElementFallEnd, OnElementFallEnd);
        }
    }
}