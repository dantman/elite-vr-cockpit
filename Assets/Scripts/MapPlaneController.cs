using System;
using UnityEngine;

namespace EVRC
{
    /**
     * A complex controller with components to control navigation of the galaxy map
     */
    public class MapPlaneController : MonoBehaviour
    {
        public vJoyInterface vJoyInterface;
        private Vector2 axisXZ = Vector2.zero;
        private float axisY = 0;

        private void OnEnable()
        {
            Reset();
            UpdateAxis();
        }

        private void OnDisable()
        {
            // @todo
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
            // @fixme Add explicit galaxy map axis to the vJoy interface instead of using the throttle/stick axis
            // @todo Also use dial for zoom instead of the same axis as the throttle

            vJoyInterface.SetThrusters(new Virtual6DOFController.ThrusterAxis(new Vector3(axisXZ.x, axisY, axisXZ.y)));
        }
    }
}
