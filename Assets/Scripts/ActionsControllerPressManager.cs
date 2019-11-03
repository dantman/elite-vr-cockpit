using UnityEngine;

namespace EVRC
{
    using OutputAction = ActionsController.OutputAction;
    using ActionChange = ActionsController.ActionChange;
    using DirectionActionChange = ActionsController.DirectionActionChange;
    using ButtonPress = ActionsController.ButtonPress;
    using ButtonActionsPress = ActionsController.ButtonActionsPress;
    using DirectionActionsPress = ActionsController.DirectionActionsPress;

    public class ActionsControllerPressManager : PressManager
    {
        public ActionsControllerPressManager(MonoBehaviour owner) : base(owner) { }

        public static bool ActionChangeComparator(ActionChange pEv, ActionChange uEv)
        {
            return uEv.hand == pEv.hand && uEv.action == pEv.action;
        }
        public static bool DirectionActionChangeComparator(DirectionActionChange pEv, DirectionActionChange uEv)
        {
            return uEv.hand == pEv.hand && uEv.action == pEv.action && uEv.direction == pEv.direction;
        }
        public static bool ButtonPressComparator(ButtonPress pEv, ButtonPress uEv)
        {
            return uEv.hand == pEv.hand && uEv.button == pEv.button;
        }
        public static bool ButtonActionsPressComparator(ButtonActionsPress pEv, ButtonActionsPress uEv)
        {
            return uEv.hand == pEv.hand && uEv.button == pEv.button;
        }
        public static bool DirectionActionsPressComparator(DirectionActionsPress pEv, DirectionActionsPress uEv)
        {
            return uEv.hand == pEv.hand && uEv.button == pEv.button && uEv.direction == pEv.direction;
        }

        private ActionsControllerPressManager AddOutputActionHandler(PressHandlerDelegate<ActionChange> handler, OutputAction outputAction)
        {
            AddHandler(handler,
                ActionChangeComparator,
                ActionsController.ActionPressed[outputAction],
                ActionsController.ActionUnpress[outputAction]);

            return this;
        }

        private ActionsControllerPressManager AddOutputActionHandler(PressHandlerDelegate<DirectionActionChange> handler, OutputAction outputAction)
        {
            AddHandler(handler,
                DirectionActionChangeComparator,
                ActionsController.DirectionActionPressed[outputAction],
                ActionsController.DirectionActionUnpressed[outputAction]);

            return this;
        }

        public ActionsControllerPressManager ResetSeatedPosition(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.ResetSeatedPosition); ;
        }

        public ActionsControllerPressManager InteractUI(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.InteractUI);
        }

        public ActionsControllerPressManager GrabHold(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.GrabHold);
        }
        public ActionsControllerPressManager GrabToggle(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.GrabToggle);
        }
        public ActionsControllerPressManager GrabPinch(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.GrabPinch);
        }

        public ActionsControllerPressManager ButtonPrimary(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.ButtonPrimary);
        }
        public ActionsControllerPressManager ButtonSecondary(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.ButtonSecondary);
        }
        public ActionsControllerPressManager ButtonAlt(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.ButtonAlt);
        }

        public ActionsControllerPressManager ButtonPOV1(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.POV1);
        }
        public ActionsControllerPressManager ButtonPOV2(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.POV2);
        }
        public ActionsControllerPressManager DirectionPOV1(PressHandlerDelegate<DirectionActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.POV1);
        }
        public ActionsControllerPressManager DirectionPOV2(PressHandlerDelegate<DirectionActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.POV2);
        }

        public ActionsControllerPressManager MenuButton(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.Menu);
        }
        public ActionsControllerPressManager MenuSelect(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.MenuSelect);
        }
        public ActionsControllerPressManager MenuNavigate(PressHandlerDelegate<DirectionActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.MenuNavigate);
        }

        public ActionsControllerPressManager UIBack(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.UIBack);
        }
        public ActionsControllerPressManager UISelect(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.UISelect);
        }
        public ActionsControllerPressManager UINavigate(PressHandlerDelegate<DirectionActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.UINavigate);
        }
        public ActionsControllerPressManager UITabPrevious(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.UITabPrevious);
        }
        public ActionsControllerPressManager UITabNext(PressHandlerDelegate<ActionChange> handler)
        {
            return AddOutputActionHandler(handler, OutputAction.UITabNext);
        }

        public ActionsControllerPressManager Trigger(PressHandlerDelegate<ButtonPress> handler)
        {
            AddHandler(
                handler,
                ButtonPressComparator,
                ActionsController.TriggerPress,
                ActionsController.TriggerUnpress);

            return this;
        }
        public ActionsControllerPressManager Grab(PressHandlerDelegate<ButtonPress> handler)
        {
            AddHandler(
                handler,
                ButtonPressComparator,
                ActionsController.GrabPress,
                ActionsController.GrabUnpress);
            return this;
        }
        public ActionsControllerPressManager Menu(PressHandlerDelegate<ButtonPress> handler)
        {
            AddHandler(
                handler,
                ButtonPressComparator,
                ActionsController.MenuPress,
                ActionsController.MenuUnpress);
            return this;
        }
        public ActionsControllerPressManager ButtonAction(PressHandlerDelegate<ButtonActionsPress> handler)
        {
            AddHandler(
                handler,
                ButtonActionsPressComparator,
                ActionsController.ButtonActionPress,
                ActionsController.ButtonActionUnpress);
            return this;
        }
        public ActionsControllerPressManager DirectionAction(PressHandlerDelegate<DirectionActionsPress> handler)
        {
            AddHandler(
                handler,
                DirectionActionsPressComparator,
                ActionsController.DirectionActionPress,
                ActionsController.DirectionActionUnpress);
            return this;
        }
    }
}
