namespace Match3Game.Core
{
    public class Match3Data
    {
        public int[] types;
        public int row { get; }
        public int column { get; }
        public int[,] map;

        public Match3Data(int row, int column, int[] types)
        {
            this.row = row;
            this.column = column;
            this.types = types;
        }
    }
}