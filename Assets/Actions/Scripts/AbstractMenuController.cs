using System;
using UnityEngine;

namespace EVRC.Core.Actions
{
    using Direction = ActionsController.Direction;
    using ActionChange = ActionsController.ActionChange;
    using DirectionActionChange = ActionsController.DirectionActionChange;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using DirectionActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange>;

    /**
     * Abstract controller for common menu navigation controllers
     */
    abstract public class AbstractMenuController : MonoBehaviour
    {
        [Tooltip("How long can the menu button be pressed before not being considered a back button press. Should sync up with the SeatedPositionResetAction hold time to ensure a position resest is not considered a back button press.")]
        public float menuButtonReleaseTimeout = 1f;

        protected ActionChangeUnpressHandler OnBack(ActionChange pEv)
        {
            float menuPressTime = Time.time;

            return (uEv) =>
            {
                if (Time.time - menuPressTime < menuButtonReleaseTimeout)
                {
                    Back();
                }
            };
        }

        protected ActionChangeUnpressHandler OnSelect(ActionChange pEv)
        {
            var unpress = Select();
            return (uEv) => unpress();
        }

        protected DirectionActionChangeUnpressHandler OnNavigateDirection(DirectionActionChange pEv)
        {
            var unpress = NavigateDirection(pEv.direction);

            return (uEv) => unpress();
        }

        abstract protected void Back();
        abstract protected Action Select();
        abstract protected Action NavigateDirection(Direction direction);
    }
}
