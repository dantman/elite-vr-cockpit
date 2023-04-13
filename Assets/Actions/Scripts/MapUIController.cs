using System;
using System.Collections.Generic;
using UnityEngine;
using static EVRC.Core.EDControlBindings;

namespace EVRC.Core.Actions
{
    using Direction = ActionsController.Direction;
    using EDControlButton = EDControlButton;
    using static KeyboardInterface;

    public class MapUIController : MonoBehaviour, IMapControlScript
    {
        protected Dictionary<Direction, EDControlButton> directionControlButtons = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.UI_Up },
            { Direction.Right, EDControlButton.UI_Right },
            { Direction.Down, EDControlButton.UI_Down },
            { Direction.Left, EDControlButton.UI_Left },
        };

        public Action POV1Direction(Direction direction)
        {
            if (directionControlButtons.ContainsKey(direction))
            {
                EDControlButton control = directionControlButtons[direction];
                return CallbackPress(EDControlBindings.GetControlButton(control));
            }

            return () => { };
        }

        public Action POV1Press()
        {
            return CallbackPress(GetControlButton(EDControlButton.UI_Toggle));
        }

        public Action POV3Direction(Direction direction)
        {
            if (directionControlButtons.ContainsKey(direction))
            {
                EDControlButton control = directionControlButtons[direction];
                return CallbackPress(EDControlBindings.GetControlButton(control));
            }

            return () => { };
        }

        public Action POV3Press()
        {
            return CallbackPress(GetControlButton(EDControlButton.UI_Toggle));
        }

    }
}