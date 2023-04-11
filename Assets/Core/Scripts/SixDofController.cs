using UnityEngine;

namespace EVRC.Settings
{
    using SettingsState = CockpitSettingsState.SettingsState;

    /**
     * Settings update for 6DOF controllers
     */
    public class SixDofController : ToggleableBaseSettings
    {
        protected override void SetEnabled(SettingsState settings, bool enabled)
        {
            //settings.sixDofControllerEnabled = enabled;
            Debug.LogError("Six DOF Controller has been removed (temporarily) by the developers");
        }

        protected override bool GetEnabled(SettingsState settings)
        {
            Debug.LogError("Six DOF Controller has been removed (temporarily) by the developers");
            return false;
        }
    }
}
