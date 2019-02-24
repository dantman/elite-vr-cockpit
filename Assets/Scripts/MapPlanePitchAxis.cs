using System;
using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * MapPlaneController component that controls an axis based pitch input
     */
    public class MapPlanePitchAxis : MonoBehaviour, IActivateable
    {
        public MapPlaneController mapPlaneController;
        [Tooltip("The distance from the 0 point of the pitch slider at which the pitch axis reaches 100% pitch in/out speed")]
        public float axisMaxDistance = 1f;

        public Action Activate(ControllerInteractionPoint interactionPoint)
        {
            var handle = new CoroutineHandle();
            StartCoroutine(InteractionLoop(interactionPoint, handle));

            return () =>
            {
                handle.Stop();
            };
        }

        protected IEnumerator InteractionLoop(ControllerInteractionPoint interactionPoint, CoroutineHandle handle)
        {
            while (handle.Running)
            {
                var pos = transform.InverseTransformPoint(interactionPoint.transform.position);
                var pitchScale = pos.y / axisMaxDistance;
                // Clamp values to the -/+1 range
                pitchScale = Mathf.Clamp(pitchScale, -1, 1);

                mapPlaneController.SetPitch(pitchScale);

                yield return null;
            }

            mapPlaneController.SetPitch(0);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Vector3.down * axisMaxDistance, Vector3.up * axisMaxDistance);
        }
    }
}
