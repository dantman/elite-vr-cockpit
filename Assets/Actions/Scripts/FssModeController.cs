using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Actions
{
    using Direction = ActionsController.Direction;
    using ActionChange = ActionsController.ActionChange;
    using DirectionActionChange = ActionsController.DirectionActionChange;
    using Vector2ActionChangeEvent = ActionsController.Vector2ActionChangeEvent;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using DirectionActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange>;
    using static KeyboardInterface;

    /**
     * Controller for handling global button inputs in FSS mode
     */
    public class FssModeController : MonoBehaviour
    {
        public ControlBindingsState controlBindingsState;

        // @fixme Make this type of calculation global also consider finding a way to abstract the handling and work on any button
        [Tooltip("How long can the menu button be pressed before not being considered a back button press. Should sync up with the SeatedPositionResetAction hold time to ensure a position resest is not considered a back button press.")]
        public float menuButtonReleaseTimeout = 1f;

        protected Dictionary<Direction, EDControlButton> pov3DirectionBindings = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.ExplorationFSSZoomIn },
            { Direction.Down, EDControlButton.ExplorationFSSZoomOut },
            { Direction.Right, EDControlButton.ExplorationFSSRadioTuningX_Increase },
            { Direction.Left, EDControlButton.ExplorationFSSRadioTuningX_Decrease },
        };

        private ActionsControllerPressManager actionsPressManager;
        private Vector2 axisRotation = Vector2.zero;

        private void OnEnable()
        {
            actionsPressManager = new ActionsControllerPressManager(this)
                .FSSCameraControl(OnCameraControl)
                .ButtonAlt(OnExit)
                .ButtonPrimary(OnTargetCurrentSignal)
                .ButtonPOV1(OnDiscoveryScan)
                .DirectionPOV3(OnZoomOrTune)
                //.FSSSteppedZoom(OnSteppedZoom)
                //.FSSTune(OnTune)
                ;
            Reset();
            vJoyInterface.instance.EnableMapAxis();
            UpdateAxis();
        }

        private void OnDisable()
        {
            actionsPressManager.Clear();
            vJoyInterface.instance.DisableMapAxis();
        }

        protected ActionChangeUnpressHandler OnExit(ActionChange pEv)
        {
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(EDControlButton.ExplorationFSSQuit));
            return (uEv) => unpress();
        }

        

        protected ActionChangeUnpressHandler OnDiscoveryScan(ActionChange pEv)
        {
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(EDControlButton.ExplorationFSSDiscoveryScan));
            return (uEv) => unpress();
        }

        private void OnCameraControl(Vector2ActionChangeEvent ev)
        {
            axisRotation = ev.state;
            UpdateAxis();
        }

        protected ActionChangeUnpressHandler OnTargetCurrentSignal(ActionChange pEv)
        {
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(EDControlButton.ExplorationFSSTarget));
            return (uEv) => unpress();
        }

        private DirectionActionChangeUnpressHandler OnZoomOrTune(DirectionActionChange pEv)
        {
            if (pov3DirectionBindings.ContainsKey(pEv.direction))
            {
                EDControlButton button = pov3DirectionBindings[pEv.direction];
                Action unpress = CallbackPress(controlBindingsState.GetControlButton(button));
                return (uEv) => unpress();
            }

            return (uEv) => { };
        }

        private void Reset()
        {
            axisRotation = Vector2.zero;
        }

        protected void UpdateAxis()
        {
            var output = vJoyInterface.instance;
            if (!output.MapAxisEnabled) return;

            output.SetMapPitchAxis(axisRotation.y);
            output.SetMapYawAxis(axisRotation.x);
        }
    }
}
