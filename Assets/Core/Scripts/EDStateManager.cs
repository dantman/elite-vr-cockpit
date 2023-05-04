using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using EVRC.Core.Overlay;
using UnityEngine;
using Valve.VR;


namespace EVRC.Core
{
    using Events = SteamVR_Events;

    public class EDStateManager : MonoBehaviour
    {
        [Header("State Objects")]
        public EliteDangerousState eliteDangerousState;
        // public EDStatusFlags StatusFlags { get; private set; }
        public EDGuiFocus EDGuiFocus { get; private set; } = EDGuiFocus.NoFocus;
        // Track the current process to see if it's Elite Dangerous
        private uint currentProcessId;
        private string currentProcessName;


        [Header("Game Events")]
        public EDStateEvent statusChanged;
        public GameEvent eliteDangerousStarted;
        // Replace these Steam Events with GameEvents
        public static Events.Event EliteDangerousStopped = new Events.Event();
        public static Events.Event<uint, string> CurrentProcessChanged = new Events.Event<uint, string>();
        public static Events.Event<EDControlBindings> BindingsChanged = new Events.Event<EDControlBindings>();
        public static Events.Event<EDGuiFocus> GuiFocusChanged = new Events.Event<EDGuiFocus>();
        public static Events.Event<EDStatusFlags> FlagsChanged = new Events.Event<EDStatusFlags>();


