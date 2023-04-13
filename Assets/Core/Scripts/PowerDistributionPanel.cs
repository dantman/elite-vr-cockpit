namespace EVRC.Core
{
    using SettingsState = CockpitSettingsState.SettingsState;

    /**
     * Settings update for power distribution panels
     */
    public class PowerDistributionPanel : ToggleableBaseSettings
    {
        protected override void SetEnabled(SettingsState settings, bool enabled)
        {
            settings.powerDistributionPanelEnabled = enabled;
        }

        protected override bool GetEnabled(SettingsState settings)
        {
            return settings.powerDistributionPanelEnabled;
        }
    }
}
