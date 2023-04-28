using EVRC.Core.Overlay;
using UnityEngine;
using UnityEngine.UI;

namespace EVRC.Core
{
    /**
     * Controls the current page selection in the edit panel and visibility of the pages and buttons
     */
    public class EditPanelPageSelection : MonoBehaviour
    {
        public Toggle menuOffPreferentialTab;
        public Toggle menuOnPreferentialTab;

        private void OnEnable()
        {
            MenuModeState.MenuModeStateChanged.Listen(OnMenuModeStateChanged);
        }

        private void OnDisable()
        {
            MenuModeState.MenuModeStateChanged.Remove(OnMenuModeStateChanged);
        }

        private void OnMenuModeStateChanged(bool menuMode)
        {
            // Switch tab when mode changes
            Toggle tab = null;
            if (menuMode) tab = menuOnPreferentialTab;
            else tab = menuOffPreferentialTab;

            if (tab)
            {
                SelectTab(tab);
            }
        }

        private static void SelectTab(Toggle tab)
        {
            tab.isOn = true;
        }
    }
}
