using QFramework;
using UnityEngine;

namespace Core.Systems
{
    public interface IPopupStackSystem : ISystem
    {
        void Push(GameObject panel);
        void Remove(GameObject panel);
        bool HandleEsc();
    }
}