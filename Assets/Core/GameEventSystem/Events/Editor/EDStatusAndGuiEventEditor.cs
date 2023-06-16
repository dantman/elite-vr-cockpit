using UnityEditor;
using UnityEngine;

namespace EVRC.Core.Editor
{
    [CustomEditor(typeof(EDStatusFlagsEvent))]
    public class StatusFlagsEventEditor : UnityEditor.Editor
    {
        private EDStatusFlags statusFlags;

        private void OnEnable()
        {
            statusFlags = EDStatusFlags.InMainShip;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Status Flags:");
            statusFlags = (EDStatusFlags)EditorGUILayout.EnumFlagsField(statusFlags);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();


            if (GUILayout.Button("Raise"))
            {
                ((EDStatusFlagsEvent)target).Raise(statusFlags);
                Debug.Log($"Raising EdStatusAndGuiEvent with: {statusFlags}");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }

    [CustomEditor(typeof(EDGuiFocusEvent))]
    public class YourScriptableObjectEditor : UnityEditor.Editor
    {
        private EDGuiFocus guiFocus;

        private void OnEnable()
        {
            guiFocus = EDGuiFocus.NoFocus;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ED Gui Focus");
            guiFocus = (EDGuiFocus)EditorGUILayout.EnumPopup(guiFocus);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();


            if (GUILayout.Button("Raise"))
            {
                ((EDGuiFocusEvent)target).Raise(guiFocus);
                Debug.Log($"Raising EdStatusAndGuiEvent with: {guiFocus}");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
