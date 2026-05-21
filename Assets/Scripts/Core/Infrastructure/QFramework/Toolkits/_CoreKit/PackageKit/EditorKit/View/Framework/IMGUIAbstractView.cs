/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace QFramework
{
    public abstract class IMGUIAbstractView : IMGUIView
    {
        private bool mVisible = true;

        public string Id { get; set; }

        public bool Visible
        {
            get => VisibleCondition == null ? mVisible : VisibleCondition();
            set => mVisible = value;
        }

        public Func<bool> VisibleCondition { get; set; }

        private List<GUILayoutOption> mLayoutOptions { get; } = new();

        protected GUILayoutOption[] LayoutStyles { get; private set; }


        protected FluentGUIStyle mStyle = new(() => new GUIStyle());

        public FluentGUIStyle Style
        {
            get => mStyle;
            protected set => mStyle = value;
        }

        public Color BackgroundColor { get; set; } = GUI.backgroundColor;

        public void RefreshNextFrame()
        {
            PushCommand(Refresh);
        }

        public void AddLayoutOption(GUILayoutOption option)
        {
            mLayoutOptions.Add(option);
        }

        public void Show()
        {
            Visible = true;
            OnShow();
        }

        protected virtual void OnShow() { }

        public void Hide()
        {
            Visible = false;
            OnHide();
        }

        protected virtual void OnHide() { }


        private Color mPreviousBackgroundColor;

        public void DrawGUI()
        {
            BeforeDraw();

            if (Visible)
            {
                mPreviousBackgroundColor = GUI.backgroundColor;
                GUI.backgroundColor = BackgroundColor;
                OnGUI();
                GUI.backgroundColor = mPreviousBackgroundColor;
            }

            if (mCommands.Count > 0)
            {
                mCommands.Dequeue().Invoke();
            }
        }

        protected void PushCommand(Action command)
        {
            mCommands.Enqueue(command);
        }

        private readonly Queue<Action> mCommands = new();

        private bool mBeforeDrawCalled;

        private void BeforeDraw()
        {
            if (!mBeforeDrawCalled)
            {
                OnBeforeDraw();

                LayoutStyles = mLayoutOptions.ToArray();

                mBeforeDrawCalled = true;
            }
        }

        protected virtual void OnBeforeDraw() { }

        public IMGUILayout Parent { get; set; }

        public void RemoveFromParent()
        {
            Parent.RemoveChild(this);
        }

        public virtual void Refresh()
        {
            OnRefresh();
        }

        protected virtual void OnRefresh() { }

        protected abstract void OnGUI();

        public void Dispose()
        {
            OnDisposed();
        }

        protected virtual void OnDisposed() { }
    }
}