using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

namespace EVRC
{
    using CockpitMode = CockpitUIMode.CockpitMode;
    using CockpitModeOverride = CockpitUIMode.CockpitModeOverride;
    using EDGuiFocus = EDStateManager.EDStatus_GuiFocus;

    public class HelpWidgetTextGenerator : MonoBehaviour
    {
        private EDGuiFocus GuiFocus;

        [Tooltip("The TextMeshPro mesh to update with the Title Text")]
        public TMPro.TMP_Text helpWidgetTitleTextMesh;
        [Tooltip("The TextMeshPro mesh to update with help text 1 info (top half)")]
        public TMPro.TMP_Text helpWidgetTopTextMesh;
        [Tooltip("The TextMeshPro mesh to update with help text 2 info (bottom half)")]
        public TMPro.TMP_Text helpWidgetBottomTextMesh;

        private string _title { get; set; }
        private string _topText { get; set; }
        private string _bottomText { get; set; }

        private CockpitMode _mode { get; set; }

        private void OnEnable()
        {
            CockpitUIMode.ModeChanged.Listen(OnCockpitUIModeChanged);
        }

        private void OnDisable()
        {
            
            CockpitUIMode.ModeChanged.Remove(OnCockpitUIModeChanged);
        }

        private void OnCockpitUIModeChanged(CockpitMode mode)
        {
            Debug.LogFormat($"CockpitMode Change Detected {mode}");
            _mode = mode;
            RefreshHelpText(mode);
        }

        private void LateUpdate()
        {
            helpWidgetTitleTextMesh.text = _title;
            helpWidgetTopTextMesh.text = _topText;
            helpWidgetBottomTextMesh.text = _bottomText;
            OnDemandRenderer.SafeDirty(gameObject);
        }

        private void RefreshHelpText(CockpitMode mode)
        {
            Debug.LogFormat($"Active Modes: {mode}");

            if (mode.HasFlag(CockpitMode.FSSMode))
            {
                _title = $"FSS MODE";
                _topText = "Right hand: Hold the grip button to activate, then use your joystick/trackpad to aim the scanner";
                _bottomText = "Left hand: Increase zoom by pressing forward/back \n adjust tuning by moving your joystick/trackpad left or right \n you may also use the holographic tuner";
                Debug.LogFormat($"FSS mode flag present");
            }
            else if (mode.HasFlag(CockpitMode.MenuMode))
            {
                _title = "Menu Mode";
                _topText = "Use joystick/trackpad to navigate. A/X to select";
                _bottomText = "Both controllers are identical in menu mode";
                Debug.LogFormat($"Menu mode flag present");
            }
            else if (mode.HasFlag(CockpitMode.InMainShip))
            {
                _title = "Main Ship";
                _topText = "You must grip the joystick or throttle in order to use your controller buttons";
                _bottomText = "While gripping, your controller bindings are mapped to in-game controls. Visit the in-game menu to review your current bindings.";
                Debug.LogFormat($"Menu mode flag present");

                switch (GuiFocus)
                {
                    case EDGuiFocus.StationServices:
                        _title = "Station Services";
                        _topText = "You can navigate Station menus by gripping your Main Ship Joystick and using your trackpad/thumstick";
                        _bottomText = "";
                        Debug.LogFormat($"Station Services Active");
                        break;
                    case EDGuiFocus.SAAMode: //DSS mode
                        _title = "Detailed Surface Scanner";
                        _topText = "Your Main Ship controls will control the direction of the DSS probe launcher. Just grip the joystick like normal";
                        _bottomText = "Launching probes and other controls will need to be configured with the in-game menu. Look at the in-game help text on the bottom of the DSS screen to see if those controls are already configured.";
                        Debug.LogFormat($"DSS Mode Active");
                        break;
                }
            }
            else if (mode.HasFlag(CockpitMode.InSRV))
            {
                _title = "SRV Mode";
                _topText = "You must grip the joystick or throttle in order to use your controller buttons";
                _bottomText = "Your holographic buttons should be unique to the SRV, feel free to move them around and place them where most convenient!";
                Debug.LogFormat($"Menu mode flag present");
            }
            else if (mode.HasFlag(CockpitMode.InFighter))
            {
                _title = "Fighter Mode";
                _topText = "You must grip the joystick or throttle in order to use your controller buttons";
                _bottomText = "Your holographic buttons should be unique to the Fighter, feel free to move them around and place them where most convenient!";
                Debug.LogFormat($"Menu mode flag present");
            }
            else
            {
                _title = $"Title Not Found";
                _topText = "This widget is not configured to handle the current game mode.";
                _bottomText = "You may write this mode down and submit a github issue to add support \n for this mode, if you like \n Have a great day!";
                Debug.LogFormat($"No flags are available for the help widget");
            }            
        }

    }
}