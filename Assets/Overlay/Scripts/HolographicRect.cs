using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Overlay
{

    public class HolographicRect : HolographicBase
    {
        public int pxWidth = 50;
        public int pxHeight = 50;
        private Texture2D texture2d;

        void OnEnable()
        {
            int w = pxWidth;
            int h = pxHeight;
            Color fillColor = Color.white;
            texture2d = new Texture2D(w, h, TextureFormat.ARGB32, false);

            Color[] colors = new Color[w * h];
            for (int i = 0; i < w * h; ++i)
            {
                colors[i] = fillColor;
            }
            texture2d.SetPixels(0, 0, w, h, colors);
            texture2d.Apply();
        }

    }
}
