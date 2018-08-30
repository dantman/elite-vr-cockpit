using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class EditLockButton : BaseButton
    {
        public Texture lockedTexture;
        public Texture unlockedTexture;
        protected CockpitStateController controller;

        override protected void OnEnable()
        {
            base.OnEnable();
            controller = CockpitStateController.instance;
        }

        override protected void Update()
        {
            base.Update();

            // @todo Move this to Refresh and create a EditLockedStateChanged event to listen to
            if (controller.editLocked)
            {
                holoButton.texture = lockedTexture;
            } else
            {
                holoButton.texture = unlockedTexture;
            }
        }

        public override void Activate()
        {
            controller.ToggleEditLocked();
        }

    }
}
