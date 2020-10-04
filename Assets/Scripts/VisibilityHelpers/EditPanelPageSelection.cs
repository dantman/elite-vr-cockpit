using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EVRC
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
            CockpitStateController.MenuModeStateChanged.Listen(OnMenuModeStateChanged);
        }

        private void OnDisable()
        {
            CockpitStateController.MenuModeStateChanged.Remove(OnMenuModeStateChanged);
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
