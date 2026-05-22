/******************************************************************************
 * Spine Runtimes Software License v2.5
 *
 * Copyright (c) 2013-2016, Esoteric Software
 * All rights reserved.
 *
 * You are granted a perpetual, non-exclusive, non-sublicensable, and
 * non-transferable license to use, install, execute, and perform the Spine
 * Runtimes software and derivative works solely for personal or internal
 * use. Without the written permission of Esoteric Software (see Section 2 of
 * the Spine Software License Agreement), you may not (a) modify, translate,
 * adapt, or develop new applications using the Spine Runtimes or otherwise
 * create derivative works or improvements of the Spine Runtimes or (b) remove,
 * delete, alter, or obscure any trademarks or any copyright, trademark, patent,
 * or other intellectual property or proprietary rights notices on or in the
 * Software, including any copy thereof. Redistributions in binary or source
 * form must include this license and terms.
 *
 * THIS SOFTWARE IS PROVIDED BY ESOTERIC SOFTWARE "AS IS" AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO
 * EVENT SHALL ESOTERIC SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES, BUSINESS INTERRUPTION, OR LOSS OF
 * USE, DATA, OR PROFITS) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
 * IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

#pragma warning disable 0219

// Original contribution by: Mitch Thompson

#define SPINE_SKELETONANIMATOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEditor.Build;
using Object = UnityEngine.Object;

namespace Spine.Unity.Editor
{
    using EventType = EventType;

    // Analysis disable once ConvertToStaticType
    [InitializeOnLoad]
    public class SpineEditorUtilities : AssetPostprocessor
    {
        public static class Icons
        {
            public static Texture2D skeleton;
            public static Texture2D nullBone;
            public static Texture2D bone;
            public static Texture2D poseBones;
            public static Texture2D boneNib;
            public static Texture2D slot;
            public static Texture2D slotRoot;
            public static Texture2D skinPlaceholder;
            public static Texture2D image;
            public static Texture2D genericAttachment;
            public static Texture2D boundingBox;
            public static Texture2D point;
            public static Texture2D mesh;
            public static Texture2D weights;
            public static Texture2D path;
            public static Texture2D clipping;
            public static Texture2D skin;
            public static Texture2D skinsRoot;
            public static Texture2D animation;
            public static Texture2D animationRoot;
            public static Texture2D spine;
            public static Texture2D userEvent;
            public static Texture2D constraintNib;
            public static Texture2D constraintRoot;
            public static Texture2D constraintTransform;
            public static Texture2D constraintPath;
            public static Texture2D constraintIK;
            public static Texture2D warning;
            public static Texture2D skeletonUtility;
            public static Texture2D hingeChain;
            public static Texture2D subMeshRenderer;
            public static Texture2D skeletonDataAssetIcon;
            public static Texture2D info;
            public static Texture2D unity;
//			public static Texture2D controllerIcon;

            private static Texture2D LoadIcon(string filename)
            {
                return (Texture2D)AssetDatabase.LoadMainAssetAtPath(editorGUIPath + "/" + filename);
            }

            public static void Initialize()
            {
                skeleton = LoadIcon("icon-skeleton.png");
                nullBone = LoadIcon("icon-null.png");
                bone = LoadIcon("icon-bone.png");
                poseBones = LoadIcon("icon-poseBones.png");
                boneNib = LoadIcon("icon-boneNib.png");
                slot = LoadIcon("icon-slot.png");
                slotRoot = LoadIcon("icon-slotRoot.png");
                skinPlaceholder = LoadIcon("icon-skinPlaceholder.png");

                genericAttachment = LoadIcon("icon-attachment.png");
                image = LoadIcon("icon-image.png");
                boundingBox = LoadIcon("icon-boundingBox.png");
                point = LoadIcon("icon-point.png");
                mesh = LoadIcon("icon-mesh.png");
                weights = LoadIcon("icon-weights.png");
                path = LoadIcon("icon-path.png");
                clipping = LoadIcon("icon-clipping.png");

                skin = LoadIcon("icon-skin.png");
                skinsRoot = LoadIcon("icon-skinsRoot.png");
                animation = LoadIcon("icon-animation.png");
                animationRoot = LoadIcon("icon-animationRoot.png");
                spine = LoadIcon("icon-spine.png");
                userEvent = LoadIcon("icon-event.png");
                constraintNib = LoadIcon("icon-constraintNib.png");

                constraintRoot = LoadIcon("icon-constraints.png");
                constraintTransform = LoadIcon("icon-constraintTransform.png");
                constraintPath = LoadIcon("icon-constraintPath.png");
                constraintIK = LoadIcon("icon-constraintIK.png");

                warning = LoadIcon("icon-warning.png");
                skeletonUtility = LoadIcon("icon-skeletonUtility.png");
                hingeChain = LoadIcon("icon-hingeChain.png");
                subMeshRenderer = LoadIcon("icon-subMeshRenderer.png");

                skeletonDataAssetIcon = LoadIcon("SkeletonDataAsset Icon.png");

                info = EditorGUIUtility.FindTexture("console.infoicon.sml");
                unity = EditorGUIUtility.FindTexture("SceneAsset Icon");
//				controllerIcon = EditorGUIUtility.FindTexture("AnimatorController Icon");
            }

            public static Texture2D GetAttachmentIcon(Attachment attachment)
            {
                // Analysis disable once CanBeReplacedWithTryCastAndCheckForNull
                if (attachment is RegionAttachment)
                    return image;
                if (attachment is MeshAttachment)
                    return ((MeshAttachment)attachment).IsWeighted() ? weights : mesh;
                if (attachment is BoundingBoxAttachment)
                    return boundingBox;
                if (attachment is PointAttachment)
                    return point;
                if (attachment is PathAttachment)
                    return path;
                if (attachment is ClippingAttachment)
                    return clipping;
                return warning;
            }
        }

        public static string editorPath = "";
        public static string editorGUIPath = "";
        public static bool initialized;

        /// HACK: This list keeps the asset reference temporarily during importing.
        /// 
        /// In cases of very large projects/sufficient RAM pressure, when AssetDatabase.SaveAssets is called,
        /// Unity can mistakenly unload assets whose references are only on the stack.
        /// This leads to MissingReferenceException and other errors.
        private static readonly List<ScriptableObject> protectFromStackGarbageCollection = new();
        private static readonly HashSet<string> assetsImportedInWrongState = new();

#if SPINE_TK2D
		const float DEFAULT_DEFAULT_SCALE = 1f;
#else
        private const float DEFAULT_DEFAULT_SCALE = 0.01f;
#endif
        private const string DEFAULT_SCALE_KEY = "SPINE_DEFAULT_SCALE";
        public static float defaultScale = DEFAULT_DEFAULT_SCALE;

        private const float DEFAULT_DEFAULT_MIX = 0.2f;
        private const string DEFAULT_MIX_KEY = "SPINE_DEFAULT_MIX";
        public static float defaultMix = DEFAULT_DEFAULT_MIX;

        private const string DEFAULT_DEFAULT_SHADER = "Spine/Skeleton";
        private const string DEFAULT_SHADER_KEY = "SPINE_DEFAULT_SHADER";
        public static string defaultShader = DEFAULT_DEFAULT_SHADER;

        private const float DEFAULT_DEFAULT_ZSPACING = 0f;
        private const string DEFAULT_ZSPACING_KEY = "SPINE_DEFAULT_ZSPACING";
        public static float defaultZSpacing = DEFAULT_DEFAULT_ZSPACING;

        private const bool DEFAULT_SHOW_HIERARCHY_ICONS = true;
        private const string SHOW_HIERARCHY_ICONS_KEY = "SPINE_SHOW_HIERARCHY_ICONS";
        public static bool showHierarchyIcons = DEFAULT_SHOW_HIERARCHY_ICONS;

        private const bool DEFAULT_SET_TEXTUREIMPORTER_SETTINGS = true;
        private const string SET_TEXTUREIMPORTER_SETTINGS_KEY = "SPINE_SET_TEXTUREIMPORTER_SETTINGS";
        public static bool setTextureImporterSettings = DEFAULT_SET_TEXTUREIMPORTER_SETTINGS;

        internal const float DEFAULT_MIPMAPBIAS = -0.5f;

        public const float DEFAULT_SCENE_ICONS_SCALE = 1f;
        public const string SCENE_ICONS_SCALE_KEY = "SPINE_SCENE_ICONS_SCALE";

        #region Initialization
        static SpineEditorUtilities()
        {
            Initialize();
        }

        private static void LoadPreferences()
        {
            defaultMix = EditorPrefs.GetFloat(DEFAULT_MIX_KEY, DEFAULT_DEFAULT_MIX);
            defaultScale = EditorPrefs.GetFloat(DEFAULT_SCALE_KEY, DEFAULT_DEFAULT_SCALE);
            defaultZSpacing = EditorPrefs.GetFloat(DEFAULT_ZSPACING_KEY, DEFAULT_DEFAULT_ZSPACING);
            defaultShader = EditorPrefs.GetString(DEFAULT_SHADER_KEY, DEFAULT_DEFAULT_SHADER);
            showHierarchyIcons = EditorPrefs.GetBool(SHOW_HIERARCHY_ICONS_KEY, DEFAULT_SHOW_HIERARCHY_ICONS);
            setTextureImporterSettings = EditorPrefs.GetBool(SET_TEXTUREIMPORTER_SETTINGS_KEY, DEFAULT_SET_TEXTUREIMPORTER_SETTINGS);
            SpineHandles.handleScale = EditorPrefs.GetFloat(SCENE_ICONS_SCALE_KEY, DEFAULT_SCENE_ICONS_SCALE);
            preferencesLoaded = true;
        }

        private static void Initialize()
        {
            LoadPreferences();

            DirectoryInfo rootDir = new(Application.dataPath);
            FileInfo[] files = rootDir.GetFiles("SpineEditorUtilities.cs", SearchOption.AllDirectories);
            editorPath = Path.GetDirectoryName(files[0].FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets"));
            editorGUIPath = editorPath + "/GUI";

            Icons.Initialize();

            // Drag and Drop
            SceneView.duringSceneGui -= SceneViewDragAndDrop;
            SceneView.duringSceneGui += SceneViewDragAndDrop;
            EditorApplication.hierarchyWindowItemOnGUI -= SpineEditorHierarchyHandler.HierarchyDragAndDrop;
            EditorApplication.hierarchyWindowItemOnGUI += SpineEditorHierarchyHandler.HierarchyDragAndDrop;

            // Hierarchy Icons
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= SpineEditorHierarchyHandler.HierarchyIconsOnPlaymodeStateChanged;
            EditorApplication.playModeStateChanged += SpineEditorHierarchyHandler.HierarchyIconsOnPlaymodeStateChanged;
            SpineEditorHierarchyHandler.HierarchyIconsOnPlaymodeStateChanged(PlayModeStateChange.EnteredEditMode);
#else
			EditorApplication.playmodeStateChanged -= SpineEditorHierarchyHandler.HierarchyIconsOnPlaymodeStateChanged;
			EditorApplication.playmodeStateChanged += SpineEditorHierarchyHandler.HierarchyIconsOnPlaymodeStateChanged;
			SpineEditorHierarchyHandler.HierarchyIconsOnPlaymodeStateChanged();
#endif

            initialized = true;
        }

        public static void ConfirmInitialization()
        {
            if (!initialized || Icons.skeleton == null)
                Initialize();
        }
        #endregion

        #region Spine Preferences and Defaults
        private static bool preferencesLoaded;

        [SettingsProvider]
        private static SettingsProvider SpineSettingsProvider()
        {
            return new SettingsProvider("Preferences/Spine", SettingsScope.User)
            {
                label = "Spine",
                guiHandler = searchContext =>
                {
                    if (!preferencesLoaded)
                        LoadPreferences();

                    EditorGUI.BeginChangeCheck();
                    showHierarchyIcons =
                        EditorGUILayout.Toggle(
                            new GUIContent("Show Hierarchy Icons", "Show relevant icons on GameObjects with Spine Components on them. Disable this if you have large, complex scenes."),
                            showHierarchyIcons);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetBool(SHOW_HIERARCHY_ICONS_KEY, showHierarchyIcons);
                        SpineEditorHierarchyHandler.HierarchyIconsOnPlaymodeStateChanged(PlayModeStateChange.EnteredEditMode);
                    }

                    EditorGUILayout.Separator();

                    EditorGUILayout.LabelField("Auto-Import Settings", EditorStyles.boldLabel);

                    EditorGUI.BeginChangeCheck();
                    defaultMix = EditorGUILayout.FloatField("Default Mix", defaultMix);
                    if (EditorGUI.EndChangeCheck())
                        EditorPrefs.SetFloat(DEFAULT_MIX_KEY, defaultMix);

                    EditorGUI.BeginChangeCheck();
                    defaultScale = EditorGUILayout.FloatField("Default SkeletonData Scale", defaultScale);
                    if (EditorGUI.EndChangeCheck())
                        EditorPrefs.SetFloat(DEFAULT_SCALE_KEY, defaultScale);

                    EditorGUI.BeginChangeCheck();
                    Shader shader = EditorGUILayout.ObjectField("Default Shader", Shader.Find(defaultShader), typeof(Shader), false) as Shader;
                    defaultShader = shader != null ? shader.name : DEFAULT_DEFAULT_SHADER;
                    if (EditorGUI.EndChangeCheck())
                        EditorPrefs.SetString(DEFAULT_SHADER_KEY, defaultShader);

                    EditorGUI.BeginChangeCheck();
                    setTextureImporterSettings =
                        EditorGUILayout.Toggle(new GUIContent("Apply Atlas Texture Settings", "Apply the recommended settings for Texture Importers."), setTextureImporterSettings);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetBool(SET_TEXTUREIMPORTER_SETTINGS_KEY, setTextureImporterSettings);
                    }

                    EditorGUILayout.Space();

                    EditorGUILayout.LabelField("Editor Instantiation", EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    defaultZSpacing = EditorGUILayout.Slider("Default Slot Z-Spacing", defaultZSpacing, -0.1f, 0f);
                    if (EditorGUI.EndChangeCheck())
                        EditorPrefs.SetFloat(DEFAULT_ZSPACING_KEY, defaultZSpacing);


                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Handles and Gizmos", EditorStyles.boldLabel);
                    EditorGUI.BeginChangeCheck();
                    SpineHandles.handleScale = EditorGUILayout.Slider("Editor Bone Scale", SpineHandles.handleScale, 0.01f, 2f);
                    SpineHandles.handleScale = Mathf.Max(0.01f, SpineHandles.handleScale);
                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorPrefs.SetFloat(SCENE_ICONS_SCALE_KEY, SpineHandles.handleScale);
                        SceneView.RepaintAll();
                    }


                    GUILayout.Space(20);
                    EditorGUILayout.LabelField("3rd Party Settings", EditorStyles.boldLabel);
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.PrefixLabel("Define TK2D");
                        if (GUILayout.Button("Enable", GUILayout.Width(64)))
                            SpineTK2DEditorUtility.EnableTK2D();
                        if (GUILayout.Button("Disable", GUILayout.Width(64)))
                            SpineTK2DEditorUtility.DisableTK2D();
                    }
                }
            };
        }
        #endregion

        #region Drag and Drop Instantiation
        public delegate Component InstantiateDelegate(SkeletonDataAsset skeletonDataAsset);

        public struct SpawnMenuData
        {
            public Vector3 spawnPoint;
            public SkeletonDataAsset skeletonDataAsset;
            public InstantiateDelegate instantiateDelegate;
            public bool isUI;
        }

        public class SkeletonComponentSpawnType
        {
            public string menuLabel;
            public InstantiateDelegate instantiateDelegate;
            public bool isUI;
        }

        internal static readonly List<SkeletonComponentSpawnType> additionalSpawnTypes = new();

        private static void SceneViewDragAndDrop(SceneView sceneview)
        {
            UnityEngine.Event current = UnityEngine.Event.current;
            Object[] references = DragAndDrop.objectReferences;
            if (current.type == EventType.Layout) return;

            // Allow drag and drop of one SkeletonDataAsset.
            if (references.Length == 1)
            {
                SkeletonDataAsset skeletonDataAsset = references[0] as SkeletonDataAsset;
                if (skeletonDataAsset != null)
                {
                    Vector2 mousePos = current.mousePosition;

                    bool invalidSkeletonData = skeletonDataAsset.GetSkeletonData(true) == null;
                    if (invalidSkeletonData)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        Handles.BeginGUI();
                        GUI.Label(new Rect(mousePos + new Vector2(20f, 20f), new Vector2(400f, 40f)),
                            new GUIContent(string.Format("{0} is invalid.\nCannot create new Spine GameObject.", skeletonDataAsset.name), Icons.warning));
                        Handles.EndGUI();
                    }
                    else
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        Handles.BeginGUI();
                        GUI.Label(new Rect(mousePos + new Vector2(20f, 20f), new Vector2(400f, 20f)),
                            new GUIContent(string.Format("Create Spine GameObject ({0})", skeletonDataAsset.skeletonJSON.name), Icons.skeletonDataAssetIcon));
                        Handles.EndGUI();

                        if (current.type == EventType.DragPerform)
                        {
                            RectTransform rectTransform = Selection.activeGameObject == null ? null : Selection.activeGameObject.GetComponent<RectTransform>();
                            Plane plane = rectTransform == null ? new Plane(Vector3.back, Vector3.zero) : new Plane(-rectTransform.forward, rectTransform.position);
                            Vector3 spawnPoint = MousePointToWorldPoint2D(mousePos, sceneview.camera, plane);
                            ShowInstantiateContextMenu(skeletonDataAsset, spawnPoint);
                            DragAndDrop.AcceptDrag();
                            current.Use();
                        }
                    }
                }
            }
        }

        public static void ShowInstantiateContextMenu(SkeletonDataAsset skeletonDataAsset, Vector3 spawnPoint)
        {
            GenericMenu menu = new();

            // SkeletonAnimation
            menu.AddItem(new GUIContent("SkeletonAnimation"), false, HandleSkeletonComponentDrop, new SpawnMenuData
            {
                skeletonDataAsset = skeletonDataAsset,
                spawnPoint = spawnPoint,
                instantiateDelegate = data => InstantiateSkeletonAnimation(data),
                isUI = false
            });

            // SkeletonGraphic
            Type skeletonGraphicInspectorType = Type.GetType("Spine.Unity.Editor.SkeletonGraphicInspector");
            if (skeletonGraphicInspectorType != null)
            {
                MethodInfo graphicInstantiateDelegate = skeletonGraphicInspectorType.GetMethod("SpawnSkeletonGraphicFromDrop", BindingFlags.Static | BindingFlags.Public);
                if (graphicInstantiateDelegate != null)
                    menu.AddItem(new GUIContent("SkeletonGraphic (UI)"), false, HandleSkeletonComponentDrop, new SpawnMenuData
                    {
                        skeletonDataAsset = skeletonDataAsset,
                        spawnPoint = spawnPoint,
                        instantiateDelegate = Delegate.CreateDelegate(typeof(InstantiateDelegate), graphicInstantiateDelegate) as InstantiateDelegate,
                        isUI = true
                    });
            }

#if SPINE_SKELETONANIMATOR
            menu.AddSeparator("");
            // SkeletonAnimator
            menu.AddItem(new GUIContent("SkeletonAnimator"), false, HandleSkeletonComponentDrop, new SpawnMenuData
            {
                skeletonDataAsset = skeletonDataAsset,
                spawnPoint = spawnPoint,
                instantiateDelegate = data => InstantiateSkeletonAnimator(data)
            });
#endif

            menu.ShowAsContext();
        }

        public static void HandleSkeletonComponentDrop(object spawnMenuData)
        {
            SpawnMenuData data = (SpawnMenuData)spawnMenuData;

            if (data.skeletonDataAsset.GetSkeletonData(true) == null)
            {
                EditorUtility.DisplayDialog("Invalid SkeletonDataAsset", "Unable to create Spine GameObject.\n\nPlease check your SkeletonDataAsset.", "Ok");
                return;
            }

            bool isUI = data.isUI;

            Component newSkeletonComponent = data.instantiateDelegate.Invoke(data.skeletonDataAsset);
            GameObject newGameObject = newSkeletonComponent.gameObject;
            Transform newTransform = newGameObject.transform;

            GameObject activeGameObject = Selection.activeGameObject;
            if (isUI && activeGameObject != null)
                newTransform.SetParent(activeGameObject.transform, false);

            newTransform.position = isUI ? data.spawnPoint : RoundVector(data.spawnPoint, 2);

            if (isUI && (activeGameObject == null || activeGameObject.GetComponent<RectTransform>() == null))
                Debug.Log("Created a UI Skeleton GameObject not under a RectTransform. It may not be visible until you parent it to a canvas.");

            if (!isUI && activeGameObject != null && activeGameObject.transform.localScale != Vector3.one)
                Debug.Log("New Spine GameObject was parented to a scaled Transform. It may not be the intended size.");

            Selection.activeGameObject = newGameObject;
            //EditorGUIUtility.PingObject(newGameObject); // Doesn't work when setting activeGameObject.
            Undo.RegisterCreatedObjectUndo(newGameObject, "Create Spine GameObject");
        }

        /// <summary>
        ///     Rounds off vector components to a number of decimal digits.
        /// </summary>
        public static Vector3 RoundVector(Vector3 vector, int digits)
        {
            vector.x = (float)Math.Round(vector.x, digits);
            vector.y = (float)Math.Round(vector.y, digits);
            vector.z = (float)Math.Round(vector.z, digits);
            return vector;
        }

        /// <summary>
        ///     Converts a mouse point to a world point on a plane.
        /// </summary>
        private static Vector3 MousePointToWorldPoint2D(Vector2 mousePosition, Camera camera, Plane plane)
        {
            Vector3 screenPos = new(mousePosition.x, camera.pixelHeight - mousePosition.y, 0f);
            Ray ray = camera.ScreenPointToRay(screenPos);
            float distance;
            bool hit = plane.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }
        #endregion

        #region Hierarchy
        private static class SpineEditorHierarchyHandler
        {
            private static readonly Dictionary<int, GameObject> skeletonRendererTable = new();
            private static readonly Dictionary<int, SkeletonUtilityBone> skeletonUtilityBoneTable = new();
            private static readonly Dictionary<int, BoundingBoxFollower> boundingBoxFollowerTable = new();

#if UNITY_2017_2_OR_NEWER
            internal static void HierarchyIconsOnPlaymodeStateChanged(PlayModeStateChange stateChange)
            {
#else
			internal static void HierarchyIconsOnPlaymodeStateChanged () {
#endif
                skeletonRendererTable.Clear();
                skeletonUtilityBoneTable.Clear();
                boundingBoxFollowerTable.Clear();

                EditorApplication.hierarchyChanged -= HierarchyIconsOnChanged;
                EditorApplication.hierarchyWindowItemOnGUI -= HierarchyIconsOnGUI;

                if (!Application.isPlaying && showHierarchyIcons)
                {
                    EditorApplication.hierarchyChanged += HierarchyIconsOnChanged;
                    EditorApplication.hierarchyWindowItemOnGUI += HierarchyIconsOnGUI;
                    HierarchyIconsOnChanged();
                }
            }

            internal static void HierarchyIconsOnChanged()
            {
                skeletonRendererTable.Clear();
                skeletonUtilityBoneTable.Clear();
                boundingBoxFollowerTable.Clear();

                SkeletonRenderer[] arr = Object.FindObjectsByType<SkeletonRenderer>(FindObjectsSortMode.None);
                foreach (SkeletonRenderer r in arr)
                    skeletonRendererTable[r.gameObject.GetInstanceID()] = r.gameObject;

                SkeletonUtilityBone[] boneArr = Object.FindObjectsByType<SkeletonUtilityBone>(FindObjectsSortMode.None);
                foreach (SkeletonUtilityBone b in boneArr)
                    skeletonUtilityBoneTable[b.gameObject.GetInstanceID()] = b;

                BoundingBoxFollower[] bbfArr = Object.FindObjectsByType<BoundingBoxFollower>(FindObjectsSortMode.None);
                foreach (BoundingBoxFollower bbf in bbfArr)
                    boundingBoxFollowerTable[bbf.gameObject.GetInstanceID()] = bbf;
            }

            internal static void HierarchyIconsOnGUI(int instanceId, Rect selectionRect)
            {
                Rect r = new(selectionRect);
                if (skeletonRendererTable.ContainsKey(instanceId))
                {
                    r.x = r.width - 15;
                    r.width = 15;
                    GUI.Label(r, Icons.spine);
                }
                else if (skeletonUtilityBoneTable.ContainsKey(instanceId))
                {
                    r.x -= 26;
                    if (skeletonUtilityBoneTable[instanceId] != null)
                    {
                        if (skeletonUtilityBoneTable[instanceId].transform.childCount == 0)
                            r.x += 13;
                        r.y += 2;
                        r.width = 13;
                        r.height = 13;
                        if (skeletonUtilityBoneTable[instanceId].mode == SkeletonUtilityBone.Mode.Follow)
                            GUI.DrawTexture(r, Icons.bone);
                        else
                            GUI.DrawTexture(r, Icons.poseBones);
                    }
                }
                else if (boundingBoxFollowerTable.ContainsKey(instanceId))
                {
                    r.x -= 26;
                    if (boundingBoxFollowerTable[instanceId] != null)
                    {
                        if (boundingBoxFollowerTable[instanceId].transform.childCount == 0)
                            r.x += 13;
                        r.y += 2;
                        r.width = 13;
                        r.height = 13;
                        GUI.DrawTexture(r, Icons.boundingBox);
                    }
                }
            }

            internal static void HierarchyDragAndDrop(int instanceId, Rect selectionRect)
            {
                // HACK: Uses EditorApplication.hierarchyWindowItemOnGUI.
                // Only works when there is at least one item in the scene.
                UnityEngine.Event current = UnityEngine.Event.current;
                EventType eventType = current.type;
                bool isDraggingEvent = eventType == EventType.DragUpdated;
                bool isDropEvent = eventType == EventType.DragPerform;
                if (isDraggingEvent || isDropEvent)
                {
                    EditorWindow mouseOverWindow = EditorWindow.mouseOverWindow;
                    if (mouseOverWindow != null)
                    {
                        // One, existing, valid SkeletonDataAsset
                        Object[] references = DragAndDrop.objectReferences;
                        if (references.Length == 1)
                        {
                            SkeletonDataAsset skeletonDataAsset = references[0] as SkeletonDataAsset;
                            if (skeletonDataAsset != null && skeletonDataAsset.GetSkeletonData(true) != null)
                            {
                                // Allow drag-and-dropping anywhere in the Hierarchy Window.
                                // HACK: string-compare because we can't get its type via reflection.
                                const string HierarchyWindow = "UnityEditor.SceneHierarchyWindow";
                                if (HierarchyWindow.Equals(mouseOverWindow.GetType().ToString(), StringComparison.Ordinal))
                                {
                                    if (isDraggingEvent)
                                    {
                                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                        current.Use();
                                    }
                                    else if (isDropEvent)
                                    {
                                        ShowInstantiateContextMenu(skeletonDataAsset, Vector3.zero);
                                        DragAndDrop.AcceptDrag();
                                        current.Use();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Auto-Import Entry Point
        private static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromAssetPaths)
        {
            if (imported.Length == 0)
                return;

            // In case user used "Assets -> Reimport All", during the import process,
            // asset database is not initialized until some point. During that period,
            // all attempts to load any assets using API (i.e. AssetDatabase.LoadAssetAtPath)
            // will return null, and as result, assets won't be loaded even if they actually exists,
            // which may lead to numerous importing errors.
            // This situation also happens if Library folder is deleted from the project, which is a pretty
            // common case, since when using version control systems, the Library folder must be excluded.
            // 
            // So to avoid this, in case asset database is not available, we delay loading the assets
            // until next time.
            //
            // Unity *always* reimports some internal assets after the process is done, so this method
            // is always called once again in a state when asset database is available.
            //
            // Checking whether AssetDatabase is initialized is done by attempting to load
            // a known "marker" asset that should always be available. Failing to load this asset
            // means that AssetDatabase is not initialized.
            assetsImportedInWrongState.UnionWith(imported);
            if (AssetDatabaseAvailabilityDetector.IsAssetDatabaseAvailable())
            {
                string[] combinedAssets = assetsImportedInWrongState.ToArray();
                assetsImportedInWrongState.Clear();
                ImportSpineContent(combinedAssets);
            }
        }

        public static void ImportSpineContent(string[] imported, bool reimport = false)
        {
            List<string> atlasPaths = new();
            List<string> imagePaths = new();
            List<string> skeletonPaths = new();

            foreach (string str in imported)
            {
                string extension = Path.GetExtension(str).ToLower();
                switch (extension)
                {
                    case ".txt":
                        if (str.EndsWith(".atlas.txt", StringComparison.Ordinal))
                            atlasPaths.Add(str);
                        break;
                    case ".png":
                    case ".jpg":
                        imagePaths.Add(str);
                        break;
                    case ".json":
                        TextAsset jsonAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(str, typeof(TextAsset));
                        if (jsonAsset != null && SkeletonDataFileValidator.IsSpineData(jsonAsset))
                            skeletonPaths.Add(str);
                        break;
                    case ".bytes":
                        if (str.ToLower().EndsWith(".skel.bytes", StringComparison.Ordinal))
                        {
                            if (SkeletonDataFileValidator.IsSpineData((TextAsset)AssetDatabase.LoadAssetAtPath(str, typeof(TextAsset))))
                                skeletonPaths.Add(str);
                        }

                        break;
                }
            }

            // Import atlases first.
            List<AtlasAsset> atlases = new();
            foreach (string ap in atlasPaths)
            {
                TextAsset atlasText = (TextAsset)AssetDatabase.LoadAssetAtPath(ap, typeof(TextAsset));
                AtlasAsset atlas = IngestSpineAtlas(atlasText);
                atlases.Add(atlas);
            }

            // Import skeletons and match them with atlases.
            bool abortSkeletonImport = false;
            foreach (string sp in skeletonPaths)
            {
                if (!reimport && SkeletonDataFileValidator.CheckForValidSkeletonData(sp))
                {
                    ReloadSkeletonData(sp);
                    continue;
                }

                string dir = Path.GetDirectoryName(sp);

#if SPINE_TK2D
				IngestSpineProject(AssetDatabase.LoadAssetAtPath(sp, typeof(TextAsset)) as TextAsset, null);
#else
                List<AtlasAsset> localAtlases = FindAtlasesAtPath(dir);
                List<string> requiredPaths = GetRequiredAtlasRegions(sp);
                AtlasAsset atlasMatch = GetMatchingAtlas(requiredPaths, localAtlases);
                if (atlasMatch != null || requiredPaths.Count == 0)
                {
                    IngestSpineProject(AssetDatabase.LoadAssetAtPath(sp, typeof(TextAsset)) as TextAsset, atlasMatch);
                }
                else
                {
                    bool resolved = false;
                    while (!resolved)
                    {
                        string filename = Path.GetFileNameWithoutExtension(sp);
                        int result = EditorUtility.DisplayDialogComplex(
                            string.Format("AtlasAsset for \"{0}\"", filename),
                            string.Format("Could not automatically set the AtlasAsset for \"{0}\". You may set it manually.", filename),
                            "Choose AtlasAssets...", "Skip this", "Stop importing all"
                        );

                        switch (result)
                        {
                            case -1:
                                //Debug.Log("Select Atlas");
                                AtlasAsset selectedAtlas = GetAtlasDialog(Path.GetDirectoryName(sp));
                                if (selectedAtlas != null)
                                {
                                    localAtlases.Clear();
                                    localAtlases.Add(selectedAtlas);
                                    atlasMatch = GetMatchingAtlas(requiredPaths, localAtlases);
                                    if (atlasMatch != null)
                                    {
                                        resolved = true;
                                        IngestSpineProject(AssetDatabase.LoadAssetAtPath(sp, typeof(TextAsset)) as TextAsset, atlasMatch);
                                    }
                                }

                                break;
                            case 0: // Choose AtlasAssets...
                                List<AtlasAsset> atlasList = MultiAtlasDialog(requiredPaths, Path.GetDirectoryName(sp), Path.GetFileNameWithoutExtension(sp));
                                if (atlasList != null)
                                    IngestSpineProject(AssetDatabase.LoadAssetAtPath(sp, typeof(TextAsset)) as TextAsset, atlasList.ToArray());

                                resolved = true;
                                break;
                            case 1: // Skip
                                Debug.Log("Skipped importing: " + Path.GetFileName(sp));
                                resolved = true;
                                break;
                            case 2: // Stop importing all
                                abortSkeletonImport = true;
                                resolved = true;
                                break;
                        }
                    }
                }

                if (abortSkeletonImport)
                    break;
#endif
            }
            // Any post processing of images
        }

        private static void ReloadSkeletonData(string skeletonJSONPath)
        {
            string dir = Path.GetDirectoryName(skeletonJSONPath);
            TextAsset textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(skeletonJSONPath, typeof(TextAsset));
            DirectoryInfo dirInfo = new(dir);
            FileInfo[] files = dirInfo.GetFiles("*.asset");

            foreach (FileInfo f in files)
            {
                string localPath = dir + "/" + f.Name;
                Object obj = AssetDatabase.LoadAssetAtPath(localPath, typeof(Object));
                SkeletonDataAsset skeletonDataAsset = obj as SkeletonDataAsset;
                if (skeletonDataAsset != null)
                {
                    if (skeletonDataAsset.skeletonJSON == textAsset)
                    {
                        if (Selection.activeObject == skeletonDataAsset)
                            Selection.activeObject = null;

                        Debug.LogFormat("Changes to '{0}' detected. Clearing SkeletonDataAsset: {1}", skeletonJSONPath, localPath);
                        skeletonDataAsset.Clear();

                        string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(skeletonDataAsset));
                        string lastHash = EditorPrefs.GetString(guid + "_hash");

                        // For some weird reason sometimes Unity loses the internal Object pointer,
                        // and as a result, all comparisons with null returns true.
                        // But the C# wrapper is still alive, so we can "restore" the object
                        // by reloading it from its Instance ID.
                        AtlasAsset[] skeletonDataAtlasAssets = skeletonDataAsset.atlasAssets;
                        if (skeletonDataAtlasAssets != null)
                        {
                            for (int i = 0; i < skeletonDataAtlasAssets.Length; i++)
                            {
                                if (!ReferenceEquals(null, skeletonDataAtlasAssets[i]) &&
                                    skeletonDataAtlasAssets[i].Equals(null) &&
                                    skeletonDataAtlasAssets[i].GetInstanceID() != 0
                                   )
                                {
                                    skeletonDataAtlasAssets[i] = EditorUtility.InstanceIDToObject(skeletonDataAtlasAssets[i].GetInstanceID()) as AtlasAsset;
                                }
                            }
                        }

                        SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
                        string currentHash = skeletonData != null ? skeletonData.Hash : null;

#if SPINE_SKELETONANIMATOR
                        if (currentHash == null || lastHash != currentHash)
                            UpdateMecanimClips(skeletonDataAsset);
#endif

                        // if (currentHash == null || lastHash != currentHash)
                        // Do any upkeep on synchronized assets

                        if (currentHash != null)
                            EditorPrefs.SetString(guid + "_hash", currentHash);
                    }
                }
            }
        }
        #endregion

        #region Match SkeletonData with Atlases
        private static readonly AttachmentType[] AtlasTypes = { AttachmentType.Region, AttachmentType.Linkedmesh, AttachmentType.Mesh };

        private static List<AtlasAsset> MultiAtlasDialog(List<string> requiredPaths, string initialDirectory, string filename = "")
        {
            List<AtlasAsset> atlasAssets = new();
            bool resolved = false;
            string lastAtlasPath = initialDirectory;
            while (!resolved)
            {
                // Build dialog box message.
                List<string> missingRegions = new(requiredPaths);
                StringBuilder dialogText = new();
                {
                    dialogText.AppendLine(string.Format("SkeletonDataAsset for \"{0}\"", filename));
                    dialogText.AppendLine("has missing regions.");
                    dialogText.AppendLine();
                    dialogText.AppendLine("Current Atlases:");

                    if (atlasAssets.Count == 0)
                        dialogText.AppendLine("\t--none--");

                    for (int i = 0; i < atlasAssets.Count; i++)
                        dialogText.AppendLine("\t" + atlasAssets[i].name);

                    dialogText.AppendLine();
                    dialogText.AppendLine("Missing Regions:");

                    foreach (AtlasAsset atlasAsset in atlasAssets)
                    {
                        Atlas atlas = atlasAsset.GetAtlas();
                        for (int i = 0; i < missingRegions.Count; i++)
                        {
                            if (atlas.FindRegion(missingRegions[i]) != null)
                            {
                                missingRegions.RemoveAt(i);
                                i--;
                            }
                        }
                    }

                    int n = missingRegions.Count;
                    if (n == 0) break;

                    const int MaxListLength = 15;
                    for (int i = 0; i < n && i < MaxListLength; i++)
                        dialogText.AppendLine("\t" + missingRegions[i]);

                    if (n > MaxListLength) dialogText.AppendLine(string.Format("\t... {0} more...", n - MaxListLength));
                }

                // Show dialog box.
                int result = EditorUtility.DisplayDialogComplex(
                    "SkeletonDataAsset has missing Atlas.",
                    dialogText.ToString(),
                    "Browse...", "Import anyway", "Cancel"
                );

                switch (result)
                {
                    case 0: // Browse...
                        AtlasAsset selectedAtlasAsset = GetAtlasDialog(lastAtlasPath);
                        if (selectedAtlasAsset != null)
                        {
                            Atlas atlas = selectedAtlasAsset.GetAtlas();
                            bool hasValidRegion = false;
                            foreach (string str in missingRegions)
                            {
                                if (atlas.FindRegion(str) != null)
                                {
                                    hasValidRegion = true;
                                    break;
                                }
                            }

                            atlasAssets.Add(selectedAtlasAsset);
                        }

                        break;
                    case 1: // Import anyway
                        resolved = true;
                        break;
                    case 2: // Cancel
                        atlasAssets = null;
                        resolved = true;
                        break;
                }
            }

            return atlasAssets;
        }

        private static AtlasAsset GetAtlasDialog(string dirPath)
        {
            string path = EditorUtility.OpenFilePanel("Select AtlasAsset...", dirPath, "asset");
            if (path == "") return null; // Canceled or closed by user.

            int subLen = Application.dataPath.Length - 6;
            string assetRelativePath = path.Substring(subLen, path.Length - subLen).Replace("\\", "/");

            Object obj = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(AtlasAsset));

            if (obj == null || obj.GetType() != typeof(AtlasAsset))
                return null;

            return (AtlasAsset)obj;
        }

        private static void AddRequiredAtlasRegionsFromBinary(string skeletonDataPath, List<string> requiredPaths)
        {
            SkeletonBinary binary = new(new AtlasRequirementLoader(requiredPaths));
            TextAsset data = (TextAsset)AssetDatabase.LoadAssetAtPath(skeletonDataPath, typeof(TextAsset));
            MemoryStream input = new(data.bytes);
            binary.ReadSkeletonData(input);
            binary = null;
        }

        public static List<string> GetRequiredAtlasRegions(string skeletonDataPath)
        {
            List<string> requiredPaths = new();

            if (skeletonDataPath.Contains(".skel"))
            {
                AddRequiredAtlasRegionsFromBinary(skeletonDataPath, requiredPaths);
                return requiredPaths;
            }

            TextAsset spineJson = (TextAsset)AssetDatabase.LoadAssetAtPath(skeletonDataPath, typeof(TextAsset));

            StringReader reader = new(spineJson.text);
            Dictionary<string, object> root = Json.Deserialize(reader) as Dictionary<string, object>;

            if (!root.ContainsKey("skins"))
                return requiredPaths;

            foreach (KeyValuePair<string, object> entry in (Dictionary<string, object>)root["skins"])
            {
                foreach (KeyValuePair<string, object> slotEntry in (Dictionary<string, object>)entry.Value)
                {
                    foreach (KeyValuePair<string, object> attachmentEntry in (Dictionary<string, object>)slotEntry.Value)
                    {
                        Dictionary<string, object> data = (Dictionary<string, object>)attachmentEntry.Value;

                        // Ignore non-atlas-requiring types.
                        if (data.ContainsKey("type"))
                        {
                            AttachmentType attachmentType;
                            string typeString = (string)data["type"];
                            try
                            {
                                attachmentType = (AttachmentType)Enum.Parse(typeof(AttachmentType), typeString, true);
                            }
                            catch (ArgumentException e)
                            {
                                // For more info, visit: http://esotericsoftware.com/forum/Spine-editor-and-runtime-version-management-6534
                                Debug.LogWarning(string.Format("Unidentified Attachment type: \"{0}\". Skeleton may have been exported from an incompatible Spine version.", typeString));
                                throw e;
                            }

                            if (!AtlasTypes.Contains(attachmentType))
                                continue;
                        }

                        if (data.ContainsKey("path"))
                            requiredPaths.Add((string)data["path"]);
                        else if (data.ContainsKey("name"))
                            requiredPaths.Add((string)data["name"]);
                        else
                            requiredPaths.Add(attachmentEntry.Key);
                    }
                }
            }

            return requiredPaths;
        }

        private static AtlasAsset GetMatchingAtlas(List<string> requiredPaths, List<AtlasAsset> atlasAssets)
        {
            AtlasAsset atlasAssetMatch = null;

            foreach (AtlasAsset a in atlasAssets)
            {
                Atlas atlas = a.GetAtlas();
                bool failed = false;
                foreach (string regionPath in requiredPaths)
                {
                    if (atlas.FindRegion(regionPath) == null)
                    {
                        failed = true;
                        break;
                    }
                }

                if (!failed)
                {
                    atlasAssetMatch = a;
                    break;
                }
            }

            return atlasAssetMatch;
        }

        public class AtlasRequirementLoader : AttachmentLoader
        {
            private readonly List<string> requirementList;

            public AtlasRequirementLoader(List<string> requirementList)
            {
                this.requirementList = requirementList;
            }

            public RegionAttachment NewRegionAttachment(Skin skin, string name, string path)
            {
                requirementList.Add(path);
                return new RegionAttachment(name);
            }

            public MeshAttachment NewMeshAttachment(Skin skin, string name, string path)
            {
                requirementList.Add(path);
                return new MeshAttachment(name);
            }

            public BoundingBoxAttachment NewBoundingBoxAttachment(Skin skin, string name)
            {
                return new BoundingBoxAttachment(name);
            }

            public PathAttachment NewPathAttachment(Skin skin, string name)
            {
                return new PathAttachment(name);
            }

            public PointAttachment NewPointAttachment(Skin skin, string name)
            {
                return new PointAttachment(name);
            }

            public ClippingAttachment NewClippingAttachment(Skin skin, string name)
            {
                return new ClippingAttachment(name);
            }
        }
        #endregion

        #region Import Atlases
        private static List<AtlasAsset> FindAtlasesAtPath(string path)
        {
            List<AtlasAsset> arr = new();
            DirectoryInfo dir = new(path);
            FileInfo[] assetInfoArr = dir.GetFiles("*.asset");

            int subLen = Application.dataPath.Length - 6;
            foreach (FileInfo f in assetInfoArr)
            {
                string assetRelativePath = f.FullName.Substring(subLen, f.FullName.Length - subLen).Replace("\\", "/");
                Object obj = AssetDatabase.LoadAssetAtPath(assetRelativePath, typeof(AtlasAsset));
                if (obj != null)
                    arr.Add(obj as AtlasAsset);
            }

            return arr;
        }

        private static AtlasAsset IngestSpineAtlas(TextAsset atlasText)
        {
            if (atlasText == null)
            {
                Debug.LogWarning("Atlas source cannot be null!");
                return null;
            }

            string primaryName = Path.GetFileNameWithoutExtension(atlasText.name).Replace(".atlas", "");
            string assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(atlasText));

            string atlasPath = assetPath + "/" + primaryName + "_Atlas.asset";

            AtlasAsset atlasAsset = (AtlasAsset)AssetDatabase.LoadAssetAtPath(atlasPath, typeof(AtlasAsset));

            List<Material> vestigialMaterials = new();

            if (atlasAsset == null)
                atlasAsset = AtlasAsset.CreateInstance<AtlasAsset>();
            else
            {
                foreach (Material m in atlasAsset.materials)
                    vestigialMaterials.Add(m);
            }

            protectFromStackGarbageCollection.Add(atlasAsset);
            atlasAsset.atlasFile = atlasText;

            //strip CR
            string atlasStr = atlasText.text;
            atlasStr = atlasStr.Replace("\r", "");

            string[] atlasLines = atlasStr.Split('\n');
            List<string> pageFiles = new();
            for (int i = 0; i < atlasLines.Length - 1; i++)
            {
                if (atlasLines[i].Trim().Length == 0)
                    pageFiles.Add(atlasLines[i + 1].Trim());
            }

            List<Material> populatingMaterials = new(pageFiles.Count); //atlasAsset.materials = new Material[pageFiles.Count];

            for (int i = 0; i < pageFiles.Count; i++)
            {
                string texturePath = assetPath + "/" + pageFiles[i];
                Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D));

                if (setTextureImporterSettings)
                {
                    TextureImporter texImporter = (TextureImporter)TextureImporter.GetAtPath(texturePath);
                    if (texImporter == null)
                    {
                        Debug.LogWarning(string.Format("{0} ::: Texture asset \"{1}\" not found. Skipping. Please check your atlas file for renamed files.", atlasAsset.name, texturePath));
                        continue;
                    }

                    texImporter.textureCompression = TextureImporterCompression.Uncompressed;
                    texImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                    texImporter.mipmapEnabled = false;
                    texImporter.alphaIsTransparency = false; // Prevent the texture importer from applying bleed to the transparent parts for PMA.
                    texImporter.spriteImportMode = SpriteImportMode.None;
                    texImporter.maxTextureSize = 2048;

                    EditorUtility.SetDirty(texImporter);
                    AssetDatabase.ImportAsset(texturePath);
                    AssetDatabase.SaveAssets();
                }

                string pageName = Path.GetFileNameWithoutExtension(pageFiles[i]);

                //because this looks silly
                if (pageName == primaryName && pageFiles.Count == 1)
                    pageName = "Material";

                string materialPath = assetPath + "/" + primaryName + "_" + pageName + ".mat";
                Material mat = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));

                if (mat == null)
                {
                    mat = new Material(Shader.Find(defaultShader));
                    AssetDatabase.CreateAsset(mat, materialPath);
                }
                else
                {
                    vestigialMaterials.Remove(mat);
                }

                mat.mainTexture = texture;
                EditorUtility.SetDirty(mat);
                AssetDatabase.SaveAssets();

                populatingMaterials.Add(mat); //atlasAsset.materials[i] = mat;
            }

            atlasAsset.materials = populatingMaterials.ToArray();

            for (int i = 0; i < vestigialMaterials.Count; i++)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(vestigialMaterials[i]));

            if (AssetDatabase.GetAssetPath(atlasAsset) == "")
                AssetDatabase.CreateAsset(atlasAsset, atlasPath);
            else
                atlasAsset.Clear();

            EditorUtility.SetDirty(atlasAsset);
            AssetDatabase.SaveAssets();

            if (pageFiles.Count != atlasAsset.materials.Length)
                Debug.LogWarning(string.Format(
                    "{0} ::: Not all atlas pages were imported. If you rename your image files, please make sure you also edit the filenames specified in the atlas file.", atlasAsset.name));
            else
                Debug.Log(string.Format("{0} ::: Imported with {1} material", atlasAsset.name, atlasAsset.materials.Length));

            // Iterate regions and bake marked.
            Atlas atlas = atlasAsset.GetAtlas();
            FieldInfo field = typeof(Atlas).GetField("regions", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.NonPublic);
            List<AtlasRegion> regions = (List<AtlasRegion>)field.GetValue(atlas);
            string atlasAssetPath = AssetDatabase.GetAssetPath(atlasAsset);
            string atlasAssetDirPath = Path.GetDirectoryName(atlasAssetPath);
            string bakedDirPath = Path.Combine(atlasAssetDirPath, atlasAsset.name);

            bool hasBakedRegions = false;
            for (int i = 0; i < regions.Count; i++)
            {
                AtlasRegion region = regions[i];
                string bakedPrefabPath = Path.Combine(bakedDirPath, GetPathSafeName(region.name) + ".prefab").Replace("\\", "/");
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(bakedPrefabPath, typeof(GameObject));
                if (prefab != null)
                {
                    BakeRegion(atlasAsset, region, false);
                    hasBakedRegions = true;
                }
            }

            if (hasBakedRegions)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            protectFromStackGarbageCollection.Remove(atlasAsset);
            return (AtlasAsset)AssetDatabase.LoadAssetAtPath(atlasPath, typeof(AtlasAsset));
        }
        #endregion

        #region Bake Atlas Region
        public static GameObject BakeRegion(AtlasAsset atlasAsset, AtlasRegion region, bool autoSave = true)
        {
            Atlas atlas = atlasAsset.GetAtlas();
            string atlasAssetPath = AssetDatabase.GetAssetPath(atlasAsset);
            string atlasAssetDirPath = Path.GetDirectoryName(atlasAssetPath);
            string bakedDirPath = Path.Combine(atlasAssetDirPath, atlasAsset.name);
            string bakedPrefabPath = Path.Combine(bakedDirPath, GetPathSafeName(region.name) + ".prefab").Replace("\\", "/");

            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(bakedPrefabPath, typeof(GameObject));
            GameObject root;
            Mesh mesh;
            bool isNewPrefab = false;

            if (!Directory.Exists(bakedDirPath))
                Directory.CreateDirectory(bakedDirPath);

            if (prefab == null)
            {
                root = new GameObject("temp", typeof(MeshFilter), typeof(MeshRenderer));
                prefab = PrefabUtility.SaveAsPrefabAsset(root, bakedPrefabPath);
                isNewPrefab = true;
                Object.DestroyImmediate(root);
            }

            mesh = (Mesh)AssetDatabase.LoadAssetAtPath(bakedPrefabPath, typeof(Mesh));

            Material mat = null;
            mesh = atlasAsset.GenerateMesh(region.name, mesh, out mat);
            if (isNewPrefab)
            {
                AssetDatabase.AddObjectToAsset(mesh, prefab);
                prefab.GetComponent<MeshFilter>().sharedMesh = mesh;
            }

            EditorUtility.SetDirty(mesh);
            EditorUtility.SetDirty(prefab);

            if (autoSave)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            prefab.GetComponent<MeshRenderer>().sharedMaterial = mat;

            return prefab;
        }
        #endregion

        #region Import SkeletonData (json or binary)
        public const string SkeletonDataSuffix = "_SkeletonData";

        private static SkeletonDataAsset IngestSpineProject(TextAsset spineJson, params AtlasAsset[] atlasAssets)
        {
            string primaryName = Path.GetFileNameWithoutExtension(spineJson.name);
            string assetPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(spineJson));
            string filePath = assetPath + "/" + primaryName + SkeletonDataSuffix + ".asset";

#if SPINE_TK2D
			if (spineJson != null) {
				SkeletonDataAsset skeletonDataAsset = (SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(filePath, typeof(SkeletonDataAsset));
				if (skeletonDataAsset == null) {
					skeletonDataAsset = SkeletonDataAsset.CreateInstance<SkeletonDataAsset>();
					skeletonDataAsset.skeletonJSON = spineJson;
					skeletonDataAsset.fromAnimation = new string[0];
					skeletonDataAsset.toAnimation = new string[0];
					skeletonDataAsset.duration = new float[0];
					skeletonDataAsset.defaultMix = defaultMix;
					skeletonDataAsset.scale = defaultScale;

					AssetDatabase.CreateAsset(skeletonDataAsset, filePath);
					AssetDatabase.SaveAssets();
				} else {
					skeletonDataAsset.Clear();
					skeletonDataAsset.GetSkeletonData(true);
				}

				return skeletonDataAsset;
			} else {
				EditorUtility.DisplayDialog("Error!", "Tried to ingest null Spine data.", "OK");
				return null;
			}

#else
            if (spineJson != null && atlasAssets != null)
            {
                SkeletonDataAsset skeletonDataAsset = (SkeletonDataAsset)AssetDatabase.LoadAssetAtPath(filePath, typeof(SkeletonDataAsset));
                if (skeletonDataAsset == null)
                {
                    skeletonDataAsset = ScriptableObject.CreateInstance<SkeletonDataAsset>();
                    {
                        skeletonDataAsset.atlasAssets = atlasAssets;
                        skeletonDataAsset.skeletonJSON = spineJson;
                        skeletonDataAsset.defaultMix = defaultMix;
                        skeletonDataAsset.scale = defaultScale;
                    }

                    AssetDatabase.CreateAsset(skeletonDataAsset, filePath);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    skeletonDataAsset.atlasAssets = atlasAssets;
                    skeletonDataAsset.Clear();
                    skeletonDataAsset.GetSkeletonData(true);
                }

                return skeletonDataAsset;
            }

            EditorUtility.DisplayDialog("Error!", "Must specify both Spine JSON and AtlasAsset array", "OK");
            return null;
#endif
        }
        #endregion

        #region SkeletonDataFileValidator
        internal static class SkeletonDataFileValidator
        {
            private static readonly int[][] compatibleBinaryVersions = { new[] { 3, 6, 0 }, new[] { 3, 5, 0 } };
            private static readonly int[][] compatibleJsonVersions = { new[] { 3, 6, 0 }, new[] { 3, 7, 0 }, new[] { 3, 5, 0 } };
            //static bool isFixVersionRequired = false;

            public static bool CheckForValidSkeletonData(string skeletonJSONPath)
            {
                string dir = Path.GetDirectoryName(skeletonJSONPath);
                TextAsset textAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(skeletonJSONPath, typeof(TextAsset));
                DirectoryInfo dirInfo = new(dir);
                FileInfo[] files = dirInfo.GetFiles("*.asset");

                foreach (FileInfo path in files)
                {
                    string localPath = dir + "/" + path.Name;
                    Object obj = AssetDatabase.LoadAssetAtPath(localPath, typeof(Object));
                    SkeletonDataAsset skeletonDataAsset = obj as SkeletonDataAsset;
                    if (skeletonDataAsset != null && skeletonDataAsset.skeletonJSON == textAsset)
                        return true;
                }

                return false;
            }

            public static bool IsSpineData(TextAsset asset)
            {
                if (asset == null)
                    return false;

                bool isSpineData = false;
                string rawVersion = null;

                int[][] compatibleVersions;
                if (asset.name.Contains(".skel"))
                {
                    try
                    {
                        rawVersion = SkeletonBinary.GetVersionString(new MemoryStream(asset.bytes));
                        isSpineData = !string.IsNullOrEmpty(rawVersion);
                        compatibleVersions = compatibleBinaryVersions;
                    }
                    catch (Exception e)
                    {
                        Debug.LogErrorFormat("Failed to read '{0}'. It is likely not a binary Spine SkeletonData file.\n{1}", asset.name, e);
                        return false;
                    }
                }
                else
                {
                    object obj = Json.Deserialize(new StringReader(asset.text));
                    if (obj == null)
                    {
                        Debug.LogErrorFormat("'{0}' is not valid JSON.", asset.name);
                        return false;
                    }

                    Dictionary<string, object> root = obj as Dictionary<string, object>;
                    if (root == null)
                    {
                        Debug.LogError("Parser returned an incorrect type.");
                        return false;
                    }

                    isSpineData = root.ContainsKey("skeleton");
                    if (isSpineData)
                    {
                        Dictionary<string, object> skeletonInfo = (Dictionary<string, object>)root["skeleton"];
                        object jv;
                        skeletonInfo.TryGetValue("spine", out jv);
                        rawVersion = jv as string;
                    }

                    compatibleVersions = compatibleJsonVersions;
                }

                // Version warning
                if (isSpineData)
                {
                    string primaryRuntimeVersionDebugString = compatibleVersions[0][0] + "." + compatibleVersions[0][1];

                    if (string.IsNullOrEmpty(rawVersion))
                    {
                        Debug.LogWarningFormat("Skeleton '{0}' has no version information. It may be incompatible with your runtime version: spine-unity v{1}", asset.name,
                            primaryRuntimeVersionDebugString);
                    }
                    else
                    {
                        string[] versionSplit = rawVersion.Split('.');
                        bool match = false;
                        foreach (int[] version in compatibleVersions)
                        {
                            bool primaryMatch = version[0] == int.Parse(versionSplit[0]);
                            bool secondaryMatch = version[1] == int.Parse(versionSplit[1]);

                            // if (isFixVersionRequired) secondaryMatch &= version[2] <= int.Parse(jsonVersionSplit[2]);

                            if (primaryMatch && secondaryMatch)
                            {
                                match = true;
                                break;
                            }
                        }

                        if (!match)
                            Debug.LogWarningFormat("Skeleton '{0}' (exported with Spine {1}) may be incompatible with your runtime version: spine-unity v{2}", asset.name, rawVersion,
                                primaryRuntimeVersionDebugString);
                    }
                }

                return isSpineData;
            }
        }
        #endregion

        #region SkeletonAnimation Menu
        public static void IngestAdvancedRenderSettings(SkeletonRenderer skeletonRenderer)
        {
            const string PMAShaderQuery = "Spine/Skeleton";
            const string TintBlackShaderQuery = "Tint Black";

            if (skeletonRenderer == null) return;
            SkeletonDataAsset skeletonDataAsset = skeletonRenderer.skeletonDataAsset;
            if (skeletonDataAsset == null) return;

            bool pmaVertexColors = false;
            bool tintBlack = false;
            foreach (AtlasAsset atlasAsset in skeletonDataAsset.atlasAssets)
            {
                if (!pmaVertexColors)
                {
                    foreach (Material m in atlasAsset.materials)
                    {
                        if (m.shader.name.Contains(PMAShaderQuery))
                        {
                            pmaVertexColors = true;
                            break;
                        }
                    }
                }

                if (!tintBlack)
                {
                    foreach (Material m in atlasAsset.materials)
                    {
                        if (m.shader.name.Contains(TintBlackShaderQuery))
                        {
                            tintBlack = true;
                            break;
                        }
                    }
                }
            }

            skeletonRenderer.pmaVertexColors = pmaVertexColors;
            skeletonRenderer.tintBlack = tintBlack;
        }

        public static SkeletonAnimation InstantiateSkeletonAnimation(SkeletonDataAsset skeletonDataAsset, string skinName, bool destroyInvalid = true)
        {
            SkeletonData skeletonData = skeletonDataAsset.GetSkeletonData(true);
            Skin skin = skeletonData != null ? skeletonData.FindSkin(skinName) : null;
            return InstantiateSkeletonAnimation(skeletonDataAsset, skin, destroyInvalid);
        }

        public static SkeletonAnimation InstantiateSkeletonAnimation(SkeletonDataAsset skeletonDataAsset, Skin skin = null, bool destroyInvalid = true)
        {
            SkeletonData data = skeletonDataAsset.GetSkeletonData(true);

            if (data == null)
            {
                for (int i = 0; i < skeletonDataAsset.atlasAssets.Length; i++)
                {
                    string reloadAtlasPath = AssetDatabase.GetAssetPath(skeletonDataAsset.atlasAssets[i]);
                    skeletonDataAsset.atlasAssets[i] = (AtlasAsset)AssetDatabase.LoadAssetAtPath(reloadAtlasPath, typeof(AtlasAsset));
                }

                data = skeletonDataAsset.GetSkeletonData(false);
            }

            if (data == null)
            {
                Debug.LogWarning("InstantiateSkeletonAnimation tried to instantiate a skeleton from an invalid SkeletonDataAsset.");
                return null;
            }

            string spineGameObjectName = string.Format("Spine GameObject ({0})", skeletonDataAsset.name.Replace("_SkeletonData", ""));
            GameObject go = new(spineGameObjectName, typeof(MeshFilter), typeof(MeshRenderer), typeof(SkeletonAnimation));
            SkeletonAnimation newSkeletonAnimation = go.GetComponent<SkeletonAnimation>();
            newSkeletonAnimation.skeletonDataAsset = skeletonDataAsset;
            IngestAdvancedRenderSettings(newSkeletonAnimation);

            try
            {
                newSkeletonAnimation.Initialize(false);
            }
            catch (Exception e)
            {
                if (destroyInvalid)
                {
                    Debug.LogWarning("Editor-instantiated SkeletonAnimation threw an Exception. Destroying GameObject to prevent orphaned GameObject.");
                    GameObject.DestroyImmediate(go);
                }

                Debug.Log(e);
            }

            // Set Defaults
            bool noSkins = data.DefaultSkin == null && (data.Skins == null || data.Skins.Count == 0); // Support attachmentless/skinless SkeletonData.
            skin = skin ?? data.DefaultSkin ?? (noSkins ? null : data.Skins.Items[0]);
            if (skin != null)
            {
                newSkeletonAnimation.initialSkinName = skin.Name;
                newSkeletonAnimation.skeleton.SetSkin(skin);
            }

            newSkeletonAnimation.zSpacing = defaultZSpacing;

            newSkeletonAnimation.skeleton.Update(0);
            newSkeletonAnimation.state.Update(0);
            newSkeletonAnimation.state.Apply(newSkeletonAnimation.skeleton);
            newSkeletonAnimation.skeleton.UpdateWorldTransform();

            return newSkeletonAnimation;
        }
        #endregion

        #region SkeletonAnimator
#if SPINE_SKELETONANIMATOR
        private static void UpdateMecanimClips(SkeletonDataAsset skeletonDataAsset)
        {
            if (skeletonDataAsset.controller == null)
                return;

            SkeletonBaker.GenerateMecanimAnimationClips(skeletonDataAsset);
        }

        public static SkeletonAnimator InstantiateSkeletonAnimator(SkeletonDataAsset skeletonDataAsset, string skinName)
        {
            return InstantiateSkeletonAnimator(skeletonDataAsset, skeletonDataAsset.GetSkeletonData(true).FindSkin(skinName));
        }

        public static SkeletonAnimator InstantiateSkeletonAnimator(SkeletonDataAsset skeletonDataAsset, Skin skin = null)
        {
            string spineGameObjectName = string.Format("Spine Mecanim GameObject ({0})", skeletonDataAsset.name.Replace("_SkeletonData", ""));
            GameObject go = new(spineGameObjectName, typeof(MeshFilter), typeof(MeshRenderer), typeof(Animator), typeof(SkeletonAnimator));

            if (skeletonDataAsset.controller == null)
            {
                SkeletonBaker.GenerateMecanimAnimationClips(skeletonDataAsset);
                Debug.Log(string.Format("Mecanim controller was automatically generated and assigned for {0}", skeletonDataAsset.name));
            }

            go.GetComponent<Animator>().runtimeAnimatorController = skeletonDataAsset.controller;

            SkeletonAnimator anim = go.GetComponent<SkeletonAnimator>();
            anim.skeletonDataAsset = skeletonDataAsset;
            IngestAdvancedRenderSettings(anim);

            SkeletonData data = skeletonDataAsset.GetSkeletonData(true);
            if (data == null)
            {
                for (int i = 0; i < skeletonDataAsset.atlasAssets.Length; i++)
                {
                    string reloadAtlasPath = AssetDatabase.GetAssetPath(skeletonDataAsset.atlasAssets[i]);
                    skeletonDataAsset.atlasAssets[i] = (AtlasAsset)AssetDatabase.LoadAssetAtPath(reloadAtlasPath, typeof(AtlasAsset));
                }

                data = skeletonDataAsset.GetSkeletonData(true);
            }

            // Set defaults
            skin = skin ?? data.DefaultSkin ?? data.Skins.Items[0];
            anim.zSpacing = defaultZSpacing;

            anim.Initialize(false);
            anim.skeleton.SetSkin(skin);
            anim.initialSkinName = skin.Name;

            anim.skeleton.Update(0);
            anim.skeleton.UpdateWorldTransform();
            anim.LateUpdate();

            return anim;
        }
#endif
        #endregion

        #region SpineTK2DEditorUtility
        internal static class SpineTK2DEditorUtility
        {
            private const string SPINE_TK2D_DEFINE = "SPINE_TK2D";

            private static bool IsInvalidGroup(BuildTargetGroup group)
            {
                int gi = (int)group;
                return
                    gi == 15 || gi == 16
                             ||
                             group == BuildTargetGroup.Unknown;
            }

            internal static void EnableTK2D()
            {
                bool added = false;
                foreach (BuildTargetGroup group in Enum.GetValues(typeof(BuildTargetGroup)))
                {
                    if (IsInvalidGroup(group))
                        continue;

                    string defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group));
                    if (!defines.Contains(SPINE_TK2D_DEFINE))
                    {
                        added = true;
                        if (defines.EndsWith(";", StringComparison.Ordinal))
                            defines = defines + SPINE_TK2D_DEFINE;
                        else
                            defines = defines + ";" + SPINE_TK2D_DEFINE;

                        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group), defines);
                    }
                }

                if (added)
                {
                    Debug.LogWarning("Setting Scripting Define Symbol " + SPINE_TK2D_DEFINE);
                }
                else
                {
                    Debug.LogWarning("Already Set Scripting Define Symbol " + SPINE_TK2D_DEFINE);
                }
            }


            internal static void DisableTK2D()
            {
                bool removed = false;
                foreach (BuildTargetGroup group in Enum.GetValues(typeof(BuildTargetGroup)))
                {
                    if (IsInvalidGroup(group))
                        continue;

                    string defines = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group));
                    if (defines.Contains(SPINE_TK2D_DEFINE))
                    {
                        removed = true;
                        if (defines.Contains(SPINE_TK2D_DEFINE + ";"))
                            defines = defines.Replace(SPINE_TK2D_DEFINE + ";", "");
                        else
                            defines = defines.Replace(SPINE_TK2D_DEFINE, "");

                        PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(group), defines);
                    }
                }

                if (removed)
                {
                    Debug.LogWarning("Removing Scripting Define Symbol " + SPINE_TK2D_DEFINE);
                }
                else
                {
                    Debug.LogWarning("Already Removed Scripting Define Symbol " + SPINE_TK2D_DEFINE);
                }
            }
        }
        #endregion

        public static string GetPathSafeName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
            {
                // Doesn't handle more obscure file name limitations.
                name = name.Replace(c, '_');
            }

            return name;
        }
    }

    public static class SpineHandles
    {
        internal static float handleScale = 1f;

        public static Color BoneColor { get => new(0.8f, 0.8f, 0.8f, 0.4f); }

        public static Color PathColor { get => new(254 / 255f, 127 / 255f, 0); }

        public static Color TransformContraintColor { get => new(170 / 255f, 226 / 255f, 35 / 255f); }

        public static Color IkColor { get => new(228 / 255f, 90 / 255f, 43 / 255f); }

        public static Color PointColor { get => new(1f, 1f, 0f, 1f); }

        private static readonly Vector3[] _boneMeshVerts =
        {
            new(0, 0, 0),
            new(0.1f, 0.1f, 0),
            new(1, 0, 0),
            new(0.1f, -0.1f, 0)
        };
        private static Mesh _boneMesh;

        public static Mesh BoneMesh
        {
            get
            {
                if (_boneMesh == null)
                {
                    _boneMesh = new Mesh
                    {
                        vertices = _boneMeshVerts,
                        uv = new Vector2[4],
                        triangles = new[] { 0, 1, 2, 2, 3, 0 }
                    };
                    _boneMesh.RecalculateBounds();
                    _boneMesh.RecalculateNormals();
                }

                return _boneMesh;
            }
        }

        private static Mesh _arrowheadMesh;

        public static Mesh ArrowheadMesh
        {
            get
            {
                if (_arrowheadMesh == null)
                {
                    _arrowheadMesh = new Mesh
                    {
                        vertices = new[]
                        {
                            new Vector3(0, 0),
                            new Vector3(-0.1f, 0.05f),
                            new Vector3(-0.1f, -0.05f)
                        },
                        uv = new Vector2[3],
                        triangles = new[] { 0, 1, 2 }
                    };
                    _arrowheadMesh.RecalculateBounds();
                    _arrowheadMesh.RecalculateNormals();
                }

                return _arrowheadMesh;
            }
        }

        private static Material _boneMaterial;

        private static Material BoneMaterial
        {
            get
            {
                if (_boneMaterial == null)
                {
                    _boneMaterial = new Material(Shader.Find("Hidden/Spine/Bones"));
                    _boneMaterial.SetColor("_Color", BoneColor);
                }

                return _boneMaterial;
            }
        }

        public static Material GetBoneMaterial()
        {
            BoneMaterial.SetColor("_Color", BoneColor);
            return BoneMaterial;
        }

        public static Material GetBoneMaterial(Color color)
        {
            BoneMaterial.SetColor("_Color", color);
            return BoneMaterial;
        }

        private static Material _ikMaterial;

        public static Material IKMaterial
        {
            get
            {
                if (_ikMaterial == null)
                {
                    _ikMaterial = new Material(Shader.Find("Hidden/Spine/Bones"));
                    _ikMaterial.SetColor("_Color", IkColor);
                }

                return _ikMaterial;
            }
        }

        private static GUIStyle _boneNameStyle;

        public static GUIStyle BoneNameStyle
        {
            get
            {
                if (_boneNameStyle == null)
                {
                    _boneNameStyle = new GUIStyle(EditorStyles.whiteMiniLabel)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        stretchWidth = true,
                        padding = new RectOffset(0, 0, 0, 0),
                        contentOffset = new Vector2(-5f, 0f)
                    };
                }

                return _boneNameStyle;
            }
        }

        private static GUIStyle _pathNameStyle;

        public static GUIStyle PathNameStyle
        {
            get
            {
                if (_pathNameStyle == null)
                {
                    _pathNameStyle = new GUIStyle(BoneNameStyle);
                    _pathNameStyle.normal.textColor = PathColor;
                }

                return _pathNameStyle;
            }
        }

        private static GUIStyle _pointNameStyle;

        public static GUIStyle PointNameStyle
        {
            get
            {
                if (_pointNameStyle == null)
                {
                    _pointNameStyle = new GUIStyle(BoneNameStyle);
                    _pointNameStyle.normal.textColor = PointColor;
                }

                return _pointNameStyle;
            }
        }

        public static void DrawBoneNames(Transform transform, Skeleton skeleton, float positionScale = 1f)
        {
            GUIStyle style = BoneNameStyle;
            foreach (Bone b in skeleton.Bones)
            {
                Vector3 pos = new Vector3(b.WorldX * positionScale, b.WorldY * positionScale, 0) + new Vector3(b.A, b.C) * (b.Data.Length * 0.5f);
                pos = transform.TransformPoint(pos);
                Handles.Label(pos, b.Data.Name, style);
            }
        }

        public static void DrawBones(Transform transform, Skeleton skeleton, float positionScale = 1f)
        {
            float boneScale = 1.8f; // Draw the root bone largest;
            DrawCrosshairs2D(skeleton.Bones.Items[0].GetWorldPosition(transform), 0.08f, positionScale);

            foreach (Bone b in skeleton.Bones)
            {
                DrawBone(transform, b, boneScale, positionScale);
                boneScale = 1f;
            }
        }

        private static readonly Vector3[] _boneWireBuffer = new Vector3[5];

        private static Vector3[] GetBoneWireBuffer(Matrix4x4 m)
        {
            for (int i = 0, n = _boneMeshVerts.Length; i < n; i++)
                _boneWireBuffer[i] = m.MultiplyPoint(_boneMeshVerts[i]);

            _boneWireBuffer[4] = _boneWireBuffer[0]; // closed polygon.
            return _boneWireBuffer;
        }

        public static void DrawBoneWireframe(Transform transform, Bone b, Color color, float skeletonRenderScale = 1f)
        {
            Handles.color = color;
            Vector3 pos = new(b.WorldX * skeletonRenderScale, b.WorldY * skeletonRenderScale, 0);
            float length = b.Data.Length;

            if (length > 0)
            {
                Quaternion rot = Quaternion.Euler(0, 0, b.WorldRotationX);
                Vector3 scale = Vector3.one * length * b.WorldScaleX * skeletonRenderScale;
                const float my = 1.5f;
                scale.y *= (handleScale + 1) * 0.5f;
                scale.y = Mathf.Clamp(scale.x, -my * skeletonRenderScale, my * skeletonRenderScale);
                Handles.DrawPolyLine(GetBoneWireBuffer(transform.localToWorldMatrix * Matrix4x4.TRS(pos, rot, scale)));
                Vector3 wp = transform.TransformPoint(pos);
                DrawBoneCircle(wp, color, transform.forward, skeletonRenderScale);
            }
            else
            {
                Vector3 wp = transform.TransformPoint(pos);
                DrawBoneCircle(wp, color, transform.forward, skeletonRenderScale);
            }
        }

        public static void DrawBone(Transform transform, Bone b, float boneScale, float skeletonRenderScale = 1f)
        {
            Vector3 pos = new(b.WorldX * skeletonRenderScale, b.WorldY * skeletonRenderScale, 0);
            float length = b.Data.Length;
            if (length > 0)
            {
                Quaternion rot = Quaternion.Euler(0, 0, b.WorldRotationX);
                Vector3 scale = Vector3.one * length * b.WorldScaleX * skeletonRenderScale;
                const float my = 1.5f;
                scale.y *= (handleScale + 1f) * 0.5f;
                scale.y = Mathf.Clamp(scale.x, -my * skeletonRenderScale, my * skeletonRenderScale);
                GetBoneMaterial().SetPass(0);
                Graphics.DrawMeshNow(BoneMesh, transform.localToWorldMatrix * Matrix4x4.TRS(pos, rot, scale));
            }
            else
            {
                Vector3 wp = transform.TransformPoint(pos);
                DrawBoneCircle(wp, BoneColor, transform.forward, boneScale * skeletonRenderScale);
            }
        }

        public static void DrawBone(Transform transform, Bone b, float boneScale, Color color, float skeletonRenderScale = 1f)
        {
            Vector3 pos = new(b.WorldX * skeletonRenderScale, b.WorldY * skeletonRenderScale, 0);
            float length = b.Data.Length;
            if (length > 0)
            {
                Quaternion rot = Quaternion.Euler(0, 0, b.WorldRotationX);
                Vector3 scale = Vector3.one * length * b.WorldScaleX;
                const float my = 1.5f;
                scale.y *= (handleScale + 1f) * 0.5f;
                scale.y = Mathf.Clamp(scale.x, -my, my);
                GetBoneMaterial(color).SetPass(0);
                Graphics.DrawMeshNow(BoneMesh, transform.localToWorldMatrix * Matrix4x4.TRS(pos, rot, scale));
            }
            else
            {
                Vector3 wp = transform.TransformPoint(pos);
                DrawBoneCircle(wp, color, transform.forward, boneScale * skeletonRenderScale);
            }
        }

        public static void DrawPaths(Transform transform, Skeleton skeleton)
        {
            foreach (Slot s in skeleton.DrawOrder)
            {
                PathAttachment p = s.Attachment as PathAttachment;
                if (p != null) DrawPath(s, p, transform, true);
            }
        }

        private static float[] pathVertexBuffer;

        public static void DrawPath(Slot s, PathAttachment p, Transform t, bool includeName)
        {
            int worldVerticesLength = p.WorldVerticesLength;

            if (pathVertexBuffer == null || pathVertexBuffer.Length < worldVerticesLength)
                pathVertexBuffer = new float[worldVerticesLength];

            float[] pv = pathVertexBuffer;
            p.ComputeWorldVertices(s, pv);

            Color ocolor = Handles.color;
            Handles.color = PathColor;

            Matrix4x4 m = t.localToWorldMatrix;
            const int step = 6;
            int n = worldVerticesLength - step;
            Vector3 p0, p1, p2, p3;
            for (int i = 2; i < n; i += step)
            {
                p0 = m.MultiplyPoint(new Vector3(pv[i], pv[i + 1]));
                p1 = m.MultiplyPoint(new Vector3(pv[i + 2], pv[i + 3]));
                p2 = m.MultiplyPoint(new Vector3(pv[i + 4], pv[i + 5]));
                p3 = m.MultiplyPoint(new Vector3(pv[i + 6], pv[i + 7]));
                DrawCubicBezier(p0, p1, p2, p3);
            }

            n += step;
            if (p.Closed)
            {
                p0 = m.MultiplyPoint(new Vector3(pv[n - 4], pv[n - 3]));
                p1 = m.MultiplyPoint(new Vector3(pv[n - 2], pv[n - 1]));
                p2 = m.MultiplyPoint(new Vector3(pv[0], pv[1]));
                p3 = m.MultiplyPoint(new Vector3(pv[2], pv[3]));
                DrawCubicBezier(p0, p1, p2, p3);
            }

            const float endCapSize = 0.05f;
            Vector3 firstPoint = m.MultiplyPoint(new Vector3(pv[2], pv[3]));
            DrawDot(firstPoint, endCapSize);

            //if (!p.Closed) SpineHandles.DrawDot(m.MultiplyPoint(new Vector3(pv[n - 4], pv[n - 3])), endCapSize);
            if (includeName) Handles.Label(firstPoint + new Vector3(0, 0.1f), p.Name, PathNameStyle);

            Handles.color = ocolor;
        }

        public static void DrawDot(Vector3 position, float size)
        {
            Handles.DotHandleCap(0, position, Quaternion.identity, size * HandleUtility.GetHandleSize(position),
                EventType.Ignore); //Handles.DotCap(0, position, Quaternion.identity, size * HandleUtility.GetHandleSize(position));			
        }

        public static void DrawBoundingBoxes(Transform transform, Skeleton skeleton)
        {
            foreach (Slot slot in skeleton.Slots)
            {
                BoundingBoxAttachment bba = slot.Attachment as BoundingBoxAttachment;
                if (bba != null) DrawBoundingBox(slot, bba, transform);
            }
        }

        public static void DrawBoundingBox(Slot slot, BoundingBoxAttachment box, Transform t)
        {
            if (box.Vertices.Length <= 2) return; // Handle cases where user creates a BoundingBoxAttachment but doesn't actually define it.

            float[] worldVerts = new float[box.WorldVerticesLength];
            box.ComputeWorldVertices(slot, worldVerts);

            Handles.color = Color.green;
            Vector3 lastVert = Vector3.zero;
            Vector3 vert = Vector3.zero;
            Vector3 firstVert = t.TransformPoint(new Vector3(worldVerts[0], worldVerts[1], 0));
            for (int i = 0; i < worldVerts.Length; i += 2)
            {
                vert.x = worldVerts[i];
                vert.y = worldVerts[i + 1];
                vert.z = 0;

                vert = t.TransformPoint(vert);

                if (i > 0)
                    Handles.DrawLine(lastVert, vert);

                lastVert = vert;
            }

            Handles.DrawLine(lastVert, firstVert);
        }

        public static void DrawPointAttachment(Bone bone, PointAttachment pointAttachment, Transform skeletonTransform)
        {
            if (bone == null) return;
            if (pointAttachment == null) return;

            Vector2 localPos;
            pointAttachment.ComputeWorldPosition(bone, out localPos.x, out localPos.y);
            float localRotation = pointAttachment.ComputeWorldRotation(bone);
            Matrix4x4 m = Matrix4x4.TRS(localPos, Quaternion.Euler(0, 0, localRotation), Vector3.one) * Matrix4x4.TRS(Vector3.right * 0.25f, Quaternion.identity, Vector3.one);

            DrawBoneCircle(skeletonTransform.TransformPoint(localPos), PointColor, Vector3.back, 1.3f);
            DrawArrowhead(skeletonTransform.localToWorldMatrix * m);
        }

        public static void DrawConstraints(Transform transform, Skeleton skeleton, float skeletonRenderScale = 1f)
        {
            Vector3 targetPos;
            Vector3 pos;
            bool active;
            Color handleColor;
            const float Thickness = 4f;
            Vector3 normal = transform.forward;

            // Transform Constraints
            handleColor = TransformContraintColor;
            foreach (TransformConstraint tc in skeleton.TransformConstraints)
            {
                Bone targetBone = tc.Target;
                targetPos = targetBone.GetWorldPosition(transform, skeletonRenderScale);

                if (tc.TranslateMix > 0)
                {
                    if (tc.TranslateMix != 1f)
                    {
                        Handles.color = handleColor;
                        foreach (Bone b in tc.Bones)
                        {
                            pos = b.GetWorldPosition(transform, skeletonRenderScale);
                            Handles.DrawDottedLine(targetPos, pos, Thickness);
                        }
                    }

                    DrawBoneCircle(targetPos, handleColor, normal, 1.3f * skeletonRenderScale);
                    Handles.color = handleColor;
                    DrawCrosshairs(targetPos, 0.2f, targetBone.A, targetBone.B, targetBone.C, targetBone.D, transform, skeletonRenderScale);
                }
            }

            // IK Constraints
            handleColor = IkColor;
            foreach (IkConstraint ikc in skeleton.IkConstraints)
            {
                Bone targetBone = ikc.Target;
                targetPos = targetBone.GetWorldPosition(transform, skeletonRenderScale);
                ExposedList<Bone> bones = ikc.Bones;
                active = ikc.Mix > 0;
                if (active)
                {
                    pos = bones.Items[0].GetWorldPosition(transform, skeletonRenderScale);
                    switch (bones.Count)
                    {
                        case 1:
                        {
                            Handles.color = handleColor;
                            Handles.DrawLine(targetPos, pos);
                            DrawBoneCircle(targetPos, handleColor, normal);
                            Matrix4x4 m = bones.Items[0].GetMatrix4x4();
                            m.m03 = targetBone.WorldX * skeletonRenderScale;
                            m.m13 = targetBone.WorldY * skeletonRenderScale;
                            DrawArrowhead(transform.localToWorldMatrix * m);
                            break;
                        }
                        case 2:
                        {
                            Bone childBone = bones.Items[1];
                            Vector3 child = childBone.GetWorldPosition(transform, skeletonRenderScale);
                            Handles.color = handleColor;
                            Handles.DrawLine(child, pos);
                            Handles.DrawLine(targetPos, child);
                            DrawBoneCircle(pos, handleColor, normal, 0.5f);
                            DrawBoneCircle(child, handleColor, normal, 0.5f);
                            DrawBoneCircle(targetPos, handleColor, normal);
                            Matrix4x4 m = childBone.GetMatrix4x4();
                            m.m03 = targetBone.WorldX * skeletonRenderScale;
                            m.m13 = targetBone.WorldY * skeletonRenderScale;
                            DrawArrowhead(transform.localToWorldMatrix * m);
                            break;
                        }
                    }
                }
                //Handles.Label(targetPos, ikc.Data.Name, SpineHandles.BoneNameStyle);
            }

            // Path Constraints
            handleColor = PathColor;
            foreach (PathConstraint pc in skeleton.PathConstraints)
            {
                active = pc.TranslateMix > 0;
                if (active)
                    foreach (Bone b in pc.Bones)
                        DrawBoneCircle(b.GetWorldPosition(transform, skeletonRenderScale), handleColor, normal, 1f * skeletonRenderScale);
            }
        }

        private static void DrawCrosshairs2D(Vector3 position, float scale, float skeletonRenderScale = 1f)
        {
            scale *= handleScale * skeletonRenderScale;
            Handles.DrawLine(position + new Vector3(-scale, 0), position + new Vector3(scale, 0));
            Handles.DrawLine(position + new Vector3(0, -scale), position + new Vector3(0, scale));
        }

        private static void DrawCrosshairs(Vector3 position, float scale, float a, float b, float c, float d, Transform transform, float skeletonRenderScale = 1f)
        {
            scale *= handleScale * skeletonRenderScale;

            Vector3 xOffset = new Vector2(a, c).normalized * scale;
            Vector3 yOffset = new Vector2(b, d).normalized * scale;
            xOffset = transform.TransformDirection(xOffset);
            yOffset = transform.TransformDirection(yOffset);

            Handles.DrawLine(position + xOffset, position - xOffset);
            Handles.DrawLine(position + yOffset, position - yOffset);
        }

        private static void DrawArrowhead2D(Vector3 pos, float localRotation, float scale = 1f)
        {
            scale *= handleScale;

            IKMaterial.SetPass(0);
            Graphics.DrawMeshNow(ArrowheadMesh, Matrix4x4.TRS(pos, Quaternion.Euler(0, 0, localRotation), new Vector3(scale, scale, scale)));
        }

        private static void DrawArrowhead(Vector3 pos, Quaternion worldQuaternion)
        {
            Graphics.DrawMeshNow(ArrowheadMesh, pos, worldQuaternion, 0);
        }

        private static void DrawArrowhead(Matrix4x4 m)
        {
            float s = handleScale;
            m.m00 *= s;
            m.m01 *= s;
            m.m02 *= s;
            m.m10 *= s;
            m.m11 *= s;
            m.m12 *= s;
            m.m20 *= s;
            m.m21 *= s;
            m.m22 *= s;

            IKMaterial.SetPass(0);
            Graphics.DrawMeshNow(ArrowheadMesh, m);
        }

        private static void DrawBoneCircle(Vector3 pos, Color outlineColor, Vector3 normal, float scale = 1f)
        {
            scale *= handleScale;

            Color o = Handles.color;
            Handles.color = outlineColor;
            float firstScale = 0.08f * scale;
            Handles.DrawSolidDisc(pos, normal, firstScale);
            const float Thickness = 0.03f;
            float secondScale = firstScale - Thickness * handleScale * scale;

            if (secondScale > 0f)
            {
                Handles.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                Handles.DrawSolidDisc(pos, normal, secondScale);
            }

            Handles.color = o;
        }

        internal static void DrawCubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Handles.DrawBezier(p0, p3, p1, p2, Handles.color, Texture2D.whiteTexture, 2f);
            //			const float dotSize = 0.01f;
            //			Quaternion q = Quaternion.identity;
            //			Handles.DotCap(0, p0, q, dotSize);
            //			Handles.DotCap(0, p1, q, dotSize);
            //			Handles.DotCap(0, p2, q, dotSize);
            //			Handles.DotCap(0, p3, q, dotSize);
            //			Handles.DrawLine(p0, p1);
            //			Handles.DrawLine(p3, p2);
        }
    }
}