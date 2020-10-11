using System.Linq;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace EVRC.Settings
{
    using SettingsState = CockpitSettingsState.SettingsState;

    /**
     * Extension to the base control settings that handles controls with an enable/disable
     */
    abstract public class ToggleableBaseSettings : BaseSettings
    {
        public SegmentedControl stateToggle;
        public Selectable offButton;
        public Selectable onButton;

        protected override void OnEnable()
        {
            stateToggle.onValueChanged.AddListener(OnStateToggleValueChanged);
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            stateToggle.onValueChanged.RemoveListener(OnStateToggleValueChanged);
            base.OnDisable();
        }

        protected void OnStateToggleValueChanged(int index)
        {
            if (index == -1) return; // @note This happens while loading before saved state is loaded
            CockpitSettingsState.instance.ChangeSettings(settings =>
            {
                OnStateToggleChanged(settings, stateToggle.segments[index]);
            });
        }

        protected override void OnSettingsRefresh(SettingsState settings)
        {
            base.OnSettingsRefresh(settings);
            OnStateToggleRefresh(settings);
        }

        /**
         * Helper to set the stateToggle segmented control to a specific state
         */
        protected void SetStateToggle(Selectable button)
        {
            var index = System.Array.IndexOf(stateToggle.segments, button);
            if (index == -1) throw new System.ArgumentException("This selectable is not part of the segmented control");
            stateToggle.selectedSegmentIndex = index;
        }

        /**
         * Handler to update settings when the segmented control toggle's state changes
         */
        protected virtual void OnStateToggleChanged(SettingsState settings, Selectable button)
        {
            if (button == offButton) SetEnabled(settings, false);
            else if (button == onButton) SetEnabled(settings, true);
        }

        /**
         * Handler to update UI state of the state toggle when settings are loaded from a save
         */
        protected virtual void OnStateToggleRefresh(SettingsState settings)
        {
            bool enabled = GetEnabled(settings);
            SetStateToggle(enabled ? onButton : offButton);
        }

        /**
         * Handler to enable/disable the control
         */
        protected abstract void SetEnabled(SettingsState settings, bool enabled);

        /**
         * Handler get the enabled/disabled state from settings
         */
        protected abstract bool GetEnabled(SettingsState settings);
    }
}
