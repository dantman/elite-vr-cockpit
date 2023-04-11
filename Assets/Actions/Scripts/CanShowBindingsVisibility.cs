using UnityEngine;

namespace EVRC
{
    public class CanShowBindingsVisibility : MonoBehaviour
    {
        [Tooltip("The GameObject to enable/disable")]
        public GameObject target;

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            var bindingsController = ActionsControllerBindingsLoader.CurrentBindingsController;
            var canShow = bindingsController != null && bindingsController.CanShowBindings();
            target.SetActive(canShow);
        }
    }
}
