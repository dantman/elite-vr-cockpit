using UnityEngine;

namespace EVRC
{
    /**
     * Helper that opens the bindings panel
     */
    public class EmitEditBindings : MonoBehaviour
    {
        public void Emit()
        {
            var bindingsController = ActionsControllerBindingsLoader.CurrentBindingsController;
            if (bindingsController != null)
            {
                bindingsController.EditBindings();
            }
        }
    }
}
