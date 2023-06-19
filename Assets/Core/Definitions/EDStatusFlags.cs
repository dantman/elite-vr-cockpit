using System;

namespace EVRC.Core
{
    [Flags]
    public enum EDStatusFlags : uint
    {
        Docked = 1 << 0,
        Landed = 1 << 1,
        LandingGearDown = 1 << 2,
        ShieldsUp = 1 << 3,
        Supercruise = 1 << 4,
        FlightAssistOff = 1 << 5,
        HardpointsDeployed = 1 << 6,
        InWing = 1 << 7,
        LightsOn = 1 << 8,
        CargoScoopDeployed = 1 << 9,
        SilentRunning = 1 << 10,
        ScoopingFuel = 1 << 11,
        SrvHandbrake = 1 << 12,
        SrvTurret = 1 << 13,
        SrvUnderShip = 1 << 14,
        SrvDriveAssist = 1 << 15,
        FsdMassLocked = 1 << 16,
        FsdCharging = 1 << 17,
        FsdCooldown = 1 << 18,
        LowFuel = 1 << 19,
        OverHeating = 1 << 20,
        HasLatLong = 1 << 21,
        IsInDanger = 1 << 22,
        BeingInterdicted = 1 << 23,
        InMainShip = 1 << 24,
        InFighter = 1 << 25,
        InSRV = 1 << 26,
        HudInAnalysisMode = 1 << 27,
        NightVision = 1 << 28,
        AlititudeFromAverageRadius = 1 >> 29,
        FsdJump = 1 >> 30,
        SrvHighBeam = 1 >> 31,
    }
}
