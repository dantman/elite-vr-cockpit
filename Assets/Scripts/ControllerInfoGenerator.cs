using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC
{
    using OutputAction = ActionsController.OutputAction;
    using NameType = ActionsController.NameType;


    public class ControllerInfoGenerator : MonoBehaviour
    {
        [Tooltip("The TextMeshPro mesh to update with joystick buttons available on the controller")]
        public TMPro.TMP_Text joystickButtonsTextMesh;
        [Tooltip("The TextMeshPro mesh to update with joystick povs available on the controller")]
        public TMPro.TMP_Text joystickPovTextMesh;
        [Tooltip("The TextMeshPro mesh to update with throttle buttons available on the controller")]
        public TMPro.TMP_Text throttleButtonsTextMesh;
        [Tooltip("The TextMeshPro mesh to update with joystick pov available on the controller")]
        public TMPro.TMP_Text throttlePovTextMesh;

        private void OnEnable()
        {
            RefreshJoystickButtons();
            RefreshJoystickPOVs();
            RefreshThrottleButtons();
            RefreshThrottlePOVs();
        }

        string[] GetBindingNames(OutputAction outputAction, NameType nameType)
            => ActionsController.GetBindingNames(ActionsControllerBindingsLoader.CurrentBindingsController, outputAction, nameType);

        string JoinBinds(string[] binds)
        {
            if (binds.Length >= 3)
            {
                string[] head = binds.Take(binds.Length - 1).ToArray();
                string last = binds.Last();
                return string.Join(", ", head) + ", and " + last;
            }
            else if (binds.Length == 2)
            {
                return string.Join(" and ", binds);
            }
            else if (binds.Length == 1)
            {
                return binds[0];
            }
            else
            {
                return "Not bound";
            }
        }

        public string Line(string button, string[] binds) => button + ": " + JoinBinds(binds) + "\n";

        /// <summary>
        /// Gets the names of the buttons on the current controller, according to how they are bound within EVRC/SteamVR
        /// </summary>
        /// <remarks>
        ///     Hint: Use the public Line to convert the values of this dictionary into a single line string.
        /// </remarks>
        /// <returns>Dictionary with "Primary", "Secondary", "Alt" keys. Each Value is an array of strings. Ex: "Trackpad (slide)"</returns>
        public Dictionary<string, string[]> GetButtonNames()
        {
            Dictionary<string, string[]> buttons = new Dictionary<string, string[]>();
            var PrimaryBinds = GetBindingNames(OutputAction.ButtonPrimary, NameType.Button);
            var SeconaryBinds = GetBindingNames(OutputAction.ButtonSecondary, NameType.Button);
            var AltBinds = GetBindingNames(OutputAction.ButtonAlt, NameType.Button);

            buttons.Add("Primary", PrimaryBinds);
            buttons.Add("Secondary", SeconaryBinds);
            buttons.Add("Alt", AltBinds);

            return buttons;
        }

        

        void RefreshJoystickButtons()
        {
            var names = GetButtonNames();          

            string text = "";
            text += Line("Primary", names["Primary"]);
            text += Line("Secondary", names["Secondary"]);
            text += Line("Alt", names["Alt"]);

            joystickButtonsTextMesh.text = text;
        }

        void RefreshJoystickPOVs()
        {
            var POV1Binds = GetBindingNames(OutputAction.POV1, NameType.Direction);
            var POV1ButtonBinds = GetBindingNames(OutputAction.POV1, NameType.Button);
            var POV2Binds = GetBindingNames(OutputAction.POV2, NameType.Direction);
            var POV2ButtonBinds = GetBindingNames(OutputAction.POV2, NameType.Button);

            string text = "";
            text += Line("POV1", POV1Binds);
            text += Line("POV1-BTN", POV1ButtonBinds);
            text += Line("POV2", POV2Binds);
            text += Line("POV2-BTN", POV2ButtonBinds);

            joystickPovTextMesh.text = text;
        }
        void RefreshThrottlePOVs()
        {
            var POV3Binds = GetBindingNames(OutputAction.POV3, NameType.Direction);
            var POV3ButtonBinds = GetBindingNames(OutputAction.POV3, NameType.Button);
            // TODO Uncomment when adding a second POV to the throttle
            // var POV4Binds = GetBindingNames(OutputAction.POV4, NameType.Direction);
            // var POV4ButtonBinds = GetBindingNames(OutputAction.POV4, NameType.Button);


            string text = "";
            text += Line("POV3", POV3Binds);
            text += Line("POV3-BTN", POV3ButtonBinds);
            // text += Line("POV4", POV4Binds);
            // text += Line("POV4-BTN", POV4ButtonBinds);

            throttlePovTextMesh.text = text;
        }

        void RefreshThrottleButtons()
        {
            var names = GetButtonNames();

            string text = "";
            text += Line("Primary", names["Primary"]);
            text += Line("Secondary", names["Secondary"]);
            text += Line("Alt", names["Alt"]);

            throttleButtonsTextMesh.text = text;

        }
    }
}