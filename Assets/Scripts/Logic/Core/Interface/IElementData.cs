using Match3Game.Config;

namespace Match3Game.Logic.Core
{
    public interface IElementData
    {
        int value { get; }
        int rowIndex { get; }
        int columnIndex { get; }
        
        int fallValue { get; }
        /// <summary>
        /// 允许下落
        /// </summary>
        bool allowFall { get; }
        /// <summary>
        /// 允许构建消除块
        /// </summary>
        bool allowBuildEliminationBlock { get; }
        /// <summary>
        /// 允许控制
        /// </summary>
        bool allowPlayerControl { get; }
        public ConfElementsData confData { get; }

        void SetFallValue(int targetValue);
        /// <summary>
        /// 设置元素索引位置
        /// </summary>
        /// <param name="newRowIndex"></param>
        /// <param name="newColumnIndex"></param>
        void SetIndex(int newRowIndex, int newColumnIndex);

        //以下四个接口函数均为处理元素与当前位置的格子相关的函数
        //上面属性为元素自身的性质，下面的函数则是元素在当前位置时的表现的属性
        /// <summary>
        /// 玩家能否控制
        /// </summary>
        /// <returns></returns>
        bool CanPlayerControl();
        /// <summary>
        /// 能否消除
        /// </summary>
        /// <returns></returns>
        bool CanEliminate();

        bool CanFall();
        /// <summary>
        /// 能否参与消除可行性检测
        /// </summary>
        /// <returns></returns>
        bool CanBuildEliminationBlock();
    }
}