using EVRC.Core.Overlay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Valve.VR;


namespace EVRC.Core
{
    using Events = SteamVR_Events;

    public class EDStateManager : MonoBehaviour
    {
        [Header("State Objects")]
        public EliteDangerousState eliteDangerousState;
        // Track the current process to see if it's Elite Dangerous
        private uint currentProcessId;
        private string currentProcessName;


        [Header("Game Events")]
        public EDStateEvent statusChanged;
        public BoolEvent eliteDangerousStartStop;
        public EDStatusFlagsEvent eDStatusFlagsEvent;
        public EDGuiFocusEvent eDGuiFocusEvent;
        // Replace these Steam Events with GameEvents
        public static Events.Event<uint, string> CurrentProcessChanged = new Events.Event<uint, string>();
        
        public static Events.Event<EDStatusFlags> FlagsChanged = new Events.Event<EDStatusFlags>();

        // If false, a new GuiFocusEvent will not be raised if only the focus panel changes. (ex: going from NoFocus to InternalPanel)
        // This should save some unnceccessary evaluation.
        // The SetRaiseForPanels method sets this value
        private bool raiseForGuiPanelEvents = false;

        void Start()
        {
            // Make sure we're starting fresh
            eliteDangerousState.Clear();

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
            SetRaiseForPanels();
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
            SetRaiseForPanels();
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
                eliteDangerousStartStop.Raise(true);
                SetRaiseForPanels();
            }
            else
            {
                eliteDangerousStartStop.Raise(false);
                eliteDangerousState.Clear();
            }
        }

        /// <summary>
        /// Evalutes whether any CockpitModeAnchors in the scene need to respond to GuiFocus events with panel changes.
        /// </summary>
        /// <remarks>
        /// If there are no Anchors in the scene that specifically activate based on looking at a certain panel (Internal, External, etc.)
        /// Then there is no need to raise those events
        /// </remarks>
        internal void SetRaiseForPanels()
        {
            // Get all CockpitModeAnchor components in the scene
            CockpitModeAnchor[] anchors = FindObjectsOfType<CockpitModeAnchor>(true);

            // Check the activationGuiFocus field for each CockpitModeAnchor
            foreach (CockpitModeAnchor anchor in anchors)
            {
                // Check if any of the CockpitModeAnchors have a value corresponding to enum values 0-4
                if (anchor.activationSettings.Any(x => (int)x.activationGuiFocus <= 4))
                {
                    // At least one CockpitModeAnchor has a value within the specified range, so set raiseForPanels to true
                    raiseForGuiPanelEvents = true;
                    return; // No need to continue the loop since the condition is already met
                }
            }
            raiseForGuiPanelEvents = false;
        }

       
        private IEnumerator WatchStatusFile()
        {
            var statusFile = Paths.StatusFilePath;
            UnityEngine.Debug.LogFormat("Watching Elite Dangerous Status.json at {0}", statusFile);
            eDGuiFocusEvent.Raise(EDGuiFocus.NoFocus); // initialize the cockpitModeAnchors
            eDStatusFlagsEvent.Raise(EDStatusFlags.InMainShip);
            eliteDangerousState.lastStatusFromFile.Flags = 0;
            eliteDangerousState.lastStatusFromFile.GuiFocus = 0;

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
                            statusChanged.Raise(eliteDangerousState);

                            if (eliteDangerousState.lastStatusFromFile.GuiFocus != status.GuiFocus)
                            {
                                var guiFocus = Enum.IsDefined(typeof(EDGuiFocus), status.GuiFocus)
                                    ? (EDGuiFocus)status.GuiFocus
                                    : EDGuiFocus.Unknown;

                                eliteDangerousState.guiFocus = guiFocus;
                                eDGuiFocusEvent.Raise(guiFocus);
                            }

                            if (eliteDangerousState.lastStatusFromFile.Flags != status.Flags)
                            {
                                // StatusFlags = (EDStatusFlags)status.Flags;
                                eliteDangerousState.statusFlags = (EDStatusFlags)status.Flags;
                                FlagsChanged.Send(eliteDangerousState.statusFlags);
                                eDStatusFlagsEvent.Raise(eliteDangerousState.statusFlags);
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
