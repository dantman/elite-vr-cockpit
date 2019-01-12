using System;
using System.Collections.Generic;
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
            // When the application is using a seated universe convert it to a standing universe transform
            if (OpenVR.Compositor.GetTrackingSpace() == ETrackingUniverseOrigin.TrackingUniverseSeated)
            {
                var seatedTransformMatrix = OpenVR.System.GetSeatedZeroPoseToStandingAbsoluteTrackingPose();
                var seatedTransform = new SteamVR_Utils.RigidTransform(seatedTransformMatrix);
                pose = seatedTransform * pose;
            }

            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;
        }

        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Vector3.zero, 0.01f);
            Gizmos.DrawLine(Vector3.zero, Vector3.forward * 0.05f);
        }
    }
}
