using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC
{
    public class StatusView : MonoBehaviour
    {
        // Public fields
        //public GameState_Base gameState;          
        [HideInInspector] public string statusUxmlLabelName; //Custom inspector lets you choose these fields from a dropdown

        // Private fields
        private Label _statusLabel;
        [SerializeField, HideInInspector] private UIDocument uiDocument;

        private void OnValidate()
        {
            uiDocument = GetComponentInParent<UIDocument>();
            
        }

        private void OnEnable()
        {
            VisualElement root = uiDocument.rootVisualElement;

            // Bind Status Values
            _statusLabel = root.Query<Label>(statusUxmlLabelName).First();
            
            if (_statusLabel == null )
            {
                UnityEngine.Debug.LogError("Status Label not found. Check the provided 'status label name' in the inspector");
            }
        }

        public void Refresh()
        {
            //_statusLabel.text = gameState.GetStatusText();
        }
    }
}
