namespace EVRC.Core
{
    using SettingsState = CockpitSettingsState.SettingsState;

    /**
     * Settings update for throttles
     */
    public class ThrottleSettings : ToggleableBaseSettings
    {
        protected override void SetEnabled(SettingsState settings, bool enabled)
        {
            settings.throttleEnabled = enabled;
        }

        protected override bool GetEnabled(SettingsState settings)
        {
            return settings.throttleEnabled;
        }
    }
}
