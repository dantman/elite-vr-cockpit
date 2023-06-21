using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EVRC.Core.Overlay.Editor
{
    //[CustomEditor(typeof(HolographicColor))]
    //public class HolographicColorEditor : UnityEditor.Editor
    //{
    //    public GUIStyle myVerticalStyle;
    //    public bool useCustomHudColor;
    //    private HudColor hudColor;
    //    private GUIStyle wrappedLabelStyle;

    //    public void OnEnable()
    //    {
    //        wrappedLabelStyle = new GUIStyle(EditorStyles.label);
    //        wrappedLabelStyle.wordWrap = true;
    //    }


    //    public override void OnInspectorGUI()
    //    {
    //        SetCustomStyles();
    //        serializedObject.Update();
    //        EditorGUI.BeginChangeCheck();

    //        // Hud Scriptable Object picker
    //        EditorGUILayout.Space();
    //        SerializedProperty hudColorProp = serializedObject.FindProperty("hudColor");
    //        hudColor = hudColorProp.objectReferenceValue as HudColor;
    //        EditorGUILayout.PropertyField(hudColorProp, true);
            
    //        // Custom Bool Checkbox
    //        EditorGUILayout.Space();
    //        SerializedProperty useCustomHudBoolProp = serializedObject.FindProperty("useHudColorMatrixOverride");
    //        useCustomHudColor = useCustomHudBoolProp.boolValue;

    //        EditorGUILayout.BeginHorizontal();
    //        useCustomHudColor = EditorGUILayout.ToggleLeft("Use Hud Color Matrix Override", useCustomHudColor);
    //        EditorGUILayout.EndHorizontal();


    //        // Color Pickers Section
    //        EditorGUILayout.Space();
    //        SerializedProperty colorProp = serializedObject.FindProperty("baseColor");
    //        SerializedProperty highlightProp = serializedObject.FindProperty("highlightColor");
    //        SerializedProperty invalidColorProp = serializedObject.FindProperty("invalidColor");
    //        SerializedProperty unavailableColorProp = serializedObject.FindProperty("unavailableColor");

    //        if (useCustomHudColor)
    //        {
    //            EditorGUILayout.HelpBox("Colors are being modified with the matrix from the GraphicsConfigurationOverride.xml file in the Elite Dangerous folder", MessageType.Info);
    //        }
            
            
    //        EditorGUILayout.BeginHorizontal();
    //        VerticalColorPickerDisplay("Base", "Used for all HUD Elements by default", colorProp);
    //        VerticalColorPickerDisplay("Highlight", "Used when an element is touching the controller", highlightProp);
    //        VerticalColorPickerDisplay("Invalid", "When an element is mis-configured (i.e. no valid binding)", invalidColorProp);
    //        EditorGUILayout.EndHorizontal();

    //        EditorGUILayout.Space();

    //        // Second row of 3 Colors
    //        EditorGUILayout.BeginHorizontal();
    //        VerticalColorPickerDisplay("Unavailable", "Cannot be used because of the game Status (i.e. wrong hud mode, missing status flag)", unavailableColorProp);
    //        EditorGUILayout.EndHorizontal();

    //        // Cleanup
    //        if (EditorGUI.EndChangeCheck())
    //        {
    //            useCustomHudBoolProp.boolValue = useCustomHudColor;
    //            if (hudColor != null && hudColor.hudColorChangedEvent != null)
    //            {
    //                hudColor.hudColorChangedEvent.Raise();
    //            }
    //        }
    //        serializedObject.ApplyModifiedProperties();
    //    }

    //    private void VerticalColorPickerDisplay(string label, string description, SerializedProperty prop)
    //    {
    //        EditorGUILayout.BeginVertical(myVerticalStyle);
    //        // Name of the Color, Centered and Larger Text
    //        EditorGUILayout.LabelField(label, new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter, fontSize = 15 });
    //        // word wrapped description of the color
    //        EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
    //        EditorGUILayout.PropertyField(prop, GUIContent.none);
    //        EditorGUILayout.EndVertical();
             
    //    }

    //    private void SetCustomStyles()
    //    {
    //        myVerticalStyle = new GUIStyle(EditorStyles.helpBox);
    //        myVerticalStyle.alignment = TextAnchor.MiddleCenter;
    //        myVerticalStyle.stretchWidth = true;
    //        myVerticalStyle.fixedWidth = Screen.width * 0.3f;
    //    }
    //}
}
