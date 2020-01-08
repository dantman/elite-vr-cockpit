using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using ActionChange = ActionsController.ActionChange;
    using DirectionActionChange = ActionsController.DirectionActionChange;
    using OutputAction = ActionsController.OutputAction;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using DirectionActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange>;
    using Direction = ActionsController.Direction;
    using HatDirection = vJoyInterface.HatDirection;

    /**
     * Outputs joystick buttons to vJoy when the associated joystick is grabbed
     */
    public class VirtualJoystickButtons : VirtualControlButtons
    {
        // Map of abstracted action presses to vJoy joystick button numbers
        private static Dictionary<OutputAction, uint> joyBtnMap = new Dictionary<OutputAction, uint>()
        {
            { OutputAction.ButtonPrimary, 1 },
            { OutputAction.ButtonSecondary, 2 },
            { OutputAction.ButtonAlt, 3 },
            { OutputAction.POV1, 4 },
            { OutputAction.POV2, 5 },
            { OutputAction.Stick, 6 },
        };
        private static Dictionary<OutputAction, uint> joyHatMap = new Dictionary<OutputAction, uint>()
        {
            { OutputAction.POV1, 1 },
            { OutputAction.POV2, 2 },
            { OutputAction.Stick, 3 },
        };
        private static Dictionary<Direction, HatDirection> directionMap = new Dictionary<Direction, HatDirection>()
        {
            { Direction.Up, HatDirection.Up },
            { Direction.Right, HatDirection.Right },
            { Direction.Down, HatDirection.Down },
            { Direction.Left, HatDirection.Left },
        };

        private ActionsControllerPressManager actionsPressManager;

        override protected void OnEnable()
        {
            base.OnEnable();
            actionsPressManager = new ActionsControllerPressManager(this)
                .ButtonPrimary(OnAction)
                .ButtonSecondary(OnAction)
                .ButtonAlt(OnAction)
                .ButtonPOV1(OnAction)
                .ButtonPOV2(OnAction)
                .DirectionPOV1(OnDirectionAction)
                .DirectionPOV2(OnDirectionAction)
                .ButtonStick(OnAction)
                .DirectionStick(OnDirectionAction);
        }

        override protected void OnDisable()
        {
            base.OnDisable();
            actionsPressManager.Clear();
        }

        private ActionChangeUnpressHandler OnAction(ActionChange pEv)
        {
            if (IsValidHand(pEv.hand) && joyBtnMap.ContainsKey(pEv.action))
            {
                uint btnIndex = joyBtnMap[pEv.action];
                PressButton(btnIndex);

                return (uEv) => { UnpressButton(btnIndex); };
            }

            return (uEv) => { };
        }

        private DirectionActionChangeUnpressHandler OnDirectionAction(DirectionActionChange pEv)
        {
            if (IsValidHand(pEv.hand) && joyHatMap.ContainsKey(pEv.action))
            {
                uint hatNumber = joyHatMap[pEv.action];
                SetHatDirection(hatNumber, directionMap[pEv.direction]);

                return (uEv) => { ReleaseHatDirection(hatNumber); };
            }

            return (uEv) => { };
        }
    }
}
