using System;
using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core
{
    /**
     * A helper that enables/disables an object based on the Elite Dangerous Flags status
     */
    public class EDFlagsVisibility : MonoBehaviour
    {
        [Serializable]
        public class VisibilityRule
        {
            public EDStatusFlags flag;
            public bool isOn = true;
            public bool visibility = true;
        }

        [Tooltip("The GameObject to enable/disable")]
        public GameObject target;
        [Tooltip("Should the GameObject become visible when editing so it can be edited, even when normally hidden")]
        public bool visibleWhenEditing = true;
        public VisibilityRule[] visibilityRules;
        [Tooltip("What should the visibility be when no rules match")]
        public bool fallbackVisibility = false;

        public OverlayEditLockState editLockedState;

        private void OnEnable()
        {
            EDStateManager.FlagsChanged.Listen(OnStatusFlagsChanged);
            Refresh();
        }

        private void OnDisable()
        {
            EDStateManager.FlagsChanged.Remove(OnStatusFlagsChanged);
        }

        private void OnEditLockedStateChanged(bool editLocked)
        {
            Refresh();
        }

        private void OnStatusFlagsChanged(EDStatusFlags flags)
        {
            Refresh();
        }

        public void Refresh()
        {
            target.SetActive(GetVisibility());
        }

        public bool GetVisibility()
        {
            if (visibleWhenEditing && !editLockedState.EditLocked)
            {
                return true;
            }

            EDStatusFlags flags = EDStateManager.instance.StatusFlags;

            foreach (VisibilityRule rule in visibilityRules)
            {
                if (flags.HasFlag(rule.flag) == rule.isOn)
                {
                    return rule.visibility;
                }
            }

            return fallbackVisibility;
        }
    }
}
