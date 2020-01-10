using UnityEngine;
using Valve.VR;

namespace EVRC.DesktopUI
{
    /**
     * Behaviour used to show/hide a controller bindings UI button if available
     */
    public class ControllerBindingsEditAvailability : MonoBehaviour
    {
        [Tooltip("Target object to enable/disable depending on edit bindings UI availability")]
        public GameObject target;

        private void OnEnable()
        {
            SteamVR_Events.Initialized.Listen(OnInitialize);
            Refresh();
        }

        private void OnDisable()
        {
            SteamVR_Events.Initialized.Remove(OnInitialize);
        }

        private void OnInitialize(bool initialized)
        {
            Refresh();
        }

        private void Refresh()
        {
            var currentController = ActionsControllerBindingsLoader.CurrentBindingsController;
            if (currentController == null) return;

            target.SetActive(currentController.CanShowBindings());
        }
    }
}
