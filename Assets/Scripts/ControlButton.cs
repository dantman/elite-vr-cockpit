using UnityEngine;

namespace EVRC
{
    using static KeyboardInterface;

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
            var holoButton = buttonImage as HolographicButton;
            if (holoButton != null)
            {
                holoButton.buttonId = controlButtonAsset.name + '#' + (++Id);
            }

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

            if (buttonImage != null)
            {
                var tex = controlButtonAsset.GetTexture();
                buttonImage.SetTexture(tex);
                if (tex == null)
                {
                    var holoButton = buttonImage as HolographicButton;
                    if (holoButton != null)
                    {
                        // As a fallback, use the backface texture if the controlButtonAsset doesn't have a texture
                        buttonImage.SetTexture(holoButton.backface);
                    }
                }
            }

            if (tooltip)
            {
                tooltip.Text = controlButtonAsset.GetText();
            }
        }

        protected override Unpress Activate()
        {
            var control = controlButtonAsset.GetControl();
            KeyCombo? defaultKeycombo = controlButtonAsset.GetDefaultKeycombo();
            var unpress = CallbackPress(EDControlBindings.GetControlButton(control, defaultKeycombo));
            return () => unpress();
        }
    }
}
