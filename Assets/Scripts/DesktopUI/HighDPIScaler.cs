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
            // Scale the UI so it is still readable on HighDPI/4K screens
            scaler.scaleFactor = Mathf.Round(Screen.dpi / 96f);
        }
    }
}
