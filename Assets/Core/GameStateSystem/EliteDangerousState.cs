using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Elite Dangerous State"), System.Serializable]
    public class EliteDangerousState : GameState
    {
        public bool running = false;
        public uint processId;
        public string processName;
        public string processDirectory;
        public EDStatusFlags statusFlags;
        public EDGuiFocus guiFocus;
        

        public EDStatus lastStatusFromFile;
        
        public void Clear()
        {
            running = false;
            processId = 0;
            processName = null;
            processDirectory = null;    
            statusFlags= 0;
            guiFocus = EDGuiFocus.NoFocus;

        // Struct for items directly read from Status.json
        lastStatusFromFile.timestamp = null;
            lastStatusFromFile.Flags = 0;
            lastStatusFromFile.Pips = new byte[] { };
            lastStatusFromFile.FireGroup = 0;
            lastStatusFromFile.GuiFocus = 0;
        }

        public override string GetStatusText()
        {
            return running ? "Running" : "Not Running";
        }

        void OnDisable()
        {
            Clear();
        }
    }

   
}