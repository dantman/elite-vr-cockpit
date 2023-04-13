using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EVRC.Core.Actions;
using EVRC.Core.Actions.Assets;
using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core
{
    public class CockpitStateSave : MonoBehaviour
    {
        public GameObject root;
        public MovableSurface metaPanel;
        public MovableSurface shipThrottle;
        public MovableSurface srvThrottle;
        //public MovableSurface sixDofController;
        public MovableSurface shipJoystick;
        public MovableSurface srvJoystick;
        public MovableSurface shipPowerDeliveryPanel;
        public MovableSurface srvPowerDeliveryPanel;
        //public MovableSurface mapPlaneController;
        public ControlButtonAssetCatalog controlButtonCatalog;
        public CockpitSettingsState cockpitSettings;

        [Serializable]
        public struct State
        {
            [Serializable]
            public struct SavedTransform
            {
                public Vector3 pos;
                public Vector3 rot;
            }

            [Serializable]
            public struct StaticLocations
            {
                public SavedTransform metaPanel;
                public SavedTransform shipThrottle;
                public SavedTransform srvThrottle;
                public SavedTransform shipJoystick;
                public SavedTransform srvJoystick;
                public SavedTransform shipPowerDeliveryPanel;
                public SavedTransform srvPowerDeliveryPanel;
                //public SavedTransform sixDofController;
                //public SavedTransform mapPlaneController;
            }

            [Serializable]
            public struct SavedControlButton
            {
                public string type;
                public SavedTransform loc;
            }

            [Serializable]
            public struct SavedBooleanSetting
            {
                public string name;
                public bool value;
            }

            public int version;
            public StaticLocations staticLocations;
            public SavedControlButton[] controlButtons;
            public SavedBooleanSetting[] booleanSettings;
        }

        public class ReadableSettings
        {
            private Dictionary<string, bool> boolSettings = new Dictionary<string, bool>();

            public ReadableSettings() { }

            /**
             * Read the settings from state
             */
            public static ReadableSettings Read(State state)
            {
                var settings = new ReadableSettings();
                if (state.booleanSettings != null)
                {
                    foreach (var boolSetting in state.booleanSettings)
                    {
                        settings.boolSettings[boolSetting.name] = boolSetting.value;
                    }
                }

                return settings;
            }

            /**
             * Write settings to state
             */
            public State Write(State state)
            {
                state.booleanSettings = boolSettings
                    .Select(boolSetting => new State.SavedBooleanSetting
                    {
                        name = boolSetting.Key,
                        value = boolSetting.Value,
                    })
                    .ToArray();

                return state;
            }

            /**
             * Get a boolean setting
             */
            public bool? GetBool(string name)
            {
                if (boolSettings.ContainsKey(name))
                {
                    return boolSettings[name];
                }

                return null;
            }

            /**
             * Set a boolean setting
             */
            public void SetBool(string name, bool value)
            {
                boolSettings[name] = value;
            }
        }

        public static CockpitStateSave _instance;
        public static CockpitStateSave instance
        {
            get
            {
                return OverlayUtils.Singleton(ref _instance, "[CockpitStateSave]", false);
            }
        }

        public static string savedStateFilePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "SavedState.json");
            }
        }

        protected State.SavedTransform SerializeTransform(Transform transform)
        {
            return new State.SavedTransform
            {
                pos = transform.localPosition,
                rot = transform.localEulerAngles,
            };
        }

        public IEnumerable<State.SavedControlButton> ReadControlButtons(IEnumerable<ControlButton> controlButtons)
        {
            foreach (var button in controlButtons)
            {
                yield return new State.SavedControlButton
                {
                    type = button.controlButtonAsset.name,
                    loc = SerializeTransform(button.transform),
                };
            }
        }

        protected void ApplyTransform(Transform transform, State.SavedTransform savedTransform)
        {
            transform.localPosition = savedTransform.pos;
            transform.localRotation = Quaternion.Euler(savedTransform.rot);
        }


        private void AddControlButton(State.SavedControlButton controlButton)
        {
            var controlButtonAsset = controlButtonCatalog.GetByName(controlButton.type);
            var button = ControlButtonManager.instance.AddControlButton(controlButtonAsset);
            ApplyTransform(button.transform, controlButton.loc);
        }

        public State ReadState()
        {
            var state = new State
            {
                version = 4
            };

            state.staticLocations.metaPanel = SerializeTransform(metaPanel.transform);
            state.staticLocations.shipThrottle = SerializeTransform(shipThrottle.transform);
            state.staticLocations.srvThrottle = SerializeTransform(srvThrottle.transform);
            state.staticLocations.shipJoystick = SerializeTransform(shipJoystick.transform);
            state.staticLocations.srvJoystick = SerializeTransform(srvJoystick.transform);
            state.staticLocations.shipPowerDeliveryPanel = SerializeTransform(shipPowerDeliveryPanel.transform);
            state.staticLocations.srvPowerDeliveryPanel = SerializeTransform(srvPowerDeliveryPanel.transform);
            //state.staticLocations.sixDofController = SerializeTransform(sixDofController.transform);
            ////state.staticLocations.mapPlaneController = SerializeTransform(mapPlaneController.transform);

            state.controlButtons = ReadControlButtons(root.GetComponentsInChildren<ControlButton>(true)).ToArray();

            var savedSettings = new ReadableSettings();

            var settings = cockpitSettings.GetSettings();
            savedSettings.SetBool("joystick.enabled", settings.joystickEnabled);
            savedSettings.SetBool("throttle.enabled", settings.throttleEnabled);
            //savedSettings.SetBool("sixDofController.enabled", settings.sixDofControllerEnabled);
            savedSettings.SetBool("powerDistributionPanel.enabled", settings.powerDistributionPanelEnabled);

            state = savedSettings.Write(state);

            return state;
        }

        public void ApplyState(State state)
        {
            ApplyTransform(metaPanel.transform, state.staticLocations.metaPanel);
            ApplyTransform(shipThrottle.transform, state.staticLocations.shipThrottle);
            ApplyTransform(srvThrottle.transform, state.staticLocations.srvThrottle);
            ApplyTransform(shipJoystick.transform, state.staticLocations.shipJoystick);
            ApplyTransform(srvJoystick.transform, state.staticLocations.srvJoystick);
            if (state.version >= 1)
            {
                //ApplyTransform(sixDofController.transform, state.staticLocations.sixDofController);
            }
            if (state.version >= 2)
            {
                ////ApplyTransform(mapPlaneController.transform, state.staticLocations.mapPlaneController);
            }
            if (state.version >= 3)
            {
                ApplyTransform(shipPowerDeliveryPanel.transform, state.staticLocations.shipPowerDeliveryPanel);
                ApplyTransform(srvPowerDeliveryPanel.transform, state.staticLocations.srvPowerDeliveryPanel);
            }

            foreach (var controlButton in state.controlButtons)
            {
                AddControlButton(controlButton);
            }

            var savedSettings = ReadableSettings.Read(state);
            cockpitSettings.ChangeSettings(settings =>
            {
                settings.joystickEnabled = savedSettings.GetBool("joystick.enabled").GetValueOrDefault(settings.joystickEnabled);
                settings.throttleEnabled = savedSettings.GetBool("throttle.enabled").GetValueOrDefault(settings.throttleEnabled);
                //settings.sixDofControllerEnabled = savedSettings.GetBool("sixDofController.enabled").GetValueOrDefault(settings.sixDofControllerEnabled);
                settings.powerDistributionPanelEnabled = savedSettings.GetBool("powerDistributionPanel.enabled").GetValueOrDefault(settings.powerDistributionPanelEnabled);
            });
        }

        public void Load()
        {
            var path = savedStateFilePath;
            if (File.Exists(path))
            {
                Debug.LogFormat("Loading from {0}", path);
                var state = JsonUtility.FromJson<State>(File.ReadAllText(path));
                ApplyState(state);
            }
        }

        public static void Save()
        {
            var cockpitStateSave = instance;
            if (cockpitStateSave == null)
            {
                Debug.LogWarning("Save failed");
                return;
            }

            var path = savedStateFilePath;
            var state = cockpitStateSave.ReadState();
            File.WriteAllText(path, JsonUtility.ToJson(state));
            Debug.LogFormat("Saved to {0}", path);
        }

        private void Awake()
        {
            StartCoroutine(DelayedLoad());
        }

        private IEnumerator DelayedLoad()
        {
            yield return null;
            Load();
        }
    }
}
