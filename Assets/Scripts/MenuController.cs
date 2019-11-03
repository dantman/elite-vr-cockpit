using System;
using System.Collections.Generic;

namespace EVRC
{
    using Direction = ActionsController.Direction;
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
                // @todo Add a menu nested toggle (expand/collapse) button
                .MenuBack(OnBack)
                .MenuSelect(OnSelect)
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
