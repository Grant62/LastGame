using QFramework;
using UnityEngine;

namespace Features.Combat.System
{
    public interface IPopupStackSystem : ISystem
    {
        void Push(GameObject panel);
        void Remove(GameObject panel);
        bool HandleEsc();
    }
}