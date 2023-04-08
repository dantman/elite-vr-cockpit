using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC
{
    /*
     * Displays the data of a log entry in the UI of the list
     */
    public class LogEntryDisplay
    {
        Label m_LogMessage;
        IMGUIContainer m_LogSprite;
        //This function retrieves a reference to the 
        //character name label inside the UI element.

        public void SetVisualElement(VisualElement visualElement)
        {
            m_LogMessage = visualElement.Q<Label>("log-message");
            m_LogSprite = visualElement.Q<IMGUIContainer>("log-sprite");
        }

        public void SetLogData(LogItem logItem)
        {
            m_LogMessage.text = logItem.message;
            m_LogSprite.style.backgroundImage = logItem.icon.texture;
            m_LogSprite.style.unityBackgroundImageTintColor = logItem.color;
        }
    }
}
