using System.Collections;
using UnityEngine;

namespace EVRC.Core.Actions
{
    /**
     * Helper that emits a dummy button press
     */
    public class EmitButtonPress : MonoBehaviour
    {
        public uint buttonNumber = 1;

        public void Press()
        {
            StartCoroutine(Emit());
        }

        public IEnumerator Emit()
        {
            var output = vJoyInterface.instance;
            output.SetButton(buttonNumber, true);
            yield return new WaitForSecondsRealtime(0.1f);
            output.SetButton(buttonNumber, false);
        }
    }
}
