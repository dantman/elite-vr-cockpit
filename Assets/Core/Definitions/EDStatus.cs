using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core
{
    /*
     * Struct to hold values that come directly from the Status.json file
     *
     * Anything else should be in the main class: EliteDangerousState
     */
    public struct EDStatus
    {
        public string timestamp;
        public uint Flags;
        public byte[] Pips;
        public byte FireGroup;
        public byte GuiFocus;
    }

}
