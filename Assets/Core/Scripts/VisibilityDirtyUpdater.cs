using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core
{
    /**
     * OnDemandRenderer helper that sends a Dirty update when the behaviour is enabled or disabled
     * suggesting that the UI within it has been shown or hidden
     */
    public class VisibilityDirtyUpdater : MonoBehaviour
    {
        private void OnEnable()
        {
            OnDemandRenderer.Dirty(gameObject);
        }

        private void OnDisable()
        {
            OnDemandRenderer.Dirty(gameObject);
        }
    }
}
