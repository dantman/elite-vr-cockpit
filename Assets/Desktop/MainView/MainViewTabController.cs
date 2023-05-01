using System.Diagnostics;
using System.Linq;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    // This script defines the tab selection logic.

    public class MainViewTabController
    {
        /* Define member variables*/
        private const string tabClassName = "tab";
        private const string currentlySelectedTabClassName = "currentlySelectedTab";
        private const string unselectedContentClassName = "unselectedContent";
        // Tab and tab content have the same prefix but different suffix
        // Define the suffix of the tab name
        private const string tabNameSuffix = "Tab";
        // Define the suffix of the tab content name
        private const string contentNameSuffix = "Content";

        private readonly VisualElement root;

        public MainViewTabController(VisualElement root)
        {
            this.root = root;
        }

        public void RegisterTabCallbacks()
        {
            UQueryBuilder<Label> tabs = GetAllTabs();
            tabs.ForEach((Label tab) => {
                tab.RegisterCallback<ClickEvent>(TabOnClick);
            });
        }

        /* Method for the tab on-click event: 

           - If it is not selected, find other tabs that are selected, unselect them 
           - Then select the tab that was clicked on
        */
        private void TabOnClick(ClickEvent evt)
        {
            Label clickedTab = evt.currentTarget as Label;

            // Also check to see if the clicked tab is 
            if (clickedTab.GetClasses().Contains("coming-soon-label"))
            {
                UnityEngine.Debug.LogWarning("This tab is not enabled.");
                return;
            }


            if (!TabIsCurrentlySelected(clickedTab))
            {
                GetAllTabs().Where(
                    (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
                ).ForEach(UnselectTab);
                SelectTab(clickedTab);
            }
        }
        //Method that returns a Boolean indicating whether a tab is currently selected
        private static bool TabIsCurrentlySelected(Label tab)
        {
            return tab.ClassListContains(currentlySelectedTabClassName);
        }

        private UQueryBuilder<Label> GetAllTabs()
        {
            return root.Query<Label>(className: tabClassName);
        }

        /* Method for the selected tab: 
           -  Takes a tab as a parameter and adds the currentlySelectedTab class
           -  Then finds the tab content and removes the unselectedContent class */
        private void SelectTab(Label tab)
        {
            tab.AddToClassList(currentlySelectedTabClassName);
            VisualElement content = FindContent(tab);
            content.RemoveFromClassList(unselectedContentClassName);
        }

        /* Method for the unselected tab: 
           -  Takes a tab as a parameter and removes the currentlySelectedTab class
           -  Then finds the tab content and adds the unselectedContent class */
        private void UnselectTab(Label tab)
        {
            tab.RemoveFromClassList(currentlySelectedTabClassName);
            VisualElement content = FindContent(tab);
            content.AddToClassList(unselectedContentClassName);
        }

        // Method to generate the associated tab content name by for the given tab name
        private static string GenerateContentName(Label tab) =>
            tab.name.Replace(tabNameSuffix, contentNameSuffix);

        // Method that takes a tab as a parameter and returns the associated content element
        private VisualElement FindContent(Label tab)
        {
            return root.Q(GenerateContentName(tab));
        }
    }
}
