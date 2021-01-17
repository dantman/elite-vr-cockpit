using UnityEngine;

namespace EVRC
{
    /**
     * A complex controller with components to control navigation of the galaxy map
     */
    public class MapPlaneController : MonoBehaviour
    {
        private Vector2 axisXZ = Vector2.zero;
        private float axisY = 0;
        private float axisPitch = 0;
        private float axisYaw = 0;
        private float axisZoom = 0;

        private void OnEnable()
        {
            Reset();
            vJoyInterface.instance.EnableMapAxis();
            UpdateAxis();
        }

        private void OnDisable()
        {
            vJoyInterface.instance.DisableMapAxis();
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

        public void SetPitch(float pitch)
        {
            axisPitch = pitch;
            UpdateAxis();
        }

        public void SetYaw(float yaw)
        {
            axisYaw = yaw;
            UpdateAxis();
        }

        public void SetZoom(float zoom)
        {
            axisZoom = zoom;
            UpdateAxis();
        }

        protected void UpdateAxis()
        {
            var output = vJoyInterface.instance;
            if (!output.MapAxisEnabled) return;

            output.SetMapTranslationAxis(new Vector3(axisXZ.x, axisXZ.y, axisY));
            output.SetMapPitchAxis(axisPitch);
            output.SetMapYawAxis(axisYaw);
            output.SetMapZoomAxis(axisZoom);
        }
    }
}
