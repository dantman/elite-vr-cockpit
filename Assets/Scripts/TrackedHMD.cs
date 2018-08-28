using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class TrackedHMD : MonoBehaviour
    {
        SteamVR_Events.Action newPosesAction;

        TrackedHMD()
        {
            newPosesAction = SteamVR_Events.NewPosesAction(OnNewPoses);
        }

        void OnEnable()
        {
            newPosesAction.enabled = true;
        }

        void OnDisable()
        {
            newPosesAction.enabled = false;
        }

        void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            var deviceIndex = OpenVR.k_unTrackedDeviceIndex_Hmd;

            if (poses.Length < deviceIndex) return;

            if (!poses[deviceIndex].bDeviceIsConnected) return;

            if (!poses[deviceIndex].bPoseIsValid) return;

            var pose = new SteamVR_Utils.RigidTransform(poses[deviceIndex].mDeviceToAbsoluteTracking);

            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;
        }
    }
}
