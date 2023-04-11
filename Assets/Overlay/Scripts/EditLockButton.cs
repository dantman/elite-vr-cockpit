using UnityEngine;

namespace EVRC
{
    public class EditLockButton : BaseButton
    {
        public Texture lockedTexture;
        public Texture unlockedTexture;
        public Tooltip tooltip;
        public string lockedSuffix;
        public string unlockedSuffix;
        protected CockpitStateController controller;

        override protected void OnEnable()
        {
            base.OnEnable();
            controller = CockpitStateController.instance;
            CockpitStateController.EditLockedStateChanged.Listen(OnEditLockStateChanged);
        }

        override protected void OnDisable()
        {
            base.OnDisable();
            CockpitStateController.EditLockedStateChanged.Remove(OnEditLockStateChanged);
        }

        private void OnEditLockStateChanged(bool editLocked)
        {
            Refresh();
        }

        override protected void Refresh()
        {
            base.Refresh();

            if (!controller) return;

            if (controller.editLocked)
            {
                if (buttonImage != null) buttonImage.SetTexture(lockedTexture);
                if (tooltip) tooltip.Suffix = lockedSuffix;
            }
            else
            {
                if (buttonImage != null) buttonImage.SetTexture(unlockedTexture);
                if (tooltip) tooltip.Suffix = unlockedSuffix;
            }
        }

        protected override Unpress Activate()
        {
            controller.ToggleEditLocked();
            return noop;
        }

    }
}
