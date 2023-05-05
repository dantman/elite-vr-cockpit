using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static EVRC.Core.KeyboardInterface;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Control Bindings State"), Serializable]
    public class ControlBindingsState : GameState
    {
        public Dictionary<EDControlButton, ControlButtonBinding> buttonBindings = new Dictionary<EDControlButton, ControlButtonBinding>();
        public bool ready = false;

        
        public override string GetStatusText()
        {
            return ready ? "Loaded" : "Not Loaded";
        }

        public IKeyPress GetControlButton(EDControlButton control, KeyboardInterface.KeyCombo? defaultKeyCombo = null)
        {
            if (!ready)
            {
                Debug.LogWarning("Control bindings not loaded");
                return null;
            }
            else
            {
                var keyBinding = GetKeyboardKeybinding(control);
                string key = "";
                string[] modifiers = null;
                if (keyBinding == null)
                {
                    if (defaultKeyCombo == null)
                    {
                        Debug.LogError($"The Elite Dangerous control: {control} has no keyboard binding and there is no default keycombo.");
                        return null;
                    }
                }
                else
                {
                    key = keyBinding.Value.Key;
                    modifiers = keyBinding.Value.Modifiers.Select(mod => mod.Key).ToArray();
                }

                // @todo Implement this as a press and release
                var keyPress = KeyboardInterface.Key(key, modifiers);
                if (keyPress == null)
                {
                    Debug.LogWarningFormat(
                        "Could not send keypress {0}, did not understand one or more of the keys",
                        KeyboardInterface.KeyComboDebugString(key, modifiers));
                }

                return keyPress;
            }
        }

        /// <summary>
        ///     Checks to see if there is a valid binding for a keyboard, based on the device that is listed in the XML
        /// </summary>
        /// <param name="button"></param>
        /// <returns> True if valid keyboard binding </returns>
        public bool HasKeyboardKeybinding(EDControlButton button)
        {
            if (!buttonBindings.ContainsKey(button)) return false;

            ControlButtonBinding buttonBinding = buttonBindings[button];
            return buttonBinding.HasKeyboardKeybinding;
        }

        /// <summary>
        ///     Checks to see if there is a valid binding for vJoy, based on the device that is listed in the XML
        /// </summary>
        /// <param name="button"></param>
        /// <returns> True if valid vJoy binding </returns>
        public bool HasVJoyKeybinding(EDControlButton button)
        {
            if (!buttonBindings.ContainsKey(button)) return false;

            ControlButtonBinding buttonBinding = buttonBindings[button];
            return buttonBinding.HasVJoyKeybinding;
        }

        /// <summary>
        /// Gets the first binding that is configured on the keyboard device. 
        /// </summary>
        /// <remarks>
        /// Checks both Primary and Secondary bindings for the key, but will only return the Secondary value if the Primary binding is not for the keyboard and/or is invalid.
        /// </remarks>
        /// <param name="button">Elite Dangerous command from Custom.X.0.binds</param>
        public ControlButtonBinding.KeyBinding? GetKeyboardKeybinding(EDControlButton button)
        {
            if (buttonBindings.TryGetValue(button, out ControlButtonBinding binding))
            {
                return binding.KeyboardKeybinding;
            }

            return null;
        }

        /// <summary>
        /// Gets the first binding that is configured on the vJoy device for a given Elite Dangerous control. 
        /// </summary>
        /// <remarks>
        /// Checks both Primary and Secondary bindings for the key, but will only return the Secondary value if the Primary binding is not for the keyboard and/or is invalid.
        /// </remarks>
        /// <param name="button">Elite Dangerous command from Custom.X.0.binds</param>
        public ControlButtonBinding.KeyBinding? GetVJoyKeybinding(EDControlButton button)
        {
            if (buttonBindings.TryGetValue(button, out ControlButtonBinding binding))
            {
                return binding.VJoyKeybinding;
            }

            return null;
        }

    }
}
