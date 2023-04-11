using UnityEngine;

namespace EVRC
{
    /**
     * A helper that enables/disables an object based on the CockpitStateController's editLocked state
     */
    public class CockpitEditStateVisibility : MonoBehaviour
    {
        [Tooltip("The GameObject to enable/disable")]
        public GameObject target;
        [Tooltip("Should the GameObject be visible when editLocked is true, or visible when editLocked is false")]
        public bool visibleWhenEditLocked = true;

        private void OnEnable()
        {
            target.SetActive(CockpitStateController.instance.editLocked == visibleWhenEditLocked);
            CockpitStateController.EditLockedStateChanged.Listen(OnEditLockedStateChanged);
        }

        private void OnDisable()
        {
            CockpitStateController.EditLockedStateChanged.Remove(OnEditLockedStateChanged);
        }

        private void OnEditLockedStateChanged(bool editLocked)
        {
            target.SetActive(editLocked == visibleWhenEditLocked);
        }
    }
}
