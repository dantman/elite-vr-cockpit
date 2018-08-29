using System;
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

        public static Events.Event<ButtonPress> TriggerPress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> TriggerUnpress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> GrabPress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> GrabUnpress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> MenuPress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> MenuUnpress = new Events.Event<ButtonPress>();

        public enum Hand
        {
            Unknown,
            Left,
            Right,
        }

        void OnEnable()
        {
            SteamVR_Events.System(EVREventType.VREvent_ButtonPress).Listen(OnButtonPress);
            SteamVR_Events.System(EVREventType.VREvent_ButtonUnpress).Listen(OnButtonUnpress);
        }

        void OnDisable()
        {

            SteamVR_Events.System(EVREventType.VREvent_ButtonPress).Remove(OnButtonPress);
            SteamVR_Events.System(EVREventType.VREvent_ButtonUnpress).Remove(OnButtonPress);
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
        }

        void OnButtonUnpress(VREvent_t ev)
        {
            ButtonPress btn;
            var hand = GetHandForDevice(ev.trackedDeviceIndex);
            var button = (EVRButtonId)ev.data.controller.button;

            if (button == triggerButton)
            {
                btn = new ButtonPress(hand, Button.Trigger, true);
                TriggerUnpress.Send(btn);
            }
            if (button == grabButton)
            {
                btn = new ButtonPress(hand, Button.Grab, true);
                GrabUnpress.Send(btn);
            }
            if (button == menuButton)
            {
                btn = new ButtonPress(hand, Button.Menu, true);
                MenuUnpress.Send(btn);
            }
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
