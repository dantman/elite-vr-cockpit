namespace EVRC.Core.Actions
{
    /**
     * Helper that emits a dummy value on the FSS Radio Tuning axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    public class EmitFssTuningRadioAxis : EmitAxis
    {
        protected override void SetAxis(AxisSign axisSign)
        {
            vJoyInterface.instance.SetFSSTuning((float)axisSign);
        }
    }
}
