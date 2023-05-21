using System.Collections.Generic;
using EVRC.Core;
using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    /// <summary>
    /// Displays the controlButtons (holographic) that are currently configured and provides an interface to add/remove buttons
    /// </summary>
    public class ControlButtonViewController : MonoBehaviour
    {
        [SerializeField] UIDocument parentUIDocument;
        [SerializeField] VisualTreeAsset controlButtonEntryTemplate;
        public SavedGameState savedState;

        [Tooltip("Name of the visual element that will hold the list of controlButtons")]
        public string targetParentName = "ControlButtonsState";
        public ControlButtonAssetCatalog controlButtonCatalog;
        public GameEvent controlButtonRemovedEvent;

        private VisualElement root; // the root of the whole UI
        private Dictionary<ButtonCategory, ControlButtonList> controlButtonLists;
        
        // the anchor object that all of the lists will go inside of
        private ScrollView controlListContainer;
        

        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;
            controlButtonLists = new Dictionary<ButtonCategory, ControlButtonList>();
            controlListContainer = root.Q<ScrollView>("control-list-container");

            savedState.Load();
            if (savedState.controlButtons != null)
            {
                DisplayControlButtons(savedState.controlButtons);
            }
        }

        public void DisplayControlButtons(List<SavedControlButton> controlButtons)
        {
            foreach (SavedControlButton item in controlButtons)
            {
                AddControlButton(item);
            }
        }

        public void AddControlButton(SavedControlButton addedControlButton)
        {
            // use the "type" to search for a matching controlButtonAsset
            string type = addedControlButton.type;
            ControlButtonAsset controlButtonAsset = controlButtonCatalog.GetByName(type);

            // Get the Button Category
            ButtonCategory cat = controlButtonAsset.category;

            // Check if a ListView exists for the item's category
            if (!controlButtonLists.ContainsKey(cat))
            {
                // Doesn't exist, create a new ControlButtonList
                var newList = new ControlButtonList(cat.ToString(), controlButtonEntryTemplate, savedState, controlButtonCatalog, controlButtonRemovedEvent);

                // Add it to the list of ControlButtonLists
                controlButtonLists.Add(cat, newList);

                // Add the Visual Element to the UI
                controlListContainer.Add(newList.visualElementContainer);
            }

            // Add to source list
            controlButtonLists[cat].Add(addedControlButton);

            
        }

    }
}
