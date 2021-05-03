using UnityEngine;
using vJoyInterfaceWrap;
using Valve.VR;
using System;

namespace EVRC
{
    /**
     * Behaviour that outputs state to a virtual HOTAS using vJoy
     */
    public class vJoyInterface : MonoBehaviour
    {
        public static vJoyInterface _instance;
        public static vJoyInterface instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[vJoy]");
            }
        }

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
        public float directionalThrustersDeadzonePercentage = 0f;

        private vJoy vjoy;
        private vJoy.JoystickState iReport = new vJoy.JoystickState();
        private vJoy.JoystickState iReport2 = new vJoy.JoystickState();

        public static uint deviceId = 1;
        public static uint secondaryDeviceId = 2;
        public static VJoyStatus vJoyStatus { get; private set; } = VJoyStatus.Unknown;
        public static SteamVR_Events.Event<VJoyStatus> VJoyStatusChange = new SteamVR_Events.Event<VJoyStatus>();

        public bool MapAxisEnabled { get; private set; } = false;
        private VirtualJoystick.StickAxis stickAxis = VirtualJoystick.StickAxis.Zero;
        private Virtual6DOFController.ThrusterAxis thrusterAxis = Virtual6DOFController.ThrusterAxis.Zero;
        private float throttle = 0f;
        public bool throttleReverse { get; private set; } = false;
        private float sensorZoom = 0f;
        private Vector3 mapTranslationAxis = Vector3.zero;
        private float mapPitchAxis = 0;
        private float mapYawAxis = 0;
        private float mapZoomAxis = 0;
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
            VjdStat secondaryDeviceStatus = vjoy.GetVJDStatus(secondaryDeviceId);
            if (!IsDeviceStatusOk(deviceId, deviceStatus) || !IsDeviceStatusOk(secondaryDeviceId, secondaryDeviceStatus))
            {
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

            if (!IsSecondaryDeviceValid(deviceId))
            {
                Debug.LogError("Secondary vJoy device is not configured correctly");
                SetStatus(VJoyStatus.DeviceMisconfigured);
                enabled = false;
                return;
            }

            if (!AcquireDevice(deviceId, deviceStatus) || !AcquireDevice(secondaryDeviceId, secondaryDeviceStatus))
            {
                enabled = false;
                return;
            }

            SetStatus(VJoyStatus.Ready);
        }

        /**
         * Verify that a vJoy device is vailable
         */
        private bool IsDeviceStatusOk(uint deviceId, VjdStat deviceStatus)
        {
            switch (deviceStatus)
            {
                case VjdStat.VJD_STAT_FREE:
                case VjdStat.VJD_STAT_OWN:
                    // We can continue if the device is free or we own it
                    return true;
                case VjdStat.VJD_STAT_MISS:
                    Debug.LogWarningFormat("vJoy Device {0} is not installed or is disabled", deviceId);
                    SetStatus(VJoyStatus.DeviceUnavailable);
                    return false;
                case VjdStat.VJD_STAT_BUSY:
                    Debug.LogWarningFormat("vJoy Device {0} is owned by another application", deviceId);
                    SetStatus(VJoyStatus.DeviceOwned);
                    return false;
                default:
                    Debug.LogError("Unknown vJoy device status error");
                    SetStatus(VJoyStatus.DeviceError);
                    return false;
            }
        }

        /**
         * Aquire or verify a vJoy device is already aquired
         */
        private bool AcquireDevice(uint deviceId, VjdStat deviceStatus)
        {
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
                    return false;
                }
            }
            else if (deviceStatus == VjdStat.VJD_STAT_OWN)
            {
                Debug.LogFormat("vJoy device {0} already aquired", deviceId);
            }

            return true;
        }

        void OnDisable()
        {
            if (vJoyStatus == VJoyStatus.Ready)
            {
                vjoy.RelinquishVJD(deviceId);
                vjoy.RelinquishVJD(secondaryDeviceId);
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
                Debug.LogWarningFormat("vJoy device has {0} directional pov hat switches, 4 configured as directional are required", hatN);
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

            var dialAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_SL1);
            if (!dialAxis)
            {
                Debug.LogWarning("vJoy device is missing the Dial/Slider2 axis needed for the map zoom axis");
                return false;
            }

            return true;
        }

        /**
         * Checks to make sure the vJoy device used for secondary axis has all the required configuration
         * @note Make sure to update this when adding code that adds buttons, axis, haptics, etc
         */
        private bool IsSecondaryDeviceValid(uint deviceId)
        {
            var xAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_X);
            var yAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Y);
            var zAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_Z);
            if (!xAxis || !yAxis || !zAxis)
            {
                Debug.LogWarningFormat("vJoy device is missing one of the X/Y/Z axis needed for galaxy map movement [X:{0}, Y: {1}, Z:{2}]", xAxis, yAxis, zAxis);
                return false;
            }

            var rxAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RX);
            var rzAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_RZ);
            if (!rxAxis || !rzAxis)
            {
                Debug.LogWarningFormat("vJoy device is missing one of the Rx/Rz axis needed for the galaxy map pitch/rotation [Rx:{0}, Rz: {1}]", rxAxis, rzAxis);
                return false;
            }

            var dialAxis = vjoy.GetVJDAxisExist(deviceId, HID_USAGES.HID_USAGE_SL1);
            if (!dialAxis)
            {
                Debug.LogWarning("vJoy device is missing the Dial/Slider2 axis needed for the map zoom axis");
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
            if (throttle >= 0 && (!throttleReverse || throttle < this.throttle))
            {
                this.throttle = throttle;
            }
            else if (throttle <= 0 && (throttleReverse ||throttle > this.throttle))
            {
                this.throttle = throttle;
            }
        }

        /**
         * Enable the reverse lock on the Throttle
         */
        public void EnableReverse()
        {
            throttleReverse = true;
        }

        /**
         * Disable the reverse lock on the Throttle
         */
        public void DisableReverse()
        {
            throttleReverse = false;
        }

        /**
         * Update the sensor zoom
         */
        public void SetSensorZoom(float sensorZoom)
        {
            this.sensorZoom = sensorZoom;
        }

        /**
         * Enable the map control axis
         */
        public void EnableMapAxis()
        {
            MapAxisEnabled = true;
            ResetMapAxis();
        }

        /**
         * Disable the map control axis
         */
        public void DisableMapAxis()
        {
            MapAxisEnabled = false;
            ResetMapAxis();
        }

        private void ResetMapAxis()
        {
            mapTranslationAxis = Vector3.zero;
        }

        /**
         * Update the map translation axis
         */
        public void SetMapTranslationAxis(Vector3 translation)
        {
            mapTranslationAxis = translation;
        }

        /**
         * Update the map pitch axis
         */
        public void SetMapPitchAxis(float pitch)
        {
            mapPitchAxis = pitch;
        }

        /**
         * Update the map yaw axis
         */
        public void SetMapYawAxis(float yaw)
        {
            mapYawAxis = yaw;
        }

        /**
         * Update the map zoom axis
         */
        public void SetMapZoomAxis(float zoom)
        {
            mapZoomAxis = zoom;
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

        int ConvertAxisRatioToAxisInt(uint deviceId, float axisRatio, HID_USAGES hid)
        {
            long min = 0, max = 0;
            // @fixme This looks wrong
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

        int ConvertStickAxisDegreesToAxisInt(uint deviceId, float axisDegres, HID_USAGES hid)
        {
            return ConvertAxisRatioToAxisInt(deviceId, axisDegres / joystickMaxDegrees, hid);
        }

        void Update()
        {
            iReport.bDevice = (byte)deviceId;
            iReport2.bDevice = (byte)secondaryDeviceId;

            // Device 1, joystick/throttle
            if (!MapAxisEnabled)
            {
                var stick = stickAxis.WithDeadzone(joystickDeadzoneDegrees);

                iReport.AxisY = ConvertStickAxisDegreesToAxisInt(deviceId, -stick.Pitch, HID_USAGES.HID_USAGE_Y);
                iReport.AxisX = ConvertStickAxisDegreesToAxisInt(deviceId, stick.Roll, HID_USAGES.HID_USAGE_X);
                iReport.AxisZRot = ConvertStickAxisDegreesToAxisInt(deviceId, stick.Yaw, HID_USAGES.HID_USAGE_RZ);

                var dThrusters = thrusterAxis.WithDeadzone(directionalThrustersDeadzonePercentage / 100f);

                iReport.AxisXRot = ConvertAxisRatioToAxisInt(deviceId, dThrusters.Value.x, HID_USAGES.HID_USAGE_RX);
                iReport.AxisYRot = ConvertAxisRatioToAxisInt(deviceId, dThrusters.Value.y, HID_USAGES.HID_USAGE_RY);
                iReport.Slider = ConvertAxisRatioToAxisInt(deviceId, dThrusters.Value.z, HID_USAGES.HID_USAGE_SL0);

                var throttleWithDeadZone = throttle;
                iReport.AxisZ = ConvertAxisRatioToAxisInt(deviceId, throttleWithDeadZone, HID_USAGES.HID_USAGE_Z);

                // Cockpit sensor zoom axis
                iReport.Dial = ConvertAxisRatioToAxisInt(deviceId, sensorZoom, HID_USAGES.HID_USAGE_SL1);
            }
            else
            {
                // Make sure all the joystick axis are reset
                iReport.AxisY = ConvertStickAxisDegreesToAxisInt(deviceId, 0, HID_USAGES.HID_USAGE_Y);
                iReport.AxisX = ConvertStickAxisDegreesToAxisInt(deviceId, 0, HID_USAGES.HID_USAGE_X);
                iReport.AxisZRot = ConvertStickAxisDegreesToAxisInt(deviceId, 0, HID_USAGES.HID_USAGE_RZ);
                iReport.AxisXRot = ConvertAxisRatioToAxisInt(deviceId, 0, HID_USAGES.HID_USAGE_RX);
                iReport.AxisYRot = ConvertAxisRatioToAxisInt(deviceId, 0, HID_USAGES.HID_USAGE_RY);
                iReport.Slider = ConvertAxisRatioToAxisInt(deviceId, 0, HID_USAGES.HID_USAGE_SL0);
                iReport.AxisZ = ConvertAxisRatioToAxisInt(deviceId, 0, HID_USAGES.HID_USAGE_Z);

                // FSS tuning axis
                iReport.Dial = ConvertAxisRatioToAxisInt(deviceId, sensorZoom, HID_USAGES.HID_USAGE_SL1);
            }

            iReport.Buttons = buttons;

            iReport.bHats = (uint)((byte)hat[3] << 12)
                | (uint)((byte)hat[2] << 8)
                | (uint)((byte)hat[1] << 4)
                | (uint)hat[0];

            // Device 2, primarily map
            if (MapAxisEnabled)
            {
                // Translation
                iReport2.AxisX = ConvertAxisRatioToAxisInt(secondaryDeviceId, mapTranslationAxis.x, HID_USAGES.HID_USAGE_X);
                iReport2.AxisY = ConvertAxisRatioToAxisInt(secondaryDeviceId, mapTranslationAxis.y, HID_USAGES.HID_USAGE_Y);
                iReport2.AxisZ = ConvertAxisRatioToAxisInt(secondaryDeviceId, mapTranslationAxis.z, HID_USAGES.HID_USAGE_Z);

                // Pitch / Yaw
                iReport2.AxisXRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, -mapPitchAxis, HID_USAGES.HID_USAGE_RX);
                iReport2.AxisZRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, mapYawAxis, HID_USAGES.HID_USAGE_RZ);

                // Zoom
                iReport2.Dial = ConvertAxisRatioToAxisInt(secondaryDeviceId, -mapZoomAxis, HID_USAGES.HID_USAGE_SL1);
            }
            else
            {
                // Make sure the map axis are reset
                iReport2.AxisX = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_X);
                iReport2.AxisY = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_Y);
                iReport2.AxisZ = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_Z);
                iReport2.AxisXRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_RX);
                iReport2.AxisZRot = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_RZ);
                iReport2.Dial = ConvertAxisRatioToAxisInt(secondaryDeviceId, 0, HID_USAGES.HID_USAGE_SL1);
            }

            if (!vjoy.UpdateVJD(deviceId, ref iReport))
            {
                Debug.LogFormat("vJoy Device {0} update failed", deviceId);
                SetStatus(VJoyStatus.DeviceError);
                enabled = false;
            }
            if (!vjoy.UpdateVJD(secondaryDeviceId, ref iReport2))
            {
                Debug.LogFormat("vJoy Device {0} update failed", secondaryDeviceId);
                SetStatus(VJoyStatus.DeviceError);
                enabled = false;
            }
        }
    }
}
