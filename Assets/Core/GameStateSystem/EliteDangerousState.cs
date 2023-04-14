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

        public string timestamp;
        public uint Flags;
        public byte[] Pips;
        public byte FireGroup;
        public byte GuiFocus;

        public void Clear()
        {
            running = false;
            processId = 0;
            processName = null;
            processDirectory = null;
            timestamp = null;
            Flags = 0;
            Pips = new byte[]{};
            FireGroup = 0;
            GuiFocus = 0;
        }

        public override string GetStatusText()
        {
            return running ? "Running" : "Not Running";
        }
    }
}
