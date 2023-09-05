using System;
using System.Diagnostics;

namespace GameTools
{

    public class BaseTimer
    {
        public int tId { get; }
        public bool autoDestroy { get; private set; }
        public TimerState state { get; private set; }

        private Stopwatch _stopwatch;

        /// <summary>
        /// 误差
        /// 由于是在Unity Update中比较剩余时间是否<=0,实际运行中很难出现恰好=的情况
        /// 为了避免在多次循环中的误差累积,在每次循环时记录误差
        /// </summary>
        private int _delta = 0;

        private uint _interval;
        private uint _loop;
        private int _curLoop;

        private Action _loopAction;
        private Action _completeAction;

        private Action onExecute;

        public BaseTimer()
        {
            tId = GetHashCode();
            state = TimerState.Ready;
            autoDestroy = true;
            _stopwatch = new Stopwatch();

        }

        internal void Init(uint interval, Action completeAction, bool autoDestroy = true)
        {
            Init(interval, null, completeAction, 1, autoDestroy);
        }

        internal void Init(uint interval, Action loopAction, Action completeAction, uint loop = 1,
            bool autoDestroy = true)
        {
            _interval = interval;
            _loop = loop;
            _loopAction = loopAction;
            _completeAction = completeAction;
            this.autoDestroy = autoDestroy;

            if (_loop == 0)
            {
                onExecute = LoopUnlimited;
            }
            else
            {
                onExecute = LoopLimited;
            }

            _curLoop = 0;
            state = TimerState.Init;
        }

        public void Start()
        {
            if (TimerState.Init != state)
            {
                UnityEngine.Debug.Log($"timer state is not init. tid:<color=red> {tId}</color>");
                return;
            }

            state = TimerState.Running;
            _stopwatch.Start();
        }

        public void Stop()
        {
            state = TimerState.Stop;
            _stopwatch.Reset();
        }

        public void Pause(bool value)
        {
            if (value)
            {
                state = TimerState.Pause;
                _stopwatch.Stop();
            }
            else
            {
                state = TimerState.Running;
                _stopwatch.Start();
            }
        }

        public void Reset()
        {
            _interval = 0;
            _loop = 0;
            _loopAction = null;
            _completeAction = null;
            autoDestroy = true;
            _curLoop = 0;
            _delta = 0;
            state = TimerState.Ready;
        }

        public void Restart()
        {
            if (state == TimerState.Ready)
            {
                throw new NotSupportedException("Timer not initialized!! ");
            }

            _stopwatch.Reset();
            _curLoop = 0;
            _delta = 0;
            state = TimerState.Init;

            Start();
        }

        public void Execute()
        {
            if (state != TimerState.Running)
            {
                return;
            }

            onExecute.Invoke();
        }

        public long GetLoopTimeRemaining()
        {
            return _interval - _delta - _stopwatch.ElapsedMilliseconds;
        }

        public long GetActiveTime()
        {
            return _stopwatch.ElapsedMilliseconds;
        }

        private void LoopLimited()
        {
            _loopAction?.Invoke();
            _curLoop++;
            if (_curLoop < _loop)
            {
                _delta = (int) (_stopwatch.ElapsedMilliseconds - _interval);
                _stopwatch.Restart();
                return;
            }

            _completeAction?.Invoke();
            Stop();
        }

        private void LoopUnlimited()
        {
            _loopAction?.Invoke();
            _delta = (int) (_stopwatch.ElapsedMilliseconds - _interval);
            _stopwatch.Restart();
        }
    }
}
