using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Events = SteamVR_Events;

    public class HudColorMatrixSyntaxErrorException : Exception
    {
        public HudColorMatrixSyntaxErrorException() { }
        public HudColorMatrixSyntaxErrorException(string message) : base(message) { }
        public HudColorMatrixSyntaxErrorException(string message, Exception inner) : base(message, inner) { }
    }

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
            HudInAnalysisMode = 1 << 27,
            NightVision = 1 << 28,
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
            Orrery = 8,
            FSSMode = 9,
            SAAMode = 10,
            Codex = 11,
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
        public uint currentPid { get; private set; }
        public string currentProcessName { get; private set; }
        public bool IsEliteDangerousRunning { get; private set; } = false;
        public HudColorMatrix hudColorMatrix { get; private set; } = HudColorMatrix.Identity();
        public EDControlBindings controlBindings;

        public static Events.Event EliteDangerousStarted = new Events.Event();
        public static Events.Event EliteDangerousStopped = new Events.Event();
        public static Events.Event<uint, string> CurrentProcessChanged = new Events.Event<uint, string>();
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
                    var savedGamesDir = WindowsUtilities.GetKnownFolderPath(WindowsUtilities.KnownFolderId.SavedGames, WindowsUtilities.KnownFolderFlag.DONT_VERIFY);
                    _saveDataPath = Path.Combine(savedGamesDir, "Frontier Developments", "Elite Dangerous");
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
        public static string CustomBindingsFolder
        {
            get
            {
                return Path.Combine(AppDataPath, "Options", "Bindings");
            }
        }
        public static string[] CustomBindingsOptionsPaths
        {
            get
            {
                return new string[] {
                    Path.Combine(CustomBindingsFolder, "Custom.3.0.binds"),
                    Path.Combine(CustomBindingsFolder, "Custom.2.0.binds"),
                    Path.Combine(CustomBindingsFolder, "Custom.1.8.binds"),
                };
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

            var applications = OpenVR.Applications;
            if (applications != null)
            {
                SetCurrentProcess(applications.GetCurrentSceneProcessId());
            }
        }

        void OnEnable()
        {
            Events.System(EVREventType.VREvent_SceneApplicationChanged).Listen(OnSceneApplicationChanged);
            Events.Initialized.AddListener(OnSteamVRInitialized);

            // Handle the case where SteamVR is already initialized
            if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess)
            {
                OnSteamVRInitialized(true);
            }
        }

        void OnDisable()
        {
            Events.System(EVREventType.VREvent_SceneApplicationChanged).Remove(OnSceneApplicationChanged);
            Events.Initialized.RemoveListener(OnSteamVRInitialized);
        }

        private void OnSteamVRInitialized(bool initialized)
        {
            if (initialized)
            {
                SetCurrentProcess(OpenVR.Compositor.GetCurrentSceneFocusProcess());
            }
        }

        private void OnSceneApplicationChanged(VREvent_t ev)
        {
            var pid = ev.data.process.pid;
            currentPid = pid;
            SetCurrentProcess(pid);
        }

        private void SetCurrentProcess(uint pid)
        {
            currentPid = pid;

            if (pid == 0)
            {
                currentProcessName = "";
                SetIsEliteDangerousRunning(false);
            }
            else
            {
                Process p = Process.GetProcessById((int)pid);
                currentProcessName = p.ProcessName;
                bool isEliteDangerous = p.ProcessName == EDProcessName32 || p.ProcessName == EDProcessName64;
                SetIsEliteDangerousRunning(isEliteDangerous);
            }

            CurrentProcessChanged.Send(currentPid, currentProcessName);
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
            try
            {
                string RedLine;
                string GreenLine;
                string BlueLine;
                try
                {
                    var doc = XDocument.Load(GraphicsConfigurationOverridePath);
                    var defaultGuiColor = doc.Descendants("GUIColour").Descendants("Default");
                    RedLine = (from el in defaultGuiColor.Descendants("MatrixRed") select el).FirstOrDefault()?.Value;
                    GreenLine = (from el in defaultGuiColor.Descendants("MatrixGreen") select el).FirstOrDefault()?.Value;
                    BlueLine = (from el in defaultGuiColor.Descendants("MatrixBlue") select el).FirstOrDefault()?.Value;
                }
                catch (XmlException e)
                {
                    throw new HudColorMatrixSyntaxErrorException("Failed to parse XML", e);
                }

                hudColorMatrix = new HudColorMatrix(
                    ParseColorLineElement(RedLine ?? "1, 0, 0"),
                    ParseColorLineElement(GreenLine ?? "0, 1, 0"),
                    ParseColorLineElement(BlueLine ?? "0, 0, 1"));
                HudColorMatrixChanged.Send(hudColorMatrix);
            }
            catch (HudColorMatrixSyntaxErrorException e)
            {
                hudColorMatrix = HudColorMatrix.Identity();

                UnityEngine.Debug.LogErrorFormat("Failed to load your HUD Color Matrix, you have a syntax error in your graphics configuration overrides file:\n{0}", GraphicsConfigurationOverridePath);
                UnityEngine.Debug.LogWarning(e.Message);
                if (e.InnerException != null)
                {
                    UnityEngine.Debug.LogWarning(e.InnerException.Message);
                }
            }

            HudColorMatrixChanged.Send(hudColorMatrix);
        }

        private float[] ParseColorLineElement(string line)
        {
            if (line.Trim() == "") throw new HudColorMatrixSyntaxErrorException("Matrix line was empty");
            return Regex.Split(line, ",\\s*").Select(nStr =>
            {
                if (float.TryParse(nStr, out float n))
                {
                    return n;
                }

                throw new HudColorMatrixSyntaxErrorException(string.Format("Could not parse \"{0}\" as a number", nStr));
            }).ToArray();
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
            var statusFile = StatusFilePath;
            UnityEngine.Debug.LogFormat("Watching Elite Dangerous Status.json at {0}", statusFile);

            while (IsEliteDangerousRunning)
            {
                try
                {
                    var text = File.ReadAllText(statusFile);
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
            foreach (var bindingsPath in CustomBindingsOptionsPaths)
            {
                if (File.Exists(bindingsPath))
                {
                    UnityEngine.Debug.LogFormat("Reading keyboard bindings from {0}", bindingsPath);
                    controlBindings = EDControlBindings.ParseFile(bindingsPath);
                    return;
                }
            }

            UnityEngine.Debug.LogWarning("No custom bindings found for ED, using an empty controls list");
            controlBindings = EDControlBindings.Empty();
        }
    }
}
