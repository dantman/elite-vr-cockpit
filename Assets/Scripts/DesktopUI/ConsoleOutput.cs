using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EVRC.DesktopUI
{
    public class ConsoleOutput : MonoBehaviour
    {
        [Serializable]
        public struct LogTypeStyle
        {
            public Color color;
            public Sprite icon;
        }
        public GameObject linePrefab;
        public int maxLines = 100;
        public LogTypeStyle infoStyle;
        public LogTypeStyle warningStyle;
        public LogTypeStyle errorStyle;
        private Queue<GameObject> lineQueue;

        private void OnEnable()
        {
            lineQueue = new Queue<GameObject>(maxLines);
            Application.logMessageReceived += OnLogMessage;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessage;
        }

        private void OnLogMessage(string text, string stackTrace, LogType type)
        {
            if (lineQueue.Count >= maxLines)
            {
                var oldLine = lineQueue.Dequeue();
                Destroy(oldLine);
            }

            var line = Instantiate(linePrefab);
            var image = line.GetComponentInChildren<Image>();
            var textMesh = line.GetComponentInChildren<TMPro.TextMeshProUGUI>();

            var style = type == LogType.Log ? infoStyle
                : type == LogType.Warning ? warningStyle
                : errorStyle;

            image.color = style.color;
            image.sprite = style.icon;
            textMesh.text = text;

            lineQueue.Enqueue(line);
            line.transform.SetParent(transform);
        }
    }
}
