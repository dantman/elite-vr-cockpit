using UnityEngine;

namespace EVRC
{
    /**
     * Helper that emits a dummy value on a map translation axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    public class EmitMapTranslationAxis : EmitAxis
    {
        public enum Axis : int
        {
            X = 0,
            Y = 1,
            Z = 2,
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
            Vector3 axis = new Vector3();
            axis[(int)outAxis] = (float)axisSign;

            vJoyInterface.instance.SetMapTranslationAxis(axis);
        }
    }
}
