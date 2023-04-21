using UnityEngine;

namespace EVRC.Core.Overlay
{
    public class HudColorMatrixEffect : MonoBehaviour
    {
        public HudColor hudColor;
        private Material mat;
        private static readonly int Matrix = Shader.PropertyToID("_Matrix");

        private void OnEnable()
        {
            mat = new Material(Shader.Find("Hidden/HudColorMatrixShader"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            mat.SetMatrix(Matrix, hudColor.Matrix);
            Graphics.Blit(source, destination, mat);
        }
    }
}
