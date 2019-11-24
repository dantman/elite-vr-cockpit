using UnityEngine;

namespace EVRC
{
    using TextAlignment = TMPro.TextAlignmentOptions;

    /**
     * Behaviour that makes a tooltip track the headset location to always face it
     */
    public class TooltipTracker : MonoBehaviour
    {
        public TooltipDisplay tooltip;
        private Transform hmd;

        private void OnEnable()
        {
            hmd = TrackedHMD.Transform;

            var trackedHand = GetComponentInParent<TrackedHand>();
            var isRight = trackedHand.hand == ActionsController.Hand.Right;

            // Flip the X position of the tooltips depending on which hand the controller is in
            var pos = transform.localPosition;
            pos.x = Mathf.Abs(pos.x) * (isRight ? -1 : 1);
            transform.localPosition = pos;

            if (tooltip)
            {
                pos = tooltip.transform.localPosition;
                pos.x = Mathf.Abs(pos.x) * (isRight ? -1 : 1);
                tooltip.transform.localPosition = pos;

                // Set the text alignment to be near the hand depending on the side the tooltip is on
                tooltip.textAlignment = isRight ? TextAlignment.Right : TextAlignment.Left;
            }
        }

        private void Update()
        {
            transform.LookAt(hmd);
            transform.localRotation *= Quaternion.AngleAxis(180f, Vector3.up);
        }
    }
}

