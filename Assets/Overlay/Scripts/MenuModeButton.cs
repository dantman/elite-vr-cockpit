using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class MenuModeButton : BaseButton
    {
        public Texture offTexture;
        public Texture onTexture;
        public Tooltip tooltip;
        public string offSuffix;
        public string onSuffix;
        public MenuModeState menuModeState;      
        
        protected override void Refresh()
        {
            base.Refresh();

            if (menuModeState.menuMode)
            {
                buttonImage?.SetTexture(onTexture);
                if (tooltip) tooltip.Suffix = onSuffix;
            }
            else
            {
                buttonImage?.SetTexture(offTexture);
                if (tooltip) tooltip.Suffix = offSuffix;
            }
        }

        protected override Unpress Activate()
        {
            menuModeState.ToggleMenuMode();
            Refresh();
            return noop;
        }

    }
}
