using System;
using System.Collections;
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

        [Flags]
        public enum EDStatus_Flags : uint
        {
            Docked = 1 << 0,
            Landed = 1 << 1,
            LandingGearDown = 1 << 2,
            ShieldsUp = 1 << 3,
            Supercruise = 1 << 4,
            FlightAssistOff = 1 << 5,
            HardpointsDeployed = 1 << 6,
            InWing = 1 << 7,
            LightsOn = 1 << 8,
            CargoScoopDeployed = 1 << 9,
            SilentRunning = 1 << 10,
            ScoopingFuel = 1 << 11,
            SrvHandbrake = 1 << 12,
            SrvTurret = 1 << 13,
            SrvUnderShip = 1 << 14,
            SrvDriveAssist = 1 << 15,
            FsdMassLocked = 1 << 16,
            FsdCharging = 1 << 17,
            FsdCooldown = 1 << 18,
            LowFuel = 1 << 19,
            OverHeating = 1 << 20,
            HasLatLong = 1 << 21,
            IsInDanger = 1 << 22,
            BeingInterdicted = 1 << 23,
            InMainShip = 1 << 24,
            InFighter = 1 << 25,
            InSRV = 1 << 26,
        }

        public enum EDStatus_GuiFocus : byte
        {
            NoFocus = 0,
            InternalPanel = 1,
            ExternalPanel = 2,
            CommsPanel = 3,
            RolePanel = 4,
            StationServices = 5,
            GalaxyMap = 6,
            SystemMap = 7,
            Unknown = byte.MaxValue
        }

        public struct EDStatus
        {
            public string timestamp;
            public uint Flags;
            public byte[] Pips;
            public byte FireGroup;
            public byte GuiFocus;
        }

        public static readonly string EDProcessName32 = "EliteDangerous32";
        public static readonly string EDProcessName64 = "EliteDangerous64";
        private uint currentPid;
        public bool IsEliteDangerousRunning { get; private set; } = false;
        public HudColorMatrix hudColorMatrix { get; private set; } = HudColorMatrix.Identity();
        public EDControlsBindings controlBindings;

        public static Events.Event EliteDangerousStarted = new Events.Event();
        public static Events.Event EliteDangerousStopped = new Events.Event();
        public static Events.Event<HudColorMatrix> HudColorMatrixChanged = new Events.Event<HudColorMatrix>();

        public EDStatus? LastStatus { get; private set; } = null;
        public EDStatus_GuiFocus GuiFocus { get; private set; } = EDStatus_GuiFocus.NoFocus;
        public EDStatus_Flags StatusFlags { get; private set; }

        public static Events.Event<EDStatus, EDStatus?> StatusChanged = new Events.Event<EDStatus, EDStatus?>();
        public static Events.Event<EDStatus_GuiFocus> GuiFocusChanged = new Events.Event<EDStatus_GuiFocus>();
        public static Events.Event<EDStatus_Flags> FlagsChanged = new Events.Event<EDStatus_Flags>();

        private static string _appDataPath;
        public static string AppDataPath
        {
            get
            {
                if (_appDataPath == null)
                {
                    var LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    _appDataPath = Path.Combine(LocalAppData, "Frontier Developments", "Elite Dangerous");
                }

                return _appDataPath;
            }
        }

        private static string _saveDataPath;
        public static string SaveDataPath
        {
            get
            {
                if (_saveDataPath == null)
                {
                    var userDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    _saveDataPath = Path.Combine(userDir, "Saved Games", "Frontier Developments", "Elite Dangerous");
                }

                return _saveDataPath;
            }
        }

        public static string GraphicsConfigurationOverridePath
        {
            get
            {
                return Path.Combine(AppDataPath, "Options", "Graphics", "GraphicsConfigurationOverride.xml");
            }
        }
        public static string CustomBindingsOptionsPath
        {
            get
            {
                return Path.Combine(AppDataPath, "Options", "Bindings", "Custom.3.0.binds");
            }
        }
        public static string StatusFilePath
        {
            get
            {
                return Path.Combine(SaveDataPath, "Status.json");
            }
        }

        void Start()
        {
            LoadHUDColorMatrix();
            LoadControlBindings();
            SetCurrentProcess(OpenVR.Applications.GetCurrentSceneProcessId());
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
            if (IsEliteDangerousRunning == running) return;
            IsEliteDangerousRunning = running;

            if (IsEliteDangerousRunning)
            {
                LoadHUDColorMatrix(); // Reload the HUD color matrix on start
                LoadControlBindings(); // Reload the control bindings on start
                StartCoroutine(WatchStatusFile());
                EliteDangerousStarted.Send();
            }
            else
            {
                EliteDangerousStopped.Send();
                LastStatus = null;
            }
        }

        /**
         * Read the user's GraphicsConfigurationOverride.xml and parse the HUD color matrix config
         */
        private void LoadHUDColorMatrix()
        {
            var doc = XDocument.Load(GraphicsConfigurationOverridePath);
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

        /**
         * Conditionally transform a color with ApplyHudColorMatrix.
         * Used to simplify coding in individual behaviours
         */
        public static Color ConditionallyApplyHudColorMatrix(bool condition, Color color)
        {
            if (condition)
            {
                return ApplyHudColorMatrix(color);
            }

            return color;
        }

        private IEnumerator WatchStatusFile()
        {
            while (IsEliteDangerousRunning)
            {
                try
                {
                    var text = File.ReadAllText(StatusFilePath);
                    if (text.Length > 0)
                    {
                        var status = JsonUtility.FromJson<EDStatus>(text);

                        if (LastStatus == null || status.timestamp != LastStatus.Value.timestamp)
                        {
                            StatusChanged.Send(status, LastStatus);

                            if (LastStatus == null || LastStatus.Value.GuiFocus != status.GuiFocus)
                            {
                                var guiFocus = Enum.IsDefined(typeof(EDStatus_GuiFocus), status.GuiFocus)
                                    ? (EDStatus_GuiFocus)status.GuiFocus
                                    : EDStatus_GuiFocus.Unknown;

                                GuiFocus = guiFocus;
                                GuiFocusChanged.Send(guiFocus);
                            }

                            if (LastStatus == null || LastStatus.Value.Flags != status.Flags)
                            {
                                StatusFlags = (EDStatus_Flags)status.Flags;
                                FlagsChanged.Send(StatusFlags);
                            }

                            LastStatus = status;
                        }
                    }
                }
                catch (IOException)
                {
                    // Ignore IO exceptions, these might be caused by inevitably reading while ED is writing
                }

                yield return new WaitForSecondsRealtime(1f);
            }
        }

        /**
         * Read the user's Custom.3.0.binds and parse the control bindings from it
         */
        private void LoadControlBindings()
        {
            // @todo Handle the situation where the custom bindings cannot be found
            controlBindings = EDControlsBindings.ParseFile(CustomBindingsOptionsPath);
        }
    }
}
