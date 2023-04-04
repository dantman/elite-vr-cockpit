using UnityEditor;
using UnityEngine;

namespace EVRC
{

    using CockpitMode = CockpitUIMode.CockpitMode;


    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            EditorGUILayout.Space();

            if (GUILayout.Button("Raise"))
            {
                ((GameEvent)target).Raise();
            }
        }
    }

    [CustomEditor(typeof(BoolEvent))]
    public class BoolEventEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            m_Data = EditorGUILayout.Toggle("Data", m_Data);

            EditorGUILayout.Space();

            if (GUILayout.Button("Raise"))
            {
                ((BoolEvent)target).Raise(m_Data);
            }
        }

        private bool m_Data;
    }

    [CustomEditor(typeof(CockpitModeEvent))]
    public class CockpitModeEventEditor : UnityEditor.Editor
    {
        CockpitMode raiseMode = CockpitMode.GameNotRunning;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            //m_Data = EditorGUILayout.Toggle("Data", m_Data);
            raiseMode = (CockpitMode)EditorGUILayout.EnumPopup(raiseMode);

            EditorGUILayout.Space();

            if (GUILayout.Button("Raise"))
            {
                ((CockpitModeEvent)target).Raise(raiseMode);
            }
        }

        private bool m_Data;
    }

    //[CustomEditor(typeof(IntEvent))]
    //public class IntEventEditor : UnityEditor.Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();

    //        GUI.enabled = Application.isPlaying;

    //        m_Data = EditorGUILayout.IntField("Data", m_Data);

    //        EditorGUILayout.Space();

    //        if (GUILayout.Button("Raise"))
    //        {
    //            ((IntEvent)target).Raise(m_Data);
    //        }
    //    }

    //    private int m_Data;
    //}


    //[CustomEditor(typeof(FloatEvent))]
    //public class FloatEventEditor : UnityEditor.Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();

    //        GUI.enabled = Application.isPlaying;

    //        m_Data = EditorGUILayout.FloatField("Data", m_Data);

    //        EditorGUILayout.Space();

    //        if (GUILayout.Button("Raise"))
    //        {
    //            ((FloatEvent)target).Raise(m_Data);
    //        }
    //    }

    //    private float m_Data;
    //}


    //[CustomEditor(typeof(DoubleEvent))]
    //public class DoubleEventEditor : UnityEditor.Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();

    //        GUI.enabled = Application.isPlaying;

    //        m_Data = EditorGUILayout.DoubleField("Data", m_Data);

    //        EditorGUILayout.Space();

    //        if (GUILayout.Button("Raise"))
    //        {
    //            ((DoubleEvent)target).Raise(m_Data);
    //        }
    //    }

    //    private double m_Data;
    //}


    //[CustomEditor(typeof(StringEvent))]
    //public class StringEventEditor : UnityEditor.Editor
    //{
    //    public override void OnInspectorGUI()
    //    {
    //        base.OnInspectorGUI();

    //        GUI.enabled = Application.isPlaying;

    //        m_Data = EditorGUILayout.TextField("Data", m_Data);

    //        EditorGUILayout.Space();

    //        if (GUILayout.Button("Raise"))
    //        {
    //            ((StringEvent)target).Raise(m_Data);
    //        }
    //    }

    //    private string m_Data;
    //}

    //[CustomEditor(typeof(GameEvent<>), true)]
    //public class GameEventEditor : Editor
    //{
    //    private bool showEnumValues;


    //    public override void OnInspectorGUI()
    //    {
    //        // Define a custom style for the property name
    //        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
    //        labelStyle.richText = true;

    //        serializedObject.Update();

    //        SerializedProperty eventType = serializedObject.FindProperty("_type");
    //        //SerializedProperty listeners = serializedObject.FindProperty("listeners");

    //        //EditorGUILayout.BeginVertical();
    //        //EditorGUILayout.LabelField(listeners);
    //        //EditorGUILayout.EndVertical();

    //        EditorGUILayout.LabelField("This GameEvent will include the following parameters when raised.");
    //        if (eventType != null && eventType.propertyType == SerializedPropertyType.Enum)
    //        {
    //            DisplayEnum(eventType, labelStyle);
    //        }
    //        else
    //        {
    //            EditorGUILayout.BeginHorizontal();
    //            GUILayout.Label("<color=blue>Type:</color> " + eventType.type, labelStyle);
    //            GUILayout.Label("<color=blue>Property Name:</color> " + eventType.name, labelStyle);
    //            EditorGUILayout.EndHorizontal();
    //        }

    //        EditorGUILayout.Space();

    //        serializedObject.ApplyModifiedProperties();
    //    }

    //    public void DisplayEnum(SerializedProperty iterator, GUIStyle labelStyle)
    //    {
    //        EditorGUILayout.Space();
    //        EditorGUILayout.BeginHorizontal();
    //        GUILayout.Label("<color=blue>Type:</color> " + iterator.type + "(" + iterator.displayName + ")", labelStyle);
    //        GUILayout.Label("<color=blue>Property Name:</color> " + iterator.name, labelStyle);
    //        EditorGUILayout.EndHorizontal();

    //        showEnumValues = EditorGUILayout.Foldout(showEnumValues, "        Enum " + "(" + iterator.displayName + ")" + " Values");

    //        if (showEnumValues)
    //        {
    //            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    //            EditorGUI.indentLevel++;
    //            foreach (string displayname in iterator.enumDisplayNames)
    //            {
    //                EditorGUILayout.LabelField(displayname);
    //            }
    //            EditorGUI.indentLevel--;
    //            EditorGUILayout.EndVertical();

    //        }

    //    }

    //}
}