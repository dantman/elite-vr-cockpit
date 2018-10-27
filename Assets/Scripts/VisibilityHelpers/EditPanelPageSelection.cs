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
            ButtonsPanel = 1 << 0,
            ControlsPanel = 1 << 1,
            ShipControlsPage = 1 << 2,
            SRVControlsPage = 1 << 3,
        };
        private Dictionary<string, Page> pageNames = new Dictionary<string, Page>
        {
            {"Buttons", Page.ButtonsPanel},
            {"ShipControls", Page.ControlsPanel | Page.ShipControlsPage},
            {"SRVControls", Page.ControlsPanel | Page.SRVControlsPage},
        };

        public GameObject buttonPanel;
        public GameObject controlsPanel;
        public GameObject shipControlsPanel;
        public GameObject srvControlsPanel;
        private Page selectedPage = Page.ButtonsPanel;
        
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
            selectedPage = menuMode ? Page.ControlsPanel | Page.ShipControlsPage : Page.ButtonsPanel;
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
            if (buttonPanel) buttonPanel.SetActive(selectedPage.HasFlag(Page.ButtonsPanel));
            if (controlsPanel) controlsPanel.SetActive(selectedPage.HasFlag(Page.ControlsPanel));
            if (shipControlsPanel) shipControlsPanel.SetActive(selectedPage.HasFlag(Page.ShipControlsPage));
            if (srvControlsPanel) srvControlsPanel.SetActive(selectedPage.HasFlag(Page.SRVControlsPage));
        }
    }
}
