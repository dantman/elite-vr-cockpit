using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class HelpButton : BaseButton
    {
        protected override Unpress Activate()
        {
            Debug.LogWarning("Help Button has not been configured");
            return () => { };
        }
    }
}