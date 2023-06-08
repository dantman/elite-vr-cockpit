using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

namespace EVRC.Core
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DescriptionAttribute : PropertyAttribute
    {
        public string DescriptionText { get; }

        public DescriptionAttribute(string descriptionText)
        {
            DescriptionText = descriptionText;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(DescriptionAttribute))]
    public class DescriptionDrawer : PropertyDrawer
    {
        public int descriptionLineCount;
        public float messageBoxHeight; 

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var describedAttribute = (DescriptionAttribute)attribute;
            var offset_x = 40;
            descriptionLineCount = CalculateLines(describedAttribute.DescriptionText, GUI.skin.GetStyle("HelpBox"), position.width - offset_x*2);
            messageBoxHeight = EditorGUIUtility.singleLineHeight * descriptionLineCount;

            //var basePropertyPosition = new Rect(position.x, position.y, position.width, basePropertyHeight);
            EditorGUI.BeginProperty(position, label, property);            
            
            EditorGUI.HelpBox(position, "", MessageType.None);
            position.y += EditorGUIUtility.standardVerticalSpacing;

            var propertyPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(propertyPosition, property, label);

            position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            var descriptionPosition = new Rect(position.x + offset_x / 2, position.y, position.width - offset_x, messageBoxHeight);
            EditorGUI.HelpBox(descriptionPosition, describedAttribute.DescriptionText, MessageType.Info);

            EditorGUI.EndProperty();
        }

        // Overridden to allow us to customize the height of the total property and fit in the extra information from the help box
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var describedAttribute = (DescriptionAttribute)attribute;

            float totalHeight = messageBoxHeight;
            totalHeight += EditorGUI.GetPropertyHeight(property, label);

            return totalHeight;
        }

        public static int CalculateLines(string text, GUIStyle style, float width)
        {
            GUIContent content = new GUIContent(text);
            float height = style.CalcHeight(content, width);
            int lines = Mathf.CeilToInt(height / EditorGUIUtility.singleLineHeight);
            return lines;
        }

    }
#endif
}