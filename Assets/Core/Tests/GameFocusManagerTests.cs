using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC.Core.Tests
{
    public class GameFocusManagerTests : MonoBehaviour
    {
        // GameFocusVisibility
        //  Refresh() => initiated by EliteDangerousStarted or EliteDangerousStopped GameEvents
        //  GameNotFocused stays inactive if eliteDangerousState is not running
        //  GameNotFocused activates if window foregroundWindowPid doesn't match currentProcessId from EDStateManager
        //  GameNotFocused stays inactive if they DO match
    }
}
