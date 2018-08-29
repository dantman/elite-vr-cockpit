using System;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class SeatedOriginTracker : MonoBehaviour
    {
        void OnEnable()
        {
            SteamVR_Events.Initialized.Listen(OnRuntimeInitialized);
            SteamVR_Events.System(EVREventType.VREvent_SeatedZeroPoseReset).Listen(OnSeatedZeroPoseReset);

            RecalculateOrigin();
        }

        void OnDisable()
        {
            SteamVR_Events.Initialized.Remove(OnRuntimeInitialized);
            SteamVR_Events.System(EVREventType.VREvent_SeatedZeroPoseReset).Remove(OnSeatedZeroPoseReset);
        }

        void OnRuntimeInitialized(bool initialized)
        {
            if (initialized)
            {
                RecalculateOrigin();
            }
        }

        private void OnSeatedZeroPoseReset(VREvent_t arg0)
        {
            RecalculateOrigin();
        }

        void RecalculateOrigin()
        {
            var vr = OpenVR.System;
            if (vr == null) return;

            var seatedTransformMatrix = vr.GetSeatedZeroPoseToStandingAbsoluteTrackingPose();
            var seatedTransform = new SteamVR_Utils.RigidTransform(seatedTransformMatrix);
            transform.position = seatedTransform.pos;
            transform.rotation = seatedTransform.rot;
        }

        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.white;

            // Dimensions
            float w = .4f;
            float h = 1f;
            float d = .6f;
            float hw = w / 2f; // Half width

            // Corners
            // b=bottom, l=left, r=right, f=forward, t=top
            var bl = Vector3.left * hw;
            var br = Vector3.right * hw;
            var f = Vector3.forward * d;
            var blf = bl + f;
            var brf = br + f;
            var t = Vector3.up * h;
            var tl = bl + t;
            var tr = br + t;

            // Bottom
            Gizmos.DrawLine(bl, br);
            Gizmos.DrawLine(bl, blf);
            Gizmos.DrawLine(br, brf);
            Gizmos.DrawLine(blf, brf);
            Gizmos.DrawLine(bl, brf);
            Gizmos.DrawLine(br, blf);
            // Back
            Gizmos.DrawLine(bl, tl);
            Gizmos.DrawLine(br, tr);
            Gizmos.DrawLine(tl, tr);
            Gizmos.DrawLine(bl, tr);
            Gizmos.DrawLine(br, tl);
        }
    }
}
