using System;
using System.Linq;
using UnityEngine;

namespace EVRC
{
    /**
     * A button type that uses a ControlButtonAsset and outputs keyboard commands
     * to control ED.
     */
    [RequireComponent(typeof(HolographicButton), typeof(Tooltip))]
    public class ControlButton : BaseButton
    {
        public ControlButtonAsset controlButtonAsset;
        protected Tooltip tooltip;

        protected static uint Id = 0;

        protected override void OnEnable()
        {
            if (!controlButtonAsset)
            {
                enabled = false;
                return;
            }

            base.OnEnable();

            tooltip = GetComponent<Tooltip>();
            holoButton.buttonId = controlButtonAsset.name + '#' + (++Id);

            controlButtonAsset.AddRefreshListener(Refresh);

            Refresh();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (controlButtonAsset)
            {
                controlButtonAsset.AddRefreshListener(Refresh);
            }
        }

        public override bool IsValid()
        {
            if (controlButtonAsset)
            {
                var control = controlButtonAsset.GetControl();
                var bindings = EDStateManager.instance.controlBindings;
                if (bindings != null)
                {
                    return bindings.HasKeyboardKeybinding(control);
                }
            }

            return true;
        }

        protected override void Refresh()
        {
            base.Refresh();

            if (holoButton)
            {
                holoButton.texture = controlButtonAsset.GetTexture();
                if (holoButton.texture == null)
                {
                    // As a fallback, use the backface texture if the controlButtonAsset doesn't have a texture
                    holoButton.texture = holoButton.backface;
                }
            }

            if (tooltip)
            {
                tooltip.Text = controlButtonAsset.GetText();
            }
        }

        public override void Activate()
        {
            var control = controlButtonAsset.GetControl();

            var bindings = EDStateManager.instance.controlBindings;
            if (bindings == null)
            {
                Debug.LogWarning("Control bindings not loaded");
            }
            else
            {
                var keyBinding = bindings.GetKeyboardKeybinding(control);
                string key = "";
                string[] modifiers = null;
                if (keyBinding == null)
                {
                    if (!controlButtonAsset.GetDefaultKeycombo(ref key, ref modifiers))
                    {
                        Debug.LogWarningFormat("Control was not bound and there is no default keycombo to fallback to");
                        return;
                    }
                }
                else
                {
                    key = keyBinding.Value.Key;
                    modifiers = keyBinding.Value.Modifiers.Select(mod => mod.Key).ToArray();
                }

                if (!KeyboardInterface.Send(key, modifiers))
                {
                    Debug.LogWarningFormat(
                        "Could not send keypress {0}, did not understand one or more of the keys",
                        KeyboardInterface.KeyComboDebugString(key, modifiers));
                }
            }
        }
    }
}
