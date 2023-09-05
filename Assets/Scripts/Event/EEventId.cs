namespace Match3Game.Event
{
    public enum EEventId
    {
        /// <summary>
        /// 交换
        /// </summary>
        OnElementExchange,
        /// <summary>
        /// 玩家移动棋子开始
        /// </summary>
        OnElementHandleBegin,
        /// <summary>
        /// 玩家移动棋子完毕
        /// </summary>
        OnElementHandleEnd,
        /// <summary>
        /// 当消除结束
        /// </summary>
        OnEliminateFinish,
        /// <summary>
        /// 棋子下落完毕
        /// </summary>
        OnElementFallEnd,
    }
}