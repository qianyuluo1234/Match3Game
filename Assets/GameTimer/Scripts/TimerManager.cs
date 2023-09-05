using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Init
// Update(){ TimerManager.instance.Tick(); }
// int tid = TimerManager.instance.AddTimerBy...    -> Start(tid)    -> (Pause(tid) -> Stop(tid) -> Restart(tid)) / DestroyTimer(tid)
// TimerManager.instance.Clear

namespace GameTools
{
    /// <summary>
    /// 计时器管理类
    /// </summary>
    public class TimerManager
    {
        private static TimerManager _instance = null;
        private static object _lcok = new object();

        public static TimerManager instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lcok)
                    {
                        if (_instance == null)
                        {
                            _instance = new TimerManager();
                        }
                    }
                }
                return _instance;
            }
        }
        
        private Dictionary<int, BaseTimer> _timerDic;
        private List<int> _readyTimerIdList;
        private List<int> _runningTimerIdList;
        private List<int> _sleepTimerIdList;
        
        private List<BaseTimer> _timerQueue;

        private MonoBehaviour _owner = null;
        
        private bool dirty { get; set; } = false;
        private bool enable { get; set; } = false;
        public int timerCount { get; private set; }

        private int _deltaTime = 10;
        private bool _isAsync = true;
        private TimerManager()
        {
        }

        /// <summary>
        /// 初始化函数,帧末排序
        /// </summary>
        /// <param name="owner"> 执行协程的对象 </param>
        public void Init(MonoBehaviour owner)
        {
            _timerDic = new Dictionary<int, BaseTimer>();
            _readyTimerIdList = new List<int>();
            _runningTimerIdList = new List<int>();
            _sleepTimerIdList = new List<int>();
            
            _timerQueue = new List<BaseTimer>();
            timerCount = 0;
            _owner = owner;

            dirty = false;
            enable = true;
            _isAsync = false;
            owner.StartCoroutine(DirtyCheckOnEndOfFrame());
        }

        /// <summary>
        /// 初始化函数,异步排序
        /// </summary>
        /// <param name="deltaTime"> 
        /// 计时器实际的刷新仍然是靠Tick函数在Unity主线程中Update函数实现,
        /// 该参数是计时器管理类对计时器做排序的时间间隔(ms)，该排序操作在其他线程中执行</param>
        public void Init(uint deltaTime = 10)
        {
            _timerDic = new Dictionary<int, BaseTimer>();
            _readyTimerIdList = new List<int>();
            _runningTimerIdList = new List<int>();
            _sleepTimerIdList = new List<int>();
            _timerQueue = new List<BaseTimer>();
            timerCount = 0;
            _deltaTime = (int) deltaTime;

            dirty = false;
            enable = true;
            _isAsync = true;
            Task.Run(DirtySortAsync);
        }

        public void Clear()
        {
            dirty = false;
            enable = false;
            timerCount = 0;
            _deltaTime = 10;
        }

        /// <summary>
        /// 更新函数
        /// </summary>
        public void Tick()
        {
            if (!enable || _runningTimerIdList.Count <= 0)
            {
                return;
            }

            int[] removeIdArray;
            if (_isAsync)
            {
                lock (_timerQueue)
                {
                    TimerQueueExecute(out removeIdArray);
                }
            }
            else
            {
                TimerQueueExecute(out removeIdArray);
            }
            
            foreach (var tid in removeIdArray)
            {
                RemoveRunningTimer(tid, out _);
            }
        }
        
        /// <summary>
        /// 新建毫秒计时器
        /// </summary>
        /// <param name="interval">计时器间隔</param>
        /// <param name="completeAction">计时器完成事件</param>
        /// <param name="autoDestroy">是否自动移除，默认为true</param>
        /// <returns></returns>
        public int AddTimerByMilliseconds(uint interval, Action completeAction, bool autoDestroy = true)
        {
            BaseTimer newTimer = new BaseTimer();
            timerCount++;

            _timerDic.Add(newTimer.tId,newTimer);
            _readyTimerIdList.Add(newTimer.tId);
            
            newTimer.Init(interval, completeAction, autoDestroy);

            return newTimer.tId;
        }

        /// <summary>
        /// 新建毫秒计时器
        /// </summary>
        /// <param name="interval">计时器间隔</param>
        /// <param name="loopAction">计时器循环事件</param>
        /// <param name="completeAction">计时器完成事件</param>
        /// <param name="loop">循环次数，默认为1。 0为无限循环</param>
        /// <param name="autoDestroy">是否自动移除，默认为true</param>
        /// <returns></returns>
        public int AddTimerByMilliseconds(uint interval, Action loopAction, Action completeAction, uint loop = 1,
            bool autoDestroy = true)
        {
            BaseTimer newTimer = new BaseTimer();
            timerCount++;

            _timerDic.Add(newTimer.tId,newTimer);
            _readyTimerIdList.Add(newTimer.tId);
            
            newTimer.Init(interval, loopAction, completeAction, loop, autoDestroy);

            return newTimer.tId;
        }

        /// <summary>
        /// 新建秒计时器
        /// </summary>
        /// <param name="interval">计时器间隔</param>
        /// <param name="completeAction">计时器完成事件</param>
        /// <param name="autoDestroy">是否自动移除，默认为true</param>
        /// <returns></returns>
        public int AddTimerBySeconds(float interval, Action completeAction, bool autoDestroy = true)
        {
            BaseTimer newTimer = new BaseTimer();
            timerCount++;
            
            _timerDic.Add(newTimer.tId,newTimer);
            _readyTimerIdList.Add(newTimer.tId);
            
            newTimer.Init((uint) (interval * 1000), completeAction, autoDestroy);

            return newTimer.tId;
        }

        /// <summary>
        /// 新建秒计时器
        /// </summary>
        /// <param name="interval">计时器间隔</param>
        /// <param name="loopAction">计时器循环事件</param>
        /// <param name="completeAction">计时器完成事件</param>
        /// <param name="loop">循环次数，默认为1。 0为无限循环</param>
        /// <param name="autoDestroy">是否自动移除，默认为true</param>
        /// <returns></returns>
        public int AddTimerBySeconds(float interval, Action loopAction, Action completeAction, uint loop = 1,
            bool autoDestroy = true)
        {
            BaseTimer newTimer = new BaseTimer();
            timerCount++;

            _timerDic.Add(newTimer.tId,newTimer);
            _readyTimerIdList.Add(newTimer.tId);

            newTimer.Init((uint) Math.Round(interval * 1000), loopAction, completeAction, loop, autoDestroy);

            return newTimer.tId;
        }

        /// <summary>
        /// 删除计时器
        /// </summary>
        /// <param name="tId"></param>
        public void DestroyTimer(int tId)
        {
            if (!_timerDic.ContainsKey(tId))
            {
                Debug.Log($"<color=red>未找到计时器: {tId}</color>");
                return;
            }

            if (_readyTimerIdList.Contains(tId))
            {
                _readyTimerIdList.Remove(tId);
                timerCount--;
            }
            else if (_runningTimerIdList.Contains(tId))
            {
                Stop(tId);
            }
            else if (_sleepTimerIdList.Contains(tId))
            {
                _sleepTimerIdList.Remove(tId);
                timerCount--;
            }
            _timerDic.Remove(tId);
        }

        /// <summary>
        /// 计时器开始
        /// </summary>
        /// <param name="tId"></param>
        public void Start(int tId)
        {
            if (!_readyTimerIdList.Contains(tId))
            {
                Debug.Log($"<color=red>未找到计时器: {tId}</color>");
                return;
            }

            _readyTimerIdList.Remove(tId);
            _runningTimerIdList.Add(tId);
            _timerDic[tId].Start();
            
            dirty = true;
        }

        /// <summary>
        /// 计时器停止
        /// </summary>
        /// <param name="tId"></param>
        public void Stop(int tId)
        {
            if (RemoveRunningTimer(tId, out BaseTimer timer))
            {
                timer.Stop();
                _timerQueue.Remove(timer);
            }
        }

        /// <summary>
        /// 计时器暂停
        /// </summary>
        /// <param name="tId"></param>
        /// <param name="value"></param>
        public void Pause(int tId, bool value)
        {
            if (!_runningTimerIdList.Contains(tId))
            {
                Debug.Log($"<color=red>未找到计时器: {tId}</color>");
                return;
            }
            _timerDic[tId].Pause(value);

            dirty = true;
        }

        /// <summary>
        /// 计时器重新开始
        /// </summary>
        /// <param name="tId"></param>
        public void Restart(int tId)
        {
            if (_runningTimerIdList.Contains(tId))
            {
                _timerDic[tId].Restart();
            }
            else if (_sleepTimerIdList.Contains(tId))
            {
                _sleepTimerIdList.Remove(tId);
                _runningTimerIdList.Add(tId);
                _timerDic[tId].Restart();
            }
            else if (_readyTimerIdList.Contains(tId))
            {
                Start(tId);
            }
            else
            {
                Debug.Log($"<color=red>未找到计时器: {tId}</color>");
            }

            dirty = true;
        }

        public long GetActiveTime(int tId)
        {
            if (_runningTimerIdList.Contains(tId))
            {
                return _timerDic[tId].GetActiveTime();
            }

            return 0;
        }

        public bool TryState(int tId,out TimerState state)
        {
            if (_timerDic.ContainsKey(tId))
            {
                state = _timerDic[tId].state;
                return true;
            }
            state = TimerState.Stop;
            return false;
        }

        private void TimerQueueExecute(out int[] stopIdTimers)
        {
            List<int> stopTimerIdList = new List<int>();
            foreach (var timer in _timerQueue)
            {
                if (timer.GetLoopTimeRemaining() > 0)
                {
                    break;
                }

                timer.Execute();
                dirty = true;
                if (timer.state == TimerState.Stop)
                {
                    stopTimerIdList.Add(timer.tId);
                }
            }
            stopIdTimers = stopTimerIdList.ToArray();
        }
        
        private bool RemoveRunningTimer(int tId, out BaseTimer timer)
        {
            timer = null;
            if (!_runningTimerIdList.Contains(tId))
            {
                return false;
            }
            
            _runningTimerIdList.Remove(tId);
            timer = _timerDic[tId];
            if (!timer.autoDestroy)
            {
                _sleepTimerIdList.Add(timer.tId);
            }
            else
            {
                _timerDic.Remove(tId);
                timerCount--;
            }

            return true;
        }

        /// <summary>
        /// 排序
        /// 按照剩余触发时间的从小到大排序
        /// </summary>
        private void Sort()
        {
            List<BaseTimer> bufferTimerQueue = new List<BaseTimer>();
            foreach (var tId in _runningTimerIdList)
            {
                BaseTimer timer = _timerDic[tId];
                if (timer.state != TimerState.Running)
                {
                    continue;
                }

                bufferTimerQueue.Add(timer);
            }

            bufferTimerQueue.Sort((t1, t2) => t1.GetLoopTimeRemaining().CompareTo(t2.GetLoopTimeRemaining()));
            if (_isAsync)
            {
                lock (_timerQueue)
                {
                    _timerQueue.Clear();
                    _timerQueue.AddRange(bufferTimerQueue);
                }
            }
            else
            {
                _timerQueue.Clear();
                _timerQueue.AddRange(bufferTimerQueue);
            }

            dirty = false;
        }

        /// <summary>
        /// 将标脏后的排序统一放置在帧末
        /// 优化排序次数
        /// 降低一帧内创建多个计时器时多次排序的问题
        /// </summary>
        /// <returns></returns>
        private IEnumerator DirtyCheckOnEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            _owner.StopCoroutine(DirtyCheckOnEndOfFrame());
            if (enable)
            {
                if (dirty)
                {
                    Sort();
                }

                _owner.StartCoroutine(DirtyCheckOnEndOfFrame());
            }
            else
            {
                ClearData();
            }
        }

        private async Task DirtySortAsync()
        {
            while (enable)
            {
                if (dirty)
                {
                    Sort();
                }

                await Task.Delay(_deltaTime);
            }

            ClearData();
        }

        private void ClearData()
        {
            foreach (var tId in _runningTimerIdList)
            {
                _timerDic[tId].Stop();
            }
            _timerDic.Clear();
            _readyTimerIdList.Clear();
            _runningTimerIdList.Clear();
            _sleepTimerIdList.Clear();
            _timerQueue.Clear();

            _timerDic = null;
            _readyTimerIdList = null;
            _runningTimerIdList = null;
            _sleepTimerIdList = null;
            _timerQueue = null;

            _owner = null;
        }
    }
}