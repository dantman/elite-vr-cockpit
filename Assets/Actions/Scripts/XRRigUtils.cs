using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;

namespace EVRC
{
    using Hand = ActionsController.Hand;
    using TrackedPose = TrackedPoseDriver.TrackedPose;

    /**
     * Helpers for using the Unity XR Management Plugin (Unity 2021.3 feature)
     */
    public class XRRigUtils
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
