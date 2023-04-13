namespace EVRC.Core.Actions
{
    /**
     * Controller for the FSS mode radio tuning
     */
    public class FssTuningRadioAxisController : AxisController
    {
        protected override void SetValue(float value)
        {
            vJoyInterface.instance.SetFSSTuning(value);
        }
    }
}
