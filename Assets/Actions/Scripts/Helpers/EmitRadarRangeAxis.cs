namespace EVRC.Core.Actions
{
    /**
     * Helper that emits a dummy value on the Radar Range (SensorZoom) axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    public class EmitRadarRangeAxis : EmitAxis
    {
        protected override void SetAxis(AxisSign axisSign)
        {
            vJoyInterface.instance.SetRadarRange((float)axisSign);
        }
    }
}
