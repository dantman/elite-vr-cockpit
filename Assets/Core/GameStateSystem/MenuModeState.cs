using System;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Menu Mode State"), Serializable]
    public class MenuModeState : GameState
    {
        [SerializeField]
        [Tooltip("Is menu mode on: Enables trackpad/joystick menu controls for controlling game menus")]
        private bool _menuMode = false;
        public bool menuMode => _menuMode;

        public static SteamVR_Events.Event<bool> MenuModeStateChanged = new SteamVR_Events.Event<bool>();

        public static MenuModeState _instance;

        public static MenuModeState instance => _instance;

        /**
         * Toggle the menuMode state
         */
        public bool ToggleMenuMode()
        {
            SetMenuMode(!_menuMode);
            return _menuMode;
        }

        /**
         * Set the editLocked state
         */
        public void SetMenuMode(bool menuMode)
        {
            if (_menuMode  != menuMode)
            {
                _menuMode = menuMode;
                MenuModeStateChanged.Send(_menuMode);
            }
        }

        public override string GetStatusText()
        {
            return _menuMode.ToString();
        }
    }
}
