using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Core.Systems
{
    public class PopupStackSystem : AbstractSystem, IPopupStackSystem
    {
        private readonly Stack<GameObject> mStack = new();

        public void Push(GameObject panel)
        {
            mStack.Push(panel);
        }

        public void Remove(GameObject panel)
        {
            if (mStack.Count > 0 && mStack.Peek() == panel)
                mStack.Pop();
        }

        public bool HandleEsc()
        {
            if (mStack.Count == 0)
                return false;

            mStack.Pop().SetActive(false);
            return true;
        }

        protected override void OnInit() { }
    }
}