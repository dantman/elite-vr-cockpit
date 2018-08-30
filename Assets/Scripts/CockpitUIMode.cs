using System;
using UnityEngine;

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
        public GameObject map;
        public GameObject cockpit;
        public GameObject shipOnlyCockpit;
        public GameObject srvOnlyCockpit;
        private EDGuiFocus GuiFocus;
        private EDStatus_Flags StatusFlags;

        [Flags]
        public enum CockpitMode : byte
        {
            GameNotRunning = 0x01,
            InGame = 0x02,
            Map = 0x04,
            Cockpit = 0x08,
            InMainShip = 0x10,
            InSRV = 0x20,
        }

        void OnEnable()
        {
            EDStateManager.EliteDangerousStarted.Listen(OnGameStartedOrStopped);
            EDStateManager.EliteDangerousStopped.Listen(OnGameStartedOrStopped);
            EDStateManager.GuiFocusChanged.Listen(OnGuiFocusChanged);
            EDStateManager.FlagsChanged.Listen(OnFlagsChanged);
            Refresh();
        }


        void OnDisable()
        {
            EDStateManager.EliteDangerousStarted.Remove(OnGameStartedOrStopped);
            EDStateManager.EliteDangerousStopped.Remove(OnGameStartedOrStopped);
            EDStateManager.GuiFocusChanged.Remove(OnGuiFocusChanged);
            EDStateManager.FlagsChanged.Listen(OnFlagsChanged);
        }

        private void OnGameStartedOrStopped()
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

        void Refresh()
        {
            if (!EDStateManager.instance.IsEliteDangerousRunning)
            {
                if (gameNotRunning != null) gameNotRunning.SetActive(true);
                SetMode(CockpitMode.GameNotRunning);
                return;
            }

            var mode = CockpitMode.InGame;

            if (GuiFocus == EDGuiFocus.GalaxyMap || GuiFocus == EDGuiFocus.SystemMap)
            {
                mode |= CockpitMode.Map;
            }
            else
            {
                mode |= CockpitMode.Cockpit;
                if (StatusFlags.HasFlag(EDStatus_Flags.InMainShip))
                {
                    mode |= CockpitMode.InMainShip;
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
                shipOnlyCockpit.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InMainShip));
            }
            if (srvOnlyCockpit)
            {
                srvOnlyCockpit.SetActive(mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InSRV));
            }
        }
    }
}
