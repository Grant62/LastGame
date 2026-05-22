/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2015 - 2023 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;

namespace QFramework
{
    public class EnumEventSystem
    {
        public static readonly EnumEventSystem Global = new();

        private readonly Dictionary<int, IEasyEvent> mEvents = new(50);

        protected EnumEventSystem() { }

        #region 功能函数
        public IUnRegister Register<T>(T key, Action<int, object[]> onEvent) where T : IConvertible
        {
            int kv = key.ToInt32(null);

            if (mEvents.TryGetValue(kv, out IEasyEvent e))
            {
                EasyEvent<int, object[]> easyEvent = e.As<EasyEvent<int, object[]>>();
                return easyEvent.Register(onEvent);
            }
            else
            {
                EasyEvent<int, object[]> easyEvent = new();
                mEvents.Add(kv, easyEvent);
                return easyEvent.Register(onEvent);
            }
        }

        public IUnRegister Register<T>(T key, Action onEvent) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            if (mEvents.TryGetValue(kv, out IEasyEvent e) && e is EasyEvent easyEvent)
            {
                return easyEvent.Register(onEvent);
            }

            EasyEvent newEvent = new();
            mEvents.Add(kv, newEvent);
            return newEvent.Register(onEvent);
        }

        public IUnRegister Register<T, T1>(T key, Action<T1> onEvent) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            if (mEvents.TryGetValue(kv, out IEasyEvent e) && e is EasyEvent<T1> typed)
            {
                return typed.Register(onEvent);
            }

            EasyEvent<T1> newEvent = new();
            mEvents.Add(kv, newEvent);
            return newEvent.Register(onEvent);
        }

        public IUnRegister Register<T, T1, T2>(T key, Action<T1, T2> onEvent) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            if (mEvents.TryGetValue(kv, out IEasyEvent e) && e is EasyEvent<T1, T2> typed)
            {
                return typed.Register(onEvent);
            }

            EasyEvent<T1, T2> newEvent = new();
            mEvents.Add(kv, newEvent);
            return newEvent.Register(onEvent);
        }

        public void UnRegister<T>(T key, Action<int, object[]> onEvent) where T : IConvertible
        {
            int kv = key.ToInt32(null);

            if (mEvents.TryGetValue(kv, out IEasyEvent e))
            {
                e.As<EasyEvent<int, object[]>>()?.UnRegister(onEvent);
            }
        }

        public void UnRegister<T>(T key) where T : IConvertible
        {
            int kv = key.ToInt32(null);

            if (mEvents.ContainsKey(kv))
            {
                mEvents.Remove(kv);
            }
        }

        public void UnRegisterAll()
        {
            mEvents.Clear();
        }

        public void Send<T>(T key, params object[] args) where T : IConvertible
        {
            int kv = key.ToInt32(null);

            if (mEvents.TryGetValue(kv, out IEasyEvent e))
            {
                e.As<EasyEvent<int, object[]>>().Trigger(kv, args);
            }
        }

        public void Send<T>(T key) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            if (mEvents.TryGetValue(kv, out IEasyEvent e) && e is EasyEvent easyEvent)
            {
                easyEvent.Trigger();
            }
        }

        public void Send<T, T1>(T key, T1 arg1) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            if (mEvents.TryGetValue(kv, out IEasyEvent e) && e is EasyEvent<T1> typed)
            {
                typed.Trigger(arg1);
            }
        }

        public void Send<T, T1, T2>(T key, T1 arg1, T2 arg2) where T : IConvertible
        {
            int kv = key.ToInt32(null);
            if (mEvents.TryGetValue(kv, out IEasyEvent e) && e is EasyEvent<T1, T2> typed)
            {
                typed.Trigger(arg1, arg2);
            }
        }
        #endregion
    }

    [Obsolete("请使用 EnumEventSystem,Please use EnumEventSystem instead", APIVersion.Force)]
    public class QEventSystem : EnumEventSystem
    {
        protected QEventSystem() { }

        [Obsolete("请使用 Global,Please use Global instead", APIVersion.Force)]
        public static EnumEventSystem Instance { get => Global; }


        [Obsolete("请使用 Global.Send,Please use Global.Send instead", APIVersion.Force)]
        public static void SendEvent<T>(T key, params object[] param) where T : IConvertible
        {
            Global.Send(key, param);
        }

        [Obsolete("请使用 Global.Register,Please use Global.Register instead", APIVersion.Force)]
        public static void RegisterEvent<T>(T key, Action<int, object[]> fun) where T : IConvertible
        {
            Global.Register(key, fun);
        }

        [Obsolete("请使用 Global.UnRegister,Please use Global.UnRegister instead", APIVersion.Force)]
        public static void UnRegisterEvent<T>(T key, Action<int, object[]> fun) where T : IConvertible
        {
            Global.UnRegister(key, fun);
        }
    }
}