using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using BtnAction = ActionsController.BtnAction;
    using ButtonActionsPress = ActionsController.ButtonActionsPress;
    using Direction = ActionsController.Direction;

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
        private float menuPressTime;

        private void OnEnable()
        {
            ActionsController.MenuPress.Listen(OnMenuPress);
            ActionsController.MenuUnpress.Listen(OnMenuUnpress);
            ActionsController.ButtonActionPress.Listen(OnActionPress);
            ActionsController.DirectionActionPress.Listen(OnDirectionPress);
        }

        private void OnDisable()
        {
            ActionsController.MenuPress.Remove(OnMenuPress);
            ActionsController.MenuUnpress.Remove(OnMenuUnpress);
            ActionsController.ButtonActionPress.Remove(OnActionPress);
            ActionsController.DirectionActionPress.Remove(OnDirectionPress);
        }

        private void OnMenuPress(ActionsController.ButtonPress ev)
        {
            menuPressTime = Time.time;
        }

        private void OnMenuUnpress(ActionsController.ButtonPress ev)
        {
            if (Time.time - menuPressTime < menuButtonReleaseTimeout)
            {
                Back();
            }
        }

        private void OnActionPress(ButtonActionsPress ev)
        {
            switch (ev.button)
            {
                case BtnAction.D1:
                case BtnAction.D2:
                    Select();
                    break;
            }
        }

        private void OnDirectionPress(ActionsController.DirectionActionsPress ev)
        {
            NavigateDirection(ev.direction, ev.button);
        }

        protected virtual void Select()
        {
            KeyboardInterface.Send("Key_Space");
        }

        protected virtual void Back()
        {
            KeyboardInterface.SendEscape();
        }

        protected virtual void NavigateDirection(Direction direction, ActionsController.DirectionAction button)
        {
            if (directionKeys.ContainsKey(direction))
            {
                var key = directionKeys[direction];
                KeyboardInterface.Send(key);
            }
        }
    }
}
