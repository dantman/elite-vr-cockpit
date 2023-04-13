namespace EVRC.Core.Actions
{
    /**
     * Helper that emits a dummy value on the map zoom axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    public class EmitMapZoomAxis : EmitAxis
    {
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
            vJoyInterface.instance.SetMapZoomAxis((float)axisSign);
        }
    }
}
