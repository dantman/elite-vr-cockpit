using EVRC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EVRC.EDControlBindings;

namespace EVRC
{
    using Direction = ActionsController.Direction;
    using static KeyboardInterface;

    public class MapPositionController : MonoBehaviour, IMapControlScript
    {

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
            if (mapAngleControlButtons.ContainsKey(direction))
            {
                EDControlButton control = mapAngleControlButtons[direction];
                return CallbackPress(EDControlBindings.GetControlButton(control));
            }

            return () => { };
        }

        public Action POV1Press()
        {
            return CallbackPress(GetControlButton(EDControlButton.CamZoomOut));
        }

        public Action POV3Direction(Direction direction)
        {
            if (mapTranslateControlButtons.ContainsKey(direction))
            {
                EDControlButton control = mapTranslateControlButtons[direction];
                return CallbackPress(EDControlBindings.GetControlButton(control));
            }

            return () => { };
        }

        public Action zTranslate(Direction direction)
        {
            if (zTranslateControlButtons.ContainsKey(direction))
            {
                EDControlButton control = zTranslateControlButtons[direction];
                return CallbackPress(EDControlBindings.GetControlButton(control));
            }

            return () => { };

        }

        public Action POV3Press()
        {
            return CallbackPress(GetControlButton(EDControlButton.CamZoomIn));
        }
    }
}