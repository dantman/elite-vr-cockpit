using UnityEngine;
using Valve.VR;

public class InitSteamVRStandalone: MonoBehaviour
{
    public void InititalizeSteamVRStandalone()
    {
        Debug.Log("Initializing SteamVR - Standalone");
        SteamVR.InitializeStandalone(EVRApplicationType.VRApplication_Overlay);
    }
}
