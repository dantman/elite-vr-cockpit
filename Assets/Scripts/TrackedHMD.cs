using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class TrackedHMD : MonoBehaviour
    {
        public static TrackedHMD _instance;
        public static TrackedHMD instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[HMD]");
            }
        }

        public static Transform Transform
        {
            get
            {
                return instance.transform;
            }
        }
            
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

            if (_instance == this)
            {
                _instance = null;
            }
        }

        void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            var deviceIndex = OpenVR.k_unTrackedDeviceIndex_Hmd;

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
            Gizmos.color = Color.black;
            float aspectRatio = 16f / 9f;
            Gizmos.DrawFrustum(Vector3.zero, 110f / aspectRatio, 1f, .05f, aspectRatio);
        }
    }
}
