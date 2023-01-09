using UnityEngine;

namespace EVRC
{
    using static KeyboardInterface;

    /**
     * A button type that uses a ControlButtonAsset and outputs keyboard commands
     * to control ED.
     */
    [RequireComponent(typeof(Tooltip))]
    public class ControlButton : BaseButton
    {
        public ControlButtonAsset controlButtonAsset;
        protected Tooltip tooltip;
        public string label;
        public RenderTexture renderTexture;

        protected static uint Id = 0;

        public void createLabelTexture()
        {
            renderTexture = new RenderTexture(128, 64, 0, RenderTextureFormat.ARGB32);
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            renderTexture.antiAliasing = 4;
            renderTexture.filterMode = FilterMode.Trilinear;
            renderTexture.name = this.name + "RenderTexture";

            renderTexture.Create();
            if (label != null && label != "")
            {
                RenderTextureTextCapture.RenderText(renderTexture, label, TMPro.TextAlignmentOptions.Top);

            }
            else
            {
                Debug.LogWarning($"Unable to render text into texture. Label is null: {this.name}");
            }
        }

        void OnValidate()
        {
            if (label != GetComponentInChildren<ButtonLabelDisplay>().label)
            {
                createLabelTexture();
            }
        }

        protected override void OnEnable()
        {
            if (!controlButtonAsset)
            {
                enabled = false;
                return;
            }

            base.OnEnable();

            tooltip = GetComponent<Tooltip>();
            label = controlButtonAsset.GetLabelText();
            GetComponentInChildren<ButtonLabelDisplay>().AddButtonLabelStateListener();
            

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

            GetComponentInChildren<ButtonLabelDisplay>().RemoveButtonLabelStateListener();
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
                tooltip.Text = controlButtonAsset.GetTooltipText();
            }

            if (label != null)
            {
                label = controlButtonAsset.GetLabelText();
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
