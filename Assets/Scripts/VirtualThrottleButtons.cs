using System.Collections.Generic;

namespace EVRC
{
    using ActionChange = ActionsController.ActionChange;
    using OutputAction = ActionsController.OutputAction;

    /**
     * Outputs joystick buttons to vJoy when the associated throttle is grabbed
     */
    public class VirtualThrottleButtons : VirtualControlButtons
    {
        // Map of abstracted action presses to vJoy joystick button numbers
        private static Dictionary<OutputAction, uint> joyBtnMap = new Dictionary<OutputAction, uint>()
        {
            { OutputAction.ButtonPrimary, 8 },
            { OutputAction.ButtonSecondary, 7 },
        };

        private ActionsControllerPressManager actionsPressManager;

        override protected void OnEnable()
        {
            base.OnEnable();
            actionsPressManager = new ActionsControllerPressManager(this)
                .ButtonPrimary(OnAction)
                .ButtonSecondary(OnAction)
                .ButtonAlt(OnAction);
        }

        override protected void OnDisable()
        {
            base.OnDisable();
            actionsPressManager.Clear();
        }

        private PressManager.UnpressHandlerDelegate<ActionChange> OnAction(ActionChange pEv)
        {
            if (IsValidHand(pEv.hand))
            {

                if (joyBtnMap.ContainsKey(pEv.action))
                {
                    uint btnIndex = joyBtnMap[pEv.action];
                    PressButton(btnIndex);

                    return (uEv) => { UnpressButton(btnIndex); };
                }

                if (pEv.action == OutputAction.ButtonAlt)
                {
                    PressReverseLock();

                    return (uEv) => { UnpressReverseLock(); };
                }
            }

            return (uEv) => { };
        }
    }
}
