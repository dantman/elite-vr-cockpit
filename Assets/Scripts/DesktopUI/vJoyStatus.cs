using UnityEngine;

namespace EVRC.DesktopUI
{
    using VJoyStatus = vJoyInterface.VJoyStatus;

    [RequireComponent(typeof(TMPro.TextMeshProUGUI))]
    public class vJoyStatus : MonoBehaviour
    {
        protected TMPro.TextMeshProUGUI textMesh;

        private void OnEnable()
        {
            textMesh = GetComponent<TMPro.TextMeshProUGUI>();
            vJoyInterface.VJoyStatusChange.Listen(OnStatusChange);
            Refresh(vJoyInterface.vJoyStatus);
        }

        private void OnDisable()
        {
            vJoyInterface.VJoyStatusChange.Remove(OnStatusChange);
        }

        private void OnStatusChange(VJoyStatus status)
        {
            Refresh(status);
        }

        private void Refresh(VJoyStatus status)
        {
            textMesh.text = GetStatusText(status);
        }

        private string GetStatusText(VJoyStatus status)
        {
            switch (status)
            {
                case VJoyStatus.NotInstalled:
                    return "Not installed";
                case VJoyStatus.VersionMismatch:
                    return "Incompatible version";
                case VJoyStatus.DeviceUnavailable:
                    return "Device is unavailable";
                case VJoyStatus.DeviceOwned:
                    return "Device in use by other application";
                case VJoyStatus.DeviceError:
                    return "Unknown device error";
                case VJoyStatus.DeviceMisconfigured:
                    return "Device misconfigured";
                case VJoyStatus.DeviceNotAquired:
                    return "Failed to aquire device";
                case VJoyStatus.Ready:
                    return string.Format("Connected to devices #{0}/#{1}", vJoyInterface.deviceId, vJoyInterface.secondaryDeviceId);
                default:
                    return "Unknown";
            }
        }
    }
}
