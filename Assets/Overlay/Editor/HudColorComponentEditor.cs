using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Core.Overlay.Editor
{
    [CustomEditor(typeof(HolographicColor))]
    public class HudColorComponentEditor : UnityEditor.Editor
    {
        public GUIStyle myVerticalStyle;
        public bool useCustomHudColor;
        private HudColor hudColor;

        public override void OnInspectorGUI()
        {
            SetCustomStyles();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            // Hud Scriptable Object picker
            EditorGUILayout.Space();
            SerializedProperty hudColorProp = serializedObject.FindProperty("hudColor");
            hudColor = hudColorProp.objectReferenceValue as HudColor;
            EditorGUILayout.PropertyField(hudColorProp, true);
            
            // Custom Bool Checkbox
            EditorGUILayout.Space();
            SerializedProperty useCustomHudBoolProp = serializedObject.FindProperty("useHudColorMatrixOverride");
            useCustomHudColor = useCustomHudBoolProp.boolValue;

            EditorGUILayout.BeginHorizontal();
            useCustomHudColor = EditorGUILayout.ToggleLeft("Use Hud Color Matrix Override", useCustomHudColor);
            EditorGUILayout.EndHorizontal();


            // Color Pickers Section
            EditorGUILayout.Space();
            SerializedProperty colorProp = serializedObject.FindProperty("baseColor");
            SerializedProperty highlightProp = serializedObject.FindProperty("highlightColor");

            if (useCustomHudColor)
            {
                EditorGUILayout.HelpBox("Colors are being modified with the matrix from the GraphicsConfigurationOverride.xml file in the Elite Dangerous folder", MessageType.Info);
            }
            EditorGUILayout.BeginHorizontal();
            VerticalColorPickerDisplay("Base Color", colorProp);
            VerticalColorPickerDisplay("Highlight Color", highlightProp);
            EditorGUILayout.EndHorizontal();


            // Cleanup
            if (EditorGUI.EndChangeCheck())
            {
                useCustomHudBoolProp.boolValue = useCustomHudColor;
                if (hudColor != null && hudColor.hudColorChangedEvent != null)
                {
                    hudColor.hudColorChangedEvent.Raise();
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void VerticalColorPickerDisplay(string label, SerializedProperty prop)
        {
            EditorGUILayout.BeginVertical(myVerticalStyle);
            EditorGUILayout.LabelField(label);
            EditorGUILayout.PropertyField(prop, GUIContent.none);
            EditorGUILayout.EndVertical();
             
        }

        private void SetCustomStyles()
        {
            myVerticalStyle = new GUIStyle(EditorStyles.helpBox);
            myVerticalStyle.alignment = TextAnchor.MiddleCenter;
            myVerticalStyle.stretchWidth = true;
            myVerticalStyle.fixedWidth = Screen.width * 0.45f;
        }
    }
}
