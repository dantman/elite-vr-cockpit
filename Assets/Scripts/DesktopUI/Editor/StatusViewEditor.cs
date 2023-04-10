using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace EVRC
{
    [CustomEditor(typeof(StatusView))]
    public class StatusViewEditor : Editor
    {
        private SerializedObject serializedStatusView;
        private SerializedProperty gameStateProperty;
        private SerializedProperty selectedStringFieldProperty;
        private SerializedProperty selectedValueLabelProperty;
        private SerializedProperty uiDocumentProperty;

        private void OnEnable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            serializedStatusView = new SerializedObject(target);
            uiDocumentProperty = serializedStatusView.FindProperty("uiDocument");
            selectedValueLabelProperty = serializedStatusView.FindProperty("statusUxmlLabelName");

            serializedStatusView.Update();
            DrawDefaultInspector();

            UIDocument uIDocument = uiDocumentProperty.objectReferenceValue as UIDocument;
            if(uiDocumentProperty != null)
            {
                var labels = uIDocument.rootVisualElement.Query<Label>().ToList();
                List<string> _labelNames = new List<string>();
                foreach (var label in labels)
                {
                    if (label.ClassListContains("statusValue"))
                    {
                        _labelNames.Add(label.name);
                    }
                }

                if (_labelNames.Count == 0)
                {
                    EditorGUILayout.HelpBox("No labels found with the 'statusValue' style class.", MessageType.Info);
                    return;
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(new GUIContent("UI Element for Status", "This is the UI Element where the status text will be displayed. This list only shows elements with the USS style class 'statusValue'"));
                int selectedIndex = EditorGUILayout.Popup(
                    GetSelectedValueLabelIndex(_labelNames.ToArray()), _labelNames.ToArray());
                selectedValueLabelProperty.stringValue = _labelNames[selectedIndex];
                EditorGUILayout.EndHorizontal();
            }

            serializedStatusView.ApplyModifiedProperties();
        }

        private int GetSelectedValueLabelIndex(string[] labelNames)
        {
            string selectedValueLabel = selectedValueLabelProperty.stringValue;

            for (int i = 0; i < labelNames.Length; i++)
            {
                if (labelNames[i] == selectedValueLabel)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}