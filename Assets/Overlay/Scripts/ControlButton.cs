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
        public HolographicText labelObject;

        void Awake()
        {
            if (labelObject == null)
            {
                labelObject = GetComponentInChildren<HolographicText>();
                labelObject.text = label;
                labelObject.createRenderTexture();
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


            var holoButton = buttonImage as HolographicOverlay;

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

        private void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.01f);
        }
    }
}
