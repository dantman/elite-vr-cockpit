using UnityEngine;

namespace EVRC
{
    public class HelpButton : BaseButton
    {
        protected override Unpress Activate()
        {
            CockpitSettingsState.instance.ChangeSettings(settings =>
            {
                settings.buttonLabelsEnabled = !settings.buttonLabelsEnabled;
            });
            return null;
        }
    }
}