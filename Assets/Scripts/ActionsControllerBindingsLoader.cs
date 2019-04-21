using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Events = SteamVR_Events;

    /**
     * Behaiour that loads the ActionsController SteamVR Input bindings implementation when SteamVR is initialized.
     * This could be updated in the future to enable other bindings implementations instead for other runtimes.
     */
    public class ActionsControllerBindingsLoader : MonoBehaviour
    {
        public ActionsController_SteamVRInputBindings steamVrInputBindings;

        private void OnEnable()
        {
            Events.Initialized.AddListener(OnSteamVRInitialized);
        }

        private void OnDisable()
        {
            Events.Initialized.RemoveListener(OnSteamVRInitialized);
        }

        private void OnSteamVRInitialized(bool initialized)
        {
            steamVrInputBindings.gameObject.SetActive(initialized);
        }
    }
}
