using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Events = SteamVR_Events;

    public class ActionsController : MonoBehaviour
    {
        // @deprecated
        public EVRButtonId triggerButton = EVRButtonId.k_EButton_SteamVR_Trigger;
        public EVRButtonId grabButton = EVRButtonId.k_EButton_Grip;
        public EVRButtonId menuButton = EVRButtonId.k_EButton_ApplicationMenu;

        [Range(0f, 1f)]
        public float trackpadCenterButtonRadius = 0.5f;
        [Range(0f, 2f)]
        public float trackpadDirectionInterval = 1f;

        public enum InputAction
        {
            // Basic interactions
            InteractUI,
            GrabHold,
            GrabToggle,
            GrabPinch,
            // Seated position resets
            ResetSeatedPosition,
            MaybeResetSeatedPosition,
            // Control buttons
            ButtonPrimary,
            ButtonSecondary,
            ButtonAlt,
            // Menu/UI buttons
            Menu,
            MenuSelect,
            MenuNavigateUp,
            MenuNavigateDown,
            MenuNavigateLeft,
            MenuNavigateRight,
            UIBack,
            UISelect,
            // @todo UI Nested Toggle (Expand/Collapse)
            UINavigateUp,
            UINavigateDown,
            UINavigateLeft,
            UINavigateRight,
            UITabPrevious,
            UITabNext,
            // Trackpad POV/Menu/UI
            POV1Trackpad,
            POV2Trackpad,
            MenuNavigateTrackpad,
            UINavigateTrackpad,
            UITabTrackpad, // @todo This needs a special abstraction to implement
        }

        public enum OutputAction
        {
            InteractUI,
            GrabHold,
            GrabToggle,
            GrabPinch,
            ResetSeatedPosition,
            // POV
            POV1,
            POV2,
            // Control buttons
            ButtonPrimary,
            ButtonSecondary,
            ButtonAlt,
            // Menu
            Menu, // @todo Rename everywhere to MenuBack since we have the Esc menu opener?
            MenuSelect,
            MenuNavigate,
            // UI Navigate/Tab
            UIBack,
            UISelect,
            UINavigate,
            UITabPrevious,
            UITabNext,
        }

        public struct ActionChange
        {
            public OutputAction action;
            public Hand hand;
            public bool state;

            public ActionChange(Hand hand, OutputAction action, bool state)
            {
                this.hand = hand;
                this.action = action;
                this.state = state;
            }
        }

        public struct DirectionActionChange
        {
            public OutputAction action;
            public Hand hand;
            public Direction direction;
            public bool state;

            public DirectionActionChange(Hand hand, OutputAction action, Direction direction, bool state)
            {
                this.hand = hand;
                this.action = action;
                this.direction = direction;
                this.state = state;
            }
        }

        // @deprecated
        public enum Button
        {
            Trigger,
            Grab,
            Menu,
        }

        // @deprecated
        public struct ButtonPress
        {
            public Button button;
            public Hand hand;
            public bool pressed;

            public ButtonPress(Hand hand, Button button, bool pressed)
            {
                this.hand = hand;
                this.button = button;
                this.pressed = pressed;
            }
        }

        // @deprecated
        public enum BtnAction
        {
            Trigger,
            Secondary, // Menu/B
            Alt, // A
            D1, // Directional input 1 center press
            D2, // Directional input 2 center press
        }

        // @deprecated
        public enum DirectionAction
        {
            D1,
            D2,
        }

        // @deprecated
        public class ButtonActionsPress
        {
            public Hand hand;
            public BtnAction button;
            public bool pressed;

            public ButtonActionsPress(Hand hand, BtnAction button, bool pressed)
            {
                this.hand = hand;
                this.button = button;
                this.pressed = pressed;
            }
        }

        // @deprecated
        public class DirectionActionsPress
        {
            public Hand hand;
            public DirectionAction button;
            public Direction direction;
            public bool pressed;

            public DirectionActionsPress(Hand hand, DirectionAction button, Direction direction, bool pressed)
            {
                this.hand = hand;
                this.button = button;
                this.direction = direction;
                this.pressed = pressed;
            }
        }

        // @todo See if we can get SteamVR Input working and replace this with an actions map
        public Dictionary<EVRButtonId, BtnAction> basicBtnActions = new Dictionary<EVRButtonId, BtnAction>()
        {
            { EVRButtonId.k_EButton_SteamVR_Trigger, BtnAction.Trigger },
            { EVRButtonId.k_EButton_ApplicationMenu, BtnAction.Secondary },
            { EVRButtonId.k_EButton_A, BtnAction.Alt },
        };

        // @deprecated
        protected Action UnpressTouchpadHandler;
        protected Dictionary<Hand, short> trackpadTouchingCoroutineId = new Dictionary<Hand, short>()
        {
            { Hand.Left, 0 },
            { Hand.Right, 0 },
        };

        private static Dictionary<OutputAction, Events.Event<T>> GenerateEventsForOutputActions<T>()
        {
            var values = Enum.GetValues(typeof(OutputAction));
            var dict = new Dictionary<OutputAction, Events.Event<T>>(values.Length);
            foreach (OutputAction outputAction in values)
            {
                dict[outputAction] = new Events.Event<T>();
            }
            return dict;
        }

        public static Dictionary<OutputAction, Events.Event<ActionChange>> ActionPressed = GenerateEventsForOutputActions<ActionChange>();
        public static Dictionary<OutputAction, Events.Event<ActionChange>> ActionUnpress = GenerateEventsForOutputActions<ActionChange>();
        public static Dictionary<OutputAction, Events.Event<DirectionActionChange>> DirectionActionPressed = GenerateEventsForOutputActions<DirectionActionChange>();
        public static Dictionary<OutputAction, Events.Event<DirectionActionChange>> DirectionActionUnpressed = GenerateEventsForOutputActions<DirectionActionChange>();

        // @deprecated
        public static Events.Event<ButtonPress> TriggerPress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> TriggerUnpress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> GrabPress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> GrabUnpress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> MenuPress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> MenuUnpress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonActionsPress> ButtonActionPress = new Events.Event<ButtonActionsPress>();
        public static Events.Event<ButtonActionsPress> ButtonActionUnpress = new Events.Event<ButtonActionsPress>();
        public static Events.Event<DirectionActionsPress> DirectionActionPress = new Events.Event<DirectionActionsPress>();
        public static Events.Event<DirectionActionsPress> DirectionActionUnpress = new Events.Event<DirectionActionsPress>();

        public enum Hand
        {
            Unknown,
            Left,
            Right,
        }

        public enum Direction : byte
        {
            Up,
            Right,
            Down,
            Left
        }

        private delegate void BooleanInputActionHandler(InputAction inputAction, Hand hand, bool newState);
        private delegate void Vector2InputActionHandler(Hand hand, Vector2 newState);
        private delegate IEnumerator TrackpadInputActionHandler(InputAction inputAction, Hand hand, DynamicRef<Vector2> position, Ref<bool> running);
        private delegate void TrackpadPressActionHandler(InputAction inputAction, Hand hand, Vector2 position, bool newState);

        private Dictionary<InputAction, BooleanInputActionHandler> booleanInputActionHandlers;
        private Dictionary<InputAction, Vector2InputActionHandler> vector2InputActionHandlers;
        private Dictionary<InputAction, TrackpadInputActionHandler> trackpadInputActionHandlers;
        private Dictionary<InputAction, TrackpadPressActionHandler> trackpadPressActionHandlers;

        private Dictionary<InputAction, OutputAction> simpleBooleanActionMapping;
        private Dictionary<InputAction, (OutputAction, Direction)> directionalBooleanActionMapping;
        private Dictionary<InputAction, OutputAction> directionalTrackpadSlideMapping;
        private Dictionary<InputAction, (OutputAction, OutputAction)> trackpadPressActionMapping;

        private Dictionary<(InputAction, Hand), Action> trackpadPressUnpressHandler = new Dictionary<(InputAction, Hand), Action>();

        void OnEnable()
        {
            booleanInputActionHandlers = new Dictionary<InputAction, BooleanInputActionHandler>
            {
                { InputAction.MaybeResetSeatedPosition, OnMaybeResetSeatedPosition },
            };
            vector2InputActionHandlers = new Dictionary<InputAction, Vector2InputActionHandler> { };
            trackpadInputActionHandlers = new Dictionary<InputAction, TrackpadInputActionHandler> { };
            trackpadPressActionHandlers = new Dictionary<InputAction, TrackpadPressActionHandler> { };
            simpleBooleanActionMapping = new Dictionary<InputAction, OutputAction>();
            directionalBooleanActionMapping = new Dictionary<InputAction, (OutputAction, Direction)>();
            directionalTrackpadSlideMapping = new Dictionary<InputAction, OutputAction>();
            trackpadPressActionMapping = new Dictionary<InputAction, (OutputAction, OutputAction)>();

            // Basic interactions
            MapBooleanInputActionToOutputAction(InputAction.InteractUI, OutputAction.InteractUI);
            MapBooleanInputActionToOutputAction(InputAction.GrabHold, OutputAction.GrabHold);
            MapBooleanInputActionToOutputAction(InputAction.GrabToggle, OutputAction.GrabToggle);
            MapBooleanInputActionToOutputAction(InputAction.GrabPinch, OutputAction.GrabPinch);
            // Basic seated position reset
            MapBooleanInputActionToOutputAction(InputAction.ResetSeatedPosition, OutputAction.ResetSeatedPosition);
            // Throttle/Joystick controls
            MapBooleanInputActionToOutputAction(InputAction.ButtonPrimary, OutputAction.ButtonPrimary);
            MapBooleanInputActionToOutputAction(InputAction.ButtonSecondary, OutputAction.ButtonSecondary);
            MapBooleanInputActionToOutputAction(InputAction.ButtonAlt, OutputAction.ButtonAlt);
            // Menu/UI buttons
            MapBooleanInputActionToOutputAction(InputAction.Menu, OutputAction.Menu);
            MapBooleanInputActionToOutputAction(InputAction.MenuSelect, OutputAction.MenuSelect);
            MapBooleanInputActionToDirectionOutputAction(InputAction.MenuNavigateUp, OutputAction.MenuNavigate, Direction.Up);
            MapBooleanInputActionToDirectionOutputAction(InputAction.MenuNavigateDown, OutputAction.MenuNavigate, Direction.Down);
            MapBooleanInputActionToDirectionOutputAction(InputAction.MenuNavigateLeft, OutputAction.MenuNavigate, Direction.Left);
            MapBooleanInputActionToDirectionOutputAction(InputAction.MenuNavigateRight, OutputAction.MenuNavigate, Direction.Right);
            MapBooleanInputActionToOutputAction(InputAction.UIBack, OutputAction.UIBack);
            MapBooleanInputActionToOutputAction(InputAction.UISelect, OutputAction.UISelect);
            MapBooleanInputActionToDirectionOutputAction(InputAction.UINavigateUp, OutputAction.UINavigate, Direction.Up);
            MapBooleanInputActionToDirectionOutputAction(InputAction.UINavigateDown, OutputAction.UINavigate, Direction.Down);
            MapBooleanInputActionToDirectionOutputAction(InputAction.UINavigateLeft, OutputAction.UINavigate, Direction.Left);
            MapBooleanInputActionToDirectionOutputAction(InputAction.UINavigateRight, OutputAction.UINavigate, Direction.Right);
            MapBooleanInputActionToOutputAction(InputAction.UITabPrevious, OutputAction.UITabPrevious);
            MapBooleanInputActionToOutputAction(InputAction.UITabNext, OutputAction.UITabNext);
            // Trackpad POV/Menu/UI
            MapTrackpadSlideToDirectionOutputAction(InputAction.POV1Trackpad, OutputAction.POV1);
            MapTrackpadPressToDirectionAndButtonOptionAction(InputAction.POV1Trackpad, OutputAction.POV1, OutputAction.POV1);
            MapTrackpadSlideToDirectionOutputAction(InputAction.POV2Trackpad, OutputAction.POV2);
            MapTrackpadPressToDirectionAndButtonOptionAction(InputAction.POV2Trackpad, OutputAction.POV2, OutputAction.POV2);
            MapTrackpadSlideToDirectionOutputAction(InputAction.MenuNavigateTrackpad, OutputAction.MenuNavigate);
            MapTrackpadPressToDirectionAndButtonOptionAction(InputAction.MenuNavigateTrackpad, OutputAction.MenuNavigate, OutputAction.MenuSelect);
            MapTrackpadSlideToDirectionOutputAction(InputAction.UINavigateTrackpad, OutputAction.UINavigate);
            MapTrackpadPressToDirectionAndButtonOptionAction(InputAction.UINavigateTrackpad, OutputAction.UINavigate, OutputAction.UISelect);

            Events.System(EVREventType.VREvent_ButtonPress).Listen(OnButtonPress);
            Events.System(EVREventType.VREvent_ButtonUnpress).Listen(OnButtonUnpress);
            Events.System(EVREventType.VREvent_ButtonTouch).Listen(OnButtonTouch);
            Events.System(EVREventType.VREvent_ButtonUntouch).Listen(OnButtonUntouch);
        }

        void OnDisable()
        {
            Events.System(EVREventType.VREvent_ButtonPress).Remove(OnButtonPress);
            Events.System(EVREventType.VREvent_ButtonUnpress).Remove(OnButtonPress);
            Events.System(EVREventType.VREvent_ButtonTouch).Remove(OnButtonTouch);
            Events.System(EVREventType.VREvent_ButtonUntouch).Remove(OnButtonUntouch);
        }

        /**
         * Interface for input binding implementations to call when the state of a boolean input action has changed
         */
        public void TriggerBooleanInputAction(InputAction inputAction, Hand hand, bool newState)
        {
            if (booleanInputActionHandlers.ContainsKey(inputAction))
            {
                booleanInputActionHandlers[inputAction](inputAction, hand, newState);
            }
            else
            {
                Debug.LogWarningFormat("No boolean handler for input action: {0}", inputAction.ToString());
            }
        }

        /**
         * Interface for input binding implementations to call to start a trackpad touching coroutine
         */
        public IEnumerator TriggerTrackpadInputAction(InputAction inputAction, Hand hand, DynamicRef<Vector2> position, Ref<bool> running)
        {
            if (trackpadInputActionHandlers.ContainsKey(inputAction))
            {
                return trackpadInputActionHandlers[inputAction](inputAction, hand, position, running);
            }
            else
            {
                Debug.LogWarningFormat("No trackpad handler for input action: {0}", inputAction.ToString());
                return null;
            }
        }

        /**
         * Interface for input binding implementations to call when a trackpad has been pressed or released
         */
        public void TriggerTrackpadPressAction(InputAction inputAction, Hand hand, Vector2 position, bool newState)
        {
            if (trackpadPressActionHandlers.ContainsKey(inputAction))
            {
                trackpadPressActionHandlers[inputAction](inputAction, hand, position, newState);
            }
            else
            {
                Debug.LogWarningFormat("No trackpad press handler for input action: {0}", inputAction.ToString());
            }
        }

        // @deprecated
        void OnButtonPress(VREvent_t ev)
        {
            ButtonPress btn;
            var hand = GetHandForDevice(ev.trackedDeviceIndex);
            var button = (EVRButtonId)ev.data.controller.button;

            if (button == triggerButton)
            {
                btn = new ButtonPress(hand, Button.Trigger, true);
                TriggerPress.Send(btn);
            }
            if (button == grabButton)
            {
                btn = new ButtonPress(hand, Button.Grab, true);
                GrabPress.Send(btn);
            }
            if (button == menuButton)
            {
                btn = new ButtonPress(hand, Button.Menu, true);
                MenuPress.Send(btn);
            }

            if (basicBtnActions.ContainsKey(button))
            {
                var btnAction = basicBtnActions[button];
                var press = new ButtonActionsPress(hand, btnAction, true);
                ButtonActionPress.Send(press);
            }

            if (button == EVRButtonId.k_EButton_SteamVR_Touchpad)
            {
                var vr = OpenVR.System;
                // For now this only handles the SteamVR Touchpad
                // In the future Joysticks and small WMR touchpads should be supported
                // Though it's probably easiest to switch to get the SteamVR Input API working to replace this first
                var err = ETrackedPropertyError.TrackedProp_Success;
                var axisTypeInt = vr.GetInt32TrackedDeviceProperty(ev.trackedDeviceIndex, ETrackedDeviceProperty.Prop_Axis0Type_Int32, ref err);
                if (err == ETrackedPropertyError.TrackedProp_Success)
                {
                    var axisType = (EVRControllerAxisType)axisTypeInt;
                    if (axisType == EVRControllerAxisType.k_eControllerAxis_TrackPad)
                    {
                        var state = new VRControllerState_t();
                        var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t));
                        if (vr.GetControllerState(ev.trackedDeviceIndex, ref state, size))
                        {

                            var axis = ControllerAxisToVector2(state.rAxis0);
                            float magnitude = 0;
                            Direction dir = GetLargestVectorDirection(axis, ref magnitude);

                            if (magnitude > trackpadCenterButtonRadius)
                            {
                                // Directional button press
                                var dirBtn = new DirectionActionsPress(hand, DirectionAction.D1, dir, true);
                                DirectionActionPress.Send(dirBtn);

                                UnpressTouchpadHandler = () =>
                                {
                                    var dirBtnUnpress = new DirectionActionsPress(hand, DirectionAction.D1, dir, false);
                                    DirectionActionUnpress.Send(dirBtnUnpress);
                                };
                            }
                            else
                            {
                                // Center button press
                                var press = new ButtonActionsPress(hand, BtnAction.D1, true);
                                ButtonActionPress.Send(press);

                                UnpressTouchpadHandler = () =>
                                {
                                    var unpress = new ButtonActionsPress(hand, BtnAction.D1, false);
                                    ButtonActionUnpress.Send(unpress);
                                };
                            }
                        }
                    }
                }
            }
        }

        // @deprecated
        void OnButtonUnpress(VREvent_t ev)
        {
            ButtonPress btn;
            var hand = GetHandForDevice(ev.trackedDeviceIndex);
            var button = (EVRButtonId)ev.data.controller.button;

            if (button == triggerButton)
            {
                btn = new ButtonPress(hand, Button.Trigger, false);
                TriggerUnpress.Send(btn);
            }
            if (button == grabButton)
            {
                btn = new ButtonPress(hand, Button.Grab, false);
                GrabUnpress.Send(btn);
            }
            if (button == menuButton)
            {
                btn = new ButtonPress(hand, Button.Menu, false);
                MenuUnpress.Send(btn);
            }

            if (basicBtnActions.ContainsKey(button))
            {
                var btnAction = basicBtnActions[button];
                var press = new ButtonActionsPress(hand, btnAction, false);
                ButtonActionUnpress.Send(press);
            }

            if (button == EVRButtonId.k_EButton_SteamVR_Touchpad)
            {
                if (UnpressTouchpadHandler != null)
                {
                    UnpressTouchpadHandler();
                    UnpressTouchpadHandler = null;
                }
            }
        }

        // @deprecated
        private void OnButtonTouch(VREvent_t ev)
        {
            var hand = GetHandForDevice(ev.trackedDeviceIndex);
            var button = (EVRButtonId)ev.data.controller.button;

            if (button == EVRButtonId.k_EButton_SteamVR_Touchpad)
            {
                // For now this only handles the SteamVR Touchpad
                // In the future Joysticks and small WMR touchpads should be supported
                // Though it's probably easiest to switch to get the SteamVR Input API working to replace this first
                var err = ETrackedPropertyError.TrackedProp_Success;
                var axisTypeInt = OpenVR.System.GetInt32TrackedDeviceProperty(ev.trackedDeviceIndex, ETrackedDeviceProperty.Prop_Axis0Type_Int32, ref err);
                if (err == ETrackedPropertyError.TrackedProp_Success)
                {
                    var axisType = (EVRControllerAxisType)axisTypeInt;
                    if (axisType == EVRControllerAxisType.k_eControllerAxis_TrackPad)
                    {
                        trackpadTouchingCoroutineId[hand]++;
                        StartCoroutine(WhileTouchingTouchpadAxis0(ev.trackedDeviceIndex, hand, trackpadTouchingCoroutineId[hand]));
                    }
                }
            }
        }

        // @deprecated
        private void OnButtonUntouch(VREvent_t ev)
        {
            var hand = GetHandForDevice(ev.trackedDeviceIndex);
            var button = (EVRButtonId)ev.data.controller.button;

            if (button == EVRButtonId.k_EButton_SteamVR_Touchpad)
            {
                if (trackpadTouchingCoroutineId.ContainsKey(hand))
                {
                    // Increment the Id so the coroutine stops
                    trackpadTouchingCoroutineId[hand]++;
                }
            }
        }

        private void EmitActionStateChange(Hand hand, OutputAction action, bool state)
        {
            var ev = new ActionChange(hand, action, state);
            if (state)
            {
                ActionPressed[action].Invoke(ev);
            }
            else
            {
                ActionUnpress[action].Invoke(ev);
            }
        }

        private void EmitDirectionActionStateChange(Hand hand, OutputAction action, Direction direction, bool state)
        {
            var ev = new DirectionActionChange(hand, action, direction, state);
            if (state)
            {
                DirectionActionPressed[action].Invoke(ev);
            }
            else
            {
                DirectionActionUnpressed[action].Invoke(ev);
            }
        }

        private void MapBooleanInputActionToOutputAction(InputAction inputAction, OutputAction outputAction)
        {
            simpleBooleanActionMapping[inputAction] = outputAction;
            booleanInputActionHandlers[inputAction] = OnMappedBooleanInputAction;
        }

        private void OnMappedBooleanInputAction(InputAction inputAction, Hand hand, bool newState)
        {
            if (simpleBooleanActionMapping.ContainsKey(inputAction))
            {
                EmitActionStateChange(hand, simpleBooleanActionMapping[inputAction], newState);
            }
            else
            {
                Debug.LogWarningFormat("No simple boolean action mapping for input action: {0}", inputAction.ToString());
            }
        }

        private void MapBooleanInputActionToDirectionOutputAction(InputAction inputAction, OutputAction outputAction, Direction direction)
        {
            directionalBooleanActionMapping[inputAction] = (outputAction, direction);
            booleanInputActionHandlers[inputAction] = OnDirectionMappedBooleanInputAction;
        }

        private void OnDirectionMappedBooleanInputAction(InputAction inputAction, Hand hand, bool newState)
        {
            if (directionalBooleanActionMapping.ContainsKey(inputAction))
            {
                var (outputAction, direction) = directionalBooleanActionMapping[inputAction];
                EmitDirectionActionStateChange(hand, outputAction, direction, newState);
            }
            else
            {
                Debug.LogWarningFormat("No directional boolean action mapping for input action: {0}", inputAction.ToString());
            }
        }

        private void MapTrackpadSlideToDirectionOutputAction(InputAction inputAction, OutputAction outputAction)
        {
            directionalTrackpadSlideMapping[inputAction] = outputAction;
            trackpadInputActionHandlers[inputAction] = OnMappedTrackpadInput;
        }

        private IEnumerator OnMappedTrackpadInput(InputAction inputAction, Hand hand, DynamicRef<Vector2> position, Ref<bool> running)
        {
            if (!directionalTrackpadSlideMapping.ContainsKey(inputAction))
            {
                Debug.LogWarningFormat("No trackpad input action mapping for input action: {0}", inputAction.ToString());
                yield break;
            }
            var outputAction = directionalTrackpadSlideMapping[inputAction];

            // @todo Abstract this trackpad slide handling so it can be used for UITabTrackpad implementation as well
            // Wait a tick before starting, a race condition results in the current position always starting as (0, 0)
            yield return null;

            Vector2 anchorPos = position.Current;
            yield return null;
            while (running.current)
            {
                var pos = position.Current;
                var deltaPos = pos - anchorPos;
                float magnitude = 0;
                Direction dir = GetLargestVectorDirection(deltaPos, ref magnitude);
                if (magnitude >= trackpadDirectionInterval)
                {
                    anchorPos = pos;
                    EmitDirectionActionStateChange(hand, outputAction, dir, true);

                    // Wait long enough for ED to recieve any keypresses
                    yield return KeyboardInterface.WaitForKeySent();

                    EmitDirectionActionStateChange(hand, outputAction, dir, false);
                }

                yield return null;
            }
        }

        private void MapTrackpadPressToDirectionAndButtonOptionAction(InputAction inputAction, OutputAction directionOutputAction, OutputAction centerOutputAction)
        {
            trackpadPressActionMapping[inputAction] = (directionOutputAction, centerOutputAction);
            trackpadPressActionHandlers[inputAction] = OnMappedTrackpadPress;
        }

        private void OnMappedTrackpadPress(InputAction inputAction, Hand hand, Vector2 position, bool newState)
        {
            if (!directionalTrackpadSlideMapping.ContainsKey(inputAction))
            {
                Debug.LogWarningFormat("No trackpad input action mapping for input action: {0}", inputAction.ToString());
                return;
            }
            var (directionOutputAction, centerOutputAction) = trackpadPressActionMapping[inputAction];
            // @todo Abstract this trackpad press handling so it can be used for UITabTrackpad implementation as well

            // Release any previous press whether we have a newState=false unpress, or somehow got a second press without an unpress
            if (trackpadPressUnpressHandler.ContainsKey((inputAction, hand)))
            {
                trackpadPressUnpressHandler[(inputAction, hand)]();
            }

            if (newState)
            {
                float magnitude = 0;
                Direction dir = GetLargestVectorDirection(position, ref magnitude);

                if (magnitude > trackpadCenterButtonRadius)
                {
                    // Directional button press
                    EmitDirectionActionStateChange(hand, directionOutputAction, dir, true);

                    trackpadPressUnpressHandler[(inputAction, hand)] = () =>
                    {
                        trackpadPressUnpressHandler.Remove((inputAction, hand));
                        EmitDirectionActionStateChange(hand, directionOutputAction, dir, false);
                    };
                }
                else
                {
                    // Center button press
                    EmitActionStateChange(hand, centerOutputAction, true);

                    trackpadPressUnpressHandler[(inputAction, hand)] = () =>
                    {
                        trackpadPressUnpressHandler.Remove((inputAction, hand));
                        EmitActionStateChange(hand, centerOutputAction, false);
                    };
                }
            }
        }

        private readonly HashSet<Hand> maybeResetSeatedPositionHandPressed = new HashSet<Hand>();
        private bool maybeResetSeatedPositionBothHandsPressed = false;

        private void OnMaybeResetSeatedPosition(InputAction inputAction, Hand hand, bool newState)
        {
            if (newState) { maybeResetSeatedPositionHandPressed.Add(hand); }
            else { maybeResetSeatedPositionHandPressed.Remove(hand); }
            bool bothPressed = maybeResetSeatedPositionHandPressed.Contains(Hand.Left) && maybeResetSeatedPositionHandPressed.Contains(Hand.Right);
            if (maybeResetSeatedPositionBothHandsPressed != bothPressed)
            {
                EmitActionStateChange(Hand.Unknown, OutputAction.ResetSeatedPosition, bothPressed);
                maybeResetSeatedPositionBothHandsPressed = bothPressed;
            }
        }

        // @deprecated
        private Vector2 ControllerAxisToVector2(VRControllerAxis_t axis)
        {
            return new Vector2(axis.x, axis.y);
        }

        private Direction GetLargestVectorDirection(Vector2 v, ref float magnitude)
        {
            if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            {
                if (v.x < 0f)
                {
                    magnitude = -v.x;
                    return Direction.Left;
                }
                else
                {
                    magnitude = v.x;
                    return Direction.Right;
                }
            }
            else
            {
                if (v.y < 0f)
                {
                    magnitude = -v.y;
                    return Direction.Down;
                }
                else
                {
                    magnitude = v.y;
                    return Direction.Up;
                }
            }
        }

        // @deprecated
        private IEnumerator WhileTouchingTouchpadAxis0(uint deviceIndex, Hand hand, short coroutineId)
        {
            var vr = OpenVR.System;
            var state = new VRControllerState_t();
            var size = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(VRControllerState_t));

            bool started = false;
            Vector2 anchorPos = Vector2.zero;
            while (vr.GetControllerState(deviceIndex, ref state, size))
            {
                var pos = ControllerAxisToVector2(state.rAxis0);
                if (started)
                {
                    var deltaPos = pos - anchorPos;
                    float magnitude = 0;
                    Direction dir = GetLargestVectorDirection(deltaPos, ref magnitude);
                    if (magnitude >= trackpadDirectionInterval)
                    {
                        anchorPos = pos;
                        var btn = new DirectionActionsPress(hand, DirectionAction.D2, dir, true);
                        DirectionActionPress.Send(btn);

                        // Wait long enough for ED to recieve any keypresses
                        yield return KeyboardInterface.WaitForKeySent();

                        new DirectionActionsPress(hand, DirectionAction.D2, dir, false);
                        DirectionActionUnpress.Send(btn);
                    }
                }
                else
                {
                    started = true;
                    anchorPos = pos;
                }

                yield return null;

                if (trackpadTouchingCoroutineId[hand] != coroutineId)
                {
                    yield break;
                }
            }

            Debug.LogWarningFormat("Failed to get controller state for device {0}", deviceIndex);
        }

        // @deprecated
        public static Hand GetHandForDevice(uint deviceIndex)
        {
            var role = OpenVR.System.GetControllerRoleForTrackedDeviceIndex(deviceIndex);
            switch (role)
            {
                case ETrackedControllerRole.LeftHand: return Hand.Left;
                case ETrackedControllerRole.RightHand: return Hand.Right;
                default: return Hand.Unknown;
            }
        }
    }
}
