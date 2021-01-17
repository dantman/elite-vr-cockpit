namespace EVRC
{
    /**
     * Helper that emits a dummy value on the SensorZoom axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    public class EmitSensorZoomAxis : EmitAxis
    {
        protected override void SetAxis(AxisSign axisSign)
        {
            vJoyInterface.instance.SetSensorZoom((float)axisSign);
        }
    }
}
