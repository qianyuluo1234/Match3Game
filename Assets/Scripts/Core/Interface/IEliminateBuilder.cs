namespace Match3Game.Core
{
    public interface IEliminateBuilder
    {
        bool TryBuildEliminationBlocks(int rowIndex, int columnIndex, int[,] map,
            bool[,] horizontalDirtyMap,
            bool[,] verticalDirtyMap,
            out EliminationBlocks blocks);

        bool TryBuildEliminationBlocksOnExchange(int rowIndex, int columnIndex, int[,] map,
            out EliminationBlocks blocks);
    }
}