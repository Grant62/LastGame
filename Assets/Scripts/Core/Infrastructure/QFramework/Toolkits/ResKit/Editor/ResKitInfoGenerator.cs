/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CSharp;
using UnityEditor;

namespace QFramework
{
    public static class ResDataCodeGenerator
    {
        public static void WriteClass(TextWriter writer, string ns)
        {
            List<AssetBundleInfo> assetBundleInfos = new();

            string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (string assetBundleName in assetBundleNames)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);


                assetBundleInfos.Add(new AssetBundleInfo(assetBundleName)
                {
                    assets = assetPaths
                        .Select(assetName => Path.GetFileNameWithoutExtension(assetName))
                        .ToArray()
                });
            }

            CodeCompileUnit compileUnit = new();
            CodeNamespace codeNamespace = new(ns);
            compileUnit.Namespaces.Add(codeNamespace);

            foreach (AssetBundleInfo assetBundleInfo in assetBundleInfos)
            {
                string className = assetBundleInfo.Name;
                string bundleName = className.Substring(0, 1).ToLower() + className.Substring(1);
                if (int.TryParse(bundleName[0].ToString(), out _))
                {
                    continue;
                }

                className = className.Substring(0, 1).ToUpper() +
                            className.Substring(1)
                                .RemoveInvalidateChars();

                CodeTypeDeclaration codeType = new(className);
                codeNamespace.Types.Add(codeType);

                CodeMemberField bundleNameField = new()
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Const,
                    Name = "BundleName",
                    Type = new CodeTypeReference(typeof(string))
                };
                codeType.Members.Add(bundleNameField);
                bundleNameField.InitExpression = new CodePrimitiveExpression(bundleName.ToLowerInvariant());

                Dictionary<string, string> checkRepeatDict = new();
                foreach (string asset in assetBundleInfo.assets)
                {
                    CodeMemberField assetField = new()
                        { Attributes = MemberAttributes.Const | MemberAttributes.Public };

                    string content = Path.GetFileNameWithoutExtension(asset);

                    if (ResKitView.GenerateClassNameStyle == ResKitView.GENERATE_NAME_STYLE_UPPERCASE)
                    {
                        assetField.Name = content.ToUpperInvariant()
                            .RemoveInvalidateChars();
                    }
                    else if (ResKitView.GenerateClassNameStyle == ResKitView.GENERATE_NAME_STYLE_KeepOriginal)
                    {
                        assetField.Name = content.RemoveInvalidateChars();
                    }

                    assetField.Type = new CodeTypeReference(typeof(string));
                    if (!assetField.Name.StartsWith("[") && !assetField.Name.StartsWith(" [") &&
                        !checkRepeatDict.ContainsKey(assetField.Name))
                    {
                        checkRepeatDict.Add(assetField.Name, asset);
                        codeType.Members.Add(assetField);
                    }

                    assetField.InitExpression = new CodePrimitiveExpression(content);
                }

                checkRepeatDict.Clear();
            }

            CSharpCodeProvider provider = new();
            CodeGeneratorOptions options = new()
            {
                BlankLinesBetweenMembers = false,
                BracingStyle = "C"
            };

            provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);
        }

        private static string RemoveInvalidateChars(this string name)
        {
            return name.Replace("/", "")
                .Replace("@", "")
                .Replace("!", "")
                .Replace(" ", "_")
                .Replace("__", "_")
                .Replace("__", "_")
                .Replace("__", "_")
                .Replace("&", "")
                .Replace("-", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("#", "");
        }
    }
}