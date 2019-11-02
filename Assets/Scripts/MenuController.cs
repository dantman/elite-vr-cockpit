using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using ActionChange = ActionsController.ActionChange;
    using DirectionActionChange = ActionsController.DirectionActionChange;
    using OutputAction = ActionsController.OutputAction;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using DirectionActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange>;
    using Direction = ActionsController.Direction;
    using ButtonPress = ActionsController.ButtonPress;
    using static KeyboardInterface;

    /**
     * Controller for outputting menu navigaion keypresses from trackpad input
     */
    public class MenuController : MonoBehaviour
    {
        protected Dictionary<Direction, string> directionKeys = new Dictionary<Direction, string>()
        {
            { Direction.Up, "Key_UpArrow" },
            { Direction.Right, "Key_RightArrow" },
            { Direction.Down, "Key_DownArrow" },
            { Direction.Left, "Key_LeftArrow" },
        };

        [Tooltip("How long can the menu button be pressed before not being considered a back button press. Should sync up with the SeatedPositionResetAction hold time to ensure a position resest is not considered a back button press.")]
        public float menuButtonReleaseTimeout = 1f;

        private ActionsControllerPressManager actionsPressManager;
        private float menuPressTime;

        private void OnEnable()
        {
            ActionsController.MenuPress.Listen(OnMenuPress);
            ActionsController.MenuUnpress.Listen(OnMenuUnpress);

            actionsPressManager = new ActionsControllerPressManager(this)
                // @todo Add a Menu action to listen for, maybe also an expand or back?
                .ButtonD1(OnAction)
                .ButtonD2(OnAction)
                .Direction1(OnDirection)
                .Direction2(OnDirection);
        }

        private void OnDisable()
        {
            ActionsController.MenuPress.Remove(OnMenuPress);
            ActionsController.MenuUnpress.Remove(OnMenuUnpress);
            actionsPressManager.Clear();
        }

        private void OnMenuPress(ButtonPress ev)
        {
            menuPressTime = Time.time;
        }

        private void OnMenuUnpress(ButtonPress ev)
        {
            if (Time.time - menuPressTime < menuButtonReleaseTimeout)
            {
                Back();
            }
        }

        private ActionChangeUnpressHandler OnAction(ActionChange pEv)
        {
            switch (pEv.action)
            {
                case OutputAction.D1:
                case OutputAction.D2:
                    var unpress = Select();
                    return (uEv) => unpress();
            }

            return (uEv) => {};
        }

        private DirectionActionChangeUnpressHandler OnDirection(DirectionActionChange pEv)
        {
            var unpress = NavigateDirection(pEv.direction, pEv.action);

            return (uEv) => unpress();
        }

        protected virtual Action Select()
        {
            return CallbackPress(Space());
        }

        protected virtual void Back()
        {
            SendEscape();
        }

        protected virtual Action NavigateDirection(Direction direction, OutputAction action)
        {
            if (directionKeys.ContainsKey(direction))
            {
                var key = directionKeys[direction];
                var keyPress = Key(key);
                keyPress.Press();
                return () => keyPress.Release();
            }

            return () => { };
        }
    }
}
