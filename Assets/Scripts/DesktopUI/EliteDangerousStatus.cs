using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EVRC.DesktopUI
{
    using EDGuiFocus = EDStateManager.EDStatus_GuiFocus;
    using EDFlags = EDStateManager.EDStatus_Flags;
    using CockpitMode = CockpitUIMode.CockpitMode;

    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class EliteDangerousStatus : MonoBehaviour
    {

        protected TMPro.TextMeshProUGUI textMesh;

        private void OnEnable()
        {
            textMesh = GetComponent<TMPro.TextMeshProUGUI>();
            EDStateManager.EliteDangerousStarted.Listen(OnGameStatedOrStopped);
            EDStateManager.EliteDangerousStarted.Listen(OnGameStatedOrStopped);
            CockpitUIMode.ModeChanged.Listen(OnCockpitUIModeChanged);
            Refresh();
        }

        private void OnDisable()
        {
            EDStateManager.EliteDangerousStarted.Remove(OnGameStatedOrStopped);
            EDStateManager.EliteDangerousStarted.Remove(OnGameStatedOrStopped);
            CockpitUIMode.ModeChanged.Remove(OnCockpitUIModeChanged);
        }

        private void OnGameStatedOrStopped()
        {
            Refresh();
        }

        private void OnCockpitUIModeChanged(CockpitMode mode)
        {
            Refresh();
        }

        private void Refresh()
        {
            textMesh.text = GetStatusText();
        }

        private string GetStatusText()
        {
            if (!EDStateManager.instance.IsEliteDangerousRunning)
            {
                return "Not running";
            }

            var GuiFocus = EDStateManager.instance.GuiFocus;
            var Flags = EDStateManager.instance.StatusFlags;

            switch (GuiFocus)
            {
                case EDGuiFocus.GalaxyMap:
                    return "Galaxy map";
                case EDGuiFocus.SystemMap:
                    return "System Map";
                case EDGuiFocus.Orrery:
                    return "Orrery";
                case EDGuiFocus.StationServices:
                    return "Station Services";
                case EDGuiFocus.SAAMode:
                    return "Detailed Surface Scanner";
                case EDGuiFocus.FSSMode:
                    return "Full Spectrum System Scanner";
                case EDGuiFocus.Codex:
                    return "Codex";
            }

            if (Flags.HasFlag(EDFlags.InMainShip)) return "Main Ship";
            if (Flags.HasFlag(EDFlags.InFighter)) return "Fighter";
            if (Flags.HasFlag(EDFlags.InSRV)) return "SRV";

            return "Running";
        }
    }
}
