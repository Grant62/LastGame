/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using Opsive.Shared.Editor.Managers;

namespace Opsive.BehaviorDesigner.Editor.Managers
{
    /// <summary>
    ///     Draws a list of all of the available add-ons.
    /// </summary>
    [OrderedEditorItem("Add-Ons", 12)]
    public class AddOnsManager : Shared.Editor.Managers.AddOnsManager
    {
        protected override string AddOnsURL { get => "https://opsive.com/asset/BehaviorDesigner/AddOnsList.txt"; }
    }
}