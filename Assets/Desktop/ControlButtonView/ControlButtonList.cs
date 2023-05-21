using EVRC.Core.Actions;
using EVRC.Core;
using EVRC.DesktopUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using System;
using EVRC.Core.Overlay;

namespace EVRC.Desktop
{
    public class ControlButtonList
    {
        public VisualElement visualElementContainer;
        public string listName;
        public List<ControlButtonDesktopItem> sourceList;

        private ListView listView; 
        private VisualTreeAsset m_ControlButtonEntryTemplate;
        private SavedGameState savedGameState;
        private ControlButtonAssetCatalog catalog;
        private GameEvent controlButtonRemovedEvent;


        public ControlButtonList(string category, VisualTreeAsset listEntryTemplate, SavedGameState savedGameStateObject, ControlButtonAssetCatalog mainCatalog, GameEvent removedEvent)
        {
            m_ControlButtonEntryTemplate = listEntryTemplate;
            savedGameState = savedGameStateObject;
            catalog = mainCatalog;
            controlButtonRemovedEvent = removedEvent;

            // Create the ListView
            listView = new ListView();
            listView.name = category;          
            //listView.showAlternatingRowBackgrounds = AlternatingRowBackground.ContentOnly;
            
            // Create a sourceList
            sourceList = new List<ControlButtonDesktopItem>();

            SetListBindingMethods();

            // Create a header label for the ListView
            Label label = new Label();
            label.text = category.ToString();
            label.style.fontSize = 18;

            // Add the category label as the first item in the ListView
            visualElementContainer = new VisualElement();
            visualElementContainer.Add(label);
            visualElementContainer.Add(listView);
        }

        

        private ControlButtonDesktopItem CreateButtonDesktopItem(ControlButtonAsset asset, SavedControlButton savedCtrlBtn)
        {
            string friendlyName = EDControlBindingsUtils.EDControlFriendlyName(asset.GetControl());          

            var newControlButtonDesktopItem = new ControlButtonDesktopItem()
            {
                officialName = asset.GetControl().ToString(),
                name = friendlyName,
                texture = asset.GetTexture(),
                controlButtonAsset = asset,
                savedControlButton = savedCtrlBtn
            };          

            return newControlButtonDesktopItem;
        }


        void SetListBindingMethods()
        {
            //Set up a make item function for a list entry
            listView.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = m_ControlButtonEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                var newListEntryLogic = new ControlButtonDisplay();

                // Assign the controller script to the visual element
                newListEntry.userData = newListEntryLogic;

                // Initialize the controller script
                newListEntryLogic.SetVisualElement(newListEntry);
                newListEntryLogic.SetParentList(this);

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            // Set up bind function for a specific list entry
            listView.bindItem += (item, index) =>
            {
                (item.userData as ControlButtonDisplay).SetButtonData(sourceList[index]);
            };

            listView.onSelectionChange += (IEnumerable<object> selectedItems) =>
            {
                // Hide all (clear out previous)
                var allActionsContainers = listView.Query<VisualElement>("actions-container").ToList();
                foreach (var actionContainer in allActionsContainers)
                {
                    actionContainer.AddToClassList("hide");
                }

                // Show visual elements for selected items
                foreach (var selectedItem in selectedItems)
                {
                    // Get the VisualElement that represents the selected item
                    VisualElement selectedVisualElement = listView.GetRootElementForIndex(sourceList.IndexOf(selectedItem as ControlButtonDesktopItem));
                    if (selectedVisualElement != null)
                    {                        
                        var actionContainer = selectedVisualElement.Q<VisualElement>("actions-container");
                        
                        // unhide just this element
                        actionContainer.RemoveFromClassList("hide");
                    }
                    
                }
            };

            // Set the actual item's source list/array
            listView.itemsSource = sourceList;
        }

        public void Add(SavedControlButton savedControlButton)
        {
            ControlButtonAsset controlButtonAsset = catalog.GetByName(savedControlButton.type);
            var desktopItem = CreateButtonDesktopItem(controlButtonAsset, savedControlButton);
            sourceList.Add(desktopItem);
            listView.Rebuild();
        }

        public void Remove(ControlButtonDesktopItem item)
        {
            sourceList.Remove(item);
            savedGameState.controlButtons.Remove(item.savedControlButton);
            savedGameState.Save();
            listView.Rebuild();
            controlButtonRemovedEvent.Raise();
        }

    }
}
