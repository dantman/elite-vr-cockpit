using UnityEngine;
using System.Collections.Generic;

namespace EVRC
{
    using Direction = ActionsController.Direction;
    using ActionChange = ActionsController.ActionChange;
    using DirectionActionChange = ActionsController.DirectionActionChange;
    using Vector2ActionChangeEvent = ActionsController.Vector2ActionChangeEvent;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using DirectionActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange>;
    using EDControlButton = EDControlBindings.EDControlButton;
    using static KeyboardInterface;
    using System;

    /**
     * Controller for handling global button inputs in FSS mode
     */
    public class FssModeController : MonoBehaviour
    {
        // @fixme Make this type of calculation global also consider finding a way to abstract the handling and work on any button
        [Tooltip("How long can the menu button be pressed before not being considered a back button press. Should sync up with the SeatedPositionResetAction hold time to ensure a position resest is not considered a back button press.")]
        public float menuButtonReleaseTimeout = 1f;

        protected Dictionary<Direction, EDControlButton> zoomDirectionBindings = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.ExplorationFSSZoomIn },
            { Direction.Down, EDControlButton.ExplorationFSSZoomOut },
        };
        protected Dictionary<Direction, EDControlButton> steppedZoomDirectionBindings = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Up, EDControlButton.ExplorationFSSMiniZoomIn },
            { Direction.Down, EDControlButton.ExplorationFSSMiniZoomOut },
        };
        protected Dictionary<Direction, EDControlButton> tuningDirectionBindings = new Dictionary<Direction, EDControlButton>()
        {
            { Direction.Right, EDControlButton.ExplorationFSSRadioTuningX_Increase },
            { Direction.Left, EDControlButton.ExplorationFSSRadioTuningX_Decrease },
        };

        private ActionsControllerPressManager actionsPressManager;
        private Vector2 axisRotation = Vector2.zero;

        private void OnEnable()
        {
            actionsPressManager = new ActionsControllerPressManager(this)
                .FSSExit(OnExit)
                .FSSCameraControl(OnCameraControl)
                .FSSTargetCurrentSignal(OnTargetCurrentSignal)
                .FSSDiscoveryScan(OnDiscoveryScan)
                .FSSZoom(OnZoom)
                .FSSSteppedZoom(OnSteppedZoom)
                .FSSTune(OnTune);
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
            var unpress = CallbackPress(EDControlBindings.GetControlButton(EDControlButton.ExplorationFSSQuit));
            return (uEv) => unpress();
        }

        protected ActionChangeUnpressHandler OnDiscoveryScan(ActionChange pEv)
        {
            var unpress = CallbackPress(EDControlBindings.GetControlButton(EDControlButton.ExplorationFSSDiscoveryScan));
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

        private DirectionActionChangeUnpressHandler OnZoom(DirectionActionChange pEv)
        {
            if (zoomDirectionBindings.ContainsKey(pEv.direction))
            {
                var button = zoomDirectionBindings[pEv.direction];
                var unpress = CallbackPress(EDControlBindings.GetControlButton(button));
                return (uEv) => unpress();
            }

            return (uEv) => { };
        }

        private DirectionActionChangeUnpressHandler OnSteppedZoom(DirectionActionChange pEv)
        {
            if (steppedZoomDirectionBindings.ContainsKey(pEv.direction))
            {
                var button = steppedZoomDirectionBindings[pEv.direction];
                var unpress = CallbackPress(EDControlBindings.GetControlButton(button));
                return (uEv) => unpress();
            }

            return (uEv) => { };
        }

        private DirectionActionChangeUnpressHandler OnTune(DirectionActionChange pEv)
        {
            if (tuningDirectionBindings.ContainsKey(pEv.direction))
            {
                var button = tuningDirectionBindings[pEv.direction];
                var unpress = CallbackPress(EDControlBindings.GetControlButton(button));
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
