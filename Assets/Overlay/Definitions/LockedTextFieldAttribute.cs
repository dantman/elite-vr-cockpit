using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EVRC.Core
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class LockedTextFieldAttribute : PropertyAttribute
    {
        public string tooltip;

        public LockedTextFieldAttribute(string tooltip = "")
        {
            this.tooltip = tooltip;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Makes a text field Lockable, so it's harder to accidentally change an important field.
    /// </summary>
    /// <remarks>
    /// Possible uses include: A string that's used as a key for a saved file
    /// </remarks>
    [UnityEditor.CustomPropertyDrawer(typeof(LockedTextFieldAttribute))]
    public class LockedTextFieldDrawer : UnityEditor.PropertyDrawer
    {
        private static GUIStyle lockStyle;
        private static GUIStyle unlockStyle;
        private bool isLocked = true;

        private string GetLockText()
        {
            return isLocked ? "Unlock" : "Lock";
        }

        private GUIStyle GetLockButtonStyle()
        {
            return isLocked ? unlockStyle : lockStyle;
        }

        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            if (lockStyle == null)
            {
                lockStyle = new GUIStyle(GUI.skin.button);
                lockStyle.normal.textColor = Color.red;
            }
            if (unlockStyle == null)
            {
                unlockStyle = new GUIStyle(GUI.skin.button);
                unlockStyle.normal.textColor = Color.green;
            }

            var lockedAttribute = attribute as LockedTextFieldAttribute;

            // Place a button at the end of the text field to Lock/Unlock the text field
            int lockButtonWidth = 50; // the default Unity object selector button width is 20
            if (GUI.Button(new Rect(position.xMax - lockButtonWidth, position.y, lockButtonWidth, EditorGUIUtility.singleLineHeight),
                    new GUIContent(GetLockText()), GetLockButtonStyle()))
            {
                isLocked = !isLocked;
            }
            GUI.enabled = !isLocked;

            EditorGUI.BeginProperty(position, label, property);
            // Draw the label and the text field
            var adjustedPosition = new Rect(position.x, position.y, position.width - lockButtonWidth, EditorGUIUtility.singleLineHeight);
            Rect textFieldPosition = EditorGUI.PrefixLabel(adjustedPosition, GUIUtility.GetControlID(FocusType.Passive), new GUIContent(label.text + " (Lockable)", lockedAttribute.tooltip));
            EditorGUI.PropertyField(textFieldPosition, property, GUIContent.none);
            
            // Draw the help box
            float yposition = adjustedPosition.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            Rect helpBoxPosition = new Rect(position.x, yposition, position.width, EditorGUIUtility.singleLineHeight*2);
            EditorGUI.HelpBox(helpBoxPosition, "This field is locked because it has critical data. See the tooltip or the parent class before changing the value!", isLocked ? MessageType.Info : MessageType.Warning);
            EditorGUI.EndProperty();

            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + (EditorGUIUtility.singleLineHeight * 3);
        }
    }
#endif
}
