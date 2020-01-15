using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC
{
    using Hand = ActionsController.Hand;
    using HatDirection = vJoyInterface.HatDirection;

    /**
     * Base class for things common to the Virtual buttons on a cockpit control
     */
    abstract public class VirtualControlButtons : MonoBehaviour
    {
        public vJoyInterface output;
        // The hand of the controller grabbing the joystick, Unknown is considered "not grabbing"
        protected Hand grabbedHand = Hand.Unknown;
        protected bool pressedReverseLock = false;
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

        virtual protected void OnEnable() { }

        virtual protected void OnDisable()
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
            if (output && pressedButtons.Contains(btnNumber))
            {
                output.SetButton(btnNumber, false);
                pressedButtons.Remove(btnNumber);
            }
        }

        protected void PressReverseLock()
        {
            if (output && !pressedReverseLock)
            {
                pressedReverseLock = true;
                output.EnableReverse();
            }
        }

        protected void UnpressReverseLock()
        {
            if (output && pressedReverseLock)
            {
                pressedReverseLock = false;
                output.DisableReverse();
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
            if (output && pressedHatDirections.Contains(hatNumber))
            {
                output.SetHatDirection(hatNumber, HatDirection.Neutral);
                pressedHatDirections.Remove(hatNumber);
            }
        }

        protected void ReleaseAllInputs()
        {
            uint[] unpressButtons = pressedButtons.ToArray();
            foreach (uint btnNumber in unpressButtons)
            {
                UnpressButton(btnNumber);
            }

            uint[] unpressHats = pressedHatDirections.ToArray();
            foreach (uint hatNumber in unpressHats)
            {
                ReleaseHatDirection(hatNumber);
            }
        }
    }
}
