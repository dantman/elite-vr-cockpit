using UnityEngine;

namespace EVRC.DesktopUI
{
    /**
     * Behaviour used on a UI Button to open the Controller Bindings interface
     */
    public class EditControllerBindingsButton : MonoBehaviour
    {
        public void Activate()
        {
            ActionsControllerBindingsLoader.CurrentBindingsController?.EditBindings();
        }
    }
}
