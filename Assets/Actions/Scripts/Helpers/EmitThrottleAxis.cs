namespace EVRC.Core.Actions
{
    /**
     * Helper that emits a dummy value on the Throttle axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    public class EmitThrottleAxis : EmitAxis
    {
        protected override void SetAxis(AxisSign axisSign)
        {
            vJoyInterface.instance.SetThrottle((float)axisSign);
        }
    }
}
