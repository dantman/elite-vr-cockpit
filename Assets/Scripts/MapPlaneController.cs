using System;
using UnityEngine;

namespace EVRC
{
    /**
     * A complex controller with components to control navigation of the galaxy map
     */
    public class MapPlaneController : MonoBehaviour
    {
        public vJoyInterface output;
        private Vector2 axisXZ = Vector2.zero;
        private float axisY = 0;

        private void OnEnable()
        {
            Reset();
            output.EnableMapAxis();
            UpdateAxis();
        }

        private void OnDisable()
        {
            output.DisableMapAxis();
        }

        private void Reset()
        {
            axisXZ = Vector2.zero;
            axisY = 0;
        }

        public void SetXZAxis(Vector2 axis)
        {
            axisXZ = axis;
            UpdateAxis();
        }

        public void SetYAxis(float axis)
        {
            axisY = axis;
            UpdateAxis();
        }

        protected void UpdateAxis()
        {
            if (!output.MapAxisEnabled) return;
            // @todo Use dial for zoom instead of the same axis as the throttle

            output.SetMapTranslationAxis(new Vector3(axisXZ.x, axisXZ.y, axisY));
        }
    }
}
