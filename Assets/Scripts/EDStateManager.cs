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

        public static Events.Event EliteDangerousStarted = new Events.Event();
        public static Events.Event EliteDangerousStopped = new Events.Event();

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
                EliteDangerousStarted.Send();
            }
            else
            {
                EliteDangerousStopped.Send();
            }
        }
    }
}
