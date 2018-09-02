using System;
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

        public struct ButtonActionsPress
        {
            public BtnAction button;
            public Hand hand;
            public bool pressed;

            public ButtonActionsPress(Hand hand, BtnAction button, bool pressed)
            {
                this.hand = hand;
                this.button = button;
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

        public static Events.Event<ButtonPress> TriggerPress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> TriggerUnpress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> GrabPress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> GrabUnpress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> MenuPress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonPress> MenuUnpress = new Events.Event<ButtonPress>();
        public static Events.Event<ButtonActionsPress> ButtonActionPress = new Events.Event<ButtonActionsPress>();
        public static Events.Event<ButtonActionsPress> ButtonActionUnpress = new Events.Event<ButtonActionsPress>();

        public enum Hand
        {
            Unknown,
            Left,
            Right,
        }

        void OnEnable()
        {
            Events.System(EVREventType.VREvent_ButtonPress).Listen(OnButtonPress);
            Events.System(EVREventType.VREvent_ButtonUnpress).Listen(OnButtonUnpress);

            SteamVR_Input.PreInitialize();
            SteamVR.IdentifyApplication();
            SteamVR_Input.Initialize();
            SteamVR_Input.IdentifyActionsFile();
            //SteamVR_Input._default.inActions.btntrigger.AddOnChangeListener(OnBtnTriggerChange, SteamVR_Input_Sources.Any);
        }

        private void OnBtnTriggerChange(SteamVR_Action_In action)
        {
            Debug.LogFormat(
                "{0}: {1} ({2}",
                action.name,
                action.fullPath,
                action.GetChanged(SteamVR_Input_Sources.Any));
        }

        void OnDisable()
        {
            Events.System(EVREventType.VREvent_ButtonPress).Remove(OnButtonPress);
            Events.System(EVREventType.VREvent_ButtonUnpress).Remove(OnButtonPress);
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
