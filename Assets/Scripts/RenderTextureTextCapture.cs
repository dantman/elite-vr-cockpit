using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EVRC
{
    using TextAlignment = TMPro.TextAlignmentOptions;

    public class RenderTextureTextCapture : MonoBehaviour
    {
        public struct Job
        {
            public RenderTexture renderTexture;
            public string text;
            public TextAlignment textAlignment;
        }

        public Camera captureCamera;
        public TMPro.TextMeshPro textMesh;
        public float height = 4f;
        private Queue<Job> jobQueue = new Queue<Job>();
        private bool queueRunning = false;

        public static RenderTextureTextCapture instance
        {
            get
            {
                return FindObjectOfType<RenderTextureTextCapture>();
            }
        }

        public static void RenderText(
            RenderTexture renderTexture,
            string text,
            TextAlignment textAlignment = TextAlignment.Center)
        {
            var job = new Job();
            job.renderTexture = renderTexture;
            job.text = text;
            job.textAlignment = textAlignment;

            var tt = instance;
            tt.jobQueue.Enqueue(job);
            tt.StartQueue();
        }

        public void StartQueue()
        {
            if (!queueRunning)
            {
                StartCoroutine(RunJobQueue());
            }
        }


        private IEnumerator RunJobQueue()
        {
            queueRunning = true;

            while (jobQueue.Count > 0)
            {
                var job = jobQueue.Dequeue();

                if (!job.renderTexture)
                {
                    Debug.LogWarning("RenderTextureTextCapture job skipped: Render texture was null");
                }
                else if (!job.renderTexture.IsCreated())
                {
                    Debug.LogWarning("RenderTextureTextCapture job skipped: Render texture was not created");
                }
                else
                {
                    // Wait till it's safe to change sizeDelta
                    yield return new WaitForEndOfFrame();

                    var aspectRatio = job.renderTexture.width / job.renderTexture.height;
                    textMesh.rectTransform.sizeDelta = new Vector2(height * aspectRatio, height);
                    textMesh.text = job.text;
                    textMesh.alignment = job.textAlignment;
                    captureCamera.orthographic = true;
                    captureCamera.orthographicSize = height / 2;
                    captureCamera.targetTexture = job.renderTexture;

                    RenderTexture.active = captureCamera.targetTexture;
                    captureCamera.Render();
                    RenderTexture.active = null;

                    captureCamera.targetTexture = null;
                    yield return null;
                }
            }
            queueRunning = false;
        }

    }
}