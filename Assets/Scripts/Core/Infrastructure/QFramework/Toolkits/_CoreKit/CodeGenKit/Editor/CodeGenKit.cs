/****************************************************************************
 * Copyright (c) 2016 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.Collections.Generic;

#if UNITY_EDITOR
namespace QFramework
{
    public class CodeGenKit : Architecture<CodeGenKit>
    {
        private static readonly Dictionary<string, ICodeGenTemplate> mTemplates = new();

        public static void RegisterTemplate(string templateName, ICodeGenTemplate codeGenTemplate)
        {
            if (mTemplates.ContainsKey(templateName))
            {
                mTemplates[templateName] = codeGenTemplate;
            }
            else
            {
                mTemplates.Add(templateName, codeGenTemplate);
            }
        }

        public static ICodeGenTemplate GetTemplate(string templateName)
        {
            return mTemplates.TryGetValue(templateName, out ICodeGenTemplate template) ? template : null;
        }

        protected override void Init() { }

        public static void Generate(IBindGroup bindGroup)
        {
            CodeGenTask task = GetTemplate(bindGroup.TemplateName).CreateTask(bindGroup);
            Generate(task);
        }

        public static void Generate(CodeGenTask task)
        {
            CodeGenKitPipeline.Default.Generate(task);
        }

        public static CodeGenKitSetting Setting { get => CodeGenKitSetting.Load(); }
    }
}
#endif