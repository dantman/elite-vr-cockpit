using System;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    public class HudColorMatrixEffect : MonoBehaviour
    {
        private Material mat;
        private static int _Matrix = Shader.PropertyToID("_Matrix");

        private void OnEnable()
        {
            mat = new Material(Shader.Find("Hidden/HudColorMatrixShader"));
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            var colorMatrix = EDStateManager.instance.hudColorMatrix;
            mat.SetMatrix(_Matrix, colorMatrix.Matrix);
            Graphics.Blit(source, destination, mat);
        }
    }
}
