using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core.Actions
{
    using static KeyboardInterface;

    /**
     * A button type that outputs keyboard commands for a fixed control button binding.
     */
    [RequireComponent(typeof(Tooltip))]
    public class ControlBindingButton : BaseButton
    {
        [Tooltip("The button binding to press the associated keyboard key for")]
        public EDControlBindings.EDControlButton buttonBinding;

        protected override void OnEnable()
        {
            base.OnEnable();

            Refresh();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override Unpress Activate()
        {
            var unpress = CallbackPress(EDControlBindings.GetControlButton(buttonBinding, null));
            return () => unpress();
        }
    }
}
