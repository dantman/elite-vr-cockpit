namespace EVRC.Core
{
    using SettingsState = CockpitSettingsState.SettingsState;

    /**
     * Settings update for joysticks
     */
    public class JoystickSettings : ToggleableBaseSettings
    {
        protected override void SetEnabled(SettingsState settings, bool enabled)
        {
            settings.joystickEnabled = enabled;
        }

        protected override bool GetEnabled(SettingsState settings)
        {
            return settings.joystickEnabled;
        }
    }
}
