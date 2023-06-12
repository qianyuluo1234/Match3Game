namespace Match3Game.Core
{
    public interface IElementDetector
    {
        bool TryBuildPreExchangeBlocks(int rowIndex, int columnIndex, int[,] map, out PreCheckBlock block);
     
        bool TryBuildEliminationBlocks(int rowIndex, int columnIndex, int[,] map,
            bool[,] horizontalDirtyMap,
            bool[,] verticalDirtyMap,
            out EliminationBlock block);

        bool TryBuildEliminationBlocksOnExchange(int rowIndex, int columnIndex, int[,] map,
            out EliminationBlock block);

    }
}