using UnityEngine;

namespace QFramework
{
    public static class CodeGenUtil
    {
        public static string GetBindBelongs2(AbstractBind bind)
        {
            Transform trans = bind.Transform;

            while (trans.parent != null)
            {
                if (trans.parent.IsViewController())
                {
                    return trans.parent.name + "(" + trans.parent.GetComponent<ViewController>().ScriptName + ")";
                }

                if (trans.parent.IsUIPanel())
                {
                    return "UIPanel" + "(" + trans.parent.GetComponent<UIPanel>().name + ")";
                }


                trans = trans.parent;
            }

            return trans.name;
        }

        public static GameObject GetBindBelongs2GameObject(AbstractBind bind)
        {
            Transform trans = bind.Transform;

            while (trans.parent != null)
            {
                if (trans.parent.IsViewController() || trans.parent.IsUIPanel())
                {
                    return trans.parent.gameObject;
                }

                trans = trans.parent;
            }

            return bind.gameObject;
        }

        public static string GetLastDirName(string absOrAssetsPath)
        {
            string name = absOrAssetsPath.Replace("\\", "/");
            string[] dirs = name.Split('/');

            return dirs[dirs.Length - 2];
        }

        public static string GenSourceFilePathFromPrefabPath(string uiPrefabPath, string prefabName)
        {
            string strFilePath = string.Empty;

            string prefabDirPattern = UIKitSettingData.Load().UIPrefabDir;

            if (uiPrefabPath.Contains(prefabDirPattern))
            {
                strFilePath = uiPrefabPath.Replace(prefabDirPattern, UIKitSettingData.Load().UIScriptDir);
            }
            else if (uiPrefabPath.Contains("/Resources"))
            {
                strFilePath = uiPrefabPath.Replace("/Resources", UIKitSettingData.Load().UIScriptDir);
            }
            else
            {
                strFilePath = uiPrefabPath.Replace("/" + GetLastDirName(uiPrefabPath), UIKitSettingData.Load().UIScriptDir);
            }

            strFilePath.Replace(prefabName + ".prefab", string.Empty).CreateDirIfNotExists();

            strFilePath = strFilePath.Replace(".prefab", ".cs");

            return strFilePath;
        }
    }
}