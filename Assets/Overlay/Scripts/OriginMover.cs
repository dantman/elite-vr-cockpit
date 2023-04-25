using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Helper To make sure the Grabbable object that is used to re-position the origin is
    /// reachable after a position reset
    /// </summary>
    public class OriginMover : MonoBehaviour
    {
        //Default position is roughly in front of the user's chest 
        public Vector3 grabbablePosition = new Vector3(0,0.8f,0.5f);

        void OnEnable()
        {
            // transform.SetPositionAndRotation(grabbablePosition, Quaternion.identity);
        }

        public void MoveOriginGrabber(Vector3 seatedTransformPos, Quaternion transformRotation)
        {
            var pos = seatedTransformPos + grabbablePosition; // offset from seated origin
            transform.SetPositionAndRotation(pos, transformRotation);


            // transform.SetPositionAndRotation(grabbablePosition, Quaternion.identity);
            // transform.position = grabbablePosition;
        }
    }
}
