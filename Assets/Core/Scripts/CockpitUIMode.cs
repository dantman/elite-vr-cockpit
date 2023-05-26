using System;
using EVRC.Core.Overlay;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core
{
    /**
     * Behaviour that keeps track of the ED state and enables/disables game objects
     * depending on the state (menu, piloting, galaxy/system map, ship, SRV, etc)
     */
    public class CockpitUIMode : MonoBehaviour
    {
        public EliteDangerousState eliteDangerousState;
        public MenuModeState menuModeState;

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
        public GameObject universalUI;
        public CockpitModeOverride modeOverride = CockpitModeOverride.None;
        private EDGuiFocus edGuiFocus;
        // private EDStatusFlags StatusFlags;

        public static CockpitMode Mode { get; private set; }

        public static SteamVR_Events.Event<CockpitMode> ModeChanged = new SteamVR_Events.Event<CockpitMode>();
        public CockpitModeEvent modeChangedGameEvent;        

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
            // EDStateManager.eliteDangerousStarted.Listen(OnGameStartedOrStopped);
            EDStateManager.EliteDangerousStopped.Listen(OnGameStartedOrStopped);
            EDStateManager.GuiFocusChanged.Listen(OnGuiFocusChanged);
            EDStateManager.FlagsChanged.Listen(OnFlagsChanged);
            Refresh();
        }

        void OnDisable()
        {
            // EDStateManager.eliteDangerousStarted.Remove(OnGameStartedOrStopped);
            EDStateManager.EliteDangerousStopped.Remove(OnGameStartedOrStopped);
            EDStateManager.GuiFocusChanged.Remove(OnGuiFocusChanged);
            EDStateManager.FlagsChanged.Remove(OnFlagsChanged);
            DisableAll();
        }

        public void OnOpenVRStateChanged(bool state)
        {
            if (state)
            {
                Refresh();
            }
            else
            {
                DisableAll();
            }
        }

        public void OnGameStartedOrStopped()
        {
            Refresh();
        }

        public void OnMenuModeChanged(bool menuMode)
        {
            Refresh();
        }

        public void OnEditModeChanged(bool arg0)
        {
            var mOverride = CockpitUIMode.CockpitModeOverride.None;
#if UNITY_EDITOR
            mOverride = modeOverride;
#endif
            Override(mOverride);
        }

        void OnGuiFocusChanged(EDGuiFocus edGuiFocus)
        {
            this.edGuiFocus = edGuiFocus;
            Refresh();
        }

        private void OnFlagsChanged(EDStatusFlags flags)
        {
            // StatusFlags = flags;
            Refresh();
        }

        public void Override(CockpitModeOverride modeOverride)
        {
            this.modeOverride = modeOverride;
            Refresh();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                if (Mode != (CockpitMode)modeOverride)
                {
                    Refresh();
                }
            }
            else
            {
                if (modeOverride != CockpitModeOverride.None)
                {
                    modeOverride = CockpitModeOverride.None;
                }
            }
        }
#endif

        void Refresh()
        {
            if (modeOverride != CockpitModeOverride.None)
            {
                SetMode((CockpitMode)modeOverride);
                return;
            }

            if (!eliteDangerousState.running)
            {
                SetMode(CockpitMode.GameNotRunning);
                return;
            }

            if (menuModeState.menuMode)
            {
                SetMode(CockpitMode.MenuMode);
                return;
            }

            var mode = CockpitMode.InGame;

            if (edGuiFocus == EDGuiFocus.GalaxyMap || edGuiFocus == EDGuiFocus.SystemMap || edGuiFocus == EDGuiFocus.Orrery)
            {
                mode |= CockpitMode.Map;
            }
            else if (edGuiFocus == EDGuiFocus.FSSMode)
            {
                mode |= CockpitMode.FSSMode;
            }
            else
            {
                mode |= CockpitMode.Cockpit;
                if (eliteDangerousState.statusFlags.HasFlag(EDStatusFlags.InMainShip) || eliteDangerousState.statusFlags.HasFlag(EDStatusFlags.InFighter))
                {
                    mode |= CockpitMode.InShip;
                }
                if (eliteDangerousState.statusFlags.HasFlag(EDStatusFlags.InMainShip))
                {
                    mode |= CockpitMode.InMainShip;
                }
                if (eliteDangerousState.statusFlags.HasFlag(EDStatusFlags.InFighter))
                {
                    mode |= CockpitMode.InFighter;
                }
                if (eliteDangerousState.statusFlags.HasFlag(EDStatusFlags.InSRV))
                {
                    mode |= CockpitMode.InSRV;
                }
                if (eliteDangerousState.statusFlags.HasFlag(EDStatusFlags.LandingGearDown))
                {
                    mode |= CockpitMode.Landing;
                }


                switch (edGuiFocus)
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
            //universalUI?.SetActive(true);

            Mode = mode;
            ModeChanged.Send(Mode);
            modeChangedGameEvent.Raise(Mode);
        }

        void DisableAll()
        {
            menuMode?.SetActive(false);
            map?.SetActive(false);
            stationServices?.SetActive(false);
            cockpit?.SetActive(false);
            shipOnlyCockpit?.SetActive(false);
            mainShipOnlyCockpit?.SetActive(false);
            fighterOnlyCockpit?.SetActive(false);
            srvOnlyCockpit?.SetActive(false);
            fssMode?.SetActive(false);
            dssMode?.SetActive(false);
            universalUI?.SetActive(false);         
        }
    }
}
