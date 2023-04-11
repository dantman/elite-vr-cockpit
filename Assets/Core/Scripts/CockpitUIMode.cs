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
        public GameObject stationServices;
        public GameObject cockpit;
        public GameObject shipOnlyCockpit;
        public GameObject mainShipOnlyCockpit;
        public GameObject fighterOnlyCockpit;
        public GameObject srvOnlyCockpit;
        public GameObject fssMode;
        public GameObject dssMode;
        public CockpitModeOverride ModeOverride = CockpitModeOverride.None;
        private EDGuiFocus GuiFocus;
        private EDStatus_Flags StatusFlags;

        public static CockpitMode Mode { get; private set; }

        public static SteamVR_Events.Event<CockpitMode> ModeChanged = new SteamVR_Events.Event<CockpitMode>();
        public CockpitModeEvent modeChangedGameEvent;


        [Flags]
        public enum CockpitMode : ushort
        {
            GameNotRunning = 1 << 0,
            InGame = 1 << 1,
            Map = 1 << 2,
            StationServices = 1 << 3,
            Cockpit = 1 << 4,
            InShip = 1 << 5,
            InSRV = 1 << 6,
            InMainShip = 1 << 7,
            InFighter = 1 << 8,
            FSSMode = 1 << 9,
            DSSMode = 1 << 10,
            CockpitPanel = 1 << 11,
            Landing = 1 << 12,
            MenuMode = 1 << 15,
        }

        public enum CockpitModeOverride : ushort
        {
            None = 0,
            GameNotRunning = CockpitMode.GameNotRunning,
            Map = CockpitMode.InGame | CockpitMode.Map,
            StationServices = CockpitMode.InGame | CockpitMode.Cockpit | CockpitMode.InShip | CockpitMode.InMainShip | CockpitMode.StationServices, // @fixme We're assuming that StationServices is only accessible from the MainShip cockpit
            MainShipCockpit = CockpitMode.InGame | CockpitMode.Cockpit | CockpitMode.InShip | CockpitMode.InMainShip,
            FighterCockpit = CockpitMode.InGame | CockpitMode.Cockpit | CockpitMode.InShip | CockpitMode.InFighter,
            SRVCockpit = CockpitMode.InGame | CockpitMode.Cockpit | CockpitMode.InSRV,
            FSSMode = CockpitMode.InGame | CockpitMode.Cockpit | CockpitMode.InShip | CockpitMode.InMainShip | CockpitMode.FSSMode,
            DSSMode = CockpitMode.InGame | CockpitMode.Cockpit | CockpitMode.InShip | CockpitMode.InMainShip | CockpitMode.DSSMode,
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
            else if (GuiFocus == EDGuiFocus.FSSMode)
            {
                mode |= CockpitMode.FSSMode;
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
                if (StatusFlags.HasFlag(EDStatus_Flags.LandingGearDown))
                {
                    mode |= CockpitMode.Landing;
                }


                switch (GuiFocus)
                {
                    case EDGuiFocus.StationServices:
                        mode |= CockpitMode.StationServices;
                        break;
                    case EDGuiFocus.SAAMode:
                        mode |= CockpitMode.DSSMode;
                        break;
                    case EDGuiFocus.InternalPanel:
                        mode |= CockpitMode.CockpitPanel; 
                        break;
                    case EDGuiFocus.CommsPanel:
                        mode |= CockpitMode.CockpitPanel;
                        break;
                    case EDGuiFocus.RolePanel:
                        mode |= CockpitMode.CockpitPanel;
                        break;
                    case EDGuiFocus.ExternalPanel:
                        mode |= CockpitMode.CockpitPanel;
                        break;
                    case EDGuiFocus.Codex:
                        mode |= CockpitMode.CockpitPanel;
                        break;


                }

                // @todo Test and add Codex as well
            }

            SetMode(mode);
        }

        void SetMode(CockpitMode mode)
        {
            gameNotRunning?.SetActive(mode.HasFlag(CockpitMode.GameNotRunning));
            menuMode?.SetActive(mode.HasFlag(CockpitMode.MenuMode));
            map?.SetActive(mode.HasFlag(CockpitMode.Map));
            stationServices?.SetActive(mode.HasFlag(CockpitMode.StationServices));
            cockpit?.SetActive(mode.HasFlag(CockpitMode.Cockpit));
            shipOnlyCockpit?.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InShip));
            mainShipOnlyCockpit?.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InMainShip));
            fighterOnlyCockpit?.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InFighter));
            srvOnlyCockpit?.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InSRV));
            fssMode?.SetActive(mode.HasFlag(CockpitMode.FSSMode));
            dssMode?.SetActive(mode.HasFlag(CockpitMode.DSSMode));

            Mode = mode;
            ModeChanged.Send(Mode);
            modeChangedGameEvent.Raise(Mode);
        }
    }
}
