using System;
using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    using static KeyboardInterface;

    /// <summary>
    /// A button type that uses a ControlButtonAsset and outputs keyboard commands to control Elite Dangerous.
    /// </summary>
    [RequireComponent(typeof(Tooltip))]
    public class ControlButton : BaseButton
    {
        public ControlBindingsState controlBindingsState;
        public ControlButtonAsset controlButtonAsset;
        public EDStatusFlags configuredStatusFlag;
        public EDGuiFocus configuredGuiFocus;
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

            if (labelObject != null
                && labelObject.text != label
                && labelObject.isActiveAndEnabled == true
                )
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
            if (!controlButtonAsset) return false;
            EDControlButton control = controlButtonAsset.GetControl();

            ControlBindingsState bindings = controlBindingsState;
            return bindings.buttonBindings != null && bindings.HasKeyboardKeybinding(control);
        }

        protected override void Refresh()
        {
            base.Refresh();

            if (buttonImage != null)
            {
                Texture tex = controlButtonAsset.GetTexture();
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
            EDControlButton control = controlButtonAsset.GetControl();
            KeyCombo? defaultKeycombo = controlButtonAsset.GetDefaultKeycombo();
            Action unpress = CallbackPress(controlBindingsState.GetControlButton(control, defaultKeycombo));
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
