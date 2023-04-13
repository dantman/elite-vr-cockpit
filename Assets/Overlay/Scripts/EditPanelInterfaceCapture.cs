using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class EditPanelInterfaceCapture : MonoBehaviour
    {
        public RectTransform canvasRect;
        public Camera captureCamera;
        public float height = 1f;

        public static EditPanelInterfaceCapture instance
        {
            get
            {
                return FindObjectOfType<EditPanelInterfaceCapture>();
            }
        }

        private void OnEnable()
        {
            var aspectRatio = captureCamera.targetTexture.width / captureCamera.targetTexture.height;
            canvasRect.sizeDelta = new Vector2(height * aspectRatio, height);
            captureCamera.orthographic = true;
            captureCamera.orthographicSize = height / 2;
        }
    }
}
