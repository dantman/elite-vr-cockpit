using UnityEngine;

namespace EVRC
{
    public class EditModeOverrideButton : BaseButton
    {
        public CockpitUIMode.CockpitModeOverride modeOverride;

        public override void Activate()
        {
            FindObjectOfType<CockpitUIMode>().Override(modeOverride);
        }
    }
}
