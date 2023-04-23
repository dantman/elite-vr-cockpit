using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SpatialTracking;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    [RequireComponent(typeof(TrackedPoseDriver))]
    public class TrackedHMD : MonoBehaviour
    {
        private TrackedPoseDriver poseDriver;

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
            
        void OnEnable()
        {
            poseDriver = GetComponent<TrackedPoseDriver>();
            poseDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice,
                TrackedPoseDriver.TrackedPose.Center);
            poseDriver.updateType = TrackedPoseDriver.UpdateType.UpdateAndBeforeRender;
            poseDriver.trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        }
        
        
        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            float aspectRatio = 16f / 9f;
            Gizmos.DrawFrustum(Vector3.zero, 110f / aspectRatio, 1f, .05f, aspectRatio);
        }
    }
}
