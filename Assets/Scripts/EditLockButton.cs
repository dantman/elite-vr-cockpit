using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class EditLockButton : BaseButton
    {
        public Texture lockedTexture;
        public Texture unlockedTexture;
        protected CockpitStateController controller;

        override protected void Start()
        {
            base.Start();
            controller = CockpitStateController.instance;
        }

        override protected void Update()
        {
            base.Update();

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
