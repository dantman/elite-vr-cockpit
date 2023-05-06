using EVRC.Core.Overlay;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EVRC.Core.Editor
{
    [CustomEditor(typeof(HudColor))]
    public class HudColorEditor : UnityEditor.Editor
    {
        private SerializedProperty rProp;
        private SerializedProperty gProp;
        private SerializedProperty bProp;
        private SerializedProperty changeEvent;
        private SerializedProperty baseColorProp;
        private SerializedProperty highlightColorProp;
        private SerializedProperty invalidColorProp;

        private const float LABEL_WIDTH = 0.25f;
        private const float FIELD_WIDTH = 0.20f;

        private void OnEnable()
        {
            rProp = serializedObject.FindProperty("R");
            gProp = serializedObject.FindProperty("G");
            bProp = serializedObject.FindProperty("B");
            changeEvent = serializedObject.FindProperty("hudColorChangedEvent");
            baseColorProp = serializedObject.FindProperty("baseColor");
            highlightColorProp = serializedObject.FindProperty("highlightColor");
            invalidColorProp = serializedObject.FindProperty("invalidColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            EditorGUILayout.PropertyField(changeEvent);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Hud Color Matrix (from override file)");
            DisplayColorRow(rProp, "Red");
            DisplayColorRow(gProp, "Green");
            DisplayColorRow(bProp, "Blue");
        
            float[] rArray = rProp.arraySize == 3 ? new[] { rProp.GetArrayElementAtIndex(0).floatValue, rProp.GetArrayElementAtIndex(1).floatValue, rProp.GetArrayElementAtIndex(2).floatValue } : new float[3];
            float[] gArray = gProp.arraySize == 3 ? new[] { gProp.GetArrayElementAtIndex(0).floatValue, gProp.GetArrayElementAtIndex(1).floatValue, gProp.GetArrayElementAtIndex(2).floatValue } : new float[3];
            float[] bArray = bProp.arraySize == 3 ? new[] { bProp.GetArrayElementAtIndex(0).floatValue, bProp.GetArrayElementAtIndex(1).floatValue, bProp.GetArrayElementAtIndex(2).floatValue } : new float[3];
        
            // Calculate the color based on the RGB values
            var color = new Color(rArray[0], gArray[1], bArray[2]);

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            // Draw the color field using the calculated color
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Color");
            baseColorProp.colorValue = EditorGUILayout.ColorField(GUIContent.none, baseColorProp.colorValue, false, true, false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Highlight Color");
            highlightColorProp.colorValue = EditorGUILayout.ColorField(GUIContent.none, highlightColorProp.colorValue, false, true, false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Invalid Color");
            invalidColorProp.colorValue = EditorGUILayout.ColorField(GUIContent.none, invalidColorProp.colorValue, false, true, false);
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                var ge = changeEvent.objectReferenceValue as GameEvent;
                ge.Raise();
            }
        
            serializedObject.ApplyModifiedProperties();
        }

        private void DisplayColorRow(SerializedProperty prop, string label)
        {
          
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(Screen.width * LABEL_WIDTH));
            prop.GetArrayElementAtIndex(0).floatValue = EditorGUILayout.FloatField(prop.GetArrayElementAtIndex(0).floatValue, GUILayout.Width(Screen.width * FIELD_WIDTH));
            prop.GetArrayElementAtIndex(1).floatValue = EditorGUILayout.FloatField(prop.GetArrayElementAtIndex(1).floatValue, GUILayout.Width(Screen.width * FIELD_WIDTH));
            prop.GetArrayElementAtIndex(2).floatValue = EditorGUILayout.FloatField(prop.GetArrayElementAtIndex(2).floatValue, GUILayout.Width(Screen.width * FIELD_WIDTH));
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
