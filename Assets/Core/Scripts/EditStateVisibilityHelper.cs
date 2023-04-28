using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core
{
    /**
     * A helper that enables/disables an object based on the editLocked state
     */
    public class EditStateVisibilityHelper : MonoBehaviour
    {
        public OverlayEditLockState editLockState;
        [Tooltip("The GameObject to enable/disable")]
        public GameObject target;
        [Tooltip("Should the GameObject be visible when editLocked is true, or visible when editLocked is false")]
        public bool visibleWhenEditLocked = false;


        private void OnEnable()
        {
            target.SetActive(editLockState.EditLocked == visibleWhenEditLocked);
            editLockState.gameEvent.Event += OnEditLockedStateChanged;
        }

        private void OnDisable()
        {
            editLockState.gameEvent.Event -= OnEditLockedStateChanged;
        }

        private void OnEditLockedStateChanged(bool editLocked)
        {
            target.SetActive(editLocked == visibleWhenEditLocked);
        }
    }
}
