/****************************************************************************
 * Copyright (c) 2015 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
namespace QFramework
{
    public class BindInspectorLocale
    {
        public bool CN
        {
            get => LocaleKitEditor.IsCN.Value;
            set => LocaleKitEditor.IsCN.Value = value;
        }

        public string Type { get => CN ? " 类型:" : " Type:"; }

        public string Comment { get => CN ? " 注释" : " Comment"; }

        public string BelongsTo { get => CN ? " 属于:" : " Belongs 2:"; }

        public string Select { get => CN ? "选择" : "Select"; }

        public string Generate { get => CN ? " 生成代码" : " Generate Code"; }

        public string Bind { get => CN ? " 绑定设置" : " Bind Setting"; }

        public string ClassName { get => CN ? "类名" : " Class Name"; }
    }
}
#endif