/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace QFramework
{
    // https://gist.github.com/Thundernerd/5085ec29819b2960f5ff2ee32ad57cbb
    public static class Docker
    {
        #region Reflection Types
        private class _EditorWindow
        {
            private readonly EditorWindow instance;
            private readonly Type type;

            public _EditorWindow(EditorWindow instance)
            {
                this.instance = instance;
                type = instance.GetType();
            }

            public object m_Parent
            {
                get
                {
                    FieldInfo field = type.GetField("m_Parent", BindingFlags.Instance | BindingFlags.NonPublic);
                    return field.GetValue(instance);
                }
            }
        }

        private class _DockArea
        {
            private readonly object instance;
            private readonly Type type;

            public _DockArea(object instance)
            {
                this.instance = instance;
                type = instance.GetType();
            }

            public object window
            {
                get
                {
                    PropertyInfo property = type.GetProperty("window", BindingFlags.Instance | BindingFlags.Public);
                    return property.GetValue(instance, null);
                }
            }

            public object s_OriginalDragSource
            {
                set
                {
                    FieldInfo field = type.GetField("s_OriginalDragSource", BindingFlags.Static | BindingFlags.NonPublic);
                    field.SetValue(null, value);
                }
            }

            public void AddTab(EditorWindow child)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                MethodInfo method = methods.FirstOrDefault(c =>
                {
                    return
                        c.Name == "AddTab" &&
                        c.GetParameters()[0].ParameterType == typeof(EditorWindow);
                });
                method.Invoke(instance, new object[] { child, true });
            }

            public void RemoveTab(EditorWindow pane)
            {
                MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                MethodInfo method = methods.FirstOrDefault(c =>
                {
                    return
                        c.Name == "RemoveTab" &&
                        c.GetParameters().Count() == 1;
                });
                method.Invoke(instance, new object[] { pane });
            }
        }

        private class _ContainerWindow
        {
            private readonly object instance;
            private readonly Type type;

            public _ContainerWindow(object instance)
            {
                this.instance = instance;
                type = instance.GetType();
            }


            public object rootSplitView
            {
                get
                {
                    PropertyInfo property = type.GetProperty("rootSplitView", BindingFlags.Instance | BindingFlags.Public);
                    return property.GetValue(instance, null);
                }
            }
        }

        private class _SplitView
        {
            private readonly object instance;
            private readonly Type type;

            public _SplitView(object instance)
            {
                this.instance = instance;
                type = instance.GetType();
            }

            public object DragOver(EditorWindow child, Vector2 screenPoint)
            {
                MethodInfo method = type.GetMethod("DragOver", BindingFlags.Instance | BindingFlags.Public);
                return method.Invoke(instance, new object[] { child, screenPoint });
            }

            public void PerformDrop(EditorWindow child, object dropInfo, Vector2 screenPoint)
            {
                MethodInfo method = type.GetMethod("PerformDrop", BindingFlags.Instance | BindingFlags.Public);
                method.Invoke(instance, new[] { child, dropInfo, screenPoint });
            }
        }
        #endregion

        public enum DockPosition
        {
            Left,
            Top,
            Right,
            Bottom
        }

        /// <summary>
        ///     Docks the second window to the first window at the given position
        /// </summary>
        public static void Dock(this EditorWindow wnd, EditorWindow other, DockPosition position)
        {
            Vector2 mousePosition = GetFakeMousePosition(wnd, position);

            _EditorWindow parent = new(wnd);
            _EditorWindow child = new(other);
            _DockArea dockArea = new(parent.m_Parent);
            _ContainerWindow containerWindow = new(dockArea.window);
            _SplitView splitView = new(containerWindow.rootSplitView);
            object dropInfo = splitView.DragOver(other, mousePosition);
            dockArea.s_OriginalDragSource = child.m_Parent;
            splitView.PerformDrop(other, dropInfo, mousePosition);
        }

        /// <summary>
        ///     Docks the second window to the first window as a tab
        /// </summary>
        public static void AddTab(this EditorWindow wnd, EditorWindow other)
        {
            _EditorWindow parent = new(wnd);
            _EditorWindow child = new(other);
            _DockArea dockArea = new(parent.m_Parent);
            _DockArea childDockArea = new(child.m_Parent);
            childDockArea.RemoveTab(other);
            dockArea.AddTab(other);
        }

        private static Vector2 GetFakeMousePosition(EditorWindow wnd, DockPosition position)
        {
            Vector2 mousePosition = Vector2.zero;

            // The 20 is required to make the docking work.
            // Smaller values might not work when faking the mouse position.
            switch (position)
            {
                case DockPosition.Left:
                    mousePosition = new Vector2(20, wnd.position.size.y / 2);
                    break;
                case DockPosition.Top:
                    mousePosition = new Vector2(wnd.position.size.x / 2, 20);
                    break;
                case DockPosition.Right:
                    mousePosition = new Vector2(wnd.position.size.x - 20, wnd.position.size.y / 2);
                    break;
                case DockPosition.Bottom:
                    mousePosition = new Vector2(wnd.position.size.x / 2, wnd.position.size.y - 20);
                    break;
            }

            return GUIUtility.GUIToScreenPoint(mousePosition);
        }
    }
}
#endif