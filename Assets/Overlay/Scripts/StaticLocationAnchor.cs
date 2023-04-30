using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    /// <summary>
    /// Helper class to send GameEvents to other classes that need to know this gameObject should be saved when the overlay is saved.
    /// </summary>
    [RequireComponent(typeof(Movable))]
    public class StaticLocationAnchor : MonoBehaviour
    {
        public Movable movable;

        /// <summary>
        /// String value used to connect this component when reading/writing to the save file
        /// @todo build a system to automatically generate these keys and a way to migrate from the legacy system (manual naming)
        /// </summary>
        [LockedTextField("This field is referenced in save files. See documentation before changing")]
        public string key = "keyForSavedFile";

        private void OnValidate()
        {
            // Set to self by default
            if (movable == null)
            {
                movable = GetComponent<Movable>();
            }
        }

        private void Awake()
        {
            if (key == "keyForSavedFile")
            {
                Debug.LogError($"invalid StaticLocationAnchor key for {movable.targetTransform.gameObject}. Key cannot be the default value: {key}");
                return;
            }
        }

        public void ApplyTransform(OverlayTransform overlayTransform)
        {
            movable.targetTransform.SetPositionAndRotation(overlayTransform.pos, Quaternion.Euler(overlayTransform.rot));
        }
    }
}
