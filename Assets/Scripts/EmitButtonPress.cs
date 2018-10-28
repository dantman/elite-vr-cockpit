using System.Collections;
using UnityEngine;

namespace EVRC
{
    /**
     * Helper that emits a dummy button press
     */
    public class EmitButtonPress : MonoBehaviour
    {
        public vJoyInterface output;
        public uint buttonNumber = 1;

        public void Press()
        {
            StartCoroutine(Emit());
        }

        public IEnumerator Emit()
        {
            output.SetButton(buttonNumber, true);
            yield return new WaitForSecondsRealtime(0.1f);
            output.SetButton(buttonNumber, false);
        }
    }
}
