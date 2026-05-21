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
    public class ViewControllerInspectorLocale
    {
        public bool CN
        {
            get => LocaleKitEditor.IsCN.Value;
            set => LocaleKitEditor.IsCN.Value = value;
        }

        public string CodegenPart { get => CN ? " 代码生成设置" : " Code Generate Setting"; }

        public string Namespace { get => CN ? "命名空间:" : "Namespace :"; }

        public string ScriptName { get => CN ? "生成脚本名:" : "Script name:"; }

        public string ScriptsFolder { get => CN ? "脚本生成目录:" : "Scripts Generate Folder:"; }

        public string GeneratePrefab { get => CN ? "生成 Prefab" : "Generate Prefab"; }

        public string PrefabGenerateFolder { get => CN ? "Prefab 生成目录:" : "Prefab Generate Folder:"; }

        public string OpenScript { get => CN ? " 打开脚本" : "Open Script File"; }

        public string SelectScript { get => CN ? " 选择脚本" : "Select Script File"; }

        public string Generate { get => CN ? " 生成代码" : " Generate Code"; }

        public string AddOtherBinds { get => CN ? " 添加 Other Binds" : " Add Other Binds"; }

        public string DragDescription { get => CN ? "请将要生成脚本的文件夹拖到下边区域 或 自行填写目录到上一栏中" : "drag file or folder below or write in above"; }

        public string PrefabDragDescription { get => CN ? "请将要生成 Prefab 的文件夹拖到下边区域 或 自行填写目录到上一栏中" : "drag file or folder below or write in above"; }
    }
}
#endif