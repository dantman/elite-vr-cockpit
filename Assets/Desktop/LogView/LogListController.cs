using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.DesktopUI
{
    /*
     * class for the list of log entries in the main view
     */
    public class LogListController: MonoBehaviour
    {
        [Serializable]
        public struct LogTypeStyle
        {
            public Color color;
            public Sprite icon;
        }

        [Header("Templates and Scene Refs")]
        // UXML template for list entries
        [SerializeField] VisualTreeAsset m_ListEntryTemplate;
        [SerializeField] UIDocument parentUIDocument;

        [Header("Configuration")]
        public LogTypeStyle infoStyle;
        public LogTypeStyle warningStyle;
        public LogTypeStyle errorStyle;
        public int maxLines = 100;

        // UI element references
        ListView m_LogList;
        
        // Logs object
        private List<LogItem> m_AllLogs = new List<LogItem>();

        public void OnEnable()
        {
            VisualElement root = parentUIDocument.rootVisualElement;

            // Store a reference to the log list element
            m_LogList = root.Q<ListView>("log-list");

            SetListBindingMethods();

            // listen for new logs
            Application.logMessageReceived += OnLogMessage;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessage;
        }

        private void OnLogMessage(string text, string stackTrace, LogType type)
        {
            var style = type == LogType.Log ? infoStyle
                : type == LogType.Warning ? warningStyle
                : errorStyle;

            LogItem logItem = new LogItem()
            {
                message = text,
                color = style.color,
                icon = style.icon
            };

            // limit the list size
            if (m_AllLogs.Count >= maxLines)
            {
                m_AllLogs.RemoveAt(0);
            }

            m_AllLogs.Add(logItem);
            m_LogList.RefreshItems();
        }

        void SetListBindingMethods()
        {
            //Set up a make item function for a list entry
            m_LogList.makeItem = () =>
            {
                // Instantiate the UXML template for the entry
                var newListEntry = m_ListEntryTemplate.Instantiate();

                // Instantiate a controller for the data
                var newListEntryLogic = new LogEntryDisplay();

                // Assign the controller script to the visual element
                newListEntry.userData = newListEntryLogic;

                // Initialize the controller script
                newListEntryLogic.SetVisualElement(newListEntry);

                // Return the root of the instantiated visual tree
                return newListEntry;
            };

            // Set up bind function for a specific list entry
            m_LogList.bindItem = (item, index) =>
            {
                (item.userData as LogEntryDisplay).SetLogData(m_AllLogs[index]);
            };

            // Set a fixed item height
            //m_LogList.fixedItemHeight = 45;

            // Set the actual item's source list/array
            m_LogList.itemsSource = m_AllLogs;
        }

    }
}
