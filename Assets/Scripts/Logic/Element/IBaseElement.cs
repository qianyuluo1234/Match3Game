using Match3Game.Logic.Core;
using UnityEngine;

namespace Match3Game.Logic.Element
{
    public interface IBaseElement
    {
        public IElementData data { get; }
        public GameObject gameObject { get; }

        public void Init(IElementData data);
    }
}