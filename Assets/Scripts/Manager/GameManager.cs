using GameTools;
using Match3Game.Config;
using Match3Game.Event;
using Match3Game.Logic.Core;
using Match3Game.Manager.Level;
using UnityEngine;

namespace Match3Game.Manager
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _game;
        public static GameManager game
        {
            get
            {
                if (_game == null)
                {
                    _game = GameObject.FindObjectOfType<GameManager>();
                    if (_game == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _game = go.AddComponent<GameManager>();
                    }
                }
                return _game;
            }
        }
        
        private void Awake()
        {
            Match3Utility.Init();
            
            EventManager.instance.Init();
            TimerManager.instance.Init(this);
            ConfigManager.instance.Init();
            LevelManager.instance.Init();
        }

        private void Start()
        {
            LevelManager.instance.GameLoad(1);
            LevelManager.instance.GameStart();
        }

        private void Update()
        {
            TimerManager.instance.Tick();
        }
    }
}