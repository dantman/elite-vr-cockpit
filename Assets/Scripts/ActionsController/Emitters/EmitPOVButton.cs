using System;
using System.Collections;
using UnityEngine;

namespace EVRC
{
    using HatDirection = vJoyInterface.HatDirection;

    /**
     * Helper that emits dummy button presses for POV buttons and directions
     */
    public class EmitPOVButton : MonoBehaviour
    {
        [Range(1, 4)]
        public uint hatNumber = 1;
        public uint buttonNumber = 1;

        public void Direction(HatDirection direction)
        {
            StartCoroutine(EmitDirection(direction));
        }
        public void Direction(string direction)
        {
            HatDirection hatDirection;
            if (Enum.TryParse(direction, out hatDirection))
            {
                Direction(hatDirection);
            }
            else
            {
                Debug.LogErrorFormat("Invalid direction: {0}", direction);
            }
        }

        public void Button()
        {
            StartCoroutine(EmitButton());
        }

        public IEnumerator EmitDirection(HatDirection direction)
        {
            var output = vJoyInterface.instance;
            output.SetHatDirection(hatNumber, direction);
            yield return new WaitForSecondsRealtime(0.1f);
            output.SetHatDirection(hatNumber, HatDirection.Neutral);
        }

        public IEnumerator EmitButton()
        {
            var output = vJoyInterface.instance;
            output.SetButton(buttonNumber, true);
            yield return new WaitForSecondsRealtime(0.1f);
            output.SetButton(buttonNumber, false);
        }
    }
}
