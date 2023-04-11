using System;
using UnityEngine;

namespace EVRC
{
    using StatusFlags = EDStateManager.EDStatus_Flags;

    /**
     * A helper that enables/disables an object based on the Elite Dangngerous Flags status
     */
    public class EDFlagsVisibility : MonoBehaviour
    {
        [Serializable]
        public class VisibilityRule
        {
            public StatusFlags flag;
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

        private void OnEnable()
        {
            CockpitStateController.EditLockedStateChanged.Listen(OnEditLockedStateChanged);
            EDStateManager.FlagsChanged.Listen(OnStatusFlagsChanged);
            Refresh();
        }

        private void OnDisable()
        {
            CockpitStateController.EditLockedStateChanged.Remove(OnEditLockedStateChanged);
            EDStateManager.FlagsChanged.Remove(OnStatusFlagsChanged);
        }

        private void OnEditLockedStateChanged(bool editLocked)
        {
            Refresh();
        }

        private void OnStatusFlagsChanged(StatusFlags flags)
        {
            Refresh();
        }

        public void Refresh()
        {
            target.SetActive(GetVisibility());
        }

        public bool GetVisibility()
        {
            if (visibleWhenEditing && !CockpitStateController.instance.editLocked)
            {
                return true;
            }

            var Flags = EDStateManager.instance.StatusFlags;

            foreach (var rule in visibilityRules)
            {
                if (Flags.HasFlag(rule.flag) == rule.isOn)
                {
                    return rule.visibility;
                }
            }

            return fallbackVisibility;
        }
    }
}
