using UnityEngine;
using UnityEditor;


namespace EVRC
{
    [CustomEditor(typeof(LogListController))]
    public class LogListControllerEditor : Editor
    {
        private bool showTestingFoldout = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            showTestingFoldout = EditorGUILayout.Foldout(showTestingFoldout, "Testing");

            if (showTestingFoldout)
            {
                if (GUILayout.Button("Info Message"))
                {
                    Debug.Log("Info Message");
                }

                if (GUILayout.Button("Test Warning Message"))
                {
                    Debug.LogWarning("Warning Message");
                }

                if (GUILayout.Button("Test Error Message"))
                {
                    Debug.LogError("Error Message");
                }
            }

            EditorGUILayout.Space();
        }
    }
}