using UnityEngine;
using UnityEngine.Events;

namespace EVRC.Core.Overlay
{
    [RequireComponent(typeof(BoolEventListener))]
    public class EditLockButton : BaseButton
    {
        public Texture lockedTexture;
        public Texture unlockedTexture;
        public Tooltip tooltip;
        public string lockedSuffix;
        public string unlockedSuffix;
        public OverlayEditLockState editLockState;

        private BoolEventListener boolEventListener;

        void OnValidate()
        {
            boolEventListener = GetComponent<BoolEventListener>();
            if (boolEventListener.Source == null)
            {
                Debug.LogWarning($"{this.gameObject.name} Event Listener not set. Using default settings automatically.");
                boolEventListener.Source = editLockState.gameEvent;
                boolEventListener.Response.AddListener(OnEditLockStateChanged);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Refresh();

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            editLockState.SetEditLocked(true);
        }

        public void OnEditLockStateChanged(bool editLocked)
        {
            Refresh();
        }

        protected override void Refresh()
        {
            base.Refresh();

            if (editLockState.EditLocked)
            {
                buttonImage?.SetTexture(lockedTexture);
                if (tooltip) tooltip.Suffix = lockedSuffix;
            }
            else
            {
                buttonImage?.SetTexture(unlockedTexture);
                if (tooltip) tooltip.Suffix = unlockedSuffix;
            }
        }

        protected override Unpress Activate()
        {
            editLockState.ToggleEditLocked();
            return noop;
        }

    }
}
