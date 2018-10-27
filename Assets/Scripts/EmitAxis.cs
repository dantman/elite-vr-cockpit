using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * Base class for helpers that emit a dummy value on a virtual axis when pressed.
     * Used as part of a UI button for binding axis to control bindings
     */
    abstract public class EmitAxis : MonoBehaviour
    {
        public enum AxisSign : int
        {
            Reset = 0,
            Positive = 1,
            Negative = -1,
        }

        public vJoyInterface output;

        public void Positive()
        {
            StartCoroutine(Emit(AxisSign.Positive));
        }
        public void Negative()
        {
            StartCoroutine(Emit(AxisSign.Negative));
        }

        public IEnumerator Emit(AxisSign axisSign)
        {
            SetAxis(axisSign);
            yield return new WaitForSecondsRealtime(0.1f);
            SetAxis(AxisSign.Reset);
        }

        abstract protected void SetAxis(AxisSign axisSign);
    }
}
