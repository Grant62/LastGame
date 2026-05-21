/****************************************************************************
 * Copyright (c) 2015 ~ 2023 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace QFramework
{
    [Serializable]
    public class OtherBind
    {
        public string MemberName;
        public Object Object;
    }

    public class OtherBindComparer : IComparer<OtherBind>
    {
        public int Compare(OtherBind a, OtherBind b)
        {
            return string.Compare(a.MemberName, b.MemberName, StringComparison.Ordinal);
        }
    }

    [RequireComponent(typeof(ViewController))]
    public class OtherBinds : MonoBehaviour
    {
        public List<OtherBind> Binds = new();

#if UNITY_EDITOR
        public void Add(string memberName, Object obj)
        {
            SerializedObject serializedObject = new(this);

            SerializedProperty bindsProperty = serializedObject.FindProperty("Binds");


            int index = Binds.FindIndex(b => b.MemberName == memberName);
            if (index != -1)
            {
                SerializedProperty element = bindsProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("Object").objectReferenceValue = obj;
            }
            else
            {
                bindsProperty.InsertArrayElementAtIndex(index);
                SerializedProperty element = bindsProperty.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("MemberName").stringValue = memberName;
                element.FindPropertyRelative("Object").objectReferenceValue = obj;
            }

            EditorUtility.SetDirty(this);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        public void Remove(string memberName)
        {
            SerializedObject serializedObject = new(this);
            SerializedProperty bindsProperty = serializedObject.FindProperty("Binds");
            int i;
            for (i = 0; i < Binds.Count; i++)
            {
                if (Binds[i].MemberName == memberName)
                {
                    break;
                }
            }

            if (i != Binds.Count)
            {
                bindsProperty.DeleteArrayElementAtIndex(i);
            }

            EditorUtility.SetDirty(this);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        public void Clear()
        {
            SerializedObject serializedObject = new(this);

            SerializedProperty bindsProperty = serializedObject.FindProperty("Binds");
            bindsProperty.ClearArray();
            EditorUtility.SetDirty(this);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        public void Sort()
        {
            SerializedObject serializedObject = new(this);
            Binds.Sort(new OtherBindComparer());
            EditorUtility.SetDirty(this);
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(OtherBinds))]
    public class ReferenceBindsEditor : Editor
    {
        private OtherBinds mOtherBinds;

        private void DelNullReference()
        {
            SerializedProperty dataProperty = serializedObject.FindProperty("Binds");
            for (int i = dataProperty.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty gameObjectProperty = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Object");
                if (gameObjectProperty.objectReferenceValue == null)
                {
                    dataProperty.DeleteArrayElementAtIndex(i);
                    EditorUtility.SetDirty(mOtherBinds);
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.UpdateIfRequiredOrScript();
                }
            }
        }

        private void OnEnable()
        {
            mOtherBinds = (OtherBinds)target;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(mOtherBinds, "Changed Settings");
            SerializedProperty dataProperty = serializedObject.FindProperty("Binds");
            EditorGUILayout.BeginHorizontal();

            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            List<int> delList = new();
            SerializedProperty property;
            for (int i = mOtherBinds.Binds.Count - 1; i >= 0; i--)
            {
                GUILayout.BeginHorizontal();
                property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("MemberName");
                property.stringValue = EditorGUILayout.TextField(property.stringValue, GUILayout.Width(150));
                property = dataProperty.GetArrayElementAtIndex(i).FindPropertyRelative("Object");
                property.objectReferenceValue =
                    EditorGUILayout.ObjectField(property.objectReferenceValue, typeof(Object), true);

                if (property.objectReferenceValue is Component component)
                {
                    List<Object> objects = new();
                    objects.AddRange(component.gameObject.GetComponents<Component>());
                    objects.Add(component.gameObject);

                    int index = objects.FindIndex(c => c.GetType() == property.objectReferenceValue.GetType());
                    int newIndex = EditorGUILayout.Popup(index, objects.Select(c => c.GetType().FullName).ToArray());
                    if (index != newIndex)
                    {
                        property.objectReferenceValue = objects[newIndex];
                    }
                }
                else if (property.objectReferenceValue is GameObject gameObject)
                {
                    List<Object> objects = new();
                    objects.AddRange(gameObject.GetComponents<Component>());
                    objects.Add(gameObject);

                    int index = objects.FindIndex(c => c.GetType() == property.objectReferenceValue.GetType());
                    int newIndex = EditorGUILayout.Popup(index, objects.Select(c => c.GetType().FullName).ToArray());
                    if (index != newIndex)
                    {
                        property.objectReferenceValue = objects[newIndex];
                    }
                }

                if (GUILayout.Button("X"))
                {
                    //将元素添加进删除list
                    delList.Add(i);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Label(LocaleKitEditor.IsCN.Value ? "将其他需要生成变量的 Object 拖拽至此" : " Drag other Object bellow to generate member variables");
            Rect sfxPathRect = EditorGUILayout.GetControlRect();
            sfxPathRect.height = 50;
            GUI.Box(sfxPathRect, string.Empty);
            EditorGUILayout.LabelField(string.Empty, GUILayout.Height(35));
            if (
                (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                && sfxPathRect.Contains(Event.current.mousePosition)
            )
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object o in DragAndDrop.objectReferences)
                    {
                        AddReference(dataProperty, o.name.RemoveString(" ", "-", "@"), o);
                    }
                }

                Event.current.Use();
            }

            GUILayout.BeginHorizontal();

            // if (GUILayout.Button(  LocaleKitEditor.IsCN.Value ? "添加引用" : "Add Ref"))
            // {
            //     AddReference(dataProperty, Guid.NewGuid().GetHashCode().ToString(), null);
            // }
            //
            // if (GUILayout.Button( LocaleKitEditor.IsCN.Value ? "全部删除" : "Clear"))
            // {
            //     mOtherBinds.Clear();
            // }
            //
            // if (GUILayout.Button(LocaleKitEditor.IsCN.Value ? "删除空引用" : "Delete Null Ref"))
            // {
            //     DelNullReference();
            // }

            // if (GUILayout.Button(LocaleKitEditor.IsCN.Value ?"排序" : "Sort"))
            // {
            //     mOtherBinds.Sort();
            // }

            EditorGUILayout.EndHorizontal();

            foreach (int i in delList)
            {
                dataProperty.DeleteArrayElementAtIndex(i);
            }

            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }

        private void AddReference(SerializedProperty dataProperty, string key, Object obj)
        {
            int index = dataProperty.arraySize;
            dataProperty.InsertArrayElementAtIndex(index);
            SerializedProperty element = dataProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("MemberName").stringValue = key;
            element.FindPropertyRelative("Object").objectReferenceValue = obj;
        }
    }
#endif
}