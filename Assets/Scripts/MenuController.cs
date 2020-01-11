using System;
using System.Collections.Generic;

namespace EVRC
{
    using Direction = ActionsController.Direction;
    using ActionChange = ActionsController.ActionChange;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using EDControlButton = EDControlBindings.EDControlButton;
    using static KeyboardInterface;

    /**
     * Controller for outputting menu navigation keypresses from trackpad/joystick input
     */
    public class MenuController : AbstractMenuController
    {
        protected Dictionary<Direction, string> directionKeys = new Dictionary<Direction, string>()
        {
            { Direction.Up, "Key_UpArrow" },
            { Direction.Right, "Key_RightArrow" },
            { Direction.Down, "Key_DownArrow" },
            { Direction.Left, "Key_LeftArrow" },
        };

        private ActionsControllerPressManager actionsPressManager;

        private void OnEnable()
        {
            actionsPressManager = new ActionsControllerPressManager(this)
                .MenuBack(OnBack)
                .MenuSelect(OnSelect)
                .MenuNestedToggle(OnNestedToggle)
                .MenuNavigate(OnNavigateDirection);
        }

        private void OnDisable()
        {
            actionsPressManager.Clear();
        }

        protected override void Back()
        {
            SendEscape();
        }

        protected override Action Select()
        {
            return CallbackPress(Space());
        }

        protected ActionChangeUnpressHandler OnNestedToggle(ActionChange pEv)
        {
            var unpress = CallbackPress(EDControlBindings.GetControlButton(EDControlButton.UI_Toggle));
            return (uEv) => unpress();
        }

        protected override Action NavigateDirection(Direction direction)
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
