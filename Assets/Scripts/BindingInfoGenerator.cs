using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EVRC

{
    using static EVRC.ActionsController;
    using static EVRC.InputBindingNameInfoManager;
    using EDControlButton = EDControlBindings.EDControlButton;
    using CockpitMode = CockpitUIMode.CockpitMode;
    using ControlButtonBinding = EDControlBindings.ControlButtonBinding;
    using Hand = ActionsController.Hand;

    public class BindingInfoGenerator : MonoBehaviour
    {
        public static BindingInfoGenerator _instance;
        public static BindingInfoGenerator instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[BindingsManager]");
            }
        }

        [Header("References")]
        private Dictionary<EDControlButton, ControlButtonBinding> vJoyBinds;
        private Hand hand;

        public struct ControlStrings
        {
            public List<string> POV;
            public List<string> buttons;
        }

        private Dictionary<CockpitMode, Dictionary<Hand, ControlStrings>> controlStrings; 

        public string[] GetControlStringsFromMode(CockpitMode mode, Hand hand)
        {
            if(controlStrings.TryGetValue(mode, out Dictionary<Hand, ControlStrings> strings))
            {
                if(strings.TryGetValue(hand, out ControlStrings handStrings))
                {
                    return new string[2] { string.Join("\n", handStrings.buttons), string.Join("\n", handStrings.POV) };
                }
                return new string[2] { $"No Controls were found for {hand}", $"No Controls were found for {hand}" };
            };
            return new string[2] { $"No Controls were found for {mode}", $"No Controls were found for {mode}" };
        }
        void Awake()
        {
            controlStrings = new Dictionary<CockpitMode, Dictionary<Hand, ControlStrings>>();
            EDStateManager.BindingsChanged.Listen(OnBindsChanged);          
        }

        public void Refresh()
        {
            GetAllBindingStrings();
        }

        #region ListenerMethods
        void OnBindsChanged(EDControlBindings edcb)
        {
            vJoyBinds = edcb.GetAllVJoyBindings(); 
            Refresh();
        }

        #endregion

        private void GetAllBindingStrings()
        {
            foreach (KeyValuePair<string, (OutputAction, NameType, Hand)> kvp in vJoyInterface.vJoyNamesMap)
            {
                // find bindings that match the vjoy string (Joy_1) === PrimaryFire, BuggyFire
                EDControlButton[] bindsForVKey = vJoyBinds
                                .Where(bind => bind.Value.VJoyKeybinding.Value.Key == kvp.Key)
                                .Select(bind => bind.Key)
                                .ToArray();

                // get the handed string value for that control
                string controlName = ActionsController.GetHandedBindingName(ActionsControllerBindingsLoader.CurrentBindingsController, kvp.Value.Item1, kvp.Value.Item2, kvp.Value.Item3);

                foreach (EDControlButton btn in bindsForVKey)
                {
                    if (EDControlBindingsMap.controlBindingsMap.TryGetValue(btn, out List<CockpitMode> modeList))
                    {
                        foreach (CockpitMode mode in modeList)
                        {
                            AddControlString(mode, kvp.Value.Item2, btn, kvp.Value.Item3, controlName);
                        }
                    }
                    else
                    {
                        throw new Exception($"Control {btn} is missing from the controlbindings map. Check the ControlBindingMap Asset.");
                    }

                }

            }
        }

        internal void AddControlString(CockpitMode mode, NameType type, EDControlButton eliteControl, Hand hand, string controlName)
        {
            switch (type)
            {
                case NameType.Button:
                    if (controlStrings[mode].TryGetValue(hand, out ControlStrings currentStrings))
                    {
                        currentStrings.buttons.Add(Line(controlName, eliteControl.ToString()));
                        controlStrings[mode][hand] = currentStrings;
                        return;
                    }
                    else
                    {
                        controlStrings[mode][hand] = new ControlStrings() { buttons = new List<string>() { Line(controlName, eliteControl.ToString()) } };
                        return;
                    };
                case NameType.Direction:
                    if (controlStrings[mode].TryGetValue(hand, out ControlStrings currentPOVStrings))
                    {
                        currentPOVStrings.POV.Add(Line(controlName, eliteControl.ToString()));
                        controlStrings[mode][hand] = currentPOVStrings;
                        return;
                    }
                    else
                    {
                        controlStrings[mode][hand] = new ControlStrings() { POV = new List<string>() { Line(controlName, eliteControl.ToString()) } };
                        return;
                    };
            }
        }

        string Line(string controlName, string eliteControl) => controlName + ": " + eliteControl + "\n";
    }
}
