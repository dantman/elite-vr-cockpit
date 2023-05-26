using System;
using EVRC.Core.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Identifies a GameObject as related to a specific CockpitMode. Tne root (bool) field
    /// indicates that this is the parent gameObject of a group of overlay objects. 
    /// </summary>
    /// <remarks>
    /// For example, when placing a controlButton, we need to know the root cockpit
    /// object, so we can place it as a child, so that it appears when the correct CockpitUI
    /// is active.
    /// </remarks>
    public class CockpitModeAnchor : MonoBehaviour
    {
        public CockpitMode cockpitUiMode = CockpitMode.Cockpit;

        [Tooltip("The object that contains the child objects for a cockpitMode")]
        public GameObject target;

        // This could be extended to an enum later to include multiple types of identifiers
        // for this anchor
        public bool root = true;


        void OnValidate()
        {
            if (target == null)
            {
                target = gameObject;
            }


            //Check for 'nothing' value
            if (cockpitUiMode == 0)
            {
                Debug.LogError($"CockpitMode cannot be set to 'Nothing' for this Anchor: {gameObject.name}");
                return;
            }

            // Check for more than one flag
            if ((cockpitUiMode & (cockpitUiMode - 1)) != 0)
            {
                Debug.LogError($"You cannot select more than one CockpitMode for this anchor: {gameObject.name}");
                return;
            }           

        }
    }
}
