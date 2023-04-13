using System;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Actions
{
    using ActionChange = ActionsController.ActionChange;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using EDControlButton = EDControlBindings.EDControlButton;
    using static KeyboardInterface;

    /**
     * Controller for handling inputs while in the map views
     */
    public class MapController : MonoBehaviour
    {
        public MapPositionController mapCameraController;
        public MapUIController mapUIController;

        private ActionsControllerPressManager actionsPressManager;

        private void OnEnable()
        {
            if (mapCameraController == null || mapUIController == null) { throw new Exception("Map UI or Map Position controller missing"); }

            actionsPressManager = new ActionsControllerPressManager(this)
                .DirectionPOV3(OnPOV3Direction)
                .DirectionPOV1(OnPOV1Direction)
                .ButtonPrimary(OnSelect)
                .ButtonSecondary(OnSelect)
                .ButtonAlt(OnBack)
                .ButtonPOV1(OnButtonPOV1)
                .ButtonPOV3(OnButtonPOV3)
                ;
        }
        private void OnDisable()
        {
            actionsPressManager.Clear();
        }

        

        private ActionChangeUnpressHandler OnButtonPOV3(ActionChange ev)
        {
            //Debug.Log($"POV 3 (LEFT) Pressed: {ev} / Grip: {SteamVR_Actions.fSSControls_CameraControlActivate.GetState(SteamVR_Input_Sources.LeftHand)}");
            Action unpress;
            if (SteamVR_Actions.fSSControls_CameraControlActivate.GetState(SteamVR_Input_Sources.RightHand))
            {
                unpress = mapCameraController.POV3Press();
            }
            else
            {
                unpress = mapUIController.POV3Press();
            }
            return (uEv) => unpress();
        }

        private ActionChangeUnpressHandler OnButtonPOV1(ActionChange ev)
        {
            //Debug.Log($"POV 1 (RG) Pressed: {ev} / Grip: {SteamVR_Actions.fSSControls_CameraControlActivate.GetState(SteamVR_Input_Sources.RightHand)}");
            Action unpress;
            if (SteamVR_Actions.fSSControls_CameraControlActivate.GetState(SteamVR_Input_Sources.RightHand))
            {
                unpress = mapCameraController.POV1Press();
            }
            else
            {
                unpress = mapUIController.POV1Press();
            }
            return (uEv) => unpress();
        }

        private PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange> OnPOV3Direction(ActionsController.DirectionActionChange ev)
        {
            //Debug.Log($"POV 3 (LEFT) Direction: {ev.direction} / Grip: {SteamVR_Actions.fSSControls_CameraControlActivate.GetState(SteamVR_Input_Sources.RightHand)}");
            Action unpress;
            if (SteamVR_Actions.fSSControls_CameraControlActivate.GetState(SteamVR_Input_Sources.RightHand))
            {
                unpress = mapCameraController.POV3Direction(ev.direction);
            }
            // Z Axis Translate when left hand grip
            else if (SteamVR_Actions.fSSControls_CameraControlActivate.GetState(SteamVR_Input_Sources.LeftHand)) 
            {
                unpress = mapCameraController.zTranslate(ev.direction);
            } 
            else
            {
                unpress = mapUIController.POV1Direction(ev.direction);
            }
            return (uEv) => unpress();
        }

        private PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange> OnPOV1Direction(ActionsController.DirectionActionChange ev)
        {
            //Debug.Log($"POV 1 (RG) Direction: {ev.direction} / Grip: {SteamVR_Actions.fSSControls_CameraControlActivate.GetState(SteamVR_Input_Sources.RightHand)}");
            Action unpress;
            if (SteamVR_Actions.fSSControls_CameraControlActivate.GetState(SteamVR_Input_Sources.RightHand))
            {
                unpress = mapCameraController.POV1Direction(ev.direction);
            }
            else
            {
                unpress = mapUIController.POV1Direction(ev.direction);
            }
            return (uEv) => unpress();
        }
        
        
        protected ActionChangeUnpressHandler OnSelect(ActionChange pEv)
        {
            var unpress = CallbackPress(EDControlBindings.GetControlButton(EDControlButton.UI_Select));
            return (uEv) => unpress();
        }

        private ActionChangeUnpressHandler OnBack(ActionChange ev)
        {
            return (uEv) => Back();
        }

        protected void Back()
        {
            //if (SteamVR_Actions.fSSControls_CameraControlActivate.state)

            var bindings = EDStateManager.instance.controlBindings;
            if (bindings != null && bindings.HasKeyboardKeybinding(EDControlButton.GalaxyMapOpen))
            {
                // On the Galaxy map this will exit
                // On the System map/orrery this will go to the galaxy map, from where you can exit
                EDControlBindings.GetControlButton(EDControlButton.GalaxyMapOpen)?.Send();
            }
            else
            {
                SendEscape();
            }
        }
    }
}