        public static EDStateManager _instance;
        public static EDStateManager instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[EDStateManager]");
            }
        }
        
        public EDControlBindings controlBindings;


        private FileSystemWatcher bindsFileWatcher;
        private FileSystemWatcher startPresetFileWatcher;

        void Start()
        {
            // LoadHUDColorMatrix();
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

            if (eliteDangerousState == null)
            {
                UnityEngine.Debug.LogError(
                    "eliteDangerousState was not correctly set. Make sure an object is assigned in the inspector.");
                eliteDangerousState = ScriptableObject.CreateInstance<EliteDangerousState>();
                eliteDangerousState.processDirectory = "C:/Users/Parker/Downloads/";
            }
        }

        void OnDisable()
        {
            Events.System(EVREventType.VREvent_SceneApplicationChanged).Remove(OnSceneApplicationChanged);
            Events.Initialized.RemoveListener(OnSteamVRInitialized);
            UnwatchControlBindings();
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
            currentProcessId = pid;
            SetCurrentProcess(pid);
        }

        
        internal void SetCurrentProcess(uint pid)
        {
            currentProcessId = pid;

            if (pid == 0)
            {
                currentProcessName = "";
                SetIsEliteDangerousRunning(false);
            }
            else
            {
                var p = Process.GetProcessById((int)pid);
                currentProcessName = p.ProcessName;
                bool isEliteDangerous = p.ProcessName is Constants.edProcessName32 or Constants.edProcessName64;
                if (isEliteDangerous)
                {
                    eliteDangerousState.processDirectory = Path.GetDirectoryName(p.MainModule?.FileName);
                }
                SetIsEliteDangerousRunning(isEliteDangerous);
            }

            CurrentProcessChanged.Send(currentProcessId, eliteDangerousState.processName);
        }

        internal void SetIsEliteDangerousRunning(bool running)
        {
            if (eliteDangerousState.running == running) return;
            eliteDangerousState.running = running;
            eliteDangerousState.processId = currentProcessId;
            eliteDangerousState.processName = currentProcessName;

            if (eliteDangerousState.running)
            {
                LoadControlBindings(); // Reload the control bindings on start
                StartCoroutine(WatchStatusFile());
                WatchControlBindings();
                eliteDangerousStarted.Raise();
            }
            else
            {
                UnwatchControlBindings();
                EliteDangerousStopped.Send();
                eliteDangerousState.Clear();
            }
        }

       
        private IEnumerator WatchStatusFile()
        {
            var statusFile = Paths.StatusFilePath;
            UnityEngine.Debug.LogFormat("Watching Elite Dangerous Status.json at {0}", statusFile);

            while (eliteDangerousState.running)
            {
                try
                {
                    var text = File.ReadAllText(statusFile);
                    if (text.Length > 0)
                    {
                        var status = JsonUtility.FromJson<EDStatus>(text);

                        if (status.timestamp != eliteDangerousState.lastStatusFromFile.timestamp)
                        {
                            // statusChanged.Send(status, eliteDangerousState);
                            statusChanged.Raise(eliteDangerousState);

                            if (eliteDangerousState.lastStatusFromFile.GuiFocus != status.GuiFocus)
                            {
                                var guiFocus = Enum.IsDefined(typeof(EDGuiFocus), status.GuiFocus)
                                    ? (EDGuiFocus)status.GuiFocus
                                    : EDGuiFocus.Unknown;

                                EDGuiFocus = guiFocus;
                                eliteDangerousState.guiFocus = guiFocus;
                                GuiFocusChanged.Send(guiFocus);
                            }

                            if (eliteDangerousState.lastStatusFromFile.Flags != status.Flags)
                            {
                                // StatusFlags = (EDStatusFlags)status.Flags;
                                eliteDangerousState.statusFlags = (EDStatusFlags)status.Flags;
                                FlagsChanged.Send(eliteDangerousState.statusFlags);
                            }

                            eliteDangerousState.lastStatusFromFile = status;
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
         * Get the potential paths for a user's bindings file
         */
        public string[] GetControlBindingsFilePaths()
        {
            string StartPreset = File.Exists(Paths.StartPresetPath) ? File.ReadAllText(Paths.StartPresetPath) : "";
            if ((StartPreset ?? "").Trim() == "")
                StartPreset = "Custom";

            // Bindings from the user's Bindings directory
            var controlBindingsPaths = new List<string> {
                Path.Combine(Paths.CustomBindingsFolder, StartPreset + ".4.0.binds"),
                Path.Combine(Paths.CustomBindingsFolder, StartPreset + ".3.0.binds"),
                Path.Combine(Paths.CustomBindingsFolder, StartPreset + ".2.0.binds"),
                Path.Combine(Paths.CustomBindingsFolder, StartPreset + ".1.8.binds"),
            };

            if (eliteDangerousState.processDirectory != null)
            {
                // Built-in game bindings presets
                // @note These will only load after the game starts
                controlBindingsPaths.Add(Path.Combine(eliteDangerousState.processDirectory, "ControlSchemes", StartPreset + ".binds"));
            }

            return controlBindingsPaths.ToArray();
        }

        /**
         * Read the user's Custom.X.0.binds and parse the control bindings from it
         */
        private void LoadControlBindings()
        {
            foreach (var bindingsPath in GetControlBindingsFilePaths())
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

        /**
         * Watch for changes to the user's bindings files
         */
        private void WatchControlBindings()
        {
            UnwatchControlBindings();

            UnityEngine.Debug.LogFormat("Watching for changes to control bindings in {0}", Paths.CustomBindingsFolder);

            // Watch *.binds
            bindsFileWatcher = new FileSystemWatcher
            {
                Path = Paths.CustomBindingsFolder,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.binds"
            };
            bindsFileWatcher.Created += OnBindsChange;
            bindsFileWatcher.Changed += OnBindsChange;
            bindsFileWatcher.EnableRaisingEvents = true;

            // Watch StartPreset.start
            startPresetFileWatcher = new FileSystemWatcher
            {
                Path = Paths.CustomBindingsFolder,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(Paths.StartPresetPath)
            };
            startPresetFileWatcher.Created += OnBindsChange;
            startPresetFileWatcher.Changed += OnBindsChange;
            startPresetFileWatcher.EnableRaisingEvents = true;
        }

        /**
         * *.binds file change event
         */
        private void OnBindsChange(object sender, FileSystemEventArgs e)
        {
            LoadControlBindings();
            BindingsChanged.Send(controlBindings);
        }

        /**
         * Cleanup the bindings file watchers
         */
        private void UnwatchControlBindings()
        {
            if (bindsFileWatcher != null)
            {
                bindsFileWatcher.EnableRaisingEvents = false;
                bindsFileWatcher.Dispose();
                bindsFileWatcher = null;
            }

            if (startPresetFileWatcher != null)
            {
                startPresetFileWatcher.EnableRaisingEvents = false;
                startPresetFileWatcher.Dispose();
                startPresetFileWatcher = null;
            }
        }
    }
}
