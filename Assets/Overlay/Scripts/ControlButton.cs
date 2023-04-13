using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
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

        protected static uint Id = 0;

        private HolographicText labelObject;

        void Awake()
        {
            if (labelObject == null)
            {
                labelObject = GetComponentInChildren<HolographicText>();
                labelObject.text = label;
                labelObject.createRenderTexture();
            }
            CockpitSettingsState.ButtonLabelStateChanged.Listen(OnLabelStateChanged);
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


            var holoButton = buttonImage as HolographicOverlay;
            //if (holoButton != null)
            //{
            //    holoButton.buttonId = controlButtonAsset.name + '#' + (++Id);
            //}

            if (labelObject != null && labelObject.text != label)
            {
                labelObject.text = label;
                labelObject.ReRender();
            }

            controlButtonAsset.AddRefreshListener(Refresh);

            Refresh();
        }

        private void OnLabelStateChanged(bool visible)
        {
            labelObject.enabled = visible;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (controlButtonAsset)
            {
                controlButtonAsset.AddRefreshListener(Refresh);
            }

            //GetComponentInChildren<ButtonLabelDisplay>().RemoveButtonLabelStateListener();
            CockpitSettingsState.ButtonLabelStateChanged.Remove(OnLabelStateChanged);
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
                //if (tex == null)
                //{
                //    var holoButton = buttonImage as HolographicOverlay;
                //    if (holoButton != null)
                //    {
                //        // As a fallback, use the backface texture of the controlButtonAsset doesn't have a texture
                //        buttonImage.SetTexture();
                //    }
                //}
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
