using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/vJoy State"), System.Serializable]
    public class VJoyState : GameState
    {
        public VJoyStatus vJoyStatus = VJoyStatus.Unknown;

        public override string GetStatusText()
        {
            switch (vJoyStatus)
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
