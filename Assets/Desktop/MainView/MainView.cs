using EVRC.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    /// <summary>
    /// Initializes the primary UI Elements of the Desktop UI. Mainly connects the root visual element to sub-components and
    /// registers callbacks, etc. The main functionality of the sub-components should be controlled in separate classes
    /// </summary>
    public class MainView : MonoBehaviour
    {
        private MainViewTabController tabController;
        private ControlButtonViewController controlButtonViewController;

        
        private void OnEnable()
        {
            UIDocument menu = GetComponent<UIDocument>();
            VisualElement root = menu.rootVisualElement;

            // Setup Tab Controls
            tabController = new(root);
            tabController.RegisterTabCallbacks();
        }

        
    }
}
