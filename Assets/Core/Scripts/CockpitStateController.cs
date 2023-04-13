using EVRC.Core.Overlay;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core
{
    public class CockpitStateController : MonoBehaviour
    {
        [SerializeField]
        private bool _editLocked = true;
        public bool editLocked
        {
            get
            {
                return _editLocked;
            }
        }
        [SerializeField]
        [Tooltip("Is menu mode on: Enables trackpad menu controls for controlling game menus")]
        private bool _menuMode = false;
        public bool menuMode
        {
            get
            {
                return _menuMode;
            }
        }

        public static SteamVR_Events.Event<bool> EditLockedStateChanged = new SteamVR_Events.Event<bool>();
        public static SteamVR_Events.Event<bool> MenuModeStateChanged = new SteamVR_Events.Event<bool>();

        public static CockpitStateController _instance;
        public static CockpitStateController instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[CockpitStateController]");
            }
        }

        /**
         * Toggle the editLocked state
         */
        public bool ToggleEditLocked()
        {
            SetEditLocked(!_editLocked);
            return _editLocked;
        }

        /**
         * Set the editLocked state
         */
        public void SetEditLocked(bool editLocked)
        {
            if (_editLocked != editLocked)
            {
                _editLocked = editLocked;
                EditLockedStateChanged.Send(_editLocked);

                if (_editLocked)
                {
                    CockpitStateSave.Save();
                    FindObjectOfType<CockpitUIMode>().Override(CockpitUIMode.CockpitModeOverride.None);
                }
            }
        }

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
    }
}
