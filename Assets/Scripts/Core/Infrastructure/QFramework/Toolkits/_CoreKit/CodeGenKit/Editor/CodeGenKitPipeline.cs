/****************************************************************************
 * Copyright (c) 2015 ~ 2023 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace QFramework
{
    public class CodeGenKitPipeline : ScriptableObject
    {
        private static CodeGenKitPipeline mInstance;

        public static CodeGenKitPipeline Default
        {
            get
            {
                if (mInstance) return mInstance;

                string filePath = Dir.Value + FileName;

                if (File.Exists(filePath))
                {
                    return mInstance = AssetDatabase.LoadAssetAtPath<CodeGenKitPipeline>(filePath);
                }

                return mInstance = CreateInstance<CodeGenKitPipeline>();
            }
        }

        public void Save()
        {
            string filePath = Dir.Value + FileName;

            if (!File.Exists(filePath))
            {
                AssetDatabase.CreateAsset(this, Dir.Value + FileName);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static readonly Lazy<string> Dir = new(() => "Assets/QFrameworkData/CodeGenKit/".CreateDirIfNotExists());

        private const string FileName = "Pipeline.asset";

        [SerializeField] public CodeGenTask CurrentTask;

        public void Generate(CodeGenTask task)
        {
            CurrentTask = task;

            CurrentTask.Status = CodeGenTaskStatus.Search;
            BindSearchHelper.Search(task);
            CurrentTask.Status = CodeGenTaskStatus.Gen;


            // var writer = File.CreateText(scriptFile);

            StringBuilder writer = new();
            writer.AppendLine("using UnityEngine;");
            writer.AppendLine("using QFramework;");

            if (CodeGenKit.Setting.IsDefaultNamespace)
            {
                writer.AppendLine("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                writer.AppendLine("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
            }

            writer.AppendLine(
                $"namespace {(string.IsNullOrWhiteSpace(task.Namespace) ? CodeGenKit.Setting.Namespace : task.Namespace)}");
            writer.AppendLine("{");
            writer.AppendLine($"\tpublic partial class {task.ClassName} : ViewController");
            writer.AppendLine("\t{");
            writer.AppendLine("\t\tvoid Start()");
            writer.AppendLine("\t\t{");
            writer.AppendLine("\t\t\t// Code Here");
            writer.AppendLine("\t\t}");
            writer.AppendLine("\t}");
            writer.AppendLine("}");

            task.MainCode = writer.ToString();
            writer.Clear();

            writer.AppendLine($"// Generate Id:{Guid.NewGuid().ToString()}");
            writer.AppendLine("using UnityEngine;");

            if (CodeGenKit.Setting.IsDefaultNamespace)
            {
                writer.AppendLine("// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间");
                writer.AppendLine("// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改");
            }

            writer.AppendLine(
                $"namespace {(string.IsNullOrWhiteSpace(task.Namespace) ? CodeGenKit.Setting.Namespace : task.Namespace)}");
            writer.AppendLine("{");
            writer.AppendLine($"\tpublic partial class {task.ClassName}");
            writer.AppendLine("\t{");

            foreach (BindInfo bindData in task.BindInfos)
            {
                if (bindData.BindScript.Comment.IsNotNullAndEmpty())
                {
                    writer.AppendLine("\t\t/// <summary>");
                    foreach (string comment in bindData.BindScript.Comment.Split('\n'))
                    {
                        writer.AppendLine($"\t\t/// {comment}");
                    }

                    writer.AppendLine("\t\t/// </summary>");
                }

                writer.AppendLine($"\t\tpublic {bindData.TypeName} {bindData.MemberName};");
            }

            if (task.GameObject.GetComponent<OtherBinds>())
            {
                OtherBinds referenceBinds = task.GameObject.GetComponent<OtherBinds>();
                foreach (OtherBind referenceBind in referenceBinds.Binds)
                {
                    writer.AppendLine($"\t\tpublic {referenceBind.Object.GetType().FullName} {referenceBind.MemberName};");
                }
            }

            writer.AppendLine("\t}");
            writer.AppendLine("}");
            task.DesignerCode = writer.ToString();
            writer.Clear();


            string scriptFile = string.Format(task.ScriptsFolder + "/{0}.cs", task.ClassName);

            if (!File.Exists(scriptFile))
            {
                scriptFile.GetFolderPath().CreateDirIfNotExists();
                File.WriteAllText(scriptFile, CurrentTask.MainCode);
            }


            scriptFile = string.Format(task.ScriptsFolder + "/{0}.Designer.cs", task.ClassName);
            File.WriteAllText(scriptFile, CurrentTask.DesignerCode);

            Save();

            CurrentTask.Status = CodeGenTaskStatus.Compile;
        }

        private void OnCompile()
        {
            if (CurrentTask == null) return;
            if (CurrentTask.Status == CodeGenTaskStatus.Compile)
            {
                string generateClassName = CurrentTask.ClassName;
                string generateNamespace = CurrentTask.Namespace;

                IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly =>
                    !assembly.FullName.StartsWith("Unity"));

                string typeName = generateNamespace + "." + generateClassName;

                Type type = assemblies.Where(a => a.GetType(typeName) != null)
                    .Select(a => a.GetType(typeName)).FirstOrDefault();

                if (type == null)
                {
                    Debug.Log("编译失败");
                    return;
                }

                Debug.Log(type);

                GameObject gameObject = CurrentTask.GameObject;

                Component scriptComponent = gameObject.GetComponent(type);

                if (!scriptComponent)
                {
                    scriptComponent = gameObject.AddComponent(type);
                }

                SerializedObject serializedObject = new(scriptComponent);

                foreach (BindInfo bindInfo in CurrentTask.BindInfos)
                {
                    string componentName = bindInfo.TypeName.Split('.').Last();
                    SerializedProperty serializedProperty = serializedObject.FindProperty(bindInfo.MemberName);
                    Component component = gameObject.transform.Find(bindInfo.PathToRoot).GetComponent(componentName);

                    if (!component)
                    {
                        component = gameObject.transform.Find(bindInfo.PathToRoot).GetComponent(bindInfo.TypeName);
                    }

                    serializedProperty.objectReferenceValue = component;
                }

                OtherBinds referenceBinds = gameObject.GetComponent<OtherBinds>();
                if (referenceBinds)
                {
                    foreach (OtherBind bind in referenceBinds.Binds)
                    {
                        SerializedProperty serializedProperty = serializedObject.FindProperty(bind.MemberName);
                        serializedProperty.objectReferenceValue = bind.Object;
                    }
                }

                ViewController codeGenerateInfo = gameObject.GetComponent<ViewController>();

                if (codeGenerateInfo)
                {
                    serializedObject.FindProperty("ScriptsFolder").stringValue = codeGenerateInfo.ScriptsFolder;
                    serializedObject.FindProperty("PrefabFolder").stringValue = codeGenerateInfo.PrefabFolder;
                    serializedObject.FindProperty("GeneratePrefab").boolValue = codeGenerateInfo.GeneratePrefab;
                    serializedObject.FindProperty("ScriptName").stringValue = codeGenerateInfo.ScriptName;
                    serializedObject.FindProperty("Namespace").stringValue = codeGenerateInfo.Namespace;

                    bool generatePrefab = codeGenerateInfo.GeneratePrefab;
                    string prefabFolder = codeGenerateInfo.PrefabFolder;

                    if (codeGenerateInfo.GetType() != type)
                    {
                        DestroyImmediate(codeGenerateInfo, true);
                    }

                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();

                    if (generatePrefab)
                    {
                        prefabFolder.CreateDirIfNotExists();

                        string generatePrefabPath = prefabFolder + "/" + gameObject.name + ".prefab";

                        if (File.Exists(generatePrefabPath))
                        {
                            // PrefabUtility.SavePrefabAsset(gameObject);
                        }
                        else
                        {
                            PrefabUtils.SaveAndConnect(generatePrefabPath, gameObject);
                        }
                    }
                }
                else
                {
                    serializedObject.FindProperty("ScriptsFolder").stringValue = "Assets/Scripts";
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }

                EditorUtility.SetDirty(gameObject);

                // EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

                CurrentTask.Status = CodeGenTaskStatus.Complete;
                CurrentTask = null;
            }
        }

        [DidReloadScripts]
        private static void Compile()
        {
            Default.OnCompile();
        }
    }
}
#endif