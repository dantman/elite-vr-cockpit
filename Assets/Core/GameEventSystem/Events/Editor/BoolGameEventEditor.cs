using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace EVRC.Core.Editor
{
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
}
