using EVRC.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Desktop
{
    public class StartStopButton : MonoBehaviour
    {
        public GameEvent StartOpenVREvent;
        public GameEvent StopOpenVREvent;
        public OpenVrState openVrState;

        private VisualElement root;
        private Button button;
        private IMGUIContainer icon;
        private bool running = false;

        [Header("GUI Settings")]
        public string startButtonText;
        public string stopButtonText;



        private void OnEnable()
        {
            UIDocument menu = GetComponent<UIDocument>();
            root = menu.rootVisualElement;

            // button and icon references
            button = root.Q<Button>("OpenVRButton");
            icon = root.Q<IMGUIContainer>("button-icon");

            // Attach an event listener to the button's onClick event
            button.clicked += OnButtonClick;

        }

        private void OnButtonClick()
        {

            if (running)
            {
                ApplyStoppedStyle();

                // Send stopped event
                StopOpenVREvent.Raise();
                running = false;
            } 
            else
            {
                ApplyRunningStyle();

                StartOpenVREvent.Raise();
                running = true;
            }
            
        }

        /// <summary>
        /// Style for when OpenVR is running
        /// </summary>
        private void ApplyRunningStyle()
        {
            button.RemoveFromClassList("startButtonBorder");            
            icon.RemoveFromClassList("startIcon");            

            button.AddToClassList("stopButtonBorder");
            icon.AddToClassList("stopIcon");
            button.text = stopButtonText;
        }

        /// <summary>
        /// Style for when OpenVR is stopped
        /// </summary>
        private void ApplyStoppedStyle()
        {           
            button.RemoveFromClassList("stopButtonBorder");
            icon.RemoveFromClassList("stopIcon");

            button.AddToClassList("startButtonBorder");
            icon.AddToClassList("startIcon");

            button.text = startButtonText;
        }
    }
}
