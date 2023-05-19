using EVRC.DesktopUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class ControlButtonDisplay
    {
        Label m_Name;
        private IMGUIContainer m_ButtonImage;
        private Button removeButton;
        private Button duplicateButton;
        private ControlButtonList parentList;
        private ControlButtonDesktopItem controlButtonDesktopItem;

        public void SetParentList(ControlButtonList controlButtonList)
        {
            parentList = controlButtonList;
        }

        public void SetVisualElement(VisualElement visualElement)
        {
            m_Name = visualElement.Q<Label>("button-label");
            m_ButtonImage = visualElement.Q<IMGUIContainer>("button-image");
            removeButton = visualElement.Q<Button>("remove-button");
            duplicateButton = visualElement.Q<Button>("duplicate-button");

            removeButton.clickable.clicked += OnRemoveButtonClick;
        }

        public void SetButtonData(ControlButtonDesktopItem controlButton)
        {
            controlButtonDesktopItem = controlButton;

            m_Name.text = controlButton.name;
            m_Name.tooltip = controlButton.officialName;
            m_ButtonImage.style.backgroundImage = (Texture2D)controlButton.texture;
        }

        private void OnRemoveButtonClick()
        {
            parentList.Remove(controlButtonDesktopItem);
        }

    }
}
