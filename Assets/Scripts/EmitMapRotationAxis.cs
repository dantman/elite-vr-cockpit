namespace EVRC
{
    /**
     * Helper that emits a dummy value on a map rotation axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    public class EmitMapRotationAxis : EmitAxis
    {
        public enum Axis
        {
            Pitch = 0,
            Yaw = 1,
        }
        public Axis outAxis;

        protected override void Setup()
        {
            vJoyInterface.instance.EnableMapAxis();
        }

        protected override void Teardown()
        {
            vJoyInterface.instance.DisableMapAxis();
        }

        protected override void SetAxis(AxisSign axisSign)
        {
            if (outAxis == Axis.Yaw)
            {
                vJoyInterface.instance.SetMapYawAxis((float)axisSign);
            }
            else
            {
                vJoyInterface.instance.SetMapPitchAxis((float)axisSign);
            }
        }
    }
}
