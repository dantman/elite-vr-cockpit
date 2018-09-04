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

        public enum HatDirection : byte
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3,
            Neutral = 0xF,
        }

        [Range(0f, 90f)]
        public float joystickDeadzoneDegrees = 0f;
        [Range(0f, 90f)]
        public float joystickMaxDegrees = 90f;
        [Range(0f, 100f)]
        public float throttleDeadzonePercentage = 0f;

        private vJoy vjoy;
        private vJoy.JoystickState iReport = new vJoy.JoystickState();

        public static uint deviceId = 1;
        public static VJoyStatus vJoyStatus { get; private set; } = VJoyStatus.Unknown;
        public static SteamVR_Events.Event<VJoyStatus> VJoyStatusChange = new SteamVR_Events.Event<VJoyStatus>();

        private VirtualJoystick.StickAxis stickAxis = VirtualJoystick.StickAxis.Zero;
        private float throttle = 0f;
        private uint buttons = 0;
        private HatDirection[] hat = new HatDirection[] {
            HatDirection.Neutral,
            HatDirection.Neutral,
            HatDirection.Neutral,
            HatDirection.Neutral,
        };

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

        /**
         * Update the joystick axis
         */
        public void SetStickAxis(VirtualJoystick.StickAxis axis)
        {
            stickAxis = axis;
        }

        /**
         * Update the throttle
         */
        public void SetThrottle(float throttle)
        {
            this.throttle = throttle;
        }

        /**
         * Update the pressed state of a button
         */
        public void SetButton(uint buttonNumber, bool pressed)
        {
            int buttonIndex = (int)buttonNumber - 1;
            if (buttonNumber == 0)
            {
                throw new System.IndexOutOfRangeException("Button number 0 is too low, button numbers are zero indexed");
            }
            if (buttonIndex >= 32)
            {
                throw new System.IndexOutOfRangeException(string.Format("Button index {0} is too high", buttonIndex));
            }

            if (pressed)
            {
                buttons |= (uint)1 << buttonIndex;
            }
            else
            {
                buttons &= ~((uint)1 << buttonIndex);
            }
        }

        public void SetHatDirection(uint hatNumber, HatDirection dir)
        {
            int hatIndex = (int)hatNumber - 1;
            if (hatNumber == 0)
            {
                throw new System.IndexOutOfRangeException("Button number 0 is too low, button numbers are zero indexed");
            }
            if (hatIndex >= 4)
            {
                throw new System.IndexOutOfRangeException(string.Format("HAT index {0} is too high", hatIndex));
            }

            hat[hatIndex] = dir;
        }

        int ConvertAxisRatioToAxisInt(float axisRatio, HID_USAGES hid)
        {
            long min = 0, max = 0;
            var gotMin = vjoy.GetVJDAxisMin(deviceId, HID_USAGES.HID_USAGE_X, ref min);
            var gotMax = vjoy.GetVJDAxisMax(deviceId, HID_USAGES.HID_USAGE_X, ref max);
            if (!gotMin || !gotMax)
            {
                Debug.LogWarningFormat("Error getting min/max of HID axis {0}", hid.ToString());
                return 0;
            }

            // Get an absolute ratio where 0 is -Max, .5 is 0, and 1 is +Max
            float absRatio = axisRatio / 2f + .5f;
            long range = max - min;
            return (int)((long)(range * absRatio) + min);
        }

        int ConvertStickAxisDegreesToAxisInt(float axisDegres, HID_USAGES hid)
        {
            return ConvertAxisRatioToAxisInt(axisDegres / joystickMaxDegrees, hid);
        }

        void Update()
        {
            iReport.bDevice = (byte)deviceId;

            var stick = stickAxis.WithDeadzone(joystickDeadzoneDegrees);

            iReport.AxisY = ConvertStickAxisDegreesToAxisInt(stick.Pitch, HID_USAGES.HID_USAGE_Y);
            iReport.AxisX = ConvertStickAxisDegreesToAxisInt(stick.Roll, HID_USAGES.HID_USAGE_X);
            iReport.AxisZRot = ConvertStickAxisDegreesToAxisInt(stick.Yaw, HID_USAGES.HID_USAGE_RZ);

            var throttleWithDeadZone = Mathf.Abs(throttle) < (throttleDeadzonePercentage / 100f) ? 0f : throttle;
            iReport.AxisZ = ConvertAxisRatioToAxisInt(throttleWithDeadZone, HID_USAGES.HID_USAGE_Z);

            iReport.Buttons = buttons;

            iReport.bHats = (uint)((byte)hat[3] << 12)
                | (uint)((byte)hat[2] << 8)
                | (uint)((byte)hat[1] << 4)
                | (uint)hat[0];

            if (!vjoy.UpdateVJD(deviceId, ref iReport))
            {
                SetStatus(VJoyStatus.DeviceError);
                enabled = false;
            }
        }
    }
}
