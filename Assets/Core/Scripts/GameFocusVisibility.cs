using UnityEngine;

namespace EVRC.Core
{
    /**
     * A helper that shows an object when Elite Dangerous is running but not focused
     */
    public class GameFocusVisibility : MonoBehaviour
    {
        public EliteDangerousState eliteDangerousState;

        [Tooltip("The GameObject to enable/disable")]
        public GameObject windowNotFocusedPanel;
        public GameObject gameNotRunningPanel;

        private void OnEnable()
        {
            // Make sure the focus manager exists and give it the same Elite Dangerous State Object
            var windowFocusManager = WindowFocusManager.Create(); 
            windowFocusManager.eliteDangerousState = eliteDangerousState;

            EDStateManager.EliteDangerousStopped.Listen(OnGameStartedOrStopped);
            WindowFocusManager.ForegroundWindowProcessChanged.Listen(OnForegroundWindowProcessChanged);

            Refresh();
        }

        void OnDisable()
        {
            EDStateManager.EliteDangerousStopped.Remove(OnGameStartedOrStopped);
            WindowFocusManager.ForegroundWindowProcessChanged.Remove(OnForegroundWindowProcessChanged);
        }

        public void OnGameStartedOrStopped()
        {
            Refresh();
        }

        public void OnEDStateChanged()
        {
            Refresh();
        }

        private void OnForegroundWindowProcessChanged(uint pid)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (!eliteDangerousState.running)
            {
                windowNotFocusedPanel.SetActive(false);
                gameNotRunningPanel.SetActive(true);
            }
            else
            {
                gameNotRunningPanel.SetActive(false);
                windowNotFocusedPanel.SetActive(WindowFocusManager.ForegroundWindowPid != eliteDangerousState.processId);
            }
        }
    }
}
