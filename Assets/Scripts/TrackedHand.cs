using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class TrackedHand : MonoBehaviour
    {
        public enum Hand
        {
            Left,
            Right,
        }

        public Hand hand = Hand.Left;

        private uint deviceIndex = OpenVR.k_unTrackedDeviceIndexInvalid;

        public ETrackedControllerRole controllerRole
        {
            get
            {
                return hand == Hand.Right
                    ? ETrackedControllerRole.RightHand
                    : ETrackedControllerRole.LeftHand;
            }
        }

        public bool hasValidDevice
        {
            get
            {
                if (deviceIndex == OpenVR.k_unTrackedDeviceIndexInvalid)
                {
                    return false;
                }

                return true;
            }
        }

        SteamVR_Events.Action newPosesAction;

        TrackedHand()
        {
            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
        }

        void OnEnable()
        {
            SteamVR_Events.Initialized.Listen(OnRuntimeInitialized);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceActivated).Listen(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceDeactivated).Listen(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceUpdated).Listen(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceRoleChanged).Listen(OnTrackedDeviceEvent);
            newPosesAction.enabled = true;

            RescanDevices();
        }

        void OnDisable()
        {
            SteamVR_Events.Initialized.Remove(OnRuntimeInitialized);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceActivated).Remove(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceDeactivated).Remove(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceUpdated).Remove(OnTrackedDeviceEvent);
            SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceRoleChanged).Remove(OnTrackedDeviceEvent);
            newPosesAction.enabled = false;
        }

        void OnRuntimeInitialized(bool initialized)
        {
            if (initialized)
            {
                RescanDevices();
            }
        }

        void OnTrackedDeviceEvent(VREvent_t ev)
        {
            RescanDevices();
        }

        // Scan the tracked device list to find the left or right hand
        void RescanDevices()
        {
            deviceIndex = OpenVR.k_unTrackedDeviceIndexInvalid;

            var vr = OpenVR.System;
            if (vr == null) return;

            deviceIndex = vr.GetTrackedDeviceIndexForControllerRole(controllerRole);
        }

        void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            if (!hasValidDevice) return;

            if (poses.Length < deviceIndex) return;

            if (!poses[deviceIndex].bDeviceIsConnected) return;

            if (!poses[deviceIndex].bPoseIsValid) return;

            var pose = new SteamVR_Utils.RigidTransform(poses[deviceIndex].mDeviceToAbsoluteTracking);

            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;
        }

        private void Update()
        {
            if (!hasValidDevice) return;
        }
    }
}
