using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace EVRC
{
    using static EVRC.ActionsController;
    using static EVRC.InputBindingNameInfoManager;
    using EDControlButton = EDControlBindings.EDControlButton;
    using CockpitMode = CockpitUIMode.CockpitMode;
    using ControlsMode = EDControlBindingsMap.ControlsMode;
    using ControlButtonBinding = EDControlBindings.ControlButtonBinding;
    using EDGuiFocus = EDStateManager.EDStatus_GuiFocus;
    using EDStatus_Flags = EDStateManager.EDStatus_Flags;
    using Hand = ActionsController.Hand;

    public class BindingInfoGenerator : MonoBehaviour
    {
        private EDGuiFocus guiFocus;
        private EDStatus_Flags statusFlags;

        Dictionary<EDControlButton, ControlButtonBinding> vJoyBinds;
        private List<EDControlButton> modeControls;
        private Dictionary<string, string[]> controlsMap;
        private Hand hand;

        public TMPro.TMP_Text ButtonsTextMesh;
        public TMPro.TMP_Text POVTextMesh;

        void OnEnable()
        {
            hand = GetComponentInParent<TrackedHand>().hand;
            //CockpitUIMode.ModeChanged.Listen(OnModeChange);
            //EDStateManager.GuiFocusChanged.Listen(OnGuiFocusChanged);
            //EDStateManager.StatusChanged.Listen(OnStatusChanged);
            //EDStateManager.BindingsChanged.Listen(OnBindsChanged);

            Refresh();
        }

        void OnDisable()
        {
            //CockpitUIMode.ModeChanged.Remove(OnModeChange);
            //EDStateManager.GuiFocusChanged.Remove(OnGuiFocusChanged);
            //EDStateManager.StatusChanged.Remove(OnStatusChanged);
            //EDStateManager.BindingsChanged.Remove(OnBindsChanged);
        }
        void Refresh()
        {         
            if (CockpitUIMode.Mode.HasFlag(CockpitMode.GameNotRunning))
            {
                ButtonsTextMesh.text = "Game Not Running. \nYour controls will display here";
                POVTextMesh.text = "Your POV/HAT bindings will display here";
                return;
            }
            if (controlsMap == null)
            {
                // get the names for the current controller "Trigger", "Grip", etc.
                GetControlNames();
            }

            // Get a list of the EDControlButtons that are valid for the current mode
            GetMappedBindings();

            // Filter the list of vJoy bindings to only the ones that are valid with the current mode.
            Dictionary<string, Dictionary<ControlButtonBinding.KeyBinding?, EDControlButton>> validBinds = GetValidControls();

            if (validBinds.Count == 0)
            {
                Debug.LogWarning("No valid bindings were found for this game mode");
                return;
            }

            SetButtonText(validBinds["Button"]);
            SetPOVText(validBinds["POV"]);
            
        }
        private void SetButtonText(Dictionary<ControlButtonBinding.KeyBinding?, EDControlButton> binds)
        {
            string controlsText = "";
            foreach (var bind in binds)
            {
                // use the real vJoy binding name to figure out which controlName it maps to
                    // ex: Joy_1 maps to "Primary"
                string controlNameToLookup = vJoyNamesMap[bind.Key.Value.Key].Item1;
                string[] controlName = controlsMap[controlNameToLookup];             

                // create a string from the EDControlButton name
                string gameControl = bind.Value.ToString();
                gameControl = Regex.Replace(gameControl, "(?<!^)([A-Z][a-z]|(?<=[a-z])[0-9]|(?<=[0-9])[A-Za-z])", " $1");

                // Skip if the control is not available on your current controllers (POV2, Alt, etc.)
                if (controlName == null || controlName.All(s => s == "")) { continue; }

                controlsText += Line(controlName, gameControl);
            }
            ButtonsTextMesh.text = controlsText;
        }

        /// <summary>
        /// Creates paragraphs from POV direction controls.
        /// </summary>
        /// <remarks>
        /// Each set of controls (POV1, POV2, etc.) is grouped and the directions are listed underneath it.
        /// </remarks>
        /// <param name="binds"></param>
        private void SetPOVText(Dictionary<ControlButtonBinding.KeyBinding?, EDControlButton> binds)
        {
            string controlsText = "";

            //sort by key, so that all of the control names are sequential
            binds = binds.OrderBy(x => x.Key.Value.Key).ToDictionary(x => x.Key, x => x.Value);

            string lastPov = "";
            foreach (var bind in binds)
            {
                // use the vJoy binding name to figure out which controlname to use
                    // ex: Joy_POV1 maps to Trackpad (press)
                    //     Joy_POV2 maps to Trackpad (slide)
                string controlNameToLookup = vJoyNamesMap[bind.Key.Value.Key].Item1;
                string[] controlName = controlsMap[controlNameToLookup];
                if (controlName == null || controlName.All(s => s == "")) { continue; }

                // extract the direction and POV name from the vJoy binding name
                string direction = ExtractPOVDirection(bind.Key.Value.Key);
                var pov = ExtractPOVNumber(bind.Key.Value.Key);
                // Skip if POV isn't relevant to the current hand
                
                // At the beginning of each new POV group, add a header line.
                if (pov != lastPov)
                {
                    lastPov = pov;
                    controlsText += Line(controlName, pov);
                }

                // convert to separate words from camelCase
                string gameControl = bind.Value.ToString();
                gameControl = Regex.Replace(gameControl, "(?<!^)([A-Z][a-z]|(?<=[a-z])[0-9]|(?<=[0-9])[A-Za-z])", " $1");           
                
                // direction listings within each POV group are indented 3 spaces.
                controlsText += POVIndentLine(direction, gameControl);

            }
            POVTextMesh.text = controlsText;
        }

        private Dictionary<string, Dictionary<ControlButtonBinding.KeyBinding?, EDControlButton>> GetValidControls()
        {
            vJoyBinds = EDStateManager.instance.controlBindings.GetAllVJoyBindings();

            // Get POV bindings
            Dictionary<ControlButtonBinding.KeyBinding?, EDControlButton> validPOVBinds = vJoyBinds
                .Where(kvp => modeControls.Contains(kvp.Key))
                //.Where(kvp => kvp.Value.VJoyKeybinding.ToString().Contains("JOY_POV"))
                .Where(kvp => kvp.Value.VJoyKeybinding.Value.Key.Contains("Joy_POV"))
                .Where(kvp => vJoyNamesMap[kvp.Value.VJoyKeybinding.Value.Key].Item3 == hand)
                .ToDictionary(kvp => kvp.Value.VJoyKeybinding, kvp => kvp.Key);

            // get non-POV bindings
            Dictionary<ControlButtonBinding.KeyBinding?, EDControlButton> validButtonBinds = vJoyBinds
                .Where(kvp => modeControls.Contains(kvp.Key))
                .Where(kvp => !kvp.Value.VJoyKeybinding.Value.Key.Contains("Joy_POV"))
                .Where(kvp => vJoyNamesMap[kvp.Value.VJoyKeybinding.Value.Key].Item3 == hand)
                .ToDictionary(kvp => kvp.Value.VJoyKeybinding, kvp => kvp.Key);

            return new Dictionary<string, Dictionary<ControlButtonBinding.KeyBinding?, EDControlButton>>()
            {
                { "POV", validPOVBinds },
                { "Button", validButtonBinds },
            };
        }
        
        public string Line(string[] name, string action) => string.Join(",",name) + ": " + action + "\n";
        public string POVIndentLine(string direction, string control) => "  " + Regex.Replace(direction, ",", "") + ": " + control + "\n";

        /// <summary>
        ///     
        /// </summary>
        /// <param name="bindString"> Expects a string in the form: Joy_POV1UP</param>
        /// <returns>The directional string at the end: UP,DOWN, etc.</returns>
        /// <exception cref="Exception"></exception>
        private string ExtractPOVDirection(string bindString)
        {
            Regex pattern = new Regex("_POV\\d(\\S+)");
            Match match = pattern.Match(bindString);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                throw new Exception($"Direction could not be extracted from binding: {bindString}");
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="bindString"> Expects a string in the form: Joy_POV1UP</param>
        /// <returns>The name of the POV and number: POV1, POV2, POV3, etc.</returns>
        /// <exception cref="Exception"></exception>
        private string ExtractPOVNumber(string bindString)
        {
            Regex pattern = new Regex("_(POV\\d)\\S+");
            Match match = pattern.Match(bindString);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                throw new Exception($"POV Number could not be extracted from binding: {bindString}");
            }
        }
        void GetMappedBindings()
        {
            CockpitMode mode = CockpitUIMode.Mode;
            if (mode.HasFlag(CockpitMode.GameNotRunning)) { return; }

            ControlsMode controlsMode = ControlsMode.NotRunning;

            //Order matters. InMainShip flag is present for lots of modes
            if (mode.HasFlag(CockpitMode.MenuMode))
            {
                controlsMode = ControlsMode.Menu;
            }
            else if (mode.HasFlag(CockpitMode.Map))
            {
                controlsMode = ControlsMode.Map;
            }
            else if (mode.HasFlag(CockpitMode.FSSMode))
            {
                controlsMode = ControlsMode.FSS;
            }
            else if (mode.HasFlag(CockpitMode.DSSMode))
            {
                controlsMode = ControlsMode.DSS;
            }
            else if (mode.HasFlag(CockpitMode.StationServices))
            {
                controlsMode = ControlsMode.StationServices;
            }
            else if (mode.HasFlag(CockpitMode.InMainShip))
            {
                switch (guiFocus)
                {
                    case EDGuiFocus.NoFocus:
                        controlsMode = ControlsMode.MainShip;
                        if (statusFlags.HasFlag(EDStatus_Flags.LandingGearDown))
                        {
                            controlsMode = ControlsMode.Landing;
                        }
                        break;
                    case EDGuiFocus.InternalPanel:
                        controlsMode = ControlsMode.CockpitPanel; 
                        break;
                    case EDGuiFocus.CommsPanel:
                        controlsMode = ControlsMode.CockpitPanel;
                        break;
                    case EDGuiFocus.ExternalPanel:
                        controlsMode = ControlsMode.CockpitPanel;
                        break;
                    case EDGuiFocus.RolePanel:
                        controlsMode = ControlsMode.CockpitPanel;
                        break;
                    case EDGuiFocus.Codex:
                        controlsMode = ControlsMode.CockpitPanel;
                        break;                    
                }
            }
            else if (mode.HasFlag(CockpitMode.InFighter))
            {
                controlsMode = ControlsMode.Fighter;
            }
            else if (mode.HasFlag(CockpitMode.InSRV))
            {
                controlsMode = ControlsMode.SRV;
            }
            else if (mode.HasFlag(CockpitMode.GameNotRunning))
            {
                controlsMode = ControlsMode.NotRunning;
            }
            else
            {
                Debug.LogError($"CockpitMode not handled by BindingInfoGenerator - {mode}");
            }

            // Get a list of EDControls that are valid in the current mode
            modeControls = EDControlBindingsMap.controlBindingsMap
                .Where(kvp => kvp.Key == controlsMode)
                .SelectMany(kvp => kvp.Value)
                .ToList();
            ;
        }

        Dictionary<string, (string, OutputAction, Hand)> vJoyNamesMap = new Dictionary<string, (string, OutputAction, Hand)>()
        {
            { "Joy_1", ("Primary", OutputAction.ButtonPrimary,Hand.Right)},
            { "Joy_2", ("Secondary", OutputAction.ButtonSecondary,Hand.Right)},
            { "Joy_3", ("Alt", OutputAction.ButtonAlt,Hand.Right)},
            { "Joy_4", ("POV1Button", OutputAction.POV1,Hand.Right)},
            { "Joy_5", ("POV2Button", OutputAction.POV2,Hand.Right)},
            { "Joy_POV1Up", ("POV1Direction", OutputAction.POV1,Hand.Right)},
            { "Joy_POV1Down", ("POV1Direction", OutputAction.POV1,Hand.Right)},
            { "Joy_POV1Left", ("POV1Direction", OutputAction.POV1,Hand.Right)},
            { "Joy_POV1Right", ("POV1Direction", OutputAction.POV1,Hand.Right)},
            { "Joy_POV2Up", ("POV2Direction", OutputAction.POV2,Hand.Right)},
            { "Joy_POV2Down", ("POV2Direction", OutputAction.POV2,Hand.Right)},
            { "Joy_POV2Left", ("POV2Direction", OutputAction.POV2,Hand.Right)},
            { "Joy_POV2Right", ("POV2Direction", OutputAction.POV2,Hand.Right)},
            { "Joy_6", ("POV3Button", OutputAction.POV3,Hand.Left)},
            { "Joy_8", ("Primary", OutputAction.ButtonPrimary,Hand.Left)},
            { "Joy_7", ("Secondary", OutputAction.ButtonSecondary,Hand.Left)},
            { "Joy_9", ("Alt", OutputAction.ButtonAlt,Hand.Left)},
            { "Joy_POV3Up", ("POV3Direction", OutputAction.POV3,Hand.Left)},
            { "Joy_POV3Down", ("POV3Direction", OutputAction.POV3,Hand.Left)},
            { "Joy_POV3Left", ("POV3Direction", OutputAction.POV3,Hand.Left)},
            { "Joy_POV3Right", ("POV3Direction", OutputAction.POV3,Hand.Left)}
        };

        void GetControlNames()
        {
            controlsMap = new Dictionary<string, string[]>
            {
                { "Primary", GetBindingNames(OutputAction.ButtonPrimary, NameType.Button) },
                { "Secondary", GetBindingNames(OutputAction.ButtonSecondary, NameType.Button) },
                { "Alt", GetBindingNames(OutputAction.ButtonAlt, NameType.Button) },
                { "POV1Direction", GetBindingNames(OutputAction.POV1, NameType.Direction) },
                { "POV1Button", GetBindingNames(OutputAction.POV1, NameType.Button) },
                { "POV2Direction", GetBindingNames(OutputAction.POV2, NameType.Direction) },
                { "POV2Button", GetBindingNames(OutputAction.POV2, NameType.Button) },
                { "POV3Direction", GetBindingNames(OutputAction.POV3, NameType.Direction) },
                { "POV3Button", GetBindingNames(OutputAction.POV3, NameType.Button) }
            };
            // TODO Uncomment when adding a second POV to the throttle
            //controlNames.Add("POV4Direction", GetBindingNames(OutputAction.POV4, NameType.Direction));
            //controlNames.Add("POV4Button", GetBindingNames(OutputAction.POV4, NameType.Button));
        }

        //void OnModeChange(CockpitUIMode.CockpitMode mode)
        //{
        //    currentMode = mode;
        //}

        //void OnGuiFocusChanged(EDGuiFocus currentFocus)
        //{
        //    guiFocus = currentFocus;
        //    Refresh();
        //}

        //void OnStatusChanged(EDStateManager.EDStatus status, EDStateManager.EDStatus? lastStatus)
        //{
        //    // convert flags int
        //    uint flags = status.Flags;
        //    EDStatus_Flags flagsEnum = (EDStatus_Flags)Enum.ToObject(typeof(EDStatus_Flags), flags);
        //    Console.WriteLine(flagsEnum);

        //    Refresh();
        //}

        //void OnBindsChanged(EDControlBindings binds)
        //{
        //    bindings = binds;
        //    vJoyBinds = bindings.GetAllVJoyBindings();
        //    Refresh();
        //}

        string[] GetBindingNames(OutputAction outputAction, NameType nameType)
            => ActionsController.GetBindingNames(ActionsControllerBindingsLoader.CurrentBindingsController, outputAction, nameType);

        Hand[] GetHandsForOutputAction(OutputAction outputAction)
            => ActionsController.GetHandsForOutputAction(ActionsControllerBindingsLoader.CurrentBindingsController, outputAction);
    }
}
