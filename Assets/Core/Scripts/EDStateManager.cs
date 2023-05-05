using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public static Events.Event<EDGuiFocus> GuiFocusChanged = new Events.Event<EDGuiFocus>();
        public static Events.Event<EDStatusFlags> FlagsChanged = new Events.Event<EDStatusFlags>();


        void Start()
        {
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

                    // Add this path to the list of places to search for control bindings configurations
                    Paths.controlBindingsPaths.Add(Path.Combine(eliteDangerousState.processDirectory, "ControlSchemes", Paths.StartPresetFileName + ".binds"));
                    
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
                StartCoroutine(WatchStatusFile());
                eliteDangerousStarted.Raise();
            }
            else
            {
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
    }
}
