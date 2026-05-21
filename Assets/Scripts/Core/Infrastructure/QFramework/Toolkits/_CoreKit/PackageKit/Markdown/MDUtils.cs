/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace QFramework
{
    public static class MDUtils
    {
        //------------------------------------------------------------------------------
        // path combine with basic normalization (reduces '.' and '..' relative paths)

        private static readonly char[] separators = { '/', '\\' };

        public static string PathCombine(string _a, string _b, string separator = "/")
        {
            string[] a = (_a ?? "").Split(separators, StringSplitOptions.RemoveEmptyEntries);
            string[] b = (_b ?? "").Split(separators, StringSplitOptions.RemoveEmptyEntries);

            IEnumerable<string> combined = a.Concat(b).Where(el => el != ".");

            List<string> path = new();

            foreach (string el in combined)
            {
                if (el != "..")
                {
                    path.Add(el);
                }
                else if (path.Count > 0)
                {
                    path.RemoveAt(path.Count - 1);
                }
            }

            return string.Join(separator, path.ToArray());
        }

        public static string PathNormalise(string _a, string separator = "/")
        {
            string[] a = (_a ?? "").Split(separators, StringSplitOptions.RemoveEmptyEntries);

            List<string> path = new();

            foreach (string el in a)
            {
                if (el == ".")
                {
                    continue;
                }

                if (el != "..")
                {
                    path.Add(el);
                }
                else if (path.Count > 0)
                {
                    path.RemoveAt(path.Count - 1);
                }
            }

            return string.Join(separator, path.ToArray());
        }
    }
}