using System;
using UnityEngine;

namespace EVRC
{
    /**
     * A helper that shows an object when Elite Dangerous is running but not focused
     */
    public class GameFocusVisibility : MonoBehaviour
    {
        [Tooltip("The GameObject to enable/disable")]
        public GameObject target;

        private void OnEnable()
        {
            WindowFocusManager.Create(); // Make sure the focus manager exists
            EDStateManager.EliteDangerousStarted.Listen(OnGameStartedOrStopped);
            EDStateManager.EliteDangerousStopped.Listen(OnGameStartedOrStopped);
            WindowFocusManager.ForegroundWindowProcessChanged.Listen(OnForegroundWindowProcessChanged);
        }

        void OnDisable()
        {
            EDStateManager.EliteDangerousStarted.Remove(OnGameStartedOrStopped);
            EDStateManager.EliteDangerousStopped.Remove(OnGameStartedOrStopped);
            WindowFocusManager.ForegroundWindowProcessChanged.Remove(OnForegroundWindowProcessChanged);
        }

        private void OnGameStartedOrStopped()
        {
            Refresh();
        }

        private void OnForegroundWindowProcessChanged(uint pid)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (!EDStateManager.instance.IsEliteDangerousRunning)
            {
                target.SetActive(false);
            }
            else
            {
                target.SetActive(WindowFocusManager.ForegroundWindowPid != EDStateManager.instance.currentPid);
            }
        }
    }
}
