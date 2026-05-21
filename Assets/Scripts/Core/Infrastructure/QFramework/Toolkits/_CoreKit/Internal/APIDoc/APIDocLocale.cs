/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
namespace QFramework
{
    public class APIDocLocale
    {
        private static bool IsCN { get => LocaleKitEditor.IsCN.Value; }

        public static string Description { get => IsCN ? "描述" : "Description"; }

        public static string ExampleCode { get => IsCN ? "示例" : "Example"; }

        public static string Methods { get => IsCN ? "方法" : "Methods"; }

        public static string Properties { get => IsCN ? "属性" : "Properties"; }

        public static string Name { get => IsCN ? "名称" : "Name"; }
    }
}
#endif