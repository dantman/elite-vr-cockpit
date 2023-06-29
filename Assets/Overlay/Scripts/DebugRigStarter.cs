using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    /// <summary>
    /// Used to enable the Overlay stuff when using the Debug Scene. Launches the correct events to enable the VR Overlay and Start Tracking
    /// </summary>
    public class DebugRigStarter : MonoBehaviour
    {
        public GameEvent StartEvent;

        private void OnEnable()
        {
            if (!StartEvent)
            {
                Debug.LogError("START Game Event must be assigned to DebugRigStarter");
                return;
            }
            StartEvent.Raise();
        }
    }
}
