using UnityEngine;

namespace EVRC.Settings
{
    using SettingsState = CockpitSettingsState.SettingsState;

    /**
     * Base control settings implementation
     * Assumes that all controls have an enable/disable
     */
    abstract public class BaseSettings : MonoBehaviour
    {
        protected virtual void OnEnable() {
            CockpitSettingsState.SettingsRefresh.AddListener(OnSettingsRefresh);
            OnSettingsRefresh(CockpitSettingsState.instance.GetSettings());
        }

        protected virtual void OnDisable() {
            CockpitSettingsState.SettingsRefresh.AddListener(OnSettingsRefresh);
        }

        /**
         * Handler to update UI state when settings are loaded from a save
         */
        protected virtual void OnSettingsRefresh(SettingsState settings) {}
    }
}
