using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Actions
{
    using Direction = ActionsController.Direction;
    using static KeyboardInterface;

    public class MapPositionController : MonoBehaviour, IMapControlScript
    {
        public ControlBindingsState controlBindingsState;

        protected Dictionary<Direction, EDControlButton> mapTranslateControlButtons = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.CamTranslateForward },
            { Direction.Right, EDControlButton.CamTranslateRight },
            { Direction.Down, EDControlButton.CamTranslateBackward},
            { Direction.Left, EDControlButton.CamTranslateLeft},
        };

        protected Dictionary<Direction, EDControlButton> zTranslateControlButtons = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.CamTranslateUp },
            { Direction.Down, EDControlButton.CamTranslateDown},
        };

        protected Dictionary<Direction, EDControlButton> mapAngleControlButtons = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.CamPitchUp },
            { Direction.Right, EDControlButton.CamYawRight },
            { Direction.Down, EDControlButton.CamPitchDown },
            { Direction.Left, EDControlButton.CamYawLeft },
        };

        public Action POV1Direction(Direction direction)
        {
            return mapAngleControlButtons.TryGetValue(direction, out EDControlButton button) ? CallbackPress(controlBindingsState.GetControlButton(button)) : () => { };
        }

        public Action POV1Press()
        {
            return CallbackPress(controlBindingsState.GetControlButton(EDControlButton.CamZoomOut));
        }

        public Action POV3Direction(Direction direction)
        {
            return mapTranslateControlButtons.TryGetValue(direction, out EDControlButton button) ? CallbackPress(controlBindingsState.GetControlButton(button)) : () => { };
        }

        public Action ZTranslate(Direction direction)
        {
            return zTranslateControlButtons.TryGetValue(direction, out EDControlButton button) ? CallbackPress(controlBindingsState.GetControlButton(button)) : () => { };
        }

        public Action POV3Press()
        {
            return CallbackPress(controlBindingsState.GetControlButton(EDControlButton.CamZoomIn));
        }
    }
}