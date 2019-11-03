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

        private Dictionary<SteamVR_Action_Boolean, InputAction> booleanActionMap = new Dictionary<SteamVR_Action_Boolean, InputAction>();
        private Dictionary<SteamVR_Action_Vector2, InputAction> vector2ActionMap = new Dictionary<SteamVR_Action_Vector2, InputAction>();
        private Dictionary<SteamVR_Action_Boolean, InputAction> trackpadSlideTouchActionMap = new Dictionary<SteamVR_Action_Boolean, InputAction>();
        private Dictionary<SteamVR_Action_Boolean, SteamVR_Action_Vector2> trackpadSlidePositionMap = new Dictionary<SteamVR_Action_Boolean, SteamVR_Action_Vector2>();
        private Dictionary<(InputAction, Hand), Action> trackpadSlideActiveCleanups = new Dictionary<(InputAction, Hand), Action>();
        private Dictionary<SteamVR_Action_Boolean, InputAction> trackpadPressActionMap = new Dictionary<SteamVR_Action_Boolean, InputAction>();
        private Dictionary<SteamVR_Action_Boolean, SteamVR_Action_Vector2> trackpadPressPositionMap = new Dictionary<SteamVR_Action_Boolean, SteamVR_Action_Vector2>();

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

        /**
         * Add a SteamVR Input listener for a vector2 action we want events from each hand individually
         */
        void AddHandedVector2ChangeListener(SteamVR_Action_Vector2 action, InputAction inputAction)
        {
            vector2ActionMap[action] = inputAction;
            action.AddOnChangeListener(OnVector2ActionChange, SteamVR_Input_Sources.LeftHand);
            action.AddOnChangeListener(OnVector2ActionChange, SteamVR_Input_Sources.RightHand);

            changeListenerCleanupActions.Add(() =>
            {
                action.RemoveOnChangeListener(OnVector2ActionChange, SteamVR_Input_Sources.LeftHand);
                action.RemoveOnChangeListener(OnVector2ActionChange, SteamVR_Input_Sources.RightHand);
            });
        }

        /**
         * Add a SteamVR Input listener for a touch/position pair of trackpad actions we want events for from each hand individually
         */
        void AddHandedTrackpadSlideChangeListener(SteamVR_Action_Boolean touchAction, SteamVR_Action_Vector2 positionAction, InputAction inputAction)
        {
            trackpadSlideTouchActionMap[touchAction] = inputAction;
            trackpadSlidePositionMap[touchAction] = positionAction;
            touchAction.AddOnChangeListener(OnTrackpadTouchChange, SteamVR_Input_Sources.LeftHand);
            touchAction.AddOnChangeListener(OnTrackpadTouchChange, SteamVR_Input_Sources.RightHand);

            changeListenerCleanupActions.Add(() =>
            {
                touchAction.RemoveOnChangeListener(OnTrackpadTouchChange, SteamVR_Input_Sources.LeftHand);
                touchAction.RemoveOnChangeListener(OnTrackpadTouchChange, SteamVR_Input_Sources.RightHand);
            });
        }

        /**
         * Add a SteamVR Input listener for a press/position pair of trackpad actions we want events for from each hand individually
         */
        void AddHandedTrackpadPressChangeListener(SteamVR_Action_Boolean pressAction, SteamVR_Action_Vector2 positionAction, InputAction inputAction)
        {
            trackpadPressActionMap[pressAction] = inputAction;
            trackpadPressPositionMap[pressAction] = positionAction;
            pressAction.AddOnChangeListener(OnTrackpadPressChange, SteamVR_Input_Sources.LeftHand);
            pressAction.AddOnChangeListener(OnTrackpadPressChange, SteamVR_Input_Sources.RightHand);

            changeListenerCleanupActions.Add(() =>
            {
                pressAction.RemoveOnChangeListener(OnTrackpadPressChange, SteamVR_Input_Sources.LeftHand);
                pressAction.RemoveOnChangeListener(OnTrackpadPressChange, SteamVR_Input_Sources.RightHand);
            });
        }

        void OnEnable()
        {
            // Basic interactions
            AddHandedBooleanChangeListener(SteamVR_Actions.default_InteractUI, InputAction.InteractUI);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_GrabHold, InputAction.GrabHold);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_GrabToggle, InputAction.GrabToggle);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_GrabPinch, InputAction.GrabPinch);
            // Seated position resets
            AddUniversalBooleanChangeListener(SteamVR_Actions.default_ResetSeatedPosition, InputAction.ResetSeatedPosition);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_MaybeResetSeatedPosition, InputAction.MaybeResetSeatedPosition);
            // Throttle/Joystick controls
            AddHandedBooleanChangeListener(SteamVR_Actions.default_ButtonPrimary, InputAction.ButtonPrimary);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_ButtonSecondary, InputAction.ButtonSecondary);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_ButtonAlt, InputAction.ButtonAlt);
            // Menu/UI buttons
            AddHandedBooleanChangeListener(SteamVR_Actions.default_MenuBack, InputAction.MenuBack);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_MenuSelect, InputAction.MenuSelect);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_MenuNavigateUp, InputAction.MenuNavigateUp);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_MenuNavigateDown, InputAction.MenuNavigateDown);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_MenuNavigateLeft, InputAction.MenuNavigateLeft);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_MenuNavigateRight, InputAction.MenuNavigateRight);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_UIBack, InputAction.UIBack);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_UISelect, InputAction.UISelect);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_UINavigateUp, InputAction.UINavigateUp);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_UINavigateDown, InputAction.UINavigateDown);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_UINavigateLeft, InputAction.UINavigateLeft);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_UINavigateRight, InputAction.UINavigateRight);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_UITabPrevious, InputAction.UITabPrevious);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_UITabNext, InputAction.UITabNext);

            // POV Trackpad
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.default_POV1TrackpadTouch,
                SteamVR_Actions.default_POV1TrackpadPosition,
                InputAction.POV1Trackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.default_POV1TrackpadPress,
                SteamVR_Actions.default_POV1TrackpadPosition,
                InputAction.POV1Trackpad);
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.default_POV2TrackpadTouch,
                SteamVR_Actions.default_POV2TrackpadPosition,
                InputAction.POV2Trackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.default_POV2TrackpadPress,
                SteamVR_Actions.default_POV2TrackpadPosition,
                InputAction.POV2Trackpad);
            // Menu Navigate Trackpad
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.default_MenuNavigateTrackpadTouch,
                SteamVR_Actions.default_MenuNavigateTrackpadPosition,
                InputAction.MenuNavigateTrackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.default_MenuNavigateTrackpadPress,
                SteamVR_Actions.default_MenuNavigateTrackpadPosition,
                InputAction.MenuNavigateTrackpad);
            // UI Navigate Trackpad
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.default_UINavigateTrackpadTouch,
                SteamVR_Actions.default_UINavigateTrackpadPosition,
                InputAction.UINavigateTrackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.default_UINavigateTrackpadPress,
                SteamVR_Actions.default_UINavigateTrackpadPosition,
                InputAction.UINavigateTrackpad);
            // UI Tab
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.default_UITabTrackpadTouch,
                SteamVR_Actions.default_UITabTrackpadPosition,
                InputAction.UITabTrackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.default_UITabTrackpadPress,
                SteamVR_Actions.default_UITabTrackpadPosition,
                InputAction.UITabTrackpad);

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

        private void OnVector2ActionChange(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            Debug.LogFormat("Change [axis: {0}, delta: {1}]", axis, delta);
        }

        private void OnTrackpadTouchChange(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (trackpadSlideTouchActionMap.ContainsKey(fromAction))
            {
                var inputAction = trackpadSlideTouchActionMap[fromAction];
                var hand = GetHandForInputSource(fromSource);
                if (newState)
                {
                    var positionSource = trackpadSlidePositionMap[fromAction];
                    // Start trackpad input coroutine
                    var position = new DynamicRef<Vector2>(() => positionSource.GetAxis(fromSource));
                    var running = new Ref<bool>(true);
                    StartCoroutine(actionsController.TriggerTrackpadInputAction(inputAction, hand, position, running));

                    void cleanup()
                    {
                        running.current = false;
                        changeListenerCleanupActions.Remove(cleanup);
                        trackpadSlideActiveCleanups.Remove((inputAction, hand));
                    }
                    changeListenerCleanupActions.Add(cleanup);
                    trackpadSlideActiveCleanups[(inputAction, hand)] = cleanup;
                }
                else
                {
                    // Do cleanup of active slide
                    if (trackpadSlideActiveCleanups.ContainsKey((inputAction, hand)))
                    {
                        trackpadSlideActiveCleanups[(inputAction, hand)]();
                    }
                };
            }
            else
            {
                Debug.LogWarningFormat("Unknown SteamVR Input action source: {0}", fromAction.fullPath);
            }
        }

        private void OnTrackpadPressChange(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (trackpadPressActionMap.ContainsKey(fromAction))
            {
                var inputAction = trackpadPressActionMap[fromAction];
                var positionAction = trackpadPressPositionMap[fromAction];
                var hand = GetHandForInputSource(fromSource);
                var position = positionAction.GetAxis(fromSource);
                actionsController.TriggerTrackpadPressAction(inputAction, hand, position, newState);
            }
            else
            {
                Debug.LogWarningFormat("Unknown SteamVR Input action source: {0}", fromAction.fullPath);
            }
        }
    }
}
