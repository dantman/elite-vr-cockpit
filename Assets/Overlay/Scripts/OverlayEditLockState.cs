using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Overlay Edit Lock State"), System.Serializable]
    public class OverlayEditLockState : GameState<bool>
    {

        [SerializeField]
        private bool editLocked = true;

        public bool EditLocked => editLocked;

        /**
         * Toggle the editLocked state
         */
        public bool ToggleEditLocked()
        {
            SetEditLocked(!editLocked);
            return editLocked;
        }

        /**
         * Set the editLocked state
         */
        public void SetEditLocked(bool editLocked)
        {
            if (this.editLocked == editLocked) return;

            this.editLocked = editLocked;
            gameEvent.Raise(editLocked);            
        }

        public override string GetStatusText()
        {
            return editLocked ? "Locked" : "Unlocked";
        }
    }
}