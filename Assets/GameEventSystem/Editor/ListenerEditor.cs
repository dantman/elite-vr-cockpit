using UnityEngine;
using UnityEditor;

namespace EVRC
{
    [CustomEditor(typeof(GameEventListener))]
    public class ListenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GameEventListener listener = (GameEventListener)target;

            GameEvent gameEvent = listener.Event;

            if (GUILayout.Button("Simulate GameEvent"))
            {
                Debug.Log($"Simulating GameEvent: {gameEvent.name}");
                gameEvent.Raise();
            }
        }
    }
}