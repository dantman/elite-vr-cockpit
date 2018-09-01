using UnityEngine;
using vJoyInterfaceWrap;
using Valve.VR;

namespace EVRC
{
    /**
     * Behaviour that outputs state to a virtual HOTAS using vJoy
     */
    public class vJoyInterface : MonoBehaviour
    {
        public enum VJoyStatus
        {
            Unknown,
            NotInstalled,
            VersionMismatch,
            DeviceUnavailable,
            DeviceOwned,
            DeviceError,
            DeviceNotAquired,
            Ready,
        }
        private vJoy vjoy;
        static public uint deviceId = 1;
        public static VJoyStatus vJoyStatus { get; private set; } = VJoyStatus.Unknown;
        public static SteamVR_Events.Event<VJoyStatus> VJoyStatusChange = new SteamVR_Events.Event<VJoyStatus>();

        void SetStatus(VJoyStatus status)
        {
            vJoyStatus = status;
            VJoyStatusChange.Send(status);
        }

        void OnEnable()
        {
            vjoy = new vJoy();

            if (!vjoy.vJoyEnabled())
            {
                SetStatus(VJoyStatus.NotInstalled);
                enabled = false;
                return;
            }

            uint DllVer = 0, DrvVer = 0;
            bool match = vjoy.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
            {
                Debug.LogFormat("vJoy Driver Version Matches vJoy DLL Version ({0:X})", DllVer);
            }
            else
            {
                Debug.LogErrorFormat("vJoy Driver Version ({0:X}) does NOT match vJoy DLL Version ({1:X})", DrvVer, DllVer);
                SetStatus(VJoyStatus.VersionMismatch);
                enabled = false;
                return;
            }

            VjdStat deviceStatus = vjoy.GetVJDStatus(deviceId);
            switch (deviceStatus)
            {
                case VjdStat.VJD_STAT_FREE:
                case VjdStat.VJD_STAT_OWN:
                    // We can continue if the device is free or we own it
                    break;
                case VjdStat.VJD_STAT_MISS:
                    Debug.LogWarningFormat("vJoy Device {0} is not installed or is disabled", deviceId);
                    SetStatus(VJoyStatus.DeviceUnavailable);
                    enabled = false;
                    return;
                case VjdStat.VJD_STAT_BUSY:
                    Debug.LogWarningFormat("vJoy Device {0} is owned by another application", deviceId);
                    SetStatus(VJoyStatus.DeviceOwned);
                    enabled = false;
                    return;
                default:
                    Debug.LogError("Unknown vJoy device status error");
                    SetStatus(VJoyStatus.DeviceError);
                    enabled = false;
                    return;
            }

            // @todo Validate the various axis and button config of the device and output an error if it is missing things

            if (deviceStatus == VjdStat.VJD_STAT_FREE)
            {
                if (vjoy.AcquireVJD(deviceId))
                {
                    Debug.LogFormat("Aquired vJoy device {0}", deviceId);
                }
                else
                {
                    Debug.LogErrorFormat("Unable to aquire vJoy device {0}", deviceId);
                    SetStatus(VJoyStatus.DeviceNotAquired);
                    enabled = false;
                    return;
                }
            }
            else if (deviceStatus == VjdStat.VJD_STAT_OWN)
            {
                Debug.LogFormat("vJoy device {0} already aquired", deviceId);
            }

            SetStatus(VJoyStatus.Ready);
        }

        void OnDisable()
        {
            if (vJoyStatus == VJoyStatus.Ready)
            {
                vjoy.RelinquishVJD(deviceId);
                SetStatus(VJoyStatus.Unknown);
            }
        }
    }
}
