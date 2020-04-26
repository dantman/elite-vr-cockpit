using System;
using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * Abstract vector axis controller for zoom/tuning controllers
     */
    public abstract class AxisController : MonoBehaviour, IActivateable
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

        private float axisValue = 0;

        private void OnEnable()
        {
            SetValue(axisValue);
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

            var lastAxisValue = axisValue;
            while (handle.Running)
            {
                var absScale = PointToMagnitude(barRegion.InverseTransformPoint(interactionPoint.transform.position));

                var delta = absScale - lastAbsScale;


                // Clamp axis scale to the -/+1 range
                axisValue = Mathf.Clamp(axisValue + delta * 2, -1, 1);

                // Set axis value
                SetValue(axisValue);

                // Move the bar and re-render the overlay on-demand when the bar has been moved a notable distance
                var updateScale = 0.01f;
                var axisValueDelta = axisValue - lastAxisValue;
                if (Math.Abs(axisValueDelta) > updateScale)
                {
                    var roundedAxisValue = Mathf.Round(axisValue / updateScale) * updateScale;
                    var barMag = ((roundedAxisValue / 2f + 0.5f) - pivot) * sizeMagnitude;
                    barMid.anchoredPosition = SetMagnitude2(barMid.anchoredPosition, barMag);

                    lastAxisValue = axisValue;
                    OnDemandRenderer.SafeDirty(gameObject);
                }

                lastAbsScale = absScale;
                yield return null;
            }
        }

        protected abstract void SetValue(float value);

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
