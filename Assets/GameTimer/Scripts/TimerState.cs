namespace GameTools
{
    public enum TimerState
    {
        /// <summary>
        /// new或Reset的状态
        /// 此时计时器未初始化，各项参数均未设置或已清空
        /// </summary>
        Ready = 0,

        /// <summary>
        /// 初始化
        /// 该状态下计时器所需参数已设置完毕
        /// </summary>
        Init,

        /// <summary>
        /// 运行状态
        /// </summary>
        Running,
        Pause,
        Stop
    }
}