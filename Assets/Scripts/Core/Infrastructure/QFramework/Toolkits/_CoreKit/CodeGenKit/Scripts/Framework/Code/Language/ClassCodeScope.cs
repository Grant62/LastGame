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
    public class ClassCodeScope : CodeScope
    {
        public ClassCodeScope(string className, string parentClassName, bool isPartial, bool isStatic)
        {
            mClassName = className;
            mParentClassName = parentClassName;
            mIsPartial = isPartial;
            mIsStatic = isStatic;
        }

        private readonly string mClassName;
        private readonly string mParentClassName;
        private readonly bool mIsPartial;
        private readonly bool mIsStatic;

        protected override void GenFirstLine(ICodeWriter codeWriter)
        {
            string staticKey = mIsStatic ? " static" : string.Empty;
            string partialKey = mIsPartial ? " partial" : string.Empty;
            string parentClassKey = !string.IsNullOrEmpty(mParentClassName) ? " : " + mParentClassName : string.Empty;

            codeWriter.WriteLine(string.Format("public{0}{1} class {2}{3}", staticKey, partialKey, mClassName,
                parentClassKey));
        }
    }

    public static partial class ICodeScopeExtensions
    {
        public static ICodeScope Class(this ICodeScope self, string className, string parentClassName, bool isPartial, bool isStatic, Action<ClassCodeScope> classCodeScopeSetting)
        {
            ClassCodeScope classCodeScope = new(className, parentClassName, isPartial, isStatic);
            classCodeScopeSetting(classCodeScope);
            self.Codes.Add(classCodeScope);
            return self;
        }
    }
}