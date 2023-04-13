using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using EVRC.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class StatusView : MonoBehaviour
    {
        // Public fields
        public GameState gameState;
        [HideInInspector] public string statusUxmlLabelName; //Custom inspector lets you choose these fields from a dropdown

        // Private fields
        private Label _statusLabel;
        [SerializeField] private UIDocument uiDocument;

        private void OnValidate()
        {
            uiDocument = GetComponentInParent<UIDocument>();


        }

        private void OnEnablePreChecks()
        {
            if (uiDocument == null)
            {
                Debug.LogWarning("UIDocument not found in parent hierarchy.");
            }
        }

        private void OnEnable()
        {
            OnEnablePreChecks();

            VisualElement root = uiDocument.rootVisualElement;

            // Bind Status Values
            _statusLabel = root.Query<Label>(statusUxmlLabelName).First();

            OnEnablePostChecks();
        }

        private void OnEnablePostChecks()
        {
            if (_statusLabel == null )
            {
                UnityEngine.Debug.LogError("Status Label not found in UI Document. Check the provided 'status Uxml label name' in the inspector");
            }

        }

        public void Refresh()
        {
            _statusLabel.text = gameState.GetStatusText();
        }
    }
}
