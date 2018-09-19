using System;
using System.Collections.Generic;
using WindowsInput;
using WindowsInput.Native;

namespace EVRC
{
    /**
     * Utility that sends keyboard keypresses ED based on parsed control bindings
     */
    public class KeyboardInterface
    {
        // A map of Elite Dangerous "Key_*" strings to native VirtualKeyCode values
        private static Dictionary<string, VirtualKeyCode> virtualKeyCodeMapping = new Dictionary<string, VirtualKeyCode>()
        {
            // ED Doesn't actually have an Esc/Escape keybinding to my knowledge,
            // But I included one here so we can use the same system for sending ESC from a button
            { "Key_Escape", VirtualKeyCode.ESCAPE },
            // @todo VirtualKeyCode has L and R version of modifier keys but says those are for the get methods
            // test it out and see if we could make use of them to output the proper keys
            { "Key_LeftShift", VirtualKeyCode.SHIFT },
            { "Key_LeftControl", VirtualKeyCode.CONTROL },
            { "Key_LeftAlt", VirtualKeyCode.MENU },
            { "Key_Backspace", VirtualKeyCode.BACK },
            { "Key_Tab", VirtualKeyCode.TAB },
            { "Key_Enter", VirtualKeyCode.RETURN },
            // { "Key_", VirtualKeyCode.CAPITAL }, @todo Find out what ED calls CapsLock
            { "Key_Space", VirtualKeyCode.SPACE },
            { "Key_PageUp", VirtualKeyCode.PRIOR },
            { "Key_PageDown", VirtualKeyCode.NEXT },
            { "Key_End", VirtualKeyCode.END },
            { "Key_Home", VirtualKeyCode.HOME },
            { "Key_LeftArrow", VirtualKeyCode.LEFT },
            { "Key_UpArrow", VirtualKeyCode.UP },
            { "Key_RightArrow", VirtualKeyCode.RIGHT },
            { "Key_DownArrow", VirtualKeyCode.DOWN },
            { "Key_Insert", VirtualKeyCode.INSERT },
            { "Key_Delete", VirtualKeyCode.DELETE },
            { "Key_0", VirtualKeyCode.VK_0 },
            { "Key_1", VirtualKeyCode.VK_1 },
            { "Key_2", VirtualKeyCode.VK_2 },
            { "Key_3", VirtualKeyCode.VK_3 },
            { "Key_4", VirtualKeyCode.VK_4 },
            { "Key_5", VirtualKeyCode.VK_5 },
            { "Key_6", VirtualKeyCode.VK_6 },
            { "Key_7", VirtualKeyCode.VK_7 },
            { "Key_8", VirtualKeyCode.VK_8 },
            { "Key_9", VirtualKeyCode.VK_9 },
            { "Key_A", VirtualKeyCode.VK_A },
            { "Key_B", VirtualKeyCode.VK_B },
            { "Key_C", VirtualKeyCode.VK_C },
            { "Key_D", VirtualKeyCode.VK_D },
            { "Key_E", VirtualKeyCode.VK_E },
            { "Key_F", VirtualKeyCode.VK_F },
            { "Key_G", VirtualKeyCode.VK_G },
            { "Key_H", VirtualKeyCode.VK_H },
            { "Key_I", VirtualKeyCode.VK_I },
            { "Key_J", VirtualKeyCode.VK_J },
            { "Key_K", VirtualKeyCode.VK_K },
            { "Key_L", VirtualKeyCode.VK_L },
            { "Key_M", VirtualKeyCode.VK_M },
            { "Key_N", VirtualKeyCode.VK_N },
            { "Key_O", VirtualKeyCode.VK_O },
            { "Key_P", VirtualKeyCode.VK_P },
            { "Key_Q", VirtualKeyCode.VK_Q },
            { "Key_R", VirtualKeyCode.VK_R },
            { "Key_S", VirtualKeyCode.VK_S },
            { "Key_T", VirtualKeyCode.VK_T },
            { "Key_U", VirtualKeyCode.VK_U },
            { "Key_V", VirtualKeyCode.VK_V },
            { "Key_W", VirtualKeyCode.VK_W },
            { "Key_X", VirtualKeyCode.VK_X },
            { "Key_Y", VirtualKeyCode.VK_Y },
            { "Key_Z", VirtualKeyCode.VK_Z },
            { "Key_Numpad_1", VirtualKeyCode.NUMPAD1 },
            { "Key_Numpad_2", VirtualKeyCode.NUMPAD2 },
            { "Key_Numpad_3", VirtualKeyCode.NUMPAD3 },
            { "Key_Numpad_4", VirtualKeyCode.NUMPAD4 },
            { "Key_Numpad_5", VirtualKeyCode.NUMPAD5 },
            { "Key_Numpad_6", VirtualKeyCode.NUMPAD6 },
            { "Key_Numpad_7", VirtualKeyCode.NUMPAD7 },
            { "Key_Numpad_8", VirtualKeyCode.NUMPAD8 },
            { "Key_Numpad_9", VirtualKeyCode.NUMPAD9 },
            { "Key_Numpad_0", VirtualKeyCode.NUMPAD0 },
            { "Key_F1", VirtualKeyCode.F1 },
            { "Key_F2", VirtualKeyCode.F2 },
            { "Key_F3", VirtualKeyCode.F3 },
            { "Key_F4", VirtualKeyCode.F4 },
            { "Key_F5", VirtualKeyCode.F5 },
            { "Key_F6", VirtualKeyCode.F6 },
            { "Key_F7", VirtualKeyCode.F7 },
            { "Key_F8", VirtualKeyCode.F8 },
            { "Key_F9", VirtualKeyCode.F9 },
            { "Key_F10", VirtualKeyCode.F10 },
            { "Key_F11", VirtualKeyCode.F11 },
            { "Key_F12", VirtualKeyCode.F12 },
            { "Key_F13", VirtualKeyCode.F13 },
            { "Key_F14", VirtualKeyCode.F14 },
            { "Key_F15", VirtualKeyCode.F15 },
            { "Key_F16", VirtualKeyCode.F16 },
            { "Key_F17", VirtualKeyCode.F17 },
            { "Key_F18", VirtualKeyCode.F18 },
            { "Key_F19", VirtualKeyCode.F19 },
            { "Key_F20", VirtualKeyCode.F20 },
            { "Key_F21", VirtualKeyCode.F21 },
            { "Key_F22", VirtualKeyCode.F22 },
            { "Key_F23", VirtualKeyCode.F23 },
            { "Key_F24", VirtualKeyCode.F24 },
            //{ "Key_", VirtualKeyCode.NUMLOCK },
            //{ "Key_", VirtualKeyCode.MULTIPLY },
            //{ "Key_", VirtualKeyCode.ADD },
            //{ "Key_", VirtualKeyCode.SEPARATOR },
            //{ "Key_", VirtualKeyCode.SUBTRACT },
            //{ "Key_", VirtualKeyCode.DECIMAL },
            //{ "Key_", VirtualKeyCode.DIVIDE },
            // Key_Equals does not appear to be present in VirtualKeyCode, or is it OEM_PLUS?
            { "Key_Equals", VirtualKeyCode.OEM_PLUS },
            //{ "Key_", VirtualKeyCode.OEM_COMMA },
            { "Key_Minus", VirtualKeyCode.OEM_MINUS },
            //{ "Key_", VirtualKeyCode.OEM_PERIOD },
            //{ "Key_", VirtualKeyCode.OEM_2 },
            //{ "Key_", VirtualKeyCode.OEM_3 },
            //{ "Key_", VirtualKeyCode.OEM_4 },
            //{ "Key_", VirtualKeyCode.OEM_5 },
            //{ "Key_", VirtualKeyCode.OEM_6 },
            //{ "Key_", VirtualKeyCode.OEM_7 },
            //{ "Key_", VirtualKeyCode.OEM_102 },
            //{ "Key_BackSlash", VirtualKeyCode. },
        };

