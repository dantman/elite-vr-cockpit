using System;
using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * MapPlaneController component that controls an axis based zoom input
     */
    public class MapPlaneZoomAxis : MonoBehaviour, IActivateable
    {
        public MapPlaneController mapPlaneController;
        [Tooltip("The distance from the 0 point of the zoom slider at which the zoom axis reaches 100% zoom in/out speed")]
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
                var zoomScale = pos.x / axisMaxDistance;
                // Clamp values to the -/+1 range
                zoomScale = Mathf.Clamp(zoomScale, -1, 1);

                mapPlaneController.SetZoom(zoomScale);

                yield return null;
            }

            mapPlaneController.SetZoom(0);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Vector3.left * axisMaxDistance, Vector3.right * axisMaxDistance);
        }
    }
}
