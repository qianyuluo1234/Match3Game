using Match3Game.Logic.Element;

namespace Match3Game.Logic.Controller
{
    public class ControllerManager
    {
        private static ControllerManager _instance;
        private static object _lock = new object();
        public static ControllerManager instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ControllerManager();
                        }
                    }
                }

                return _instance;
            }
        }

        private NormalChessController _normalController;
        public IBaseElement selected { get; private set; }
        private ControllerManager()
        {
            _normalController = new NormalChessController();
        }
        
        public void AddListener(BaseChess chess)
        {
            if (chess is NormalChess)
            {
                _normalController.AddListener(chess as NormalChess);
            }
        }

        public void SetSelected(IBaseElement element)
        {
            selected = element;
        }
    }
}