/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System;

namespace QFramework
{
    public class CustomCodeScope : CodeScope
    {
        private string mFirstLine { get; }

        public CustomCodeScope(string firstLine)
        {
            mFirstLine = firstLine;
        }

        protected override void GenFirstLine(ICodeWriter codeWriter)
        {
            codeWriter.WriteLine(mFirstLine);
        }
    }

    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope CustomScope(this ICodeScope self, string firstLine, bool semicolon, Action<CustomCodeScope> customCodeScopeSetting)
        {
            CustomCodeScope custom = new(firstLine);
            custom.Semicolon = semicolon;
            customCodeScopeSetting(custom);
            self.Codes.Add(custom);
            return self;
        }
    }
}