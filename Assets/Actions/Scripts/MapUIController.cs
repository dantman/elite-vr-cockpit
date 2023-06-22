using System;
using System.Collections.Generic;
using UnityEngine;
using WindowsInput;

namespace EVRC.Core.Actions
{
    using Direction = ActionsController.Direction;
    using static KeyboardInterface;

    public class MapUiController : MonoBehaviour, IMapControlScript
    {
        public ControlBindingsState controlBindingsState;
        
        protected Dictionary<Direction, EDControlButton> directionControlButtons = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.UI_Up },
            { Direction.Right, EDControlButton.UI_Right },
            { Direction.Down, EDControlButton.UI_Down },
            { Direction.Left, EDControlButton.UI_Left },
        };
        
        public Action POV1Direction(Direction direction)
        {
            return directionControlButtons.TryGetValue(direction, out EDControlButton button) ? CallbackPress(controlBindingsState.GetControlButton(button)) : () => { };
        }

        public Action POV1Press()
        {
            //return CallbackPress(controlBindingsState.GetControlButton(EDControlButton.UI_Toggle));
            return CallbackPress(Key("Key_V", new string[1] { "Key_LeftControl" })); 
        }

        public Action POV3Direction(Direction direction)
        {
            return directionControlButtons.TryGetValue(direction, out EDControlButton button) ? CallbackPress(controlBindingsState.GetControlButton(button)) : () => { };
        }

        public Action POV3Press()
        {
            return CallbackPress(controlBindingsState.GetControlButton(EDControlButton.UI_Toggle));
        }

    }
}