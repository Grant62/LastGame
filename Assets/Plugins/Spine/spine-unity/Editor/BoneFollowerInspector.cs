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

using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Spine.Unity.Editor
{
    using Editor = UnityEditor.Editor;
    using Event = UnityEngine.Event;

    [CustomEditor(typeof(BoneFollower))] [CanEditMultipleObjects]
    public class BoneFollowerInspector : Editor
    {
        private SerializedProperty boneName, skeletonRenderer, followZPosition, followBoneRotation, followLocalScale, followSkeletonFlip;
        private BoneFollower targetBoneFollower;
        private bool needsReset;

        #region Context Menu Item
        [MenuItem("CONTEXT/SkeletonRenderer/Add BoneFollower GameObject")]
        private static void AddBoneFollowerGameObject(MenuCommand cmd)
        {
            SkeletonRenderer skeletonRenderer = cmd.context as SkeletonRenderer;
            GameObject go = new("BoneFollower");
            Transform t = go.transform;
            t.SetParent(skeletonRenderer.transform);
            t.localPosition = Vector3.zero;

            BoneFollower f = go.AddComponent<BoneFollower>();
            f.skeletonRenderer = skeletonRenderer;

            EditorGUIUtility.PingObject(t);

            Undo.RegisterCreatedObjectUndo(go, "Add BoneFollower");
        }

        // Validate
        [MenuItem("CONTEXT/SkeletonRenderer/Add BoneFollower GameObject", true)]
        private static bool ValidateAddBoneFollowerGameObject(MenuCommand cmd)
        {
            SkeletonRenderer skeletonRenderer = cmd.context as SkeletonRenderer;
            return skeletonRenderer.valid;
        }
        #endregion

        private void OnEnable()
        {
            skeletonRenderer = serializedObject.FindProperty("skeletonRenderer");
            boneName = serializedObject.FindProperty("boneName");
            followBoneRotation = serializedObject.FindProperty("followBoneRotation");
            followZPosition = serializedObject.FindProperty("followZPosition");
            followLocalScale = serializedObject.FindProperty("followLocalScale");
            followSkeletonFlip = serializedObject.FindProperty("followSkeletonFlip");

            targetBoneFollower = (BoneFollower)target;
            if (targetBoneFollower.SkeletonRenderer != null)
                targetBoneFollower.SkeletonRenderer.Initialize(false);

            if (!targetBoneFollower.valid || needsReset)
            {
                targetBoneFollower.Initialize();
                targetBoneFollower.LateUpdate();
                needsReset = false;
                SceneView.RepaintAll();
            }
        }

        public void OnSceneGUI()
        {
            BoneFollower tbf = target as BoneFollower;
            SkeletonRenderer skeletonRendererComponent = tbf.skeletonRenderer;
            if (skeletonRendererComponent == null) return;

            Transform transform = skeletonRendererComponent.transform;
            Skeleton skeleton = skeletonRendererComponent.skeleton;

            if (string.IsNullOrEmpty(boneName.stringValue))
            {
                SpineHandles.DrawBones(transform, skeleton);
                SpineHandles.DrawBoneNames(transform, skeleton);
                Handles.Label(tbf.transform.position, "No bone selected", EditorStyles.helpBox);
            }
            else
            {
                Bone targetBone = tbf.bone;
                if (targetBone == null) return;
                SpineHandles.DrawBoneWireframe(transform, targetBone, SpineHandles.TransformContraintColor);
                Handles.Label(targetBone.GetWorldPosition(transform), targetBone.Data.Name, SpineHandles.BoneNameStyle);
            }
        }

        public override void OnInspectorGUI()
        {
            if (serializedObject.isEditingMultipleObjects)
            {
                if (needsReset)
                {
                    needsReset = false;
                    foreach (Object o in targets)
                    {
                        BoneFollower bf = (BoneFollower)o;
                        bf.Initialize();
                        bf.LateUpdate();
                    }

                    SceneView.RepaintAll();
                }

                EditorGUI.BeginChangeCheck();
                DrawDefaultInspector();
                needsReset |= EditorGUI.EndChangeCheck();
                return;
            }

            if (needsReset && Event.current.type == EventType.Layout)
            {
                targetBoneFollower.Initialize();
                targetBoneFollower.LateUpdate();
                needsReset = false;
                SceneView.RepaintAll();
            }

            serializedObject.Update();

            // Find Renderer
            if (skeletonRenderer.objectReferenceValue == null)
            {
                SkeletonRenderer parentRenderer = targetBoneFollower.GetComponentInParent<SkeletonRenderer>();
                if (parentRenderer != null && parentRenderer.gameObject != targetBoneFollower.gameObject)
                {
                    skeletonRenderer.objectReferenceValue = parentRenderer;
                    Debug.Log("Inspector automatically assigned BoneFollower.SkeletonRenderer");
                }
            }

            EditorGUILayout.PropertyField(skeletonRenderer);
            SkeletonRenderer skeletonRendererReference = skeletonRenderer.objectReferenceValue as SkeletonRenderer;
            if (skeletonRendererReference != null)
            {
                if (skeletonRendererReference.gameObject == targetBoneFollower.gameObject)
                {
                    skeletonRenderer.objectReferenceValue = null;
                    EditorUtility.DisplayDialog("Invalid assignment.",
                        "BoneFollower can only follow a skeleton on a separate GameObject.\n\nCreate a new GameObject for your BoneFollower, or choose a SkeletonRenderer from a different GameObject.",
                        "Ok");
                }
            }

            if (!targetBoneFollower.valid)
            {
                needsReset = true;
            }

            if (targetBoneFollower.valid)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(boneName);
                needsReset |= EditorGUI.EndChangeCheck();

                EditorGUILayout.PropertyField(followBoneRotation);
                EditorGUILayout.PropertyField(followZPosition);
                EditorGUILayout.PropertyField(followLocalScale);
                EditorGUILayout.PropertyField(followSkeletonFlip);

                RecommendRigidbodyButton(targetBoneFollower);
            }
            else
            {
                SkeletonRenderer boneFollowerSkeletonRenderer = targetBoneFollower.skeletonRenderer;
                if (boneFollowerSkeletonRenderer == null)
                {
                    EditorGUILayout.HelpBox("SkeletonRenderer is unassigned. Please assign a SkeletonRenderer (SkeletonAnimation or SkeletonAnimator).", MessageType.Warning);
                }
                else
                {
                    boneFollowerSkeletonRenderer.Initialize(false);

                    if (boneFollowerSkeletonRenderer.skeletonDataAsset == null)
                        EditorGUILayout.HelpBox("Assigned SkeletonRenderer does not have SkeletonData assigned to it.", MessageType.Warning);

                    if (!boneFollowerSkeletonRenderer.valid)
                        EditorGUILayout.HelpBox("Assigned SkeletonRenderer is invalid. Check target SkeletonRenderer, its SkeletonDataAsset or the console for other errors.",
                            MessageType.Warning);
                }
            }

            Event current = Event.current;
            bool wasUndo = current.type == EventType.ValidateCommand && current.commandName == "UndoRedoPerformed";
            if (wasUndo)
                targetBoneFollower.Initialize();

            serializedObject.ApplyModifiedProperties();
        }

        internal static void RecommendRigidbodyButton(Component component)
        {
            bool hasCollider2D = component.GetComponent<Collider2D>() != null || component.GetComponent<BoundingBoxFollower>() != null;
            bool hasCollider3D = !hasCollider2D && component.GetComponent<Collider>();
            bool missingRigidBody = hasCollider2D && component.GetComponent<Rigidbody2D>() == null || hasCollider3D && component.GetComponent<Rigidbody>() == null;
            if (missingRigidBody)
            {
                using (new SpineInspectorUtility.BoxScope())
                {
                    EditorGUILayout.HelpBox(
                        "Collider detected. Unity recommends adding a Rigidbody to the parent Transforms of any colliders that are intended to be dynamically repositioned and rotated.",
                        MessageType.Warning);
                    Type rbType = hasCollider2D ? typeof(Rigidbody2D) : typeof(Rigidbody);
                    string rbLabel = string.Format("Add {0}", rbType.Name);
                    GUIContent rbContent = SpineInspectorUtility.TempContent(rbLabel, SpineInspectorUtility.UnityIcon(rbType),
                        "Add a rigidbody to this GameObject to be the Physics body parent of the attached collider.");
                    if (SpineInspectorUtility.CenteredButton(rbContent)) component.gameObject.AddComponent(rbType);
                }
            }
        }
    }
}