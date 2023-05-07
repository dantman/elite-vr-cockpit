using EVRC.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    // This script attaches the tabbed menu logic to the game.
    public class MainView : MonoBehaviour
    {
        public GameEvent StartOpenVREvent;

        private MainViewTabController tabController;
        private SettingsViewController settingsViewController;
        
        private void OnEnable()
        {
            UIDocument menu = GetComponent<UIDocument>();
            VisualElement root = menu.rootVisualElement;

            // Setup Tab Controls
            tabController = new(root);
            tabController.RegisterTabCallbacks();

            // // Set up Setting Bindings
            // settingsViewController = GetComponentInChildren<SettingsViewController>();
            // settingsViewController.root = root;
            // settingsViewController.RegisterCallbacks();

            // Configure OpenVRButton controls in the sidebar
            // Button button = FindButtonByName(uiDocument, buttonName);
            Button button = root.Q<Button>("OpenVRButton");
            if (button != null)
            {
                // Attach an event listener to the button's onClick event
                button.clicked += StartOpenVREvent.Raise;
            }
            else
            {
                Debug.LogWarning("Open VR Button could not be found in Desktop UI.");
            }

        }

        
    }
}
