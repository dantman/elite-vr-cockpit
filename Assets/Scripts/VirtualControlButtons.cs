using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using Hand = ActionsController.Hand;
    using BtnAction = ActionsController.BtnAction;
    using ButtonActionsPress = ActionsController.ButtonActionsPress;
    using DirectionAction = ActionsController.DirectionAction;
    using Direction = ActionsController.Direction;
    using HatDirection = vJoyInterface.HatDirection;

    /**
     * Base class for things common to the Virtual buttons on a cockpit control
     */
    abstract public class VirtualControlButtons : MonoBehaviour
    {
        public vJoyInterface output;
        // The hand of the controller grabbing the joystick, Unknown is considered "not grabbing"
        protected Hand grabbedHand = Hand.Unknown;
        protected HashSet<uint> pressedButtons = new HashSet<uint>();
        protected HashSet<uint> pressedHatDirections = new HashSet<uint>();

        public virtual void Grabbed(Hand hand)
        {
            grabbedHand = hand;
        }

        public virtual void Ungrabbed()
        {
            grabbedHand = Hand.Unknown;

            // Release all buttons when ungrabbed
            ReleaseAllInputs();
        }

        private void OnDisable()
        {
            // Automatically ungrab the control when it is hidden
            Ungrabbed();
        }

        /**
         * Check if the hand used in an event is the same hand that is being grabbed
         */
        protected bool IsValidHand(Hand hand)
        {
            if (grabbedHand == Hand.Unknown) return false; // not grabbing
            if (grabbedHand != hand) return false; // wrong hand
            return true;
        }

        protected void PressButton(uint btnNumber)
        {
            if (output)
            {
                pressedButtons.Add(btnNumber);
                output.SetButton(btnNumber, true);
            }
        }

        protected void UnpressButton(uint btnNumber)
        {
            if (output)
            {
                output.SetButton(btnNumber, false);
                pressedButtons.Remove(btnNumber);
            }
        }

        protected void SetHatDirection(uint hatNumber, HatDirection hatDirection)
        {
            if (output)
            {
                pressedHatDirections.Add(hatNumber);
                output.SetHatDirection(hatNumber, hatDirection);
            }
        }

        protected void ReleaseHatDirection(uint hatNumber)
        {
            if (output)
            {
                output.SetHatDirection(hatNumber, HatDirection.Neutral);
                pressedHatDirections.Remove(hatNumber);
            }
        }

        protected void ReleaseAllInputs()
        {
            foreach (uint btnNumber in pressedButtons)
            {
                output.SetButton(btnNumber, false);
            }
            foreach (uint hatNumber in pressedHatDirections)
            {
                pressedHatDirections.Remove(hatNumber);
            }
        }
    }
}
