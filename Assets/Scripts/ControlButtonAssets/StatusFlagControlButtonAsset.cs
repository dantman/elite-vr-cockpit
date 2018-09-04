using System;
using UnityEngine;

namespace EVRC
{
    using EDStatus_Flags = EDStateManager.EDStatus_Flags;

    /**
     * A boolean control button asset that is toggled by one of the Status.json Flags bits
     */
    [CreateAssetMenu(fileName = "StatusFlagControlButtonAsset", menuName = "EVRC/ControlButtonAssets/StatusFlagControlButtonAsset", order = 2)]
    public class StatusFlagControlButtonAsset : BooleanControlButtonAsset
    {
        public EDStatus_Flags flag;
        protected bool isOn;

        private void OnEnable()
        {
            isOn = IsOn();
            EDStateManager.FlagsChanged.Listen(OnFlagsChanged);
        }

        private void OnDisable()
        {
            EDStateManager.FlagsChanged.Remove(OnFlagsChanged);
        }

        private void OnFlagsChanged(EDStatus_Flags flags)
        {
            if (flags.HasFlag(flag) != isOn)
            {
                isOn = flags.HasFlag(flag);
                TriggerRefresh();
            }
        }

        public override bool IsOn()
        {
            return EDStateManager.instance.StatusFlags.HasFlag(flag);
        }
    }
}
