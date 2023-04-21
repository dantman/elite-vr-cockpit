using System.Collections.Generic;
using UnityEngine.SpatialTracking;

namespace EVRC.Core.Actions
{
    using Hand = ActionsController.Hand;
    using TrackedPose = TrackedPoseDriver.TrackedPose;

    /**
     * Helpers for using the Unity XR Management Plugin (Unity 2021.3 feature)
     */
    public static class XRRigUtils
    {
        private static Dictionary<TrackedPose, Hand> xrTrackedPoseToHandMap = new Dictionary<TrackedPose, Hand>()
        {
            { TrackedPose.LeftPose, Hand.Left},
            { TrackedPose.RightPose, Hand.Right},
        };

        public static Hand GetHand(TrackedPose pose)
        {
            return xrTrackedPoseToHandMap[pose];
        }
    }
}
