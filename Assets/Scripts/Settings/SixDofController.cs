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
            settings.sixDofControllerEnabled = enabled;
        }

        protected override bool GetEnabled(SettingsState settings)
        {
            return settings.sixDofControllerEnabled;
        }
    }
}
