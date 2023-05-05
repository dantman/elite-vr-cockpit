using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core
{
    public class ControlBindingsManager : MonoBehaviour
    {
        public ControlBindingsState controlBindingsState;

        private FileSystemWatcher bindsFileWatcher;
        private FileSystemWatcher startPresetFileWatcher;

        private string bindingsFile;
        private string bindingsPath;

        public void Reload()
        {
            LoadControlBindings();
            WatchControlBindings();
        }

        private void SetBindingsFile()
        {
            bindingsFile = Paths.ControlBindingsPath;
            bindingsPath = Path.GetDirectoryName(bindingsFile);
        }
        /// <summary>
        /// Overload for setting a custom path (mostly for testing)
        /// </summary>
        /// <param name="path"></param>
        public void SetBindingsFile(string path)
        {
            bindingsFile = path;
            bindingsPath = Path.GetDirectoryName(bindingsFile);
        }


        /**
         * Read the user's Custom.X.0.binds and parse the control bindings from it
         */
        public void LoadControlBindings()
        {
            if (bindingsFile == null) SetBindingsFile();
            Debug.LogFormat($"Reading keyboard bindings from {bindingsFile}");

            controlBindingsState.buttonBindings.Clear();
            controlBindingsState.buttonBindings = EDControlBindingsUtils.ParseFile(bindingsFile);
            controlBindingsState.ready = true;
        }



        /**
         * Watch for changes to the user's bindings files
         */
        private void WatchControlBindings()
        {
            UnwatchControlBindings();

            Debug.LogFormat($"Watching for changes to control bindings in {bindingsPath}");

            // Watch *.binds
            bindsFileWatcher = new FileSystemWatcher
            {
                Path = bindingsPath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = "*.binds"
            };
            bindsFileWatcher.Created += OnBindsChange;
            bindsFileWatcher.Changed += OnBindsChange;
            bindsFileWatcher.EnableRaisingEvents = true;

            // Watch StartPreset.start
            startPresetFileWatcher = new FileSystemWatcher
            {
                Path = bindingsPath,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(Paths.StartPresetPath)
            };
            startPresetFileWatcher.Created += OnBindsChange;
            startPresetFileWatcher.Changed += OnBindsChange;
            startPresetFileWatcher.EnableRaisingEvents = true;
        }

        /**
         * *.binds file change event
         */
        private void OnBindsChange(object sender, FileSystemEventArgs e)
        {
            LoadControlBindings();
            controlBindingsState.gameEvent.Raise();
        }

        /**
         * Cleanup the bindings file watchers
         */
        private void UnwatchControlBindings()
        {
            if (bindsFileWatcher != null)
            {
                bindsFileWatcher.EnableRaisingEvents = false;
                bindsFileWatcher.Dispose();
                bindsFileWatcher = null;
            }

            if (startPresetFileWatcher == null) return;

            startPresetFileWatcher.EnableRaisingEvents = false;
            startPresetFileWatcher.Dispose();
            startPresetFileWatcher = null;
        }
    }
}