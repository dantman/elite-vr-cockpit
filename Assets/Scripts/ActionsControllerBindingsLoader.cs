using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Events = SteamVR_Events;
    using Hand = ActionsController.Hand;
    using InputAction = ActionsController.InputAction;
    using NameType = InputBindingNameInfoManager.NameType;
    using TrackpadInterval = ActionsController.TrackpadInterval;

    /**
     * Behaiour that loads the ActionsController SteamVR Input bindings implementation when SteamVR is initialized.
     * This could be updated in the future to enable other bindings implementations instead for other runtimes.
     */
    public class ActionsControllerBindingsLoader : MonoBehaviour, IBindingsController
    {
        public ActionsController_SteamVRInputBindings steamVrInputBindings;

        /**
         * The currently active bindings controller
         */
        public IBindingsController CurrentController
        {
            get
            {
                if (steamVrInputBindings.isActiveAndEnabled)
                {
                    return steamVrInputBindings;
                }

                return null;
            }
        }

        public static IBindingsController CurrentBindingsController
        {
            get
            {
                var loader = FindObjectOfType<ActionsControllerBindingsLoader>();
                if (loader)
                {
                    return loader.CurrentController;
                }

                return null;
            }
        }

        private void OnEnable()
        {
            Events.Initialized.AddListener(OnSteamVRInitialized);

            // Handle the case where SteamVR is already initialized
            if (SteamVR.initializedState == SteamVR.InitializedStates.InitializeSuccess)
            {
                OnSteamVRInitialized(true);
            }
        }

        private void OnDisable()
        {
            Events.Initialized.RemoveListener(OnSteamVRInitialized);
        }

        private void OnSteamVRInitialized(bool initialized)
        {
            steamVrInputBindings.gameObject.SetActive(initialized);
        }

        // IBindingsController forwarding
        public TrackpadInterval GetTrackpadSwipeInterval(Hand hand)
        {
            return CurrentController?.GetTrackpadSwipeInterval(hand) ?? TrackpadInterval.Default;
        }

        public string[] GetBindingNames(InputAction inputAction, NameType nameType)
        {
            return CurrentController?.GetBindingNames(inputAction, nameType) ?? new string[] { };
        }

        public bool CanShowBindings()
        {
            return CurrentController?.CanShowBindings() ?? false;
        }

        public void ShowBindings(BindingsHintCategory hintCategory)
        {
            var controller = CurrentController;
            if (controller != null)
            {
                CurrentController.ShowBindings(hintCategory);
            }
            else
            {
                Debug.LogWarning("Bindings Controller not available");
            }
        }

        public void EditBindings()
        {
            var controller = CurrentController;
            if (controller != null)
            {
                CurrentController.EditBindings();
            }
            else
            {
                Debug.LogWarning("Bindings Controller not available");
            }
        }
    }
}
