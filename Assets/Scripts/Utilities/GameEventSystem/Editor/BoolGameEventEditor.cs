using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using EVRC;

[CustomEditor(typeof(BoolEvent))]
public class BoolEventEditor : Editor
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
