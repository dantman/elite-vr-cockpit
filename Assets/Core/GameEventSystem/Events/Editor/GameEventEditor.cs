using UnityEngine;
using EVRC;
#if UNITY_EDITOR
using UnityEditor;
#endif


[CustomEditor(typeof(GameEvent))]
public class GameEventEditor : Editor
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