using UnityEngine;

namespace EVRC
{
    public class HelpButton : BaseButton
    {
        public BindingsHintCategory hintCategory = BindingsHintCategory.Default;
        protected override Unpress Activate()
        {
            var bindingsController = ActionsControllerBindingsLoader.CurrentBindingsController;
            if (bindingsController != null)
            {
                bindingsController.ShowBindings(hintCategory);
            }
            return null;
        }
    }
}