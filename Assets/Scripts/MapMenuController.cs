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
     * Controller for outputting UI navigation keypresses from trackpad/joystick input for the map views
     */
    public class MapMenuController : AbstractMenuController
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
                // On the Galaxy map this will exit
                // On the System map/orrery this will go to the galaxy map, from where you can exit
                EDControlBindings.GetControlButton(EDControlButton.GalaxyMapOpen)?.Send();
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
                var control = directionControlButtons[direction];
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
