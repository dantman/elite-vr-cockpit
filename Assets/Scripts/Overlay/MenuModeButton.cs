using UnityEngine;

namespace EVRC
{
    public class MenuModeButton : BaseButton
    {
        public Texture offTexture;
        public Texture onTexture;
        public Tooltip tooltip;
        public string offSuffix;
        public string onSuffix;
        protected CockpitStateController controller;

        override protected void OnEnable()
        {
            base.OnEnable();
            controller = CockpitStateController.instance;
            CockpitStateController.MenuModeStateChanged.Listen(OnMenuModStateChanged);
        }

        override protected void OnDisable()
        {
            base.OnDisable();
            CockpitStateController.MenuModeStateChanged.Remove(OnMenuModStateChanged);
        }

        private void OnMenuModStateChanged(bool editLocked)
        {
            Refresh();
        }

        override protected void Refresh()
        {
            base.Refresh();

            if (!controller) return;

            if (controller.menuMode)
            {
                if (buttonImage != null) buttonImage.SetTexture(onTexture);
                if (tooltip) tooltip.Suffix = onSuffix;
            }
            else
            {
                if (buttonImage != null) buttonImage.SetTexture(offTexture);
                if (tooltip) tooltip.Suffix = offSuffix;
            }
        }

        protected override Unpress Activate()
        {
            controller.ToggleMenuMode();
            return noop;
        }

    }
}
