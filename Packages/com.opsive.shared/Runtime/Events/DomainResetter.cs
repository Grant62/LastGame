/// ---------------------------------------------
/// Opsive Shared
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;

namespace Opsive.Shared.Events
{
    /// <summary>
    ///     Resets the event handler static variables.
    /// </summary>
    public class DomainResetter
    {
        /// <summary>
        ///     Reset the static variables for domain reloading.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void DomainReset()
        {
            EventHandler.DomainReset();
        }
    }
}