using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

public class PostBuildExportDLLs : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        var buildDir = Path.GetDirectoryName(report.summary.outputPath);
        var pluginDir = Path.Combine(buildDir, "Elite VR Cockpit_Data", "Plugins");

        foreach (var dllPath in Directory.GetFiles(pluginDir, "*.dll"))
        {
            string overwrote = "";
            var dest = Path.Combine(buildDir, Path.GetFileName(dllPath));
            if (File.Exists(dest))
            {
                overwrote = " (overwrote)";
                File.Delete(dest);
            }
            File.Move(dllPath, dest);
            Debug.LogFormat("Copied to build root {0} {1}", overwrote, Path.GetFileName(dllPath));
        }
    }
}
