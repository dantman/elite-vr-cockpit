using System;
using System.IO;
using UnityEngine;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace EVRC
{
    public class PostBuildExportDLLs : IPostprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPostprocessBuild(BuildReport report)
        {
            var buildDir = Path.GetDirectoryName(report.summary.outputPath);
            var pluginDir = Path.Combine(buildDir, "Elite VR Cockpit_Data", "Plugins");

            foreach (var dllPath in Directory.GetFiles(pluginDir, "*.dll"))
            {
                File.Move(dllPath, Path.Combine(buildDir, Path.GetFileName(dllPath)));
            }
        }
    }
}
