/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace QFramework
{
    [CreateAssetMenu(menuName = "@QPM/SingleFileCreator")]
    public class SingleFileCreator : ScriptableObject
    {
        [TextArea] public string LicenseCode = "liangxie";

        public string FileName;

        public MonoScript[] Scripts;

        public MonoScript[] DependencyScripts;

        [HideInInspector] public string OutputFilePath = "Assets/";
    }

    [CustomEditor(typeof(SingleFileCreator))]
    public class SingleFileCreatorInspector : Editor
    {
        private SerializedProperty mOutputFilepathProperty;

        private void OnEnable()
        {
            mOutputFilepathProperty = serializedObject.FindProperty("OutputFilePath");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();

            GUILayout.Label(mOutputFilepathProperty.stringValue);

            if (GUILayout.Button("..."))
            {
                mOutputFilepathProperty.stringValue =
                    EditorUtility.OpenFolderPanel("Select Folder", mOutputFilepathProperty.stringValue, "");
            }


            GUILayout.EndHorizontal();

            if (GUILayout.Button("Open Output Folder"))
            {
                EditorUtility.RevealInFinder(mOutputFilepathProperty.stringValue);
            }

            if (GUILayout.Button("Create"))
            {
                string folderPath = mOutputFilepathProperty.stringValue;
                SingleFileCreator singleFileCreator = target.As<SingleFileCreator>();
                string codeFilePath = folderPath.CombinePath(singleFileCreator.FileName);

                HashSet<string> namespaces = new()
                {
                    $"using QFramework.{singleFileCreator.FileName.GetFileNameWithoutExtend()}SingleFile.Dependency.Internal;"
                };

                List<string> codeLines = new();

                foreach (MonoScript monoScript in singleFileCreator.Scripts)
                {
                    foreach (string codeLine in monoScript.text.Split('\n'))
                    {
                        string codeLineTrim = codeLine.Trim();

                        if (codeLineTrim.StartsWith("using "))
                        {
                            namespaces.Add(codeLineTrim);
                        }
                        else if (codeLineTrim.StartsWith("/***") || codeLineTrim.StartsWith("*") ||
                                 codeLineTrim.StartsWith("****"))
                        {
                            // continue
                        }
                        else
                        {
                            codeLines.Add(codeLine);
                        }
                    }
                }

                foreach (MonoScript monoScript in singleFileCreator.DependencyScripts)
                {
                    foreach (string codeLine in monoScript.text.Split('\n'))
                    {
                        string codeLineTrim = codeLine.Trim();

                        if (codeLineTrim.StartsWith("using "))
                        {
                            namespaces.Add(codeLineTrim);
                        }
                        else if (codeLineTrim.StartsWith("/***") || codeLineTrim.StartsWith("*") ||
                                 codeLineTrim.StartsWith("****"))
                        {
                            // continue
                        }
                        else if (codeLineTrim.StartsWith("namespace QFramework"))
                        {
                            codeLines.Add(
                                $"namespace QFramework.{singleFileCreator.FileName.GetFileNameWithoutExtend()}SingleFile.Dependency.Internal");
                        }
                        else if ((codeLineTrim.StartsWith("public class") ||
                                  codeLineTrim.StartsWith("public static class"))
                                 && !codeLineTrim.Contains("EasyEvent")
                                 && !codeLineTrim.Contains("TableIndex")
                                )
                        {
                            codeLines.Add(codeLine.Replace("public", "internal"));
                        }
                        else
                        {
                            codeLines.Add(codeLine);
                        }
                    }
                }

                codeFilePath.DeleteFileIfExists();

                File.WriteAllLines(codeFilePath,
                    new[] { singleFileCreator.LicenseCode, "" }.Concat(namespaces).Concat(codeLines));

                AssetDatabase.Refresh();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif