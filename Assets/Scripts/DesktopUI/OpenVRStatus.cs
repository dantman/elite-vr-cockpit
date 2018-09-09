using UnityEngine;
using Valve.VR;

namespace EVRC.DesktopUI
{
    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class OpenVRStatus : MonoBehaviour
    {
        protected TMPro.TextMeshProUGUI textMesh;

        private void OnEnable()
        {
            textMesh = GetComponent<TMPro.TextMeshProUGUI>();
            SteamVR_Events.Initialized.Listen(OnInitialize);
            Refresh();
        }

        private void OnDisable()
        {
            SteamVR_Events.Initialized.Remove(OnInitialize);
        }

        private void OnInitialize(bool initialized)
        {
            Refresh();
        }

        void Refresh()
        {
            textMesh.text = GetStatusText();
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
    }
}
