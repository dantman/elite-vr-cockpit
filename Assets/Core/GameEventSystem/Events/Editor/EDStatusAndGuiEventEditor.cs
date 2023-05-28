using UnityEditor;
using UnityEngine;

namespace EVRC.Core.Editor
{
    [CustomEditor(typeof(EDStatusAndGuiEvent))]
    public class YourScriptableObjectEditor : UnityEditor.Editor
    {
        private SerializedProperty guiFocusProp;
        private EDGuiFocus guiFocus;
        private EDStatusFlags statusFlags;

        private void OnEnable()
        {
            guiFocus = EDGuiFocus.NoFocus;
            statusFlags = EDStatusFlags.InMainShip;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("GUI Focus:");
            guiFocus = (EDGuiFocus)EditorGUILayout.EnumPopup(guiFocus);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Status Flags:");
            statusFlags = (EDStatusFlags)EditorGUILayout.EnumFlagsField(statusFlags);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();


            if (GUILayout.Button("Raise"))
            {
                ((EDStatusAndGuiEvent)target).Raise(statusFlags,guiFocus);
                Debug.Log($"Raising EdStatusAndGuiEvent with: {statusFlags} | {guiFocus}");
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
