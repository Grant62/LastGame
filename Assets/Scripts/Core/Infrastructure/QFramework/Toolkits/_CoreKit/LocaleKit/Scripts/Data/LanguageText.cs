/****************************************************************************
 * Copyright (c) 2015 - 2022 liangxiegame UNDER MIT License
 *
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/


using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace QFramework
{
    [Serializable]
    public class LanguageText
    {
        [SerializeField] public int LanguageIndex;
        [HideInInspector] public Language Language;

        [TextArea(1, int.MaxValue)] public string Text;
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LanguageText))]
    public class LanguageTextDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            GUILayout.BeginVertical("box");
            {
                SerializedProperty languageIndex = property.FindPropertyRelative("LanguageIndex");
                SerializedProperty language = property.FindPropertyRelative("Language");

                SerializedProperty text = property.FindPropertyRelative("Text");

                List<LanguageDefine> languages = LanguageDefineConfig.Default.LanguageDefines;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Language:");

                    EditorGUI.BeginChangeCheck();
                    languageIndex.intValue = EditorGUILayout.Popup(languageIndex.intValue,
                        languages.Select(l => l.Language.ToString()).ToArray());

                    if (EditorGUI.EndChangeCheck())
                    {
                        language.intValue = (int)languages[languageIndex.intValue].Language;
                    }

                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.PropertyField(text);
            }
            GUILayout.EndVertical();
            EditorGUI.EndProperty();
        }
    }
#endif
}