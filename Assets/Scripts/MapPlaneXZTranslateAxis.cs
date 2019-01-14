using System;
using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * MapPlaneController component that controls an axis based XZ translation input
     */
    public class MapPlaneXZTranslateAxis : MonoBehaviour, IActivateable
    {
        public MapHorizontalPlaneController mapHorizontalPlaneController;
        [Tooltip("The physical radius of the center deadzone to use when calculating normalized position on the axis")]
        [Range(0f, 1f)]
        public float centerRadius = 0f;
        [Tooltip("The physical radius of the axis to use when calculating normalized position on the axis")]
        [Range(0f, 1f)]
        public float axisRadius = 1f;

        public Action Activate(ControllerInteractionPoint interactionPoint)
        {
            if (!mapHorizontalPlaneController.Reserve()) return () => { };
            
            var handle = new CoroutineHandle();
            StartCoroutine(InteractionLoop(interactionPoint, handle));

            return () =>
            {
                handle.Stop();
                mapHorizontalPlaneController.Release();
            };
        }

        protected IEnumerator InteractionLoop(ControllerInteractionPoint interactionPoint, CoroutineHandle handle)
        {
            while (handle.Running)
            {
                Vector2 axis;
                var pos = transform.InverseTransformPoint(interactionPoint.transform.position);
                var axisPos = new Vector2(pos.x, pos.y);
                if (axisPos.magnitude < centerRadius)
                {
                    axis = Vector2.zero;
                }
                else
                {
                    // Subtract the center radius from the magnitute
                    axisPos = axisPos.normalized * (axisPos.magnitude - centerRadius);
                    // Divide by the size of the usable axis area to get an axis value
                    float x = (axisRadius - centerRadius);
                    axis = axisPos / x;
                    // Clamp values to the -/+1 range
                    axis.x = Mathf.Clamp(axis.x, -1, 1);
                    axis.y = Mathf.Clamp(axis.y, -1, 1);
                }

                mapHorizontalPlaneController.SetAxis(axis);

                yield return null;
            }

            mapHorizontalPlaneController.SetAxis(Vector2.zero);
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(Vector3.zero, centerRadius);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(Vector3.zero, axisRadius);
        }
    }
}
