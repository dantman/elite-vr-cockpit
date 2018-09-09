using UnityEngine;
using UnityEngine.UI;

namespace EVRC.DesktopUI
{
    [RequireComponent(typeof(CanvasScaler))]
    public class HighDPIScaler : MonoBehaviour
    {
        private void OnEnable()
        {
            var scaler = GetComponent<CanvasScaler>();
#if UNITY_EDITOR
            // The Unity Editor's game view already sets a 3x scale factor so don't scale further
            scaler.scaleFactor = 1;
#else
            // Scale the UI so it is still readable on HighDPI/4K screens
            scaler.scaleFactor = Mathf.Round(Screen.dpi / 96f);
#endif
        }
    }
}
