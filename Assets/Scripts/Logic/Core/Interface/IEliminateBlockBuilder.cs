namespace Match3Game.Logic.Core
{
    public interface IEliminateBlockBuilder
    {
        bool TryBuildPreCheckBlocks(int rowIndex, int columnIndex, Match3MapData map, out PreCheckBlock block);
        
        bool TryBuildEliminationBlocks(int rowIndex, int columnIndex, Match3MapData map,
            bool[,] horizontalDirtyMap,
            bool[,] verticalDirtyMap,
            out EliminationBlock block);
        bool TryBuildEliminationBlocksOnExchange(int rowIndex, int columnIndex, Match3MapData map,
            out EliminationBlock block);
        
    }
}