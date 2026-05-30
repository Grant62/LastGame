using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace QFramework
{
    public class ConfigFileUtility
    {
        public static ResDatas BuildEditorDataTable()
        {
            ResDatas resDatas = new();
            AddABInfo2ResDatas(resDatas);
            return resDatas;
        }

        public static void AddABInfo2ResDatas(IResDatas assetBundleConfigFile, string[] abNames = null)
        {
#if UNITY_EDITOR
            AssetDatabase.RemoveUnusedAssetBundleNames();

            string[] assetBundleNames = abNames ?? AssetDatabase.GetAllAssetBundleNames();
            foreach (string abName in assetBundleNames)
            {
                string[] depends = AssetDatabase.GetAssetBundleDependencies(abName, false);
                AssetDataGroup group;
                int abIndex = assetBundleConfigFile.AddAssetBundleName(abName, depends, out group);
                if (abIndex < 0)
                {
                    continue;
                }

                string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(abName);
                foreach (string cell in assets)
                {
                    Type type = AssetDatabase.GetMainAssetTypeAtPath(cell);

                    short code = type.ToCode();

                    group.AddAssetData(cell.EndsWith(".unity")
                        ? new AssetData(AssetPath2Name(cell), ResLoadType.ABScene, abIndex, abName, code)
                        : new AssetData(AssetPath2Name(cell), ResLoadType.ABAsset, abIndex, abName, code));
                }
            }
#endif
        }

        public static string AssetPath2Name(string assetPath)
        {
            int startIndex = assetPath.LastIndexOf("/") + 1;

            int endIndex = assetPath.LastIndexOf(".");
            if (endIndex > 0)
            {
                int length = endIndex - startIndex;
                return assetPath.Substring(startIndex, length).ToLower();
            }

            return assetPath.Substring(startIndex).ToLower();
        }
    }
}