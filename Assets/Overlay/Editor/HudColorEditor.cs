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
        private SerializedProperty unavailableColorProp;
        private SerializedProperty notYetConfiguredColor1Prop;
        private SerializedProperty notYetConfiguredColor2Prop;

        private bool matrixFoldout = false;
        private bool unusedColorFoldout = false;

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
            unavailableColorProp = serializedObject.FindProperty("unavailableColor");
            notYetConfiguredColor1Prop = serializedObject.FindProperty("notYetConfiguredColor1");
            notYetConfiguredColor2Prop = serializedObject.FindProperty("notYetConfiguredColor2");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
        
            EditorGUILayout.PropertyField(changeEvent);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();            

            #region ------------------ Color Display Section ------------------
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            // Color 1
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Base Color");
            baseColorProp.colorValue = EditorGUILayout.ColorField(GUIContent.none, baseColorProp.colorValue, false, true, false);
            EditorGUILayout.EndHorizontal();

            // Color 2
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Highlight Color");
            highlightColorProp.colorValue = EditorGUILayout.ColorField(GUIContent.none, highlightColorProp.colorValue, false, true, false);
            EditorGUILayout.EndHorizontal();

            // Color 3
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Invalid Color");
            invalidColorProp.colorValue = EditorGUILayout.ColorField(GUIContent.none, invalidColorProp.colorValue, false, true, false);
            EditorGUILayout.EndHorizontal();

            // Color 4
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Unavailable Color");
            unavailableColorProp.colorValue = EditorGUILayout.ColorField(GUIContent.none, unavailableColorProp.colorValue, false, true, false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            #region ----------- (Currently) Unused Colors ------------
            unusedColorFoldout = EditorGUILayout.Foldout(unusedColorFoldout, "Display Unused Colors");
            if (unusedColorFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Box(GUIContent.none, GUILayout.Height(2), GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField("These colors are not in use yet. They must be added to the IColorable objects in order for them to be used.", EditorStyles.wordWrappedLabel);
                EditorGUILayout.Space();

                // Color 5
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Not Yet Configured Color 1");
                notYetConfiguredColor1Prop.colorValue = EditorGUILayout.ColorField(GUIContent.none, notYetConfiguredColor1Prop.colorValue, false, true, false);
                EditorGUILayout.EndHorizontal();

                // Color 6
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Not Yet Configured Color 2");
                notYetConfiguredColor2Prop.colorValue = EditorGUILayout.ColorField(GUIContent.none, notYetConfiguredColor2Prop.colorValue, false, true, false);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            #endregion
            #endregion

            #region ---------- File Color Matrix -----------
            matrixFoldout = EditorGUILayout.Foldout(matrixFoldout, "Display Color Matrix from File");
            if (matrixFoldout)
            {

                EditorGUILayout.LabelField("Edit the Matrix File to change the values below. https://elite-dangerous.fandom.com/wiki/HUD_Color_Editor#Instructions", EditorStyles.wordWrappedMiniLabel);
                EditorGUILayout.Space();
                DisplayColorRow(rProp, "Red");
                DisplayColorRow(gProp, "Green");
                DisplayColorRow(bProp, "Blue");

                float[] rArray = rProp.arraySize == 3 ? new[] { rProp.GetArrayElementAtIndex(0).floatValue, rProp.GetArrayElementAtIndex(1).floatValue, rProp.GetArrayElementAtIndex(2).floatValue } : new float[3];
                float[] gArray = gProp.arraySize == 3 ? new[] { gProp.GetArrayElementAtIndex(0).floatValue, gProp.GetArrayElementAtIndex(1).floatValue, gProp.GetArrayElementAtIndex(2).floatValue } : new float[3];
                float[] bArray = bProp.arraySize == 3 ? new[] { bProp.GetArrayElementAtIndex(0).floatValue, bProp.GetArrayElementAtIndex(1).floatValue, bProp.GetArrayElementAtIndex(2).floatValue } : new float[3];

                // Calculate the color based on the RGB values
                var color = new Color(rArray[0], gArray[1], bArray[2]);
            }
            #endregion

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
            EditorGUILayout.LabelField(prop.GetArrayElementAtIndex(0).floatValue.ToString(), EditorStyles.centeredGreyMiniLabel, GUILayout.Width(Screen.width * FIELD_WIDTH));
            EditorGUILayout.LabelField(prop.GetArrayElementAtIndex(1).floatValue.ToString(), EditorStyles.centeredGreyMiniLabel, GUILayout.Width(Screen.width * FIELD_WIDTH));
            EditorGUILayout.LabelField(prop.GetArrayElementAtIndex(2).floatValue.ToString(), EditorStyles.centeredGreyMiniLabel, GUILayout.Width(Screen.width * FIELD_WIDTH));
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
