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
        public GameObject target;

        private void OnEnable()
        {
            // Make sure the focus manager exists and give it the same Elite Dangerous State Object
            var windowFocusManager = WindowFocusManager.Create(); 
            windowFocusManager.eliteDangerousState = eliteDangerousState;

            EDStateManager.EliteDangerousStopped.Listen(OnGameStartedOrStopped);
            WindowFocusManager.ForegroundWindowProcessChanged.Listen(OnForegroundWindowProcessChanged);
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

        private void OnForegroundWindowProcessChanged(uint pid)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (!eliteDangerousState.running)
            {
                target.SetActive(false);
            }
            else
            {
                target.SetActive(WindowFocusManager.ForegroundWindowPid != eliteDangerousState.processId);
            }
        }
    }
}
