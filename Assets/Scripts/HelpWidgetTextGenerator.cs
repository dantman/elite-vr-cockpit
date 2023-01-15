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

    public class HelpWidgetTextGenerator : MonoBehaviour
    {
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
                _bottomText = "Left hand: Increase zoom by pressing forward/back \n adjust tuning by swiping left/right or by moving the holographic tuner";
                Debug.LogFormat($"FSS mode flag present");
            }
            else if (mode.HasFlag(CockpitMode.MenuMode))
            {
                _title = "Menu Mode";
                _topText = "Use joystick/trackpad to navigate. A/X to select";
                _bottomText = "Both controllers are identical in menu mode";
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