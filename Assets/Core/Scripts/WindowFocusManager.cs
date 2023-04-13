using System;
using System.Runtime.InteropServices;
using EVRC.Core.Overlay;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core
{
    /**
     * Uses native Windows APIs to monitor changes in the foreground window
     */
    public class WindowFocusManager : MonoBehaviour
    {
        public static WindowFocusManager _instance;
        public static WindowFocusManager instance
        {
            get
            {
                return Create();
            }
        }

        public static WindowFocusManager Create()
        {
            return OverlayUtils.Singleton(ref _instance, "[WindowFocusManager]");
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        const uint WINEVENT_OUTOFCONTEXT = 0x0000; // Events are ASYNC
        const uint WINEVENT_SKIPOWNPROCESS = 0x0002; // Don't call back for events on installer's process
        const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
        delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
            IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
            WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);

        public static uint ForegroundWindowPid { get; private set; }
        public IntPtr foregroundWinEvent { get; private set; } = IntPtr.Zero;

        public static SteamVR_Events.Event<uint> ForegroundWindowProcessChanged = new SteamVR_Events.Event<uint>();

        void OnEnable()
        {
            foregroundWinEvent = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, OnForegroundChanged, 0, 0, WINEVENT_OUTOFCONTEXT);
            UpdateForegroundWindow(GetForegroundWindow());
        }

        void OnDisable()
        {
            if (foregroundWinEvent != IntPtr.Zero)
            {
                UnhookWinEvent(foregroundWinEvent);
                foregroundWinEvent = IntPtr.Zero;
            }
        }

        private void OnDestroy()
        {
            if (foregroundWinEvent != IntPtr.Zero)
            {
                UnhookWinEvent(foregroundWinEvent);
                foregroundWinEvent = IntPtr.Zero;
            }
        }

        // @warning Native WinEvents appear to eventually lose track of `this` resulting in it being null
        // So this code MUST be static and not use references to `this`
        private static void OnForegroundChanged(IntPtr hWinEventHook, uint eventType, IntPtr hWnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            UpdateForegroundWindow(hWnd);
        }

        private static void UpdateForegroundWindow(IntPtr hWnd)
        {
            uint pid = 0;
            if (hWnd != IntPtr.Zero)
            {
                GetWindowThreadProcessId(hWnd, out pid);
            }

            if (pid != ForegroundWindowPid)
            {
                ForegroundWindowPid = pid;
                ForegroundWindowProcessChanged.Send(ForegroundWindowPid);
            }
        }

        public bool IsEliteDangerousFocused
        {
            get
            {
                var stateManager = EDStateManager.instance;
                if (!stateManager.IsEliteDangerousRunning) return false;
                return stateManager.currentPid == ForegroundWindowPid;
            }
        }
    }
}
