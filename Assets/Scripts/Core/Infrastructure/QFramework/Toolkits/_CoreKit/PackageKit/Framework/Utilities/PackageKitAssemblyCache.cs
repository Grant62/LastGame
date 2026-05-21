/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QFramework
{
    public static class PackageKitAssemblyCache
    {
        private static readonly Lazy<List<Assembly>> mCachedAssemblies = new(() =>
        {
            List<Assembly> cachedAssemblies = new();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith("QF") || assembly.FullName.Contains("Kit") ||
                    assembly.FullName.StartsWith("Assembly-CSharp"))
                {
                    if (!cachedAssemblies.Contains(assembly))
                    {
                        cachedAssemblies.Add(assembly);
                    }
                }
            }

            return cachedAssemblies;
        });


        public static IEnumerable<Type> GetAllTypes()
        {
            return mCachedAssemblies.Value.SelectMany(a => a.GetTypes());
        }


        public static IEnumerable<Type> GetDerivedTypes<T>(bool includeAbstract = false, bool includeBase = true)
        {
            Type type = typeof(T);
            if (includeBase)
                yield return type;
            if (includeAbstract)
            {
                foreach (Type t in mCachedAssemblies.Value.SelectMany(assembly => assembly
                             .GetTypes()
                             .Where(x => type.IsAssignableFrom(x))))
                {
                    yield return t;
                }
            }
            else
            {
                List<Type> items = new();
                foreach (Assembly assembly in mCachedAssemblies.Value)
                {
                    try
                    {
                        items.AddRange(assembly.GetTypes()
                            .Where(x => type.IsAssignableFrom(x) && !x.IsAbstract));
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                }

                foreach (Type item in items)
                    yield return item;
            }
        }
    }
}