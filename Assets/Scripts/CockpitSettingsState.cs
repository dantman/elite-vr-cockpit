using System;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    /**
     * Behaviour that manages enabled/disabled and other settings for various controls
     */
    public class CockpitSettingsState : MonoBehaviour
    {
        public static CockpitSettingsState _instance;
        public static CockpitSettingsState instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[CockpitSettingsState]", false);
            }
        }

        [Serializable]
        public class SettingsState
        {
            public bool joystickEnabled = true;
            public bool throttleEnabled = true;
            public bool sixDofControllerEnabled = true;
            //public bool threeDofThrusterEnabled = true;
            //public bool threeDofStickEnabled = true;
            public bool powerDistributionPanelEnabled = true;
            public bool buttonLabelsEnabled = false;
        }

        [SerializeField]
        [Tooltip("The current state of settings")]
        protected SettingsState settings;

        [Header("Enable/disable objects")]
        [Tooltip("Joystick objects to enable/disable")]
        public GameObject[] joysticks;
        [Tooltip("Throttle objects to enable/disable")]
        public GameObject[] throttles;
        [Tooltip("6DOF controller objects to enable/disable")]
        public GameObject[] sixDofControllers;
        //[Tooltip("3DOF thruster controller objects to enable/disable")]
        //public GameObject[] threeDofThrusterControllers;
        //[Tooltip("3DOF thruster stick objects to enable/disable")]
        //public GameObject[] threeDofThrusterSticks;
        [Tooltip("Power Distribution panel objects to enable/disable")]
        public GameObject[] powerDistributionPanels;

        public static SteamVR_Events.Event<SettingsState> SettingsRefresh = new SteamVR_Events.Event<SettingsState>();
        public static SteamVR_Events.Event<bool> ButtonLabelStateChanged = new SteamVR_Events.Event<bool>();

        /**
         * Update controls to reflect the current settings
         */
        public void Refresh()
        {
            /**
               * change enabled state of groups/lists of objects
               */
            SetEnabled(joysticks, settings.joystickEnabled);
            SetEnabled(throttles, settings.throttleEnabled);
            SetEnabled(sixDofControllers, settings.sixDofControllerEnabled);
            //SetEnabled(threeDofThrusterControllers, settings.threeDofThrusterEnabled);
            //SetEnabled(threeDofThrusterSticks, settings.threeDofStickEnabled);
            SetEnabled(powerDistributionPanels, settings.powerDistributionPanelEnabled);

            /**
               * change enabled state of components with listeners
               * (i.e. dynamic lists of components)
               */
            ButtonLabelStateChanged.Send(settings.buttonLabelsEnabled);


        }

        /**
         * Internal helper to enable or disable a group of controls
         */
        private void SetEnabled(GameObject[] controls, bool enabled)
        {
            foreach (GameObject control in controls)
            {
                control.SetActive(enabled);
            }
        }

        /**
         * Handler used to change settings
         * It is passed the settings state which can be modified.
         */
        public delegate void ChangeSettingsHandler(SettingsState settings);

        /**
         * Change settings and refresh
         * 
         * This uses a handler instead of making the state public so we can ensure a Refresh is always done after settings are changed.
         */
        public void ChangeSettings(ChangeSettingsHandler handler)
        {
            handler(settings);
            Refresh();
            SettingsRefresh.Invoke(settings);
        }

        /**
         * Get the current state of the settings
         */
        public SettingsState GetSettings()
        {
            return settings;
        }
    }
}
