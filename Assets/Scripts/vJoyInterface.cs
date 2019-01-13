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
            DeviceMisconfigured,
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
        [Range(0f, 100f)]
        public float directionalThrustersDeadzonePercentage = 0f;

        private vJoy vjoy;
        private vJoy.JoystickState iReport = new vJoy.JoystickState();

        public static uint deviceId = 1;
        public static VJoyStatus vJoyStatus { get; private set; } = VJoyStatus.Unknown;
        public static SteamVR_Events.Event<VJoyStatus> VJoyStatusChange = new SteamVR_Events.Event<VJoyStatus>();

        private VirtualJoystick.StickAxis stickAxis = VirtualJoystick.StickAxis.Zero;
        private Virtual6DOFController.ThrusterAxis thrusterAxis = Virtual6DOFController.ThrusterAxis.Zero;
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

            if (!IsDeviceValid(deviceId))
            {
                Debug.LogError("vJoy device is not configured correctly");
                SetStatus(VJoyStatus.DeviceMisconfigured);
                enabled = false;
                return;
            }

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
         * Checks to make sure the vJoy device has all the required configuration
         * @note Make sure to update this when adding code that adds buttons, axis, haptics, etc
         */
        private bool IsDeviceValid(uint deviceId)
        {
            var buttonN = vjoy.GetVJDButtonNumber(deviceId);
            var hatN = vjoy.GetVJDDiscPovNumber(deviceId);

            if (buttonN < 8)
            {
                Debug.LogWarningFormat("vJoy device has {0} buttons, at least 8 are required", buttonN);
                return false;
            }

            if (hatN < 4)
            {
                Debug.LogWarningFormat("vJoy device has {0} directional pov hat switches, 4 configured as directional are required", buttonN);
                return false;
            }

            var xAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_X);
            var yAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Y);
            var rzAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RZ);
            if (!xAxis || !yAxis || !rzAxis)
            {
                Debug.LogWarningFormat("vJoy device is missing one of the X/Y/Rz axis needed for the joystick [X:{0}, Y: {1}, Rz:{2}]", xAxis, yAxis, rzAxis);
                return false;
            }

            var zAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Z);
            if (!zAxis)
            {
                Debug.LogWarning("vJoy device is missing the Z axis needed for the throttle");
                return false;
            }

            var rxAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RX);
            var ryAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RY);
            var sliderAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_SL0);
            if (!rxAxis || !ryAxis || !sliderAxis)
            {
                Debug.LogWarningFormat("vJoy device is missing one of the Rx/Ry/Slider axis needed for the thruster axis [Rx:{0}, Ry: {1}, Slider:{2}]", rxAxis, ryAxis, sliderAxis);
                return false;
            }

            return true;
        }

        /**
         * Update the joystick axis
         */
        public void SetStickAxis(VirtualJoystick.StickAxis axis)
        {
            stickAxis = axis;
        }

        /**
         * Update the axis used for directional thrusters
         */
        public void SetThrusters(Virtual6DOFController.ThrusterAxis axis)
        {
            thrusterAxis = axis;
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

            iReport.AxisY = ConvertStickAxisDegreesToAxisInt(-stick.Pitch, HID_USAGES.HID_USAGE_Y);
            iReport.AxisX = ConvertStickAxisDegreesToAxisInt(stick.Roll, HID_USAGES.HID_USAGE_X);
            iReport.AxisZRot = ConvertStickAxisDegreesToAxisInt(stick.Yaw, HID_USAGES.HID_USAGE_RZ);

            var dThrusters = thrusterAxis.WithDeadzone(directionalThrustersDeadzonePercentage / 100f);

            iReport.AxisXRot = ConvertAxisRatioToAxisInt(dThrusters.Value.x, HID_USAGES.HID_USAGE_RX);
            iReport.AxisYRot = ConvertAxisRatioToAxisInt(dThrusters.Value.y, HID_USAGES.HID_USAGE_RY);
            iReport.Slider = ConvertAxisRatioToAxisInt(dThrusters.Value.z, HID_USAGES.HID_USAGE_SL0);

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
