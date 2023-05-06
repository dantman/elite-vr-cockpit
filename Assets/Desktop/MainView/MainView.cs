using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    // This script attaches the tabbed menu logic to the game.
    public class MainView : MonoBehaviour
    {
        private MainViewTabController tabController;
        private SettingsViewController settingsViewController;

        private void OnEnable()
        {
            UIDocument menu = GetComponent<UIDocument>();
            VisualElement root = menu.rootVisualElement;

            // Setup Tab Controls
            tabController = new(root);
            tabController.RegisterTabCallbacks();


            // Set up Setting Bindings
            settingsViewController = GetComponentInChildren<SettingsViewController>();
            settingsViewController.root = root;
            settingsViewController.RegisterCallbacks();

        }

        
    }
}
