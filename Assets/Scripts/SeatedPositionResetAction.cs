using System.Collections;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class SeatedPositionResetAction : MonoBehaviour
    {
        public float holdForSeconds = 1f;
        private bool leftMenuPressed = false;
        private bool rightMenuPressed = false;

        void OnEnable()
        {
            ActionsController.MenuPress.Listen(OnMenuPress);
            ActionsController.MenuUnpress.Listen(OnMenuUnpress);
        }

        void OnDisable()
        {
            ActionsController.MenuPress.Remove(OnMenuPress);
            ActionsController.MenuUnpress.Remove(OnMenuUnpress);
        }

        private void OnMenuPress(ActionsController.ButtonPress ev)
        {
            switch (ev.hand)
            {
                case ActionsController.Hand.Left:
                    leftMenuPressed = true;
                    break;
                case ActionsController.Hand.Right:
                    rightMenuPressed = true;
                    break;
            }

            if (leftMenuPressed && rightMenuPressed)
            {
                StartCoroutine(HoldingMenuButtons());
            }
        }

        private void OnMenuUnpress(ActionsController.ButtonPress ev)
        {
            switch (ev.hand)
            {
                case ActionsController.Hand.Left:
                    leftMenuPressed = false;
                    break;
                case ActionsController.Hand.Right:
                    rightMenuPressed = false;
                    break;
            }

            StopAllCoroutines();
        }

        private IEnumerator HoldingMenuButtons()
        {
            yield return new WaitForSeconds(holdForSeconds);

            if (leftMenuPressed && rightMenuPressed)
            {
                OpenVR.System.ResetSeatedZeroPose();
            }
        }
    }
}
