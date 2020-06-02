using UnityEngine;

namespace EVRC
{
    using ActionChange = ActionsController.ActionChange;
    using Vector2ActionChangeEvent = ActionsController.Vector2ActionChangeEvent;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using EDControlButton = EDControlBindings.EDControlButton;
    using static KeyboardInterface;
    using System;

    /**
     * Controller for handling global button inputs in FSS mode
     */
    public class FssModeController : MonoBehaviour
    {
        public vJoyInterface output;
        // @fixme Make this type of calculation global also consider finding a way to abstract the handling and work on any button
        [Tooltip("How long can the menu button be pressed before not being considered a back button press. Should sync up with the SeatedPositionResetAction hold time to ensure a position resest is not considered a back button press.")]
        public float menuButtonReleaseTimeout = 1f;

        private ActionsControllerPressManager actionsPressManager;
        private Vector2 axisRotation = Vector2.zero;

        private void OnEnable()
        {
            actionsPressManager = new ActionsControllerPressManager(this)
                .FSSExit(OnExit)
                .FSSCameraControl(OnCameraControl)
                .FSSTargetCurrentSignal(OnTargetCurrentSignal);
            Reset();
            output.EnableMapAxis();
            UpdateAxis();
        }

        private void OnDisable()
        {
            actionsPressManager.Clear();
            output.DisableMapAxis();
        }

        protected ActionChangeUnpressHandler OnExit(ActionChange pEv)
        {
            var unpress = CallbackPress(EDControlBindings.GetControlButton(EDControlButton.ExplorationFSSQuit));
            return (uEv) => unpress();
        }

        private void OnCameraControl(Vector2ActionChangeEvent ev)
        {
            axisRotation = ev.state;
            UpdateAxis();
        }

        protected ActionChangeUnpressHandler OnTargetCurrentSignal(ActionChange pEv)
        {
            var unpress = CallbackPress(EDControlBindings.GetControlButton(EDControlButton.ExplorationFSSTarget));
            return (uEv) => unpress();
        }

        private void Reset()
        {
            axisRotation = Vector2.zero;
        }

        protected void UpdateAxis()
        {
            if (!output.MapAxisEnabled) return;

            output.SetMapPitchAxis(axisRotation.y);
            output.SetMapYawAxis(axisRotation.x);
        }
    }
}