        public static InputSimulator simulator = new InputSimulator();

        /**
         * Can we map a Key_* string from elite to a VirtualKeyCode
         */
        public static bool IsKeyValid(string key)
        {
            return virtualKeyCodeMapping.ContainsKey(key);
        }

        /**
         * Get a debug string like "Key_LeftShift+Key_A" for a key combo
         */
        public static string KeyComboDebugString(string key, string[] modifiers = null)
        {
            var keys = new string[(modifiers == null ? modifiers.Length : 0) + 1];
            if (modifiers != null)
            {
                Array.Copy(modifiers, keys, modifiers.Length);
            }

            keys[keys.Length - 1] = key;

            return string.Join("+", keys);
        }

        /**
         * Get a VirtualKeyCode to use for a Key_* string
         */
        public static VirtualKeyCode GetKeyCode(string key)
        {
            return virtualKeyCodeMapping[key];
        }

        /**
         * Send a simulated keypress combo
         */
        public static bool Send(string key, string[] modifiers = null)
        {
            if (!IsKeyValid(key)) return false;
            VirtualKeyCode keyCode = GetKeyCode(key);
            VirtualKeyCode[] modifierCodes = new VirtualKeyCode[modifiers == null ? 0 : modifiers.Length];

            if (modifiers != null)
            {
                for (int i = 0; i < modifiers.Length; ++i)
                {
                    if (!IsKeyValid(modifiers[i])) return false;
                    modifierCodes[i] = GetKeyCode(modifiers[i]);
                }
            }

            Send(keyCode, modifierCodes);
            return true;
        }

        /**
         * Send a simulated keypress combo
         */
        public static void Send(VirtualKeyCode key, VirtualKeyCode[] modifiers = null)
        {
            var mod = new Stack<VirtualKeyCode>();
            if (modifiers != null && modifiers.Length > 0)
            {
                foreach (var modifier in modifiers)
                {
                    simulator.Keyboard.KeyDown(modifier);
                    mod.Push(modifier);
                }
            }
            
            simulator.Keyboard.KeyDown(key);
            simulator.Keyboard.Sleep(10);
            simulator.Keyboard.KeyUp(key);

            while (mod.Count > 0)
            {
                simulator.Keyboard.KeyUp(mod.Pop());
            }
        }

        /**
         * Send a simulated Escape key press
         */
        public static void SendEscape()
        {
            simulator.Keyboard.KeyDown(VirtualKeyCode.ESCAPE);
            simulator.Keyboard.Sleep(100);
            simulator.Keyboard.KeyUp(VirtualKeyCode.ESCAPE);
        }
    }
}
