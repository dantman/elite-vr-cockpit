using System;

namespace EVRC.Core
{
        [Flags]
        public enum CockpitMode : ushort
        {
            GameNotRunning = 1 << 0,
            InGame = 1 << 1,
            Map = 1 << 2,
            StationServices = 1 << 3,
            Cockpit = 1 << 4,
            InShip = 1 << 5,
            InSRV = 1 << 6,
            InMainShip = 1 << 7,
            InFighter = 1 << 8,
            FSSMode = 1 << 9,
            DSSMode = 1 << 10,
            CockpitPanel = 1 << 11,
            Landing = 1 << 12,
            MenuMode = 1 << 15,
        }
}