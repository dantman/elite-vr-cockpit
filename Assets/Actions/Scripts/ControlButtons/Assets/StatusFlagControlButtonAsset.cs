using System;
using UnityEngine;

namespace EVRC.Core.Actions
{
    /**
     * A boolean control button asset that is toggled by one of the Status.json Flags bits
     */
    [CreateAssetMenu(fileName = "StatusFlagControlButtonAsset", menuName = Constants.CONTROL_BUTTON_PATH + "/StatusFlagControlButtonAsset", order = 2)]
    public class StatusFlagControlButtonAsset : BooleanControlButtonAsset
    {
        public EliteDangerousState eliteDangerousState;
        public EDStatusFlags flag;
        protected bool isOn;

        private void OnEnable()
        {
            isOn = IsOn();
            EDStateManager.FlagsChanged.Listen(OnFlagsChanged);

            if (eliteDangerousState == null)
            {
                throw new Exception(
                    $"eliteDangerousState asset must be assigned to StatusFlagControlButtonAsset: {this.name}");
            }
        }

        private void OnDisable()
        {
            EDStateManager.FlagsChanged.Remove(OnFlagsChanged);
        }

        private void OnFlagsChanged(EDStatusFlags flags)
        {
            if (flags.HasFlag(flag) != isOn)
            {
                isOn = flags.HasFlag(flag);
                TriggerRefresh();
            }
        }

        public override bool IsOn()
        {
            if (!Application.isPlaying) return true;
            return eliteDangerousState.statusFlags.HasFlag(flag);
        }
    }
}
