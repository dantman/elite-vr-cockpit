namespace EVRC.Core
{
    /// <summary>
    /// Value indicates the GUI screen that is focused/selected. This is a single value, not a flag, and more than one item cannot be flagged at once.
    /// </summary>
    public enum EDGuiFocus : byte
    {
        NoFocus = 0,
        InternalPanel = 1,
        ExternalPanel = 2,
        CommsPanel = 3,
        RolePanel = 4,
        StationServices = 5,
        GalaxyMap = 6,
        SystemMap = 7,
        Orrery = 8,
        FSSMode = 9,
        SAAMode = 10,
        Codex = 11,
        Unknown = byte.MaxValue
    }
}