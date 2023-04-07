using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC
{
    // This script attaches the tabbed menu logic to the game.
    //Inherits from class `MonoBehaviour`. This makes it attachable to a game object as a component.
    public class TabbedMenu : MonoBehaviour
    {
        private TabbedMenuController controller;

        private void OnEnable()
        {
            UIDocument menu = GetComponent<UIDocument>();
            VisualElement root = menu.rootVisualElement;

            controller = new(root);

            controller.RegisterTabCallbacks();
        }
    }
}
