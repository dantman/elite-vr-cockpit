using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

namespace EVRC
{
    public class CockpitStateSave : MonoBehaviour
    {
        public MovableSurface metaPanel;
        public MovableSurface shipThrottle;
        public MovableSurface srvThrottle;
        public MovableSurface shipJoystick;
        public MovableSurface srvJoystick;

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
            }

            public StaticLocations staticLocations;

            // @todo Save control button list
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

        protected void ApplyTransform(Transform transform, State.SavedTransform savedTransform)
        {
            transform.localPosition = savedTransform.pos;
            transform.localRotation = Quaternion.Euler(savedTransform.rot);
        }

        public State ReadState()
        {
            var state = new State();

            state.staticLocations.metaPanel = SerializeTransform(metaPanel.transform);
            state.staticLocations.shipThrottle = SerializeTransform(shipThrottle.transform);
            state.staticLocations.srvThrottle = SerializeTransform(srvThrottle.transform);
            state.staticLocations.shipJoystick = SerializeTransform(shipJoystick.transform);
            state.staticLocations.srvJoystick = SerializeTransform(srvJoystick.transform);

            return state;
        }

        public void ApplyState(State state)
        {
            ApplyTransform(metaPanel.transform, state.staticLocations.metaPanel);
            ApplyTransform(shipThrottle.transform, state.staticLocations.shipThrottle);
            ApplyTransform(srvThrottle.transform, state.staticLocations.srvThrottle);
            ApplyTransform(shipJoystick.transform, state.staticLocations.shipJoystick);
            ApplyTransform(srvJoystick.transform, state.staticLocations.srvJoystick);
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
            Load();
        }
    }
}
