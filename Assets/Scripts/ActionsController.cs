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
        public EVRButtonId triggerButton = EVRButtonId.k_EButton_SteamVR_Trigger;
        public EVRButtonId grabButton = EVRButtonId.k_EButton_Grip;
        public EVRButtonId menuButton = EVRButtonId.k_EButton_ApplicationMenu;
        [Range(0f, 1f)]
        public float trackpadCenterButtonRadius = 0.5f;
        [Range(0f, 2f)]
        public float trackpadDirectionInterval = 1f;

        public enum Button
        {
            Trigger,
            Grab,
            Menu,
        }

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

        public enum BtnAction
        {
            Trigger,
            Secondary, // Menu/B
            Alt, // A
            D1, // Directional input 1 center press
            D2, // Directional input 2 center press
        }

        public enum DirectionAction
        {
            D1,
            D2,
        }

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

        protected Action UnpressTouchpadHandler;
        protected Dictionary<Hand, short> trackpadTouchingCoroutineId = new Dictionary<Hand, short>()
        {
            { Hand.Left, 0 },
            { Hand.Right, 0 },
        };

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

        void OnEnable()
        {
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
