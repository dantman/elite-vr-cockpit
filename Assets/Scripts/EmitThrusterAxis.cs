using UnityEngine;

namespace EVRC
{
    /**
     * Helper that emits a dummy value on a thruster axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    public class EmitThrusterAxis : EmitAxis
    {
        public enum Axis : int
        {
            X = 0,
            Y = 1,
            Z = 2,
        }
        public Axis outAxis;

        protected override void SetAxis(AxisSign axisSign)
        {
            Vector3 axis = new Vector3();
            axis[(int)outAxis] = (float)axisSign;
            output.SetThrusters(new Virtual6DOFController.ThrusterAxis(axis));
        }
    }
}
