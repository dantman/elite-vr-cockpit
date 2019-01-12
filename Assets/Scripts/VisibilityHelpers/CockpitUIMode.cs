using System;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using EDGuiFocus = EDStateManager.EDStatus_GuiFocus;
    using EDStatus_Flags = EDStateManager.EDStatus_Flags;

    /**
     * Behaviour that keeps track of the ED state and enables/disables game objects
     * depending on the state (menu, piloting, galaxy/system map, ship, SRV, etc)
     */
    public class CockpitUIMode : MonoBehaviour
    {
        public GameObject gameNotRunning;
        public GameObject menuMode;
        public GameObject map;
        public GameObject cockpit;
        public GameObject shipOnlyCockpit;
        public GameObject mainShipOnlyCockpit;
        public GameObject fighterOnlyCockpit;
        public GameObject srvOnlyCockpit;
        public CockpitModeOverride ModeOverride = CockpitModeOverride.None;
        private EDGuiFocus GuiFocus;
        private EDStatus_Flags StatusFlags;

        public static CockpitMode Mode { get; private set; }

        public static SteamVR_Events.Event<CockpitMode> ModeChanged = new SteamVR_Events.Event<CockpitMode>();

        [Flags]
        public enum CockpitMode : ushort
        {
            GameNotRunning = 1 << 0,
            InGame = 1 << 1,
            Map = 1 << 2,
            Cockpit = 1 << 3,
            InShip = 1 << 4,
            InSRV = 1 << 5,
            InMainShip = 1 << 6,
            InFighter = 1 << 7,
            MenuMode = 1 << 15,
        }

        public enum CockpitModeOverride : ushort
        {
            None = 0,
            GameNotRunning = CockpitMode.GameNotRunning,
            Map = CockpitMode.InGame | CockpitMode.Map,
            MainShipCockpit = CockpitMode.InGame | CockpitMode.Cockpit | CockpitMode.InShip | CockpitMode.InMainShip,
            FighterCockpit = CockpitMode.InGame | CockpitMode.Cockpit | CockpitMode.InShip | CockpitMode.InFighter,
            SRVCockpit = CockpitMode.InGame | CockpitMode.Cockpit | CockpitMode.InSRV,
            MenuMode = CockpitMode.MenuMode,
        }

        void OnEnable()
        {
            EDStateManager.EliteDangerousStarted.Listen(OnGameStartedOrStopped);
            EDStateManager.EliteDangerousStopped.Listen(OnGameStartedOrStopped);
            EDStateManager.GuiFocusChanged.Listen(OnGuiFocusChanged);
            EDStateManager.FlagsChanged.Listen(OnFlagsChanged);
            CockpitStateController.MenuModeStateChanged.Listen(OnMenuModeChanged);
            Refresh();
        }

        void OnDisable()
        {
            EDStateManager.EliteDangerousStarted.Remove(OnGameStartedOrStopped);
            EDStateManager.EliteDangerousStopped.Remove(OnGameStartedOrStopped);
            EDStateManager.GuiFocusChanged.Remove(OnGuiFocusChanged);
            EDStateManager.FlagsChanged.Remove(OnFlagsChanged);
            CockpitStateController.MenuModeStateChanged.Remove(OnMenuModeChanged);
        }

        private void OnGameStartedOrStopped()
        {
            Refresh();
        }

        private void OnMenuModeChanged(bool menuMode)
        {
            Refresh();
        }

        void OnGuiFocusChanged(EDGuiFocus guiFocus)
        {
            GuiFocus = guiFocus;
            Refresh();
        }

        private void OnFlagsChanged(EDStatus_Flags flags)
        {
            StatusFlags = flags;
            Refresh();
        }

        public void Override(CockpitModeOverride modeOverride)
        {
            ModeOverride = modeOverride;
            Refresh();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (Mode != (CockpitMode)ModeOverride)
                {
                    Refresh();
                }
            }
            else
            {
                if (ModeOverride != CockpitModeOverride.None)
                {
                    ModeOverride = CockpitModeOverride.None;
                }
            }
        }
#endif

        void Refresh()
        {
            if (ModeOverride != CockpitModeOverride.None)
            {
                SetMode((CockpitMode)ModeOverride);
                return;
            }

            if (!EDStateManager.instance.IsEliteDangerousRunning)
            {
                if (gameNotRunning != null) gameNotRunning.SetActive(true);
                SetMode(CockpitMode.GameNotRunning);
                return;
            }

            if (CockpitStateController.instance.menuMode)
            {
                SetMode(CockpitMode.MenuMode);
                return;
            }

            var mode = CockpitMode.InGame;

            if (GuiFocus == EDGuiFocus.GalaxyMap || GuiFocus == EDGuiFocus.SystemMap || GuiFocus == EDGuiFocus.Orrery)
            {
                mode |= CockpitMode.Map;
            }
            else
            {
                mode |= CockpitMode.Cockpit;
                if (StatusFlags.HasFlag(EDStatus_Flags.InMainShip) || StatusFlags.HasFlag(EDStatus_Flags.InFighter))
                {
                    mode |= CockpitMode.InShip;
                }
                if (StatusFlags.HasFlag(EDStatus_Flags.InMainShip))
                {
                    mode |= CockpitMode.InMainShip;
                }
                if (StatusFlags.HasFlag(EDStatus_Flags.InFighter))
                {
                    mode |= CockpitMode.InFighter;
                }
                if (StatusFlags.HasFlag(EDStatus_Flags.InSRV))
                {
                    mode |= CockpitMode.InSRV;
                }
            }

            SetMode(mode);
        }

        void SetMode(CockpitMode mode)
        {
            if (gameNotRunning)
            {
                gameNotRunning.SetActive(mode.HasFlag(CockpitMode.GameNotRunning));
            }
            if (menuMode)
            {
                menuMode.SetActive(mode.HasFlag(CockpitMode.MenuMode));
            }
            if (map)
            {
                map.SetActive(mode.HasFlag(CockpitMode.Map));
            }
            if (cockpit)
            {
                cockpit.SetActive(mode.HasFlag(CockpitMode.Cockpit));
            }
            if (shipOnlyCockpit)
            {
                shipOnlyCockpit.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InShip));
            }
            if (mainShipOnlyCockpit)
            {
                mainShipOnlyCockpit.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InMainShip));
            }
            if (fighterOnlyCockpit)
            {
                fighterOnlyCockpit.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InFighter));
            }
            if (srvOnlyCockpit)
            {
                srvOnlyCockpit.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InSRV));
            }

            Mode = mode;
            ModeChanged.Send(Mode);
        }
    }
}
