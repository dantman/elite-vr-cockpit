using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EVRC.Core.Actions;
using UnityEngine;
using Valve.Newtonsoft.Json;

namespace EVRC.Core.Overlay
{
    /// <summary>
    /// Read and Write from the various SavedState Files used by the overlay. Not used to interface with
    /// any of the Elite Dangerous files 
    /// </summary>
    public static class OverlayFileUtils
    {
        #region ---------------Load---------------------
        public static SavedStateFile LoadFromFile()
        {
            var _savedFilePath = Paths.OverlayStatePath;
            if (_savedFilePath != null && File.Exists(_savedFilePath))
            {
                return Load(_savedFilePath);
            }

            Debug.Log($"CockpitState file was not found. Loading a fresh profile. \n Expected filepath: {_savedFilePath}");
            return new SavedStateFile(true);
        }

        public static SavedStateFile LoadFromFile(string filePath)
        {
            if (filePath != null && File.Exists(filePath))
            {
                return Load(filePath);
            }

            Debug.Log($"Could not find the provided path: {filePath} Loading a fresh profile.");
            return new SavedStateFile(true);
        }

        private static SavedStateFile Load(string path)
        {
            Debug.LogFormat("Loading from {0}", path);

            var returnState = new SavedStateFile();
            var fileVersion = TryGetSavedStateVersion(path);
            
            
            // If it's not the current file version, start the upgrade process, which will
            // return an updated 
            if (fileVersion < OverlayManager.currentFileVersion)
            {
                Debug.LogWarning($"File version: {fileVersion} is not current. Starting upgrade...");
                OverlayStateUpgradeManager upgradeManager = new OverlayStateUpgradeManager();
                returnState = upgradeManager.UpgradeOverlayStateFile(path, fileVersion);
                return returnState;
            }
        
            // If it's already the right version, Deserialize and return
            returnState = JsonConvert.DeserializeObject<SavedStateFile>(File.ReadAllText(path));
            return returnState;
        }

        public static int TryGetSavedStateVersion(string filePath)
        {
            string json = File.ReadAllText(filePath);
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            
            if (data.TryGetValue("version", out object value))
            {
                int version = ((IConvertible)value).ToInt32(null);
                return version;
            }
            else
            {
                Debug.LogError($"Could not find version in SavedState File: {filePath}. Starting a fresh SavedState file.");
                return OverlayManager.currentFileVersion;
            }

        }

        #endregion


        #region ---------------Save---------------------
        public static void WriteToFile(SavedStateFile state)
        {
            string savedStatePath = Paths.OverlayStatePath;
            WriteToFile(state, savedStatePath);
        }

        public static void WriteToFile(SavedStateFile state, string overrideSavePath)
        {
            if (!File.Exists(overrideSavePath))
            {
                Debug.LogWarning($"Path: {overrideSavePath} does not exist. Creating new file.");
            }

            File.WriteAllText(overrideSavePath, JsonUtility.ToJson(state));
            Debug.LogFormat($"SavedState Saved to {overrideSavePath}");
        }
        #endregion

    }
}