using System;
using System.Collections.Generic;

namespace EVRC
{
    using Direction = ActionsController.Direction;
    using EDControlButton = EDControlBindings.EDControlButton;
    using static KeyboardInterface;

    /**
     * Controller for outputting UI navigaion keypresses from trackpad input for the map views
     */
    public class MapMenuController : MenuController
    {
        protected Dictionary<Direction, EDControlButton> directionControlButtons = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.UI_Up },
            { Direction.Right, EDControlButton.UI_Right },
            { Direction.Down, EDControlButton.UI_Down },
            { Direction.Left, EDControlButton.UI_Left },
        };
        protected Dictionary<Direction, EDControlButton> specialDirectionControlButtons = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.UI_Up },
            { Direction.Down, EDControlButton.UI_Down },
            { Direction.Left, EDControlButton.CyclePreviousPanel },
            { Direction.Right, EDControlButton.CycleNextPanel },
        };

        protected override Action Select()
        {
            return CallbackPress(EDControlBindings.GetControlButton(EDControlButton.UI_Select));
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
                base.Back();
            }
        }

        protected override Action NavigateDirection(Direction direction, ActionsController.DirectionAction button)
        {
            if (button == ActionsController.DirectionAction.D1)
            {
                // @fixme On the Vive D1 is trackpad press which works for category switch
                //        however this may be different for Touch/WMR/Knuckles.
                //        This should probably be dealt with by adding Menu Up/Down/Left/Right
                //        and Menu Select, Menu Back, Menu Category Prev/Next as actions
                //        in SteamVR Input when we implement it.
                if (specialDirectionControlButtons.ContainsKey(direction))
                {
                    var control = specialDirectionControlButtons[direction];
                    return CallbackPress(EDControlBindings.GetControlButton(control));
                }
            }
            else
            {
                if (directionControlButtons.ContainsKey(direction))
                {
                    var control = directionControlButtons[direction];
                    return CallbackPress(EDControlBindings.GetControlButton(control));
                }
            }

            return () => { };
        }
    }
}
