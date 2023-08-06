using EVRC.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class EliteStatusViewController : MonoBehaviour
    {
        [SerializeField] UIDocument parentUIDocument;
        [SerializeField] private EliteDangerousState eliteDangerousState;


        private VisualElement root; // the root of the whole UI
        private Label timestamp;
        private Label guiFocus;
        private Label statusFlags;

        public void OnEnable()
        {
            root = parentUIDocument.rootVisualElement;
            timestamp = root.Q<Label>("timestamp-value");
            guiFocus = root.Q<Label>("guiFocus-value");
            statusFlags = root.Q<Label>("statusFlags-value");

            if (eliteDangerousState == null) 
            {
                Debug.LogError($"Elite State Object is missing on object: {this.name}");
            }
        }
    }
}
