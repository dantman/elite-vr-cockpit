using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace EVRC.Core
{
    public static class Paths
    {
        private static string _appDataPath;
        public static string AppDataPath
        {
            get
            {
                if (_appDataPath == null)
                {
                    var LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    _appDataPath = Path.Combine(LocalAppData, "Frontier Developments", "Elite Dangerous");
                }

                return _appDataPath;
            }
        }

        private static string _saveDataPath;

        public static string SaveDataPath
        {
            get
            {
                if (_saveDataPath == null)
                {
                    var savedGamesDir = WindowsUtilities.GetKnownFolderPath(WindowsUtilities.KnownFolderId.SavedGames, WindowsUtilities.KnownFolderFlag.DONT_VERIFY);
                    _saveDataPath = Path.Combine(savedGamesDir, "Frontier Developments", "Elite Dangerous");
                }

                return _saveDataPath;
            }
        }

        public static string GraphicsConfigurationOverridePath
            => Path.Combine(AppDataPath, "Options", "Graphics", "GraphicsConfigurationOverride.xml");
        public static string CustomBindingsFolder
            => Path.Combine(AppDataPath, "Options", "Bindings");
        public static string StartPresetPath
            => Path.Combine(CustomBindingsFolder, "StartPreset.start");
        public static string StatusFilePath
            => Path.Combine(SaveDataPath, "Status.json");

        public static string OverlayStatePath
            => CockpitStatePath();

        public static string CockpitStatePath()
        {
            string overlayStateFilePath = Path.Combine(Application.persistentDataPath, "SavedState.json");

            #if UNITY_EDITOR
                overlayStateFilePath = Path.Combine(Application.persistentDataPath, "SavedState_Editor.json");
            #endif

            return overlayStateFilePath;
        }

    }  
}
