namespace Match3Game.Config
{
    public enum ConfCellStateEnum
    {
        Normal = 0,
        /// <summary>
        /// 环境格子，表示该格子上不存放元素，不参与游戏
        /// </summary>
        Env,
        Ice,
    }
}