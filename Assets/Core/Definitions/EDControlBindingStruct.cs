using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    public struct ControlButtonBinding
    {
        public struct KeyModifier
        {
            public string Device;
            public string Key;
        }

        public struct KeyBinding
        {
            public string Device;
            public string Key;
            //public string DeviceIndex;
            public HashSet<KeyModifier> Modifiers;

            public bool IsValid
            {
                get { return Device != "{NoDevice}"; }
            }

            // Is this a Keyboard key press we can act on?
            public bool IsValidKeypress
            {
                get
                {
                    // Is it on the Keyboard device?
                    if (Device != "Keyboard") return false;

                    foreach (var modifier in Modifiers)
                    {
                        if (modifier.Device != "Keyboard") return false;
                    }

                    return true;
                }
            }

            // Is there a VJoy action we can act on?
            public bool IsValidVJoyPress
            {
                get
                {
                    // Is it on the vJoy device?
                    if (Device != "vJoy") return false;
                    if (Modifiers.Count > 0) return false;
                    return true;
                }
            }
        }

        public KeyBinding Primary;
        public KeyBinding Secondary;

        public bool HasKeyboardKeybinding
        {
            get
            {
                return Primary.IsValidKeypress || Secondary.IsValidKeypress;
            }
        }

        public bool HasVJoyKeybinding
        {
            get
            {
                return Primary.IsValidVJoyPress || Secondary.IsValidVJoyPress;
            }
        }

        public KeyBinding? KeyboardKeybinding
        {
            get
            {
                if (Primary.IsValidKeypress) return Primary;
                if (Secondary.IsValidKeypress) return Secondary;
                return null;
            }
        }

        public KeyBinding? VJoyKeybinding
        {
            get
            {
                if (Primary.IsValidVJoyPress) return Primary;
                if (Secondary.IsValidVJoyPress) return Secondary;
                return null;
            }
        }
    }

}
