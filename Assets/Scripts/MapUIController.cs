using EVRC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EVRC.EDControlBindings;

namespace EVRC
{
    using Direction = ActionsController.Direction;
    using ActionChange = ActionsController.ActionChange;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using EDControlButton = EDControlBindings.EDControlButton;
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

        public Action POV1Direction(ActionsController.Direction direction)
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

        public Action POV3Direction(ActionsController.Direction direction)
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