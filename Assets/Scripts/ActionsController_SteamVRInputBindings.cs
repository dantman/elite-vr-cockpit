using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Hand = ActionsController.Hand;
    using InputAction = ActionsController.InputAction;
    using BindingMode = InputBindingNameInfoManager.BindingMode;
    using NameType = InputBindingNameInfoManager.NameType;
    using TrackpadInterval = ActionsController.TrackpadInterval;

    /**
     * Behaviour that maps SteamVR Input API action events to ActionController actions.
     * This is to help keep ActionController generic enough that it would be possible to support Oculus API or OpenXR in a theoretical future.
     * It also should help make ActionController code simpler, since SteamVR's event api makes for a lot of duplication.
     */
    public class ActionsController_SteamVRInputBindings : MonoBehaviour, IBindingsController
    {
        public ActionsController actionsController;

        private Dictionary<SteamVR_Action_Boolean, InputAction> booleanActionMap = new Dictionary<SteamVR_Action_Boolean, InputAction>();
        private Dictionary<SteamVR_Action_Vector2, InputAction> vector2ActionMap = new Dictionary<SteamVR_Action_Vector2, InputAction>();
        private Dictionary<SteamVR_Action_Boolean, InputAction> trackpadSlideTouchActionMap = new Dictionary<SteamVR_Action_Boolean, InputAction>();
        private Dictionary<SteamVR_Action_Boolean, SteamVR_Action_Vector2> trackpadSlidePositionMap = new Dictionary<SteamVR_Action_Boolean, SteamVR_Action_Vector2>();
        private Dictionary<(InputAction, Hand), Action> trackpadSlideActiveCleanups = new Dictionary<(InputAction, Hand), Action>();
        private Dictionary<SteamVR_Action_Boolean, InputAction> trackpadPressActionMap = new Dictionary<SteamVR_Action_Boolean, InputAction>();
        private Dictionary<SteamVR_Action_Boolean, SteamVR_Action_Vector2> trackpadPressPositionMap = new Dictionary<SteamVR_Action_Boolean, SteamVR_Action_Vector2>();
        private Dictionary<SteamVR_Action_Vector2, InputAction> joystickActionMap = new Dictionary<SteamVR_Action_Vector2, InputAction>();

        private InputBindingNameInfoManager inputBindingNameInfo = new InputBindingNameInfoManager();

        private readonly List<Action> changeListenerCleanupActions = new List<Action>();

        private delegate void OnPoseChangeDelegate(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource, Vector3? newPosition, Quaternion? newRotation);

        void AddHandedPoseChangeListener(SteamVR_Action_Pose action, OnPoseChangeDelegate onPoseChange)
        {
            void OnValidPoseChange(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource, bool validPose)
            {
                if (validPose)
                {
                    action.AddOnChangeListener(fromSource, OnPoseChangeListener);
                }
                else
                {
                    action.RemoveOnChangeListener(fromSource, OnPoseChangeListener);
                    onPoseChange(fromAction, fromSource, null, null);
                }
            }

            void OnPoseChangeListener(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource)
            {
                onPoseChange(fromAction, fromSource, fromAction.GetLocalPosition(fromSource), fromAction.GetLocalRotation(fromSource));
            }

            action.AddOnValidPoseChanged(SteamVR_Input_Sources.LeftHand, OnValidPoseChange);
            action.AddOnValidPoseChanged(SteamVR_Input_Sources.RightHand, OnValidPoseChange);

            changeListenerCleanupActions.Add(() =>
            {
                action.RemoveOnValidPoseChanged(SteamVR_Input_Sources.LeftHand, OnValidPoseChange);
                action.RemoveOnValidPoseChanged(SteamVR_Input_Sources.RightHand, OnValidPoseChange);
                action.RemoveOnChangeListener(SteamVR_Input_Sources.LeftHand, OnPoseChangeListener);
                action.RemoveOnChangeListener(SteamVR_Input_Sources.RightHand, OnPoseChangeListener);
            });
        }

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

            var Deregister = inputBindingNameInfo.RegisterBinding(inputAction, BindingMode.Button, action, new SteamVR_Input_Sources[] {
                SteamVR_Input_Sources.LeftHand,
                SteamVR_Input_Sources.RightHand,
            });

            changeListenerCleanupActions.Add(() =>
            {
                action.RemoveOnChangeListener(OnBooleanActionChange, SteamVR_Input_Sources.LeftHand);
                action.RemoveOnChangeListener(OnBooleanActionChange, SteamVR_Input_Sources.RightHand);
                Deregister();
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

            var Deregister = inputBindingNameInfo.RegisterBinding(inputAction, BindingMode.TrackpadSwipe, touchAction, new SteamVR_Input_Sources[] {
                SteamVR_Input_Sources.LeftHand,
                SteamVR_Input_Sources.RightHand,
            });

            changeListenerCleanupActions.Add(() =>
            {
                touchAction.RemoveOnChangeListener(OnTrackpadTouchChange, SteamVR_Input_Sources.LeftHand);
                touchAction.RemoveOnChangeListener(OnTrackpadTouchChange, SteamVR_Input_Sources.RightHand);
                Deregister();
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

            var Deregister = inputBindingNameInfo.RegisterBinding(inputAction, BindingMode.TrackpadPress, pressAction, new SteamVR_Input_Sources[] {
                SteamVR_Input_Sources.LeftHand,
                SteamVR_Input_Sources.RightHand,
            });

            changeListenerCleanupActions.Add(() =>
            {
                pressAction.RemoveOnChangeListener(OnTrackpadPressChange, SteamVR_Input_Sources.LeftHand);
                pressAction.RemoveOnChangeListener(OnTrackpadPressChange, SteamVR_Input_Sources.RightHand);
                Deregister();
            });
        }

        /**
         * Add a SteamVR Input listener for a josystick position actions we want events for from each hand individually
         */
        void AddHandedJoystickChangeListener(SteamVR_Action_Vector2 positionAction, InputAction inputAction)
        {
            joystickActionMap[positionAction] = inputAction;
            positionAction.AddOnChangeListener(OnJoystickPositionChange, SteamVR_Input_Sources.LeftHand);
            positionAction.AddOnChangeListener(OnJoystickPositionChange, SteamVR_Input_Sources.RightHand);

            var Deregister = inputBindingNameInfo.RegisterBinding(inputAction, BindingMode.Joystick, positionAction, new SteamVR_Input_Sources[] {
                SteamVR_Input_Sources.LeftHand,
                SteamVR_Input_Sources.RightHand,
            });

            changeListenerCleanupActions.Add(() =>
            {
                positionAction.RemoveOnChangeListener(OnJoystickPositionChange, SteamVR_Input_Sources.LeftHand);
                positionAction.RemoveOnChangeListener(OnJoystickPositionChange, SteamVR_Input_Sources.RightHand);
                Deregister();
            });
        }

        void OnEnable()
        {
            // Activate all action sets
            // Our code already ignores actions when they aren't relevant
            // And we never bothered to create a backchannel to switch action sets
            SteamVR_Actions._default.Activate(SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions._default.Activate(SteamVR_Input_Sources.RightHand);
            SteamVR_Actions.Menu.Activate(SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions.Menu.Activate(SteamVR_Input_Sources.RightHand);
            SteamVR_Actions.UI.Activate(SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions.UI.Activate(SteamVR_Input_Sources.RightHand);
            SteamVR_Actions.CockpitControls.Activate(SteamVR_Input_Sources.LeftHand);
            SteamVR_Actions.CockpitControls.Activate(SteamVR_Input_Sources.RightHand);

            // Poses/etc
            AddHandedPoseChangeListener(SteamVR_Actions.default_Pose, OnHandPoseChange);

            // Basic interactions
            AddHandedBooleanChangeListener(SteamVR_Actions.default_InteractUI, InputAction.InteractUI);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_GrabHold, InputAction.GrabHold);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_GrabToggle, InputAction.GrabToggle);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_GrabPinch, InputAction.GrabPinch);
            // Seated position resets
            AddUniversalBooleanChangeListener(SteamVR_Actions.default_ResetSeatedPosition, InputAction.ResetSeatedPosition);
            AddHandedBooleanChangeListener(SteamVR_Actions.default_MaybeResetSeatedPosition, InputAction.MaybeResetSeatedPosition);
            // Throttle/Joystick controls
            AddHandedBooleanChangeListener(SteamVR_Actions.cockpitControls_ButtonPrimary, InputAction.ButtonPrimary);
            AddHandedBooleanChangeListener(SteamVR_Actions.cockpitControls_ButtonSecondary, InputAction.ButtonSecondary);
            AddHandedBooleanChangeListener(SteamVR_Actions.cockpitControls_ButtonAlt, InputAction.ButtonAlt);
            AddHandedBooleanChangeListener(SteamVR_Actions.cockpitControls_ButtonPOV1, InputAction.ButtonPOV1);
            AddHandedBooleanChangeListener(SteamVR_Actions.cockpitControls_ButtonPOV2, InputAction.ButtonPOV2);
            // Menu/UI buttons
            AddHandedBooleanChangeListener(SteamVR_Actions.menu_MenuBack, InputAction.MenuBack);
            AddHandedBooleanChangeListener(SteamVR_Actions.menu_MenuSelect, InputAction.MenuSelect);
            AddHandedBooleanChangeListener(SteamVR_Actions.menu_MenuNestedToggle, InputAction.MenuNestedToggle);
            AddHandedBooleanChangeListener(SteamVR_Actions.menu_MenuNavigateUp, InputAction.MenuNavigateUp);
            AddHandedBooleanChangeListener(SteamVR_Actions.menu_MenuNavigateDown, InputAction.MenuNavigateDown);
            AddHandedBooleanChangeListener(SteamVR_Actions.menu_MenuNavigateLeft, InputAction.MenuNavigateLeft);
            AddHandedBooleanChangeListener(SteamVR_Actions.menu_MenuNavigateRight, InputAction.MenuNavigateRight);
            AddHandedBooleanChangeListener(SteamVR_Actions.uI_UIBack, InputAction.UIBack);
            AddHandedBooleanChangeListener(SteamVR_Actions.uI_UISelect, InputAction.UISelect);
            AddHandedBooleanChangeListener(SteamVR_Actions.uI_UINavigateUp, InputAction.UINavigateUp);
            AddHandedBooleanChangeListener(SteamVR_Actions.uI_UINavigateDown, InputAction.UINavigateDown);
            AddHandedBooleanChangeListener(SteamVR_Actions.uI_UINavigateLeft, InputAction.UINavigateLeft);
            AddHandedBooleanChangeListener(SteamVR_Actions.uI_UINavigateRight, InputAction.UINavigateRight);
            AddHandedBooleanChangeListener(SteamVR_Actions.uI_UITabPrevious, InputAction.UITabPrevious);
            AddHandedBooleanChangeListener(SteamVR_Actions.uI_UITabNext, InputAction.UITabNext);

            // POV Trackpad
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.cockpitControls_POV1TrackpadTouch,
                SteamVR_Actions.cockpitControls_POV1TrackpadPosition,
                InputAction.POV1Trackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.cockpitControls_POV1TrackpadPress,
                SteamVR_Actions.cockpitControls_POV1TrackpadPosition,
                InputAction.POV1Trackpad);
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.cockpitControls_POV2TrackpadTouch,
                SteamVR_Actions.cockpitControls_POV2TrackpadPosition,
                InputAction.POV2Trackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.cockpitControls_POV2TrackpadPress,
                SteamVR_Actions.cockpitControls_POV2TrackpadPosition,
                InputAction.POV2Trackpad);
            // POV Joystick
            AddHandedJoystickChangeListener(
                SteamVR_Actions.cockpitControls_POV1JoystickPosition,
                InputAction.POV1Joystick);
            AddHandedJoystickChangeListener(
                SteamVR_Actions.cockpitControls_POV2JoystickPosition,
                InputAction.POV2Joystick);
            // Menu Navigate Trackpad
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.menu_MenuNavigateTrackpadTouch,
                SteamVR_Actions.menu_MenuNavigateTrackpadPosition,
                InputAction.MenuNavigateTrackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.menu_MenuNavigateTrackpadPress,
                SteamVR_Actions.menu_MenuNavigateTrackpadPosition,
                InputAction.MenuNavigateTrackpad);
            // Menu Navigate Joystick
            AddHandedJoystickChangeListener(
                SteamVR_Actions.menu_MenuNavigateJoystickPosition,
                InputAction.MenuNavigateJoystick);
            // UI Navigate Trackpad
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.uI_UINavigateTrackpadTouch,
                SteamVR_Actions.uI_UINavigateTrackpadPosition,
                InputAction.UINavigateTrackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.uI_UINavigateTrackpadPress,
                SteamVR_Actions.uI_UINavigateTrackpadPosition,
                InputAction.UINavigateTrackpad);
            // UI Navigate Joystick
            AddHandedJoystickChangeListener(
                SteamVR_Actions.uI_UINavigateJoystickPosition,
                InputAction.UINavigateJoystick);
            // UI Tab Trackpad
            AddHandedTrackpadSlideChangeListener(
                SteamVR_Actions.uI_UITabTrackpadTouch,
                SteamVR_Actions.uI_UITabTrackpadPosition,
                InputAction.UITabTrackpad);
            AddHandedTrackpadPressChangeListener(
                SteamVR_Actions.uI_UITabTrackpadPress,
                SteamVR_Actions.uI_UITabTrackpadPosition,
                InputAction.UITabTrackpad);
            // UI Tab Joystick
            AddHandedJoystickChangeListener(
                SteamVR_Actions.uI_UITabJoystickPosition,
                InputAction.UITabJoystick);

            Debug.Log("SteamVR Input bindings <b>enabled</b>");
        }

        void OnDisable()
        {
            var cleanupHandlers = changeListenerCleanupActions.ToArray();
            foreach (var cleanup in cleanupHandlers)
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

        public static SteamVR_Input_Sources GetInputSourceForHand(Hand hand)
        {
            switch (hand)
            {
                case Hand.Left: return SteamVR_Input_Sources.LeftHand;
                case Hand.Right: return SteamVR_Input_Sources.RightHand;
                default: return SteamVR_Input_Sources.Any;
            }
        }

        private void OnHandPoseChange(SteamVR_Action_Pose fromAction, SteamVR_Input_Sources fromSource, Vector3? newPosition, Quaternion? newRotation)
        {
            actionsController.HandPoseChange(GetHandForInputSource(fromSource), newPosition, newRotation);
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

        /**
         * Return a relative scale size for different trackpads
         * This adjusts the swipe sensitivity to handle trackpads of different physical sizes
         */
        public TrackpadInterval GetTrackpadSwipeInterval(Hand hand)
        {
            uint deviceIndex = SteamVR_Actions.default_Pose.GetDeviceIndex(GetInputSourceForHand(hand));
            string controllerType = "";
            if (deviceIndex != OpenVR.k_unTrackedDeviceIndexInvalid)
            {
                controllerType = SteamVR.instance.GetStringProperty(ETrackedDeviceProperty.Prop_ControllerType_String, deviceIndex);
            }

            switch (controllerType)
            {
                case "vive_controller":
                    // The Vive trackpad can comfortably fit about 8 intervals end to end
                    // (Though you can only really get 7)
                    return TrackpadInterval.Circular(0.25f);
                case "knuckles":
                    // The Index controllers can comfortably fit a similar armount to the Vive vertically
                    // However the horizontal swipes feel better at about half because of the oval shape
                    return TrackpadInterval.Oval(horizontal: 0.50f, vertical: 0.25f);
                default:
                    // For safety assume trackpads on unknown controllers are small and fallback to a default with a sensibly low sensitivity
                    return TrackpadInterval.Default;
            }
        }

        public void OnJoystickPositionChange(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            if (joystickActionMap.ContainsKey(fromAction))
            {
                var inputAction = joystickActionMap[fromAction];
                var hand = GetHandForInputSource(fromSource);
                actionsController.TriggerJoystickAxisChangeAction(inputAction, hand, axis);
            }
            else
            {
                Debug.LogWarningFormat("Unknown SteamVR Input action source: {0}", fromAction.fullPath);
            }
        }


        public string[] GetBindingNames(InputAction inputAction, NameType nameType)
        {
            return inputBindingNameInfo.GetBindingNames(inputAction, nameType);
        }

        public bool CanShowBindings()
        {
            return true;
        }

        public void ShowBindings(BindingsHintCategory hintCategory)
        {
            switch (hintCategory)
            {
                case BindingsHintCategory.Default:
                    SteamVR_Input.ShowBindingHints(SteamVR_Actions._default);
                    break;
                case BindingsHintCategory.Menu:
                    SteamVR_Input.ShowBindingHints(SteamVR_Actions.Menu);
                    break;
                case BindingsHintCategory.UI:
                    SteamVR_Input.ShowBindingHints(SteamVR_Actions.UI);
                    break;
                case BindingsHintCategory.CockpitControls:
                    SteamVR_Input.ShowBindingHints(SteamVR_Actions.CockpitControls);
                    break;
            }
        }

        public void EditBindings()
        {
            SteamVR_Input.OpenBindingUI();
        }
    }
}
