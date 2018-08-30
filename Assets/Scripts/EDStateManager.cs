using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Events = SteamVR_Events;

    public class EDStateManager : MonoBehaviour
    {
        public static EDStateManager _instance;
        public static EDStateManager instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[EDStateManager]");
            }
        }

        public static readonly string EDProcessName32 = "EliteDangerous32";
        public static readonly string EDProcessName64 = "EliteDangerous64";
        private uint currentPid;
        public bool isEliteDangerousRunning { get; private set; } = false;
        public HudColorMatrix hudColorMatrix { get; private set; } = HudColorMatrix.Identity();

        private static string _saveDataPath;
        public static string saveDataPath
        {
            get
            {
                if (_saveDataPath == null)
                {
                    var LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    _saveDataPath = Path.Combine(LocalAppData, "Frontier Developments", "Elite Dangerous");
                }

                return _saveDataPath;
            }
        }
        public static string graphicsConfigurationOverridePath
        {
            get
            {
                return Path.Combine(saveDataPath, "Options", "Graphics", "GraphicsConfigurationOverride.xml");
            }
        }

        public static Events.Event EliteDangerousStarted = new Events.Event();
        public static Events.Event EliteDangerousStopped = new Events.Event();
        public static Events.Event<HudColorMatrix> HudColorMatrixChanged = new Events.Event<HudColorMatrix>();

        void Start()
        {
            LoadHUDColorMatrix();
        }

        void OnEnable()
        {
            Events.System(EVREventType.VREvent_ProcessConnected).Listen(OnProcessConnected);
            Events.System(EVREventType.VREvent_ProcessDisconnected).Listen(OnProcessDisconnected);
        }

        void OnDisable()
        {
            Events.System(EVREventType.VREvent_ProcessConnected).Remove(OnProcessConnected);
            Events.System(EVREventType.VREvent_ProcessDisconnected).Remove(OnProcessDisconnected);
        }

        private void OnProcessConnected(VREvent_t ev)
        {
            var pid = ev.data.process.pid;
            currentPid = pid;
            SetCurrentProcess(pid);
        }

        private void OnProcessDisconnected(VREvent_t ev)
        {
            var pid = ev.data.process.pid;
            if (currentPid == pid)
            {
                SetCurrentProcess(0);
            }
        }

        private void SetCurrentProcess(uint pid)
        {
            currentPid = pid;

            if (pid == 0)
            {
                SetIsEliteDangerousRunning(false);
            }
            else
            {
                Process p = Process.GetProcessById((int)pid);
                bool isEliteDangerous = p.ProcessName == EDProcessName32 || p.ProcessName == EDProcessName64;
                SetIsEliteDangerousRunning(isEliteDangerous);
            }
        }

        private void SetIsEliteDangerousRunning(bool running)
        {
            if (isEliteDangerousRunning == running) return;
            isEliteDangerousRunning = running;

            if (isEliteDangerousRunning)
            {
                LoadHUDColorMatrix(); // Reload the HUD color matrix on start
                EliteDangerousStarted.Send();
            }
            else
            {
                EliteDangerousStopped.Send();
            }
        }

        /**
         * Read the user's GraphicsConfigurationOverride.xml and parse the HUD color matrix config
         */
        private void LoadHUDColorMatrix()
        {
            var doc = XDocument.Load(graphicsConfigurationOverridePath);
            var defaultGuiColor = doc.Descendants("GUIColour").Descendants("Default");
            var RedLine = (from el in defaultGuiColor.Descendants("MatrixRed") select el).First().Value;
            var GreenLine = (from el in defaultGuiColor.Descendants("MatrixGreen") select el).First().Value;
            var BlueLine = (from el in defaultGuiColor.Descendants("MatrixBlue") select el).First().Value;

            hudColorMatrix = new HudColorMatrix(
                ParseColorLine(RedLine),
                ParseColorLine(GreenLine),
                ParseColorLine(BlueLine));
            HudColorMatrixChanged.Send(hudColorMatrix);
        }

        private float[] ParseColorLine(string line)
        {
            return Regex.Split(line, ",\\s*").Select(nStr => float.Parse(nStr)).ToArray();
        }

        /**
         * Transform a color with the Elite Dangerous HUD's color matrix
         */
        public static Color ApplyHudColorMatrix(Color color)
        {
            return instance.hudColorMatrix.Apply(color);
        }
    }
}
