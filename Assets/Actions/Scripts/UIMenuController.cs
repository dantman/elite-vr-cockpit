using System;
using System.Collections.Generic;

namespace EVRC.Core.Actions
{
    using Direction = ActionsController.Direction;
    using ActionChange = ActionsController.ActionChange;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using static KeyboardInterface;

    /**
     * Controller for outputting UI navigation keypresses from trackpad/joystick input for station services and other cockpit interface views
     */
    public class UIMenuController : AbstractMenuController
    {
        public ControlBindingsState controlBindingsState;

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
                .ButtonAlt(OnBack)
                .ButtonPrimary(OnSelect)
                .ButtonSecondary(OnSelect)
                .DirectionPOV3(OnNavigateDirection)
                .DirectionPOV1(OnNavigateDirection)
                .ButtonPOV3(OnTabPrevious)
                .ButtonPOV1(OnTabNext);
        }

        private void OnDisable()
        {
            actionsPressManager.Clear();
        }

        protected override void Back()
        {
            ControlBindingsState bindings = controlBindingsState;
            if (bindings.buttonBindings != null && bindings.HasKeyboardKeybinding(EDControlButton.GalaxyMapOpen))
            {
                // @fixme See if it's reasonable to swap this with a CallbackPress using pattern like Select()
                controlBindingsState.GetControlButton(EDControlButton.UI_Back)?.Send();
            }
            else
            {
                SendEscape();
            }
        }

        protected override Action Select()
        {
            return CallbackPress(controlBindingsState.GetControlButton(EDControlButton.UI_Select));
        }

        protected override Action NavigateDirection(Direction direction)
        {
            return directionControlButtons.TryGetValue(direction, out EDControlButton button) ? CallbackPress(controlBindingsState.GetControlButton(button)) : () => { };
        }

        protected ActionChangeUnpressHandler OnTabPrevious(ActionChange pEv)
        {
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(EDControlButton.CyclePreviousPanel));
            return (uEv) => unpress();
        }

        protected ActionChangeUnpressHandler OnTabNext(ActionChange pEv)
        {
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(EDControlButton.CycleNextPanel));
            return (uEv) => unpress();
        }
    }
}
