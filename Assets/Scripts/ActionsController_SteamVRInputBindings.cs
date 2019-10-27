using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Hand = ActionsController.Hand;
    using InputAction = ActionsController.InputAction;

    /**
     * Behaviour that maps SteamVR Input API action events to ActionController actions.
     * This is to help keep ActionController generic enough that it would be possible to support Oculus API or OpenXR in a theoretical future.
     * It also should help make ActionController code simpler, since SteamVR's event api makes for a lot of duplication.
     */
    public class ActionsController_SteamVRInputBindings : MonoBehaviour
    {
        public ActionsController actionsController;

        private Dictionary<SteamVR_Action_Boolean, InputAction> booleanActionMap = new Dictionary<SteamVR_Action_Boolean, ActionsController.InputAction>();
        private readonly List<Action> changeListenerCleanupActions = new List<Action>();

        /**
         * Add a SteamVR Input listener for a boolean (button) action we only want one event for if any input is triggering the action
         */
        void AddUniversalBooleanChangeListener(SteamVR_Action_Boolean action, InputAction inputAction)
        {
            booleanActionMap[action] = inputAction;
            action.AddOnChangeListener(OnBooleanActionChange, SteamVR_Input_Sources.Any);

            changeListenerCleanupActions.Add(() =>
            {
                action.RemoveOnChangeListener(OnBooleanActionChange, SteamVR_Input_Sources.Any);
            });
        }

        /**
         * Add a SteamVR Input listener for a boolean (button) action we want events from each hand individually
         */
        void AddHandedBooleanChangeListener(SteamVR_Action_Boolean action, InputAction inputAction)
        {
            booleanActionMap[action] = inputAction;
            action.AddOnChangeListener(OnBooleanActionChange, SteamVR_Input_Sources.LeftHand);
            action.AddOnChangeListener(OnBooleanActionChange, SteamVR_Input_Sources.RightHand);

            changeListenerCleanupActions.Add(() =>
            {
                action.RemoveOnChangeListener(OnBooleanActionChange, SteamVR_Input_Sources.LeftHand);
                action.RemoveOnChangeListener(OnBooleanActionChange, SteamVR_Input_Sources.RightHand);
            });
        }

        void OnEnable()
        {
            AddHandedBooleanChangeListener(SteamVR_Actions.default_InteractUI, InputAction.InteractUI);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_GrabHold, InputAction.GrabHold);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_GrabToggle, InputAction.GrabToggle);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_GrabPinch, InputAction.GrabPinch);
            AddUniversalBooleanChangeListener(SteamVR_Actions.default_ResetSeatedPosition, InputAction.ResetSeatedPosition);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_MaybeResetSeatedPosition, InputAction.MaybeResetSeatedPosition);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_ButtonPrimary, InputAction.ButtonPrimary);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_ButtonSecondary, InputAction.ButtonSecondary);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_ButtonAlt, InputAction.ButtonAlt);
            Debug.Log("SteamVR Input bindings <b>enabled</b>");
        }

        void OnDisable()
        {
            foreach (var cleanup in changeListenerCleanupActions)
            {
                cleanup();
            }
            changeListenerCleanupActions.Clear();

            Debug.Log("SteamVR Input bindings <b>disabled</b>");
        }

        public static Hand GetHandForInputSource(SteamVR_Input_Sources source)
        {
            switch (source)
            {
                case SteamVR_Input_Sources.LeftHand: return Hand.Left;
                case SteamVR_Input_Sources.RightHand: return Hand.Right;
                default: return Hand.Unknown;
            }
        }

        private void OnBooleanActionChange(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (booleanActionMap.ContainsKey(fromAction))
            {
                var inputAction = booleanActionMap[fromAction];
                var hand = GetHandForInputSource(fromSource);
                actionsController.TriggerBooleanInputAction(inputAction, hand, newState);
            }
            else
            {
                Debug.LogWarningFormat("Unknown SteamVR Input action source: {0}", fromAction.fullPath);
            }
        }
    }
}
