using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace EVRC.Core.Editor
{
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


    //[CustomEditor(typeof(GameEvent<>), true)]
    //public class GameEventEditor<T> : UnityEditor.Editor
    //{
    //    private T value;

    //    public override void OnInspectorGUI()
    //    {
    //        SerializedObject serializedObject = this.serializedObject;
    //        SerializedProperty prop = serializedObject.FindProperty("Event");

    //        base.OnInspectorGUI();
    //        GUI.enabled = Application.isPlaying;

    //        EditorGUILayout.Space();

    //        value = EditorGUILayout.ObjectField(prop, new Type(prop.type), "Value");

    //        GameEvent<T> gameEvent = target as GameEvent<T>;

    //        if (GUILayout.Button("Raise"))
    //        {
    //            gameEvent.Raise(value);
    //        }
    //    }
    //}
}