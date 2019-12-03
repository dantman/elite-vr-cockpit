using System;
using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * Controller for the sensor zoom axis
     */
    public class SensorZoomAxisController : MonoBehaviour, IActivateable
    {
        public enum Orientation
        {
            Horizontal,
            VerticalUp,
        }
        public vJoyInterface output;
        public RectTransform barRegion;
        public RectTransform barMid;
        public Orientation orientation = Orientation.Horizontal;

        private float sensorZoom = 0;

        private void OnEnable()
        {
            output.SetSensorZoom(sensorZoom);
        }

        public Action Activate(ControllerInteractionPoint interactionPoint)
        {
            // @fixme Activations end when the controller leaves
            // we probably actyally want it to continue until released
            // So we'll need some way to tell the interaction point to not auto-release
            var handle = new CoroutineHandle();
            StartCoroutine(InteractionLoop(interactionPoint, handle));

            return () =>
            {
                handle.Stop();
            };
        }

        protected IEnumerator InteractionLoop(ControllerInteractionPoint interactionPoint, CoroutineHandle handle)
        {
            var sizeMagnitude = GetMagnitude(barRegion.rect.size);
            var pivot = GetMagnitude(barRegion.pivot);

            float PointToMagnitude(Vector3 pos)
            {
                var magnitude = (sizeMagnitude * pivot) + GetMagnitude(pos);
                // Scale to 0-1 values, with unclamped values <0,>1
                return magnitude / sizeMagnitude;
            }

            var lastAbsScale = PointToMagnitude(barRegion.InverseTransformPoint(interactionPoint.transform.position));
            yield return null;

            var lastSensorZoom = sensorZoom;
            while (handle.Running)
            {
                var absScale = PointToMagnitude(barRegion.InverseTransformPoint(interactionPoint.transform.position));

                var delta = absScale - lastAbsScale;
                

                // Clamp sensor zoom scale to the -/+1 range
                sensorZoom = Mathf.Clamp(sensorZoom + delta * 2, -1, 1);

                // Set vJoy axis
                output.SetSensorZoom(sensorZoom);

                // Move the bar and re-render the overlay on-demand when the bar has been moved a notable distance
                var updateScale = 0.01f;
                var sensorZoomDelta = sensorZoom - lastSensorZoom;
                if (Math.Abs(sensorZoomDelta) > updateScale)
                {
                    var roundedSensorZoom = Mathf.Round(sensorZoom / updateScale) * updateScale;
                    var barMag = ((roundedSensorZoom / 2f + 0.5f) - pivot) * sizeMagnitude;
                    barMid.anchoredPosition = SetMagnitude2(barMid.anchoredPosition, barMag);

                    lastSensorZoom = sensorZoom;
                    OnDemandRenderer.SafeDirty(gameObject);
                }

                lastAbsScale = absScale;
                yield return null;
            }
        }

        private float GetMagnitude(Vector3 vector)
        {
            switch (orientation)
            {
                case Orientation.Horizontal: return vector.x;
                case Orientation.VerticalUp: return vector.y;
                default: throw new NotImplementedException();
            }
        }

        private Vector2 SetMagnitude2(Vector2 vector, float magnitude)
        {
            switch (orientation)
            {
                case Orientation.Horizontal: return new Vector2(magnitude, vector.y);
                case Orientation.VerticalUp: return new Vector2(vector.x, magnitude);
                default: throw new NotImplementedException();
            }
        }
    }
}
