using EVRC.Core;
using EVRC.Core.Actions;
using EVRC.Core.Overlay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    
    public class AddControlButtonForm : MonoBehaviour
    {

        public ControlButtonAddedEvent controlButtonAddedEvent;
        [SerializeField] UIDocument parentUIDocument;
        public ControlButtonAssetCatalog catalog;
        public SavedGameState savedGameState;
        private VisualElement root; // the root of the whole UI
        private VisualElement target; // the form this will control
        private DropdownField buttonCategoryDropdown;
        private DropdownField controlButtonDropdown;
        private UnityEngine.UIElements.Button addButton;
        private ControlButtonSpawnManager spawnManager;


        private ButtonCategory selectedCategory;
        private ControlButtonAsset selectedControlButton;

        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;
            buttonCategoryDropdown = root.Q<DropdownField>("button-category-dropdown");
            controlButtonDropdown = root.Q<DropdownField>("control-button-dropdown");
            addButton = root.Q<UnityEngine.UIElements.Button>("add-button");

            //Populate the category dropdown
            List<string> enumList = new List<string>(Enum.GetNames(typeof(ButtonCategory)).ToList());
            buttonCategoryDropdown.choices = enumList;

            controlButtonDropdown.choices = new List<string>(); //blank until a category is chosen

            //Register the callbacks
            buttonCategoryDropdown.RegisterValueChangedCallback(OnCategoryChanged);
            controlButtonDropdown.RegisterValueChangedCallback(OnButtonSelectionChanged);
            addButton.RegisterCallback<ClickEvent>(Submit);

            // Initialize a Spawn manager instance
            spawnManager = new ControlButtonSpawnManager(savedGameState);
        }


        public void OnCategoryChanged(ChangeEvent<string> evt)
        {
            selectedCategory = Enum.Parse<ButtonCategory>(evt.newValue);
            controlButtonDropdown.value = "";
            selectedControlButton = null;

            //Populate the assets for that category
            List<string> availableControlButtons = new List<string>();
            availableControlButtons.AddRange(catalog.controlButtons
                .Where(x => x.category == selectedCategory)
                .Select(x => x.name).ToList()
                );

            controlButtonDropdown.choices = availableControlButtons;
        }

        public void OnButtonSelectionChanged(ChangeEvent<string> evt)
        {
            selectedControlButton = catalog.controlButtons.FirstOrDefault(x => x.name == evt.newValue);
           
        }
        
        public void Submit(ClickEvent evt)
        {
            if (selectedControlButton != null)
            {
                // Get the location
                var placePosition = spawnManager.GetSpawnLocation();
                var addedControlButton = new SavedControlButton()
                {
                    type = selectedControlButton.name,
                    overlayTransform = new OverlayTransform()
                    {
                        pos = placePosition,
                        rot = Vector3.zero,
                    }
                };

                // Write the new button to the SavedGameState
                savedGameState.controlButtons.Add(addedControlButton);
                savedGameState.Save();

                controlButtonAddedEvent.Raise(addedControlButton);
            }
        }
    }
}
