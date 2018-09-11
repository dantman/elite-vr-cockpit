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
        private ScrollRect scrollRect;

        private void OnEnable()
        {
            scrollRect = GetComponentInParent<ScrollRect>();
            lineQueue = new Queue<GameObject>(maxLines);
            Application.logMessageReceived += OnLogMessage;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= OnLogMessage;
        }

        private void OnLogMessage(string text, string stackTrace, LogType type)
        {
            var bottomAligned = scrollRect.verticalNormalizedPosition <= 0.0001f;

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
            line.transform.SetParent(transform, false);

            RecalculateHeight();

            if (bottomAligned)
            {
                scrollRect.verticalNormalizedPosition = 0;
            }
        }

        private void RecalculateHeight()
        {
            float height = 0f;
            for (int i = transform.childCount - 1; i >= 0; --i)
            {
                var line = (RectTransform)transform.GetChild(i);
                height += line.sizeDelta.y;
            }

            var t = (RectTransform)transform;
            t.sizeDelta = new Vector2(t.sizeDelta.x, height);
        }
    }
}
