using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using InputAction = ActionsController.InputAction;

    /**
     * Special collection for managing localized name information for input bindings (SteamVR specific?)
     */
    public class InputBindingNameInfoManager
    {
        public enum BindingMode
        {
            Button,
            TrackpadSwipe,
            TrackpadPress,
            Joystick,
        }
        public enum NameType
        {
            Button,
            Direction,
        }

        public struct BindingNameInfo
        {
            public string localizedInputSource;
            public BindingMode bindingMode;

            public bool IsValidFor(NameType nameType)
            {
                // Joysticks cannot be buttons
                if (bindingMode == BindingMode.Joystick && nameType == NameType.Button) return false;
                // Trackpad swipes cannot be buttons
                if (bindingMode == BindingMode.TrackpadSwipe && nameType == NameType.Button) return false;
                // Buttons cannot be POV directions
                if (bindingMode == BindingMode.Button && nameType == NameType.Direction) return false;

                return true;
            }

            public string GetNameFor(NameType nameType)
            {
                string name = localizedInputSource;
                switch (bindingMode)
                {
                    case BindingMode.TrackpadSwipe:
                        name += " (swipe)";
                        break;
                    case BindingMode.TrackpadPress:
                        switch (nameType)
                        {
                            case NameType.Button:
                                name += " (center press)";
                                break;
                            case NameType.Direction:
                                name += " (edge press)";
                                break;
                        }
                        break;
                    case BindingMode.Joystick:
                        name += " (stick)";
                        break;
                }

                return name;
            }
        }

        private readonly Dictionary<InputAction, Dictionary<(SteamVR_Action, SteamVR_Input_Sources), BindingNameInfo>> inputActionBindingNameInfo = new Dictionary<InputAction, Dictionary<(SteamVR_Action, SteamVR_Input_Sources), BindingNameInfo>>();

        public string[] GetBindingNames(InputAction inputAction, NameType nameType)
        {
            if (!inputActionBindingNameInfo.ContainsKey(inputAction))
            {
                return new string[] { };
            }

            return inputActionBindingNameInfo[inputAction].Values
                .Where(nameInfo => nameInfo.IsValidFor(nameType))
                .Select(nameInfo => nameInfo.GetNameFor(nameType))
                .Distinct(StringComparer.InvariantCulture)
                .ToArray();
        }

        /**
         * Computes binding name information for a SteamVR boolean action
         */
        BindingNameInfo GetBindingNameInfo(InputAction inputAction, BindingMode bindingMode, SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            return new BindingNameInfo
            {
                localizedInputSource = fromAction.GetLocalizedOriginPart(fromSource, new EVRInputStringBits[] { EVRInputStringBits.VRInputString_InputSource }),
                bindingMode = bindingMode,
            };
        }

        /**
         * Computes binding name information for a SteamVR Vector2 action
         */
        BindingNameInfo GetBindingNameInfo(InputAction inputAction, BindingMode bindingMode, SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource)
        {
            return new BindingNameInfo
            {
                localizedInputSource = fromAction.GetLocalizedOriginPart(fromSource, new EVRInputStringBits[] { EVRInputStringBits.VRInputString_InputSource }),
                bindingMode = bindingMode,
            };
        }

        /**
         * Public handler to register listeners for a SteamVR boolean action
         */
        public Action RegisterBinding(InputAction inputAction, BindingMode bindingMode, SteamVR_Action_Boolean action, SteamVR_Input_Sources[] inputSources)
        {
            void OnActiveChange(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSouce, bool newState)
            {
                OnBindingActiveChange(inputAction, bindingMode, fromAction, fromSouce, newState);
            }

            foreach (var inputSource in inputSources)
            {
                action.AddOnActiveChangeListener(OnActiveChange, inputSource);
            }

            return () =>
            {
                foreach (var inputSource in inputSources)
                {
                    action.RemoveOnActiveChangeListener(OnActiveChange, inputSource);
                }
            };
        }

        /**
         * Public handler to register listeners for a SteamVR Vector2 action
         */
        public Action RegisterBinding(InputAction inputAction, BindingMode bindingMode, SteamVR_Action_Vector2 action, SteamVR_Input_Sources[] inputSources)
        {
            void OnActiveChange(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSouce, bool newState)
            {
                OnBindingActiveChange(inputAction, bindingMode, fromAction, fromSouce, newState);
            }

            foreach (var inputSource in inputSources)
            {
                action.AddOnActiveChangeListener(OnActiveChange, inputSource);
            }

            return () =>
            {
                foreach (var inputSource in inputSources)
                {
                    action.RemoveOnActiveChangeListener(OnActiveChange, inputSource);
                }
            };
        }

        /**
         * Altered event handler for handling activation/deactivation of a SteamVR boolean action
         */
        private void OnBindingActiveChange(InputAction inputAction, BindingMode bindingMode, SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (newState)
            {
                AddBindingNameInfo(inputAction, fromAction, fromSource, GetBindingNameInfo(inputAction, bindingMode, fromAction, fromSource));
            }
            else
            {
                RemoveBindingNameInfo(inputAction, fromAction, fromSource);
            }
        }

        /**
         * Altered event handler for handling activation/deactivation of a SteamVR Vector2 action
         */
        private void OnBindingActiveChange(InputAction inputAction, BindingMode bindingMode, SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, bool newState)
        {
            if (newState)
            {
                AddBindingNameInfo(inputAction, fromAction, fromSource, GetBindingNameInfo(inputAction, bindingMode, fromAction, fromSource));
            }
            else
            {
                RemoveBindingNameInfo(inputAction, fromAction, fromSource);
            }
        }

        /**
         * Adds new binding name info
         */
        private void AddBindingNameInfo(InputAction inputAction, SteamVR_Action fromAction, SteamVR_Input_Sources fromSource, BindingNameInfo bindingNameInfo)
        {
            if (!inputActionBindingNameInfo.ContainsKey(inputAction))
            {
                inputActionBindingNameInfo[inputAction] = new Dictionary<(SteamVR_Action, SteamVR_Input_Sources), BindingNameInfo>();
            }
            inputActionBindingNameInfo[inputAction][(fromAction, fromSource)] = bindingNameInfo;
        }

        /**
         * Removes a binding name info for a boolean action
         */
        private void RemoveBindingNameInfo(InputAction inputAction, SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (inputActionBindingNameInfo.ContainsKey(inputAction))
            {
                if (inputActionBindingNameInfo[inputAction].ContainsKey((fromAction, fromSource)))
                {
                    inputActionBindingNameInfo[inputAction].Remove((fromAction, fromSource));
                }
            }
        }

        /**
         * Removes a binding name info for a vector2 action
         */
        private void RemoveBindingNameInfo(InputAction inputAction, SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource)
        {
            if (inputActionBindingNameInfo.ContainsKey(inputAction))
            {
                if (inputActionBindingNameInfo[inputAction].ContainsKey((fromAction, fromSource)))
                {
                    inputActionBindingNameInfo[inputAction].Remove((fromAction, fromSource));
                }
            }
        }
    }
}
