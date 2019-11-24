using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EVRC
{
    using OutputAction = ActionsController.OutputAction;
    using NameType = InputBindingNameInfoManager.NameType;

    public class JoystickBindingsInfoGenerator : MonoBehaviour
    {
        [Tooltip("The TextMeshPro mesh to update with buttons binding info")]
        public TMPro.TMP_Text buttonsTextMesh;
        [Tooltip("The TextMeshPro mesh to update with pov binding info")]
        public TMPro.TMP_Text povTextMesh;

        private void OnEnable()
        {
            RefreshButtons();
            RefreshPOVs();
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

        void RefreshButtons()
        {
            var PrimaryBinds = GetBindingNames(OutputAction.ButtonPrimary, NameType.Button);
            var SeconaryBinds = GetBindingNames(OutputAction.ButtonSecondary, NameType.Button);
            var AltBinds = GetBindingNames(OutputAction.ButtonAlt, NameType.Button);

            string Line(string button, string[] binds) => button + ": " + JoinBinds(binds) + "\n";

            string text = "";
            text += Line("Primary", PrimaryBinds);
            text += Line("Secondary", SeconaryBinds);
            text += Line("Alt", AltBinds);

            buttonsTextMesh.text = text;
        }

        void RefreshPOVs()
        {
            var POV1Binds = GetBindingNames(OutputAction.POV1, NameType.Direction);
            var POV1ButtonBinds = GetBindingNames(OutputAction.POV1, NameType.Button);
            var POV2Binds = GetBindingNames(OutputAction.POV2, NameType.Direction);
            var POV2ButtonBinds = GetBindingNames(OutputAction.POV2, NameType.Button);

            string Line(string button, string[] binds) => button + ": " + JoinBinds(binds) + "\n";

            string text = "";
            text += Line("POV1", POV1Binds);
            text += Line("POV1-BTN", POV1ButtonBinds);
            text += Line("POV2", POV2Binds);
            text += Line("POV2-BTN", POV2ButtonBinds);

            povTextMesh.text = text;
        }
    }
}