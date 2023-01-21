using UnityEngine;

namespace EVRC
{
    public class HelpButton : MonoBehaviour
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
    }
}