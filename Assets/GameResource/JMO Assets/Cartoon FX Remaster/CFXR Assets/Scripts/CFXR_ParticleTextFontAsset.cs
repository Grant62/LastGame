//--------------------------------------------------------------------------------------------------------------------------------
// Cartoon FX
// (c) 2012-2025 Jean Moreno
//--------------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CartoonFX
{
    public class CFXR_ParticleTextFontAsset : ScriptableObject
    {
        public enum LetterCase
        {
            Both,
            Upper,
            Lower
        }

        public string CharSequence = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!?-.#@$ ";
        public LetterCase letterCase = LetterCase.Upper;
        public Sprite[] CharSprites;
        public Kerning[] CharKerningOffsets;

        [Serializable]
        public class Kerning
        {
            public string name = "A";
            public float pre;
            public float post;
        }

        private void OnValidate()
        {
            hideFlags = HideFlags.None;

            if (CharKerningOffsets == null || CharKerningOffsets.Length != CharSequence.Length)
            {
                CharKerningOffsets = new Kerning[CharSequence.Length];
                for (int i = 0; i < CharKerningOffsets.Length; i++)
                {
                    CharKerningOffsets[i] = new Kerning { name = CharSequence[i].ToString() };
                }
            }
        }

        public bool IsValid()
        {
            bool valid = !string.IsNullOrEmpty(CharSequence) && CharSprites != null && CharSprites.Length == CharSequence.Length && CharKerningOffsets != null
                         && CharKerningOffsets.Length == CharSprites.Length;

            if (!valid)
            {
                Debug.LogError(string.Format("Invalid ParticleTextFontAsset: '{0}'\n", name), this);
            }

            return valid;
        }

#if UNITY_EDITOR
        // [MenuItem("Tools/Create font asset")]
        private static void CreateFontAsset()
        {
            CFXR_ParticleTextFontAsset instance = CreateInstance<CFXR_ParticleTextFontAsset>();
            AssetDatabase.CreateAsset(instance, "Assets/Font.asset");
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CFXR_ParticleTextFontAsset))]
    public class ParticleTextFontAssetEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Kerning"))
            {
                CFXR_ParticleTextFontAsset ptfa = target as CFXR_ParticleTextFontAsset;
                string path = EditorUtility.SaveFilePanel("Export Kerning Settings", Application.dataPath, ptfa.name + " kerning", ".txt");
                if (!string.IsNullOrEmpty(path))
                {
                    string output = "";
                    foreach (CFXR_ParticleTextFontAsset.Kerning k in ptfa.CharKerningOffsets)
                    {
                        output += k.name + "\t" + k.pre + "\t" + k.post + "\n";
                    }

                    File.WriteAllText(path, output);
                }
            }

            if (GUILayout.Button("Import Kerning"))
            {
                string path = EditorUtility.OpenFilePanel("Import Kerning Settings", Application.dataPath, "txt");
                if (!string.IsNullOrEmpty(path))
                {
                    string text = File.ReadAllText(path);
                    string[] split = text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    CFXR_ParticleTextFontAsset ptfa = target as CFXR_ParticleTextFontAsset;
                    Undo.RecordObject(ptfa, "Import Kerning Settings");
                    List<CFXR_ParticleTextFontAsset.Kerning> kerningList = new(ptfa.CharKerningOffsets);
                    for (int i = 0; i < split.Length; i++)
                    {
                        string[] data = split[i].Split('\t');

                        foreach (CFXR_ParticleTextFontAsset.Kerning cko in kerningList)
                        {
                            if (cko.name == data[0])
                            {
                                cko.pre = float.Parse(data[1]);
                                cko.post = float.Parse(data[2]);
                                break;
                            }
                        }
                    }

                    ptfa.CharKerningOffsets = kerningList.ToArray();
                }
            }

            GUILayout.EndHorizontal();
        }
    }
#endif
}