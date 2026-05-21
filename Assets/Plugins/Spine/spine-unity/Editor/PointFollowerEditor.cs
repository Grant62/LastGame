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

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Editor
{
    using Editor = UnityEditor.Editor;
    using Event = UnityEngine.Event;

    [CustomEditor(typeof(PointFollower))] [CanEditMultipleObjects]
    public class PointFollowerEditor : Editor
    {
        private SerializedProperty slotName, pointAttachmentName, skeletonRenderer, followZPosition, followBoneRotation, followSkeletonFlip;
        private PointFollower targetPointFollower;
        private bool needsReset;

        #region Context Menu Item
        [MenuItem("CONTEXT/SkeletonRenderer/Add PointFollower GameObject")]
        private static void AddBoneFollowerGameObject(MenuCommand cmd)
        {
            SkeletonRenderer skeletonRenderer = cmd.context as SkeletonRenderer;
            GameObject go = new("PointFollower");
            Transform t = go.transform;
            t.SetParent(skeletonRenderer.transform);
            t.localPosition = Vector3.zero;

            PointFollower f = go.AddComponent<PointFollower>();
            f.skeletonRenderer = skeletonRenderer;

            EditorGUIUtility.PingObject(t);

            Undo.RegisterCreatedObjectUndo(go, "Add PointFollower");
        }

        // Validate
        [MenuItem("CONTEXT/SkeletonRenderer/Add PointFollower GameObject", true)]
        private static bool ValidateAddBoneFollowerGameObject(MenuCommand cmd)
        {
            SkeletonRenderer skeletonRenderer = cmd.context as SkeletonRenderer;
            return skeletonRenderer.valid;
        }
        #endregion

        private void OnEnable()
        {
            skeletonRenderer = serializedObject.FindProperty("skeletonRenderer");
            slotName = serializedObject.FindProperty("slotName");
            pointAttachmentName = serializedObject.FindProperty("pointAttachmentName");

            targetPointFollower = (PointFollower)target;
            if (targetPointFollower.skeletonRenderer != null)
                targetPointFollower.skeletonRenderer.Initialize(false);

            if (!targetPointFollower.IsValid || needsReset)
            {
                targetPointFollower.Initialize();
                targetPointFollower.LateUpdate();
                needsReset = false;
                SceneView.RepaintAll();
            }
        }

        public void OnSceneGUI()
        {
            PointFollower tbf = target as PointFollower;
            SkeletonRenderer skeletonRendererComponent = tbf.skeletonRenderer;
            if (skeletonRendererComponent == null)
                return;

            Skeleton skeleton = skeletonRendererComponent.skeleton;
            Transform skeletonTransform = skeletonRendererComponent.transform;

            if (string.IsNullOrEmpty(pointAttachmentName.stringValue))
            {
                // Draw all active PointAttachments in the current skin
                Skin currentSkin = skeleton.Skin;
                if (currentSkin != skeleton.Data.DefaultSkin) DrawPointsInSkin(skeleton.Data.DefaultSkin, skeleton, skeletonTransform);
                if (currentSkin != null) DrawPointsInSkin(currentSkin, skeleton, skeletonTransform);
            }
            else
            {
                int slotIndex = skeleton.FindSlotIndex(slotName.stringValue);
                if (slotIndex >= 0)
                {
                    Slot slot = skeleton.Slots.Items[slotIndex];
                    PointAttachment point = skeleton.GetAttachment(slotIndex, pointAttachmentName.stringValue) as PointAttachment;
                    if (point != null)
                    {
                        DrawPointAttachmentWithLabel(point, slot.Bone, skeletonTransform);
                    }
                }
            }
        }

        private static void DrawPointsInSkin(Skin skin, Skeleton skeleton, Transform transform)
        {
            foreach (KeyValuePair<Skin.AttachmentKeyTuple, Attachment> skinEntry in skin.Attachments)
            {
                PointAttachment attachment = skinEntry.Value as PointAttachment;
                if (attachment != null)
                {
                    Skin.AttachmentKeyTuple skinKey = skinEntry.Key;
                    Slot slot = skeleton.Slots.Items[skinKey.slotIndex];
                    DrawPointAttachmentWithLabel(attachment, slot.Bone, transform);
                }
            }
        }

        private static void DrawPointAttachmentWithLabel(PointAttachment point, Bone bone, Transform transform)
        {
            Vector3 labelOffset = new(0f, -0.2f, 0f);
            SpineHandles.DrawPointAttachment(bone, point, transform);
            Handles.Label(labelOffset + point.GetWorldPosition(bone, transform), point.Name, SpineHandles.PointNameStyle);
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
                targetPointFollower.Initialize();
                targetPointFollower.LateUpdate();
                needsReset = false;
                SceneView.RepaintAll();
            }

            serializedObject.Update();

            DrawDefaultInspector();

            // Find Renderer
            if (skeletonRenderer.objectReferenceValue == null)
            {
                SkeletonRenderer parentRenderer = targetPointFollower.GetComponentInParent<SkeletonRenderer>();
                if (parentRenderer != null && parentRenderer.gameObject != targetPointFollower.gameObject)
                {
                    skeletonRenderer.objectReferenceValue = parentRenderer;
                    Debug.Log("Inspector automatically assigned PointFollower.SkeletonRenderer");
                }
            }

            SkeletonRenderer skeletonRendererReference = skeletonRenderer.objectReferenceValue as SkeletonRenderer;
            if (skeletonRendererReference != null)
            {
                if (skeletonRendererReference.gameObject == targetPointFollower.gameObject)
                {
                    skeletonRenderer.objectReferenceValue = null;
                    EditorUtility.DisplayDialog("Invalid assignment.",
                        "PointFollower can only follow a skeleton on a separate GameObject.\n\nCreate a new GameObject for your PointFollower, or choose a SkeletonRenderer from a different GameObject.",
                        "Ok");
                }
            }

            if (!targetPointFollower.IsValid)
            {
                needsReset = true;
            }

            Event current = Event.current;
            bool wasUndo = current.type == EventType.ValidateCommand && current.commandName == "UndoRedoPerformed";
            if (wasUndo)
                targetPointFollower.Initialize();

            serializedObject.ApplyModifiedProperties();
        }
    }
}