using System;
using UnityEngine;
using Valve.VR;

namespace EVRC.DesktopUI
{
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class OpenVRStatus : MonoBehaviour
    {
        protected TMPro.TextMeshProUGUI textMesh;
        protected string currentProcessName;

        private void OnEnable()
        {
            textMesh = GetComponent<TMPro.TextMeshProUGUI>();
            SteamVR_Events.Initialized.Listen(OnInitialize);
            EDStateManager.CurrentProcessChanged.Listen(OnCurrentProcessChanged);
            currentProcessName = EDStateManager.instance.currentProcessName;
            Refresh();
        }

        private void OnDisable()
        {
            SteamVR_Events.Initialized.Remove(OnInitialize);
            EDStateManager.CurrentProcessChanged.Remove(OnCurrentProcessChanged);
        }

        private void OnInitialize(bool initialized)
        {
            Refresh();
        }

        private void OnCurrentProcessChanged(uint pid, string processName)
        {
            currentProcessName = processName;
            Refresh();
        }

        void Refresh()
        {
            textMesh.text = GetStatusText() + "\n" + GetExeText();
        }

        private string GetStatusText()
        {
            var vr = OpenVR.System;
            if (vr == null)
            {
                return "Not connected";
            }
            else
            {
                return "Connected";
            }
        }

        private string GetExeText()
        {
            return "App: " + currentProcessName;
        }
    }
}
