using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Elite Dangerous State"), System.Serializable]
    public partial class EliteDangerousState : GameState
    {
        public bool running = false;
        public uint processId;
        public string processName;
        public string processDirectory;
        public EDStatus lastStatus;

        public void Clear()
        {
            running = false;
            processId = 0;
            processName = null;
            processDirectory = null;

            // Struct for items directly read from Status.json
            lastStatus.timestamp = null;
            lastStatus.Flags = 0;
            lastStatus.Pips = new byte[] { };
            lastStatus.FireGroup = 0;
            lastStatus.GuiFocus = 0;
        }

        public override string GetStatusText()
        {
            return running ? "Running" : "Not Running";
        }
    }

   
}