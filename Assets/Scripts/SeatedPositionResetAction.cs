﻿using System.Collections;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using ActionChange = ActionsController.ActionChange;

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

        private PressManager.UnpressHandlerDelegate<ActionChange> OnResetSeatedPosition(ActionChange pEv)
        {
            StartCoroutine(HoldingMenuButtons());
            return (uEv) => StopAllCoroutines();
        }

        private IEnumerator HoldingMenuButtons()
        {
            yield return new WaitForSeconds(holdForSeconds);
            Debug.Log("Resetting seated position");
            // OpenVR.System.ResetSeatedZeroPose();
        }
    }
}
