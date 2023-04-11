using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    [CreateAssetMenu]
    public class vJoyState : GameState, ITextStatus
    {
        public VJoyStatus vJoyStatus = VJoyStatus.Unknown;

        public string GetStatus()
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
