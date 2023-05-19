using System;
using EVRC.Core.Overlay;
using UnityEngine;
using UnityEngine.UI;

namespace EVRC.Core.Actions
{
    using CockpitMode = CockpitUIMode.CockpitMode;

    [RequireComponent(typeof(Button))]
    public class ControlButtonUIButtonHandler : MonoBehaviour
    {
        public ControlButtonAsset controlButton;
        public SpawnZone spawnZone;
        private Button button;

        private void OnEnable()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(OnClick);
            button.interactable = IsVisible(CockpitUIMode.Mode, controlButton.category);

            CockpitUIMode.ModeChanged.Listen(OnCockpitUIModeChanged);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnClick);
            CockpitUIMode.ModeChanged.Remove(OnCockpitUIModeChanged);
        }

        private void OnCockpitUIModeChanged(CockpitMode mode)
        {
            button.interactable = IsVisible(mode, controlButton.category);
        }

        public void OnClick()
        {
            // var button = ControlButtonManager.InstantiateControlButton(controlButton);
            if (button == null) return;
            if (!spawnZone.Spawn(button.gameObject))
            {
                Debug.LogWarningFormat("Failed to spawn {0}, destroying", button.name);
                Destroy(button.gameObject);
            }
        }

        private bool IsVisible(CockpitMode mode, ButtonCategory category)
        {
            switch (category)
            {
                case ButtonCategory.Cockpit:
                    return mode.HasFlag(CockpitMode.Cockpit);
                case ButtonCategory.ShipCockpit:
                    return mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InShip);
                case ButtonCategory.MainShipCockpit:
                    return mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InMainShip);
                case ButtonCategory.FighterCockpit:
                    return mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InFighter);
                case ButtonCategory.SRVCockpit:
                    return mode.HasFlag(CockpitMode.Cockpit) && mode.HasFlag(CockpitMode.InSRV);
                default:
                    throw new Exception("Unknown CockpitMode");
            }
        }
    }
}
