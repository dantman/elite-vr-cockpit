using System.Collections;
using System.IO;
using EVRC.Core.Actions;
using UnityEngine;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Read and Write from the various SavedState Files used by the overlay. Not used to interface with
    /// any of the Elite Dangerous files 
    /// </summary>
    public static class OverlayFileUtils
    {
        #region ---------------Load---------------------
        public static OverlayState LoadFromFile()
        {
            var _savedFilePath = Paths.OverlayStatePath;
            if (_savedFilePath != null && File.Exists(_savedFilePath))
            {
                return Load(_savedFilePath);
            }

            Debug.Log($"CockpitState file was not found. Loading a fresh profile. \n Expected filepath: {_savedFilePath}");
            return new OverlayState();
        }

        public static OverlayState LoadFromFile(string filePath)
        {
            if (filePath != null && File.Exists(filePath))
            {
                return Load(filePath);
            }

            Debug.Log($"Could not find the provided path: {filePath} Loading a fresh profile.");
            return new OverlayState();
        }

        private static OverlayState Load(string path)
        {
            Debug.LogFormat("Loading from {0}", path);
            var state = JsonUtility.FromJson<OverlayState>(File.ReadAllText(path));

            if (state.version < OverlayManager.currentFileVersion)
            {
                state = OverlayStateUpgradeManager.UpgradeOverlayStateFile(state);
            }

            return state;
        }
        #endregion


        #region ---------------Save---------------------
        public static void WriteToFile(OverlayState state)
        {
            string savedStatePath = Paths.OverlayStatePath;
            WriteToFile(state, savedStatePath);
        }

        public static void WriteToFile(OverlayState state, string overrideSavePath)
        {
            if (!File.Exists(overrideSavePath))
            {
                Debug.LogWarning($"Path: {overrideSavePath} does not exist. Creating new file.");
            }

            File.WriteAllText(overrideSavePath, JsonUtility.ToJson(state));
            Debug.LogFormat($"OverlayState Saved to {overrideSavePath}");
        }
        #endregion

    }
}