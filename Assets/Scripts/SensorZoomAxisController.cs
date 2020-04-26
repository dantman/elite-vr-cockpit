namespace EVRC
{
    /**
     * Controller for the sensor zoom axis
     */
    public class SensorZoomAxisController : AxisController
    {
        protected override void SetValue(float value)
        {
            output.SetSensorZoom(value);
        }
    }
}
