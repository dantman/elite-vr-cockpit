using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using UnityEngine.UIElements;

namespace EVRC.DesktopUI
{
    // This script attaches the tabbed menu logic to the game.
    public class MainView : MonoBehaviour
    {
        private MainViewTabController controller;

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
