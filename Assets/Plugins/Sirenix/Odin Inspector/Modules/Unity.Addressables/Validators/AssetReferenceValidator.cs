//-----------------------------------------------------------------------
// <copyright file="AssetReferenceValidator.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR

#if !SIRENIX_INTERNAL
#pragma warning disable
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor.Validation;
using Sirenix.OdinInspector.Modules.Addressables.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

#if ODIN_VALIDATOR_3_1
[assembly: RegisterValidationRule(typeof(AssetReferenceValidator), Description =
	"This validator provides robust integrity checks for your asset references within Unity. " +
	"It validates whether an asset reference has been assigned, and if it's missing, raises an error. " +
	"It further checks the existence of the main asset at the assigned path, ensuring it hasn't been " +
	"inadvertently deleted or moved. The validator also verifies if the assigned asset is addressable " +
	"and, if not, offers a fix to make it addressable. Moreover, it ensures the asset adheres to " +
	"specific label restrictions set through the AssetReferenceUILabelRestriction attribute. " +
	"Lastly, it performs checks on any sub-object linked to the asset, making sure it hasn't gone missing. " +
	"This comprehensive validation system prevents hard-to-spot bugs and errors, " +
	"fostering a more robust and efficient development workflow.")]
#else
[assembly: RegisterValidator(typeof(AssetReferenceValidator))]
#endif

namespace Sirenix.OdinInspector.Modules.Addressables.Editor
{
    public class AssetReferenceValidator : ValueValidator<AssetReference>
    {
        [Tooltip("If true and the AssetReference is not marked with the Optional attribute, " +
                 "the validator will display an error message if the AssetReference is not set. " +
                 "If false, the validator will only display an error message if the AssetReference is set, " +
                 "but the assigned asset does not exist.")]
        [ToggleLeft]
        public bool RequiredByDefault;

        private bool required;
        private bool optional;
        private string requiredMessage;

        private List<AssetReferenceUIRestriction> restrictions;

        protected override void Initialize()
        {
            RequiredAttribute requiredAttr = Property.GetAttribute<RequiredAttribute>();

            requiredMessage = requiredAttr?.ErrorMessage ?? $"<b>{Property.NiceName}</b> is required.";

            if (RequiredByDefault)
            {
                required = true;
                optional = Property.GetAttribute<OptionalAttribute>() != null;
            }
            else
            {
                required = requiredAttr != null;
                optional = false;
            }

            restrictions = new List<AssetReferenceUIRestriction>();
            foreach (Attribute attr in Property.Attributes)
            {
                if (attr is AssetReferenceUIRestriction r)
                {
                    restrictions.Add(r);
                }
            }
        }

        protected override void Validate(ValidationResult result)
        {
            // If the Addressables settings have not been created, nothing else is really valid.
            if (!AddressableAssetSettingsDefaultObject.SettingsExists)
            {
                result.AddError("Addressables Settings have not been created.")
                    .WithButton("Open Settings Window", () => OdinAddressableUtility.OpenGroupsWindow());
                return;
            }

            AssetReference assetReference = Value;
            bool assetReferenceHasBeenAssigned = !string.IsNullOrEmpty(assetReference?.AssetGUID);

            // No item has been assigned.
            if (!assetReferenceHasBeenAssigned)
            {
                if (!optional && required) // Optional == false & required? Nice.
                {
                    result.AddError(requiredMessage).EnableRichText();
                }

                return;
            }

            string assetPath = AssetDatabase.GUIDToAssetPath(assetReference.AssetGUID);
            Object mainAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);

            // The item has been assigned, but is now missing.
            if (mainAsset == null)
            {
                result.AddError($"The previously assigned main asset with path <b>'{assetPath}'</b> is missing. GUID <b>'{assetReference.AssetGUID}'</b>");
                return;
            }

            AddressableAssetEntry addressableAssetEntry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(assetReference.AssetGUID, true);
            bool isAddressable = addressableAssetEntry != null;

            // Somehow an item sneaked through all of unity's validation measures and ended up not being addressable
            // while still ending up in the asset reference object field.
            if (!isAddressable)
            {
                result.AddError("Assigned item is not addressable.")
                    .WithFix<MakeAddressableFixArgs>("Make Addressable", args => OdinAddressableUtility.MakeAddressable(mainAsset, args.Group));
            }
            // Check the assigned item against any and all label restrictions.
            else
            {
                if (!OdinAddressableUtility.ValidateAssetReferenceRestrictions(restrictions, mainAsset, out AssetReferenceUIRestriction failedRestriction))
                {
                    if (failedRestriction is AssetReferenceUILabelRestriction labelRestriction)
                    {
                        result.AddError(
                                $"Asset reference is restricted to items with these specific labels <b>'{string.Join(", ", labelRestriction.m_AllowedLabels)}'</b>. The currently assigned item has none of them.")
                            .WithFix<AddLabelsFixArgs>("Add Labels", args => SetLabels(mainAsset, args.AssetLabels));
                    }
                    else
                    {
                        result.AddError("Restriction failed: " + failedRestriction);
                    }
                }
            }

