using UnityEngine;

namespace EVRC
{
    /**
     * Helper that emits a dummy value on a joystick axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    public class EmitJoystickAxis : EmitAxis
    {
        public enum Axis : int
        {
            Pitch = 0,
            Roll = 1,
            Yaw = 2,
        }
        public Axis outAxis;

        protected override void SetAxis(AxisSign axisSign)
        {
            var output = vJoyInterface.instance;
            Vector3 axis = new Vector3();
            axis[(int)outAxis] = (float)axisSign * output.joystickMaxDegrees;
            output.SetStickAxis(new VirtualJoystick.StickAxis(axis));
        }
    }
}
