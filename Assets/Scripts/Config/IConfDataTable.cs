namespace Match3Game.Config
{
    public interface IConfDataTable
    {
        void Parse();
    }

    public interface IConfDataTable<T> : IConfDataTable where T : struct
    {
        T? GetData(int id);
    }
}