            // The assigned item had a sub object, but it's missing.
            if (!string.IsNullOrEmpty(assetReference.SubObjectName))
            {
                IEnumerable<Object> subObjects = OdinAddressableUtility.EnumerateAllActualAndVirtualSubAssets(mainAsset, assetPath);

                bool hasMissingSubObject = true;

                foreach (Object subObject in subObjects)
                {
                    if (subObject.name == assetReference.SubObjectName)
                    {
                        hasMissingSubObject = false;
                        break;
                    }
                }

                if (hasMissingSubObject)
                {
                    result.AddError($"The previously assigned sub asset with name <b>'{assetReference.SubObjectName}'</b> is missing.").EnableRichText();
                }
            }

            if (assetReference.ValidateAsset(mainAsset) || assetReference.ValidateAsset(assetPath))
                return;

            if (assetReference is AssetReferenceSprite && assetReference.editorAsset is Sprite)
                return;

            result.AddError($"{assetReference.GetType().GetNiceFullName()}.ValidateAsset failed to validate assigned asset.");
        }

        private static void SetLabels(Object obj, List<AssetLabel> assetLabels)
        {
            if (!AddressableAssetSettingsDefaultObject.SettingsExists) return;

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
            AddressableAssetEntry entry = settings.FindAssetEntry(guid, false);

            foreach (AssetLabel assetLabel in assetLabels.Where(a => a.Toggled))
            {
                entry.SetLabel(assetLabel.Label, true, false, false);
            }

            settings.SetDirty(AddressableAssetSettings.ModificationEvent.LabelAdded, entry, false, true);
        }

        private class MakeAddressableFixArgs
        {
            [ValueDropdown(nameof(GetGroups))]
            [OnInspectorInit(nameof(SelectDefault))]
            public AddressableAssetGroup Group;

            private void SelectDefault()
            {
                Group = AddressableAssetSettingsDefaultObject.SettingsExists
                    ? AddressableAssetSettingsDefaultObject.Settings.DefaultGroup
                    : null;
            }

            private static IEnumerable<ValueDropdownItem> GetGroups()
            {
                return !AddressableAssetSettingsDefaultObject.SettingsExists
                    ? Enumerable.Empty<ValueDropdownItem>()
                    : AddressableAssetSettingsDefaultObject.Settings.groups
                        .Where(group => !group.ReadOnly)
                        .Select(group => new ValueDropdownItem(group.Name, group));
            }

            [Button(SdfIconType.ListNested)] [PropertySpace(8f)]
            private void OpenAddressablesGroups()
            {
                OdinAddressableUtility.OpenGroupsWindow();
            }
        }

        private class AddLabelsFixArgs
        {
            [HideIf("@true")]
            public List<AssetLabel> AssetLabels
            {
                get
                {
                    if (!AddressableAssetSettingsDefaultObject.SettingsExists) return assetLabels;

                    AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
                    List<AssetLabel> labels = settings
                        .GetLabels()
                        .Select(l => new AssetLabel { Label = l, Toggled = false })
                        .ToList();

                    foreach (AssetLabel assetLabel in assetLabels)
                    {
                        AssetLabel label = labels.FirstOrDefault(l => l.Label == assetLabel.Label);

                        if (label != null)
                        {
                            label.Toggled = assetLabel.Toggled;
                        }
                    }

                    assetLabels = labels;
                    return assetLabels;
                }
            }

            private List<AssetLabel> assetLabels = new();

            [OnInspectorGUI]
            private void Draw()
            {
                Rect togglesRect = EditorGUILayout.GetControlRect(false, Mathf.CeilToInt(AssetLabels.Count / 2f) * 20f);

                for (int i = 0; i < AssetLabels.Count; i++)
                {
                    AssetLabel assetLabel = AssetLabels[i];
                    Rect toggleRect = togglesRect.SplitGrid(togglesRect.width / 2f, 20, i);
                    assetLabel.Toggled = GUI.Toggle(toggleRect, assetLabel.Toggled, assetLabel.Label);
                }

                if (!AddressableAssetSettingsDefaultObject.SettingsExists) return;

                GUILayout.Space(8f);

                Rect buttonsRect = EditorGUILayout.GetControlRect(false, 20f);

                if (SirenixEditorGUI.SDFIconButton(buttonsRect, "Open Addressables Labels", SdfIconType.TagsFill))
                {
                    OdinAddressableUtility.OpenLabelsWindow();
                }
            }
        }

        private class AssetLabel
        {
            public bool Toggled;
            public string Label;
        }
    }
}

#endif