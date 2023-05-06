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

        public static string StartPresetFileName
            => GetStartPresetFileName();

        public static string GetStartPresetFileName()
        {
            string startPreset = File.Exists(StartPresetPath) ? File.ReadAllText(StartPresetPath) : "";
            
            if ((startPreset ?? "").Trim() == "")
                startPreset = "Custom";

            return startPreset;
        }


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

        public static string ControlBindingsPath
            => GetFirstValidControlBindingsPath();

        public static string GetFirstValidControlBindingsPath()
        {
            string returnBindingsPath = "";

            // Seed the path list if empty
            controlBindingsPaths.Add(Path.Combine(CustomBindingsFolder, StartPresetFileName + ".4.0.binds"));
            controlBindingsPaths.Add(Path.Combine(CustomBindingsFolder, StartPresetFileName + ".3.0.binds"));
            controlBindingsPaths.Add(Path.Combine(CustomBindingsFolder, StartPresetFileName + ".2.0.binds"));
            controlBindingsPaths.Add(Path.Combine(CustomBindingsFolder, StartPresetFileName + ".1.8.binds"));
            
            // Find the first valid entry and return it.
            foreach (var bindingsPath in controlBindingsPaths)
            {
                if (File.Exists(bindingsPath))
                {
                    return bindingsPath;
                }
            }

            Debug.LogError("Could not find valid path for controlBindings;");
            return returnBindingsPath;
        }
        
        public static HashSet<string> controlBindingsPaths = new HashSet<string>();

    }  
}
