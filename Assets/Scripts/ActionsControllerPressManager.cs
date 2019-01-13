using UnityEngine;

namespace EVRC
{
    using ButtonPress = ActionsController.ButtonPress;
    using ButtonActionsPress = ActionsController.ButtonActionsPress;
    using DirectionActionsPress = ActionsController.DirectionActionsPress;

    public class ActionsControllerPressManager : PressManager
    {
        public ActionsControllerPressManager(MonoBehaviour owner) : base(owner) { }

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

        public ActionsControllerPressManager Trigger(PressHandlerDelegate<ButtonPress> handler)
        {
            AddHandler(
                handler,
                ButtonPressComparator,
                ActionsController.TriggerPress,
                ActionsController.TriggerUnpress);

            //UnityAction<ButtonPress> ephemeralHandler = (ButtonPress pEv) =>
            //{
            //    var unpressHandler = handler(pEv);
            //    UnityAction<ButtonPress> ephemeralUnpressHandler = null;
            //    ephemeralUnpressHandler = (ButtonPress uEv) =>
            //    {
            //        // @todo Make this a generic helper
            //        // @todo Add code/callback to validate that this is the same press
            //        if (uEv.hand != pEv.hand || uEv.button != pEv.button)
            //        {
            //            return;
            //        }

            //        unpressHandler(uEv);
            //        ActionsController.TriggerUnpress.Remove(ephemeralUnpressHandler);
            //    };
            //    ActionsController.TriggerUnpress.Listen(ephemeralUnpressHandler);
            //};
            //// UnityEngine.Events.UnityAction<ButtonPress> ephemeralHandler = null;
            //ActionsController.TriggerPress.Listen(ephemeralHandler);
            //// Add this to a Clear()
            //// ActionsController.TriggerPress.Remove(ephemeralHandler);
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
