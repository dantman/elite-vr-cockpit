using System;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Menu Mode State"), Serializable]
    public class MenuModeState : GameState<bool>
    {
        [SerializeField]
        [Tooltip("Is menu mode on: Enables trackpad/joystick menu controls for controlling game menus")]
        private bool _menuMode = false;
        public bool menuMode => _menuMode;

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
                gameEvent.Raise(_menuMode);
            }
        }

        public override string GetStatusText()
        {
            return _menuMode.ToString();
        }
    }
}
