namespace EVRC.Core.Overlay
{
    public class EditModeOverrideButton : BaseButton
    {
        public CockpitUIMode.CockpitModeOverride modeOverride;

        protected override Unpress Activate()
        {
            FindObjectOfType<CockpitUIMode>().Override(modeOverride);
            return noop;
        }
    }
}
