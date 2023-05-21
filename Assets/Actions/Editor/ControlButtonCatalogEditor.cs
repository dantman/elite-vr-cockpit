using EVRC.Core.Overlay;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EVRC.Core.Actions.Editor
{
    [CustomEditor(typeof(ControlButtonAssetCatalog))]
    public class ControlButtonCatalogEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var catalog = (ControlButtonAssetCatalog)target;

            if (GUILayout.Button("Regenerate List"))
            {
                var files = Directory.GetFiles(Path.Combine(Application.dataPath, "Actions", "Assets","ControlButtons"), "*.asset");
                
                Debug.Log(Path.Combine(Application.dataPath, "Actions", "Assets", "ControlButtons"));
                catalog.controlButtons = files
                    .Select(path =>
                    {
                        path = "Assets/Actions/Assets/ControlButtons/" + Path.GetFileName(path);
                        return AssetDatabase.LoadAssetAtPath<ControlButtonAsset>(path);
                    })
                    .Where(controlButton => controlButton != null)
                    .OrderBy(controlButton => controlButton.name)
                    .ToArray();

                if (catalog.controlButtons.Length < 1)
                {
                    Debug.LogError("ControlButton Assets not found");
                }

                EditorUtility.SetDirty(catalog);
                AssetDatabase.SaveAssets();
                Debug.Log("ControlButton Catalog list regenerated.");
            }
        }
    }
}
