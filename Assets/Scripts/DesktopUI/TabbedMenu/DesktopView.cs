using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.UIElements;

namespace EVRC
{
    // This script attaches the tabbed menu logic to the game.
    //Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
    public class DesktopView : MonoBehaviour
    {
        private TabbedMenuController controller;

        [Header("Tab Page Controllers")]
        public LogListController logListController;

        private void OnEnable()
        {
            UIDocument menu = GetComponent<UIDocument>();
            VisualElement root = menu.rootVisualElement;

            // Setup Tab Controls
            controller = new(root);
            controller.RegisterTabCallbacks();           
            
        }

        
    }
}
