using System.Collections;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class SeatedPositionResetAction : MonoBehaviour
    {
        public float holdForSeconds = 1f;

        private ActionsControllerPressManager actionsPressManager;

        void OnEnable()
        {
            actionsPressManager = new ActionsControllerPressManager(this)
                .ResetSeatedPosition(OnResetSeatedPosition);
        }

        void OnDisable()
        {
            actionsPressManager.Clear();
        }
        
        private PressManager.UnpressHandlerDelegate<ActionsController.ActionPress> OnResetSeatedPosition(ActionsController.ActionPress pEv)
        {
            StartCoroutine(HoldingMenuButtons());
            return (uEv) => StopAllCoroutines();
        }

        private IEnumerator HoldingMenuButtons()
        {
            yield return new WaitForSeconds(holdForSeconds);
            OpenVR.System.ResetSeatedZeroPose();
        }
    }
}
