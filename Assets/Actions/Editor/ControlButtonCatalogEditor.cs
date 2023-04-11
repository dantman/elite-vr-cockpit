using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace EVRC
{
    [CustomEditor(typeof(ControlButtonAssetCatalog))]
    public class ControlButtonCatalogEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var catalog = (ControlButtonAssetCatalog)target;

            if (GUILayout.Button("Regenerate List"))
            {
                var files = Directory.GetFiles(Application.dataPath + "/ControlButtons", "*.asset");
                catalog.controlButtons = files
                    .Select(path =>
                    {
                        path = "Assets/ControlButtons/" + Path.GetFileName(path);
                        return AssetDatabase.LoadAssetAtPath<ControlButtonAsset>(path);
                    })
                    .Where(controlButton => controlButton != null)
                    .OrderBy(controlButton => controlButton.name)
                    .ToArray();

                EditorUtility.SetDirty(catalog);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
