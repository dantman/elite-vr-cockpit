using System.Collections;
using System.Collections.Generic;
using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core.Actions
{
    public class ControlButtonDeleteZone : MonoBehaviour
    {
        [Tooltip("How many seconds must the button be in contact with the zone before being deleted")]
        public float waitSeconds = 1f;
        private Dictionary<ControlButton, Coroutine> intersectingButtons = new Dictionary<ControlButton, Coroutine>();

        private void OnTriggerEnter(Collider other)
        {
            var controlButton = other.GetComponent<ControlButton>();
            if (controlButton != null)
            {
                intersectingButtons.Add(controlButton, StartCoroutine(DeletionCountdown(controlButton)));
            }
        }

        private void OnTriggerExit(Collider other)
        {

            var controlButton = other.GetComponent<ControlButton>();
            if (controlButton != null && intersectingButtons.ContainsKey(controlButton))
            {
                StopCoroutine(intersectingButtons[controlButton]);
                intersectingButtons.Remove(controlButton);
            }
        }

        private IEnumerator DeletionCountdown(ControlButton controlButton)
        {
            yield return new WaitForSecondsRealtime(waitSeconds);

            Debug.LogFormat("Deleting {0} control button", controlButton.name);
            Destroy(controlButton.gameObject);

            intersectingButtons.Remove(controlButton);
        }
    }
}
