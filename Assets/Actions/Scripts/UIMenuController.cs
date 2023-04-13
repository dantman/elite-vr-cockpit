using System;
using System.Collections.Generic;

namespace EVRC.Core.Actions
{
    using Direction = ActionsController.Direction;
    using ActionChange = ActionsController.ActionChange;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using EDControlButton = EDControlBindings.EDControlButton;
    using static KeyboardInterface;

    /**
     * Controller for outputting UI navigation keypresses from trackpad/joystick input for station services and other cockpit interface views
     */
    public class UIMenuController : AbstractMenuController
    {
        protected Dictionary<Direction, EDControlButton> directionControlButtons = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.UI_Up },
            { Direction.Right, EDControlButton.UI_Right },
            { Direction.Down, EDControlButton.UI_Down },
            { Direction.Left, EDControlButton.UI_Left },
        };

        private ActionsControllerPressManager actionsPressManager;

        private void OnEnable()
        {
            actionsPressManager = new ActionsControllerPressManager(this)
                .UIBack(OnBack)
                .UISelect(OnSelect)
                .UINavigate(OnNavigateDirection)
                .UITabPrevious(OnTabPrevious)
                .UITabNext(OnTabNext);
        }

        private void OnDisable()
        {
            actionsPressManager.Clear();
        }

        protected override void Back()
        {
            var bindings = EDStateManager.instance.controlBindings;
            if (bindings != null && bindings.HasKeyboardKeybinding(EDControlButton.GalaxyMapOpen))
            {
                // @fixme See if it's reasonable to swap this with a CallbackPress using pattern like Select()
                EDControlBindings.GetControlButton(EDControlButton.UI_Back)?.Send();
            }
            else
            {
                SendEscape();
            }
        }

        protected override Action Select()
        {
            return CallbackPress(EDControlBindings.GetControlButton(EDControlButton.UI_Select));
        }

        protected override Action NavigateDirection(Direction direction)
        {
            if (directionControlButtons.ContainsKey(direction))
            {
                EDControlButton control = directionControlButtons[direction];
                return CallbackPress(EDControlBindings.GetControlButton(control));
            }

            return () => { };
        }

        protected ActionChangeUnpressHandler OnTabPrevious(ActionChange pEv)
        {
            var unpress = CallbackPress(EDControlBindings.GetControlButton(EDControlButton.CyclePreviousPanel));
            return (uEv) => unpress();
        }

        protected ActionChangeUnpressHandler OnTabNext(ActionChange pEv)
        {
            var unpress = CallbackPress(EDControlBindings.GetControlButton(EDControlButton.CycleNextPanel));
            return (uEv) => unpress();
        }
    }
}
