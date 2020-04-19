using System;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    /**
     * Controls the current page selection in the edit panel and visibility of the pages and buttons
     */
    public class EditPanelPageSelection : MonoBehaviour
    {
        [Flags]
        public enum Page
        {
            ButtonPanel,
            ControlsPanel,
        };
        private Dictionary<string, Page> pageNames = new Dictionary<string, Page>
        {
            {"Buttons", Page.ButtonPanel},
            {"ControlsPanel", Page.ControlsPanel},
        };

        public GameObject buttonPanel;
        public GameObject controlsPanel;
        private Page selectedPage = Page.ButtonPanel;
        
        private void OnEnable()
        {
            CockpitStateController.MenuModeStateChanged.Listen(OnMenuModeStateChanged);
            Refresh();
        }

        private void OnDisable()
        {
            CockpitStateController.MenuModeStateChanged.Remove(OnMenuModeStateChanged);
        }

        private void OnMenuModeStateChanged(bool menuMode)
        {
            // Auto switch tabs
            selectedPage = menuMode ? Page.ControlsPanel : Page.ButtonPanel;
            Refresh();
        }

        public void SetSelectedPage(Page page)
        {
            selectedPage = page;
            Refresh();
        }

        public void SetSelectedPage(string page)
        {
            if (pageNames.ContainsKey(page))
            {
                SetSelectedPage(pageNames[page]);
            } else
            {
                Debug.LogErrorFormat("Unknown page name: {0}", page);
            }
        }

        private void Refresh()
        {
            if (buttonPanel) buttonPanel.SetActive(selectedPage == Page.ButtonPanel);
            if (controlsPanel) controlsPanel.SetActive(selectedPage == Page.ControlsPanel);
        }
    }
}
