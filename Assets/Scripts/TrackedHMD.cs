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
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TrackedHMD>();

                    if (_instance == null)
                    {
                        var hmd = new GameObject("[HMD]");
                        _instance = hmd.AddComponent<TrackedHMD>();
                    }
                }

                return _instance;
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

            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;
        }
    }
}
