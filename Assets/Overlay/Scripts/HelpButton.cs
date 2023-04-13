namespace EVRC.Core.Overlay
{
    public class HelpButton : BaseButton
    {
        public void ToggleButtonLabels()
        {
            CockpitSettingsState.instance.ChangeSettings(settings =>
            {
                settings.buttonLabelsEnabled = !settings.buttonLabelsEnabled;
            });
        }

        public void ToggleHelpPanel()
        {
            CockpitSettingsState.instance.ChangeSettings(settings =>
            {
                settings.helpPanelEnabled = !settings.helpPanelEnabled;
            });
        }

        protected override Unpress Activate()
        {
            return () => { };
        }
    }
}