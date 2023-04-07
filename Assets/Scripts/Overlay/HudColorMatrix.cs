using UnityEngine;

namespace EVRC
{
    /**
     * Implements a color matrix that transforms colors the same way as Elite Dangerous'
     * color matrix for the HUD.
     */
    public class HudColorMatrix
    {
        public float[] r { get; private set; }
        public float[] g { get; private set; }
        public float[] b { get; private set; }

        public Matrix4x4 Matrix
        {
            get
            {
                return new Matrix4x4(
                    new Vector4(r[0], r[1], r[2], 0),
                    new Vector4(g[0], g[1], g[2], 0),
                    new Vector4(b[0], b[1], b[2], 0),
                    new Vector4(0, 0, 0, 1));
            }
        }

        public HudColorMatrix(float[] r, float[] g, float[] b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public static HudColorMatrix Identity()
        {
            return new HudColorMatrix(
                new float[] { 1, 0, 0 },
                new float[] { 0, 1, 0 },
                new float[] { 0, 0, 1 });
        }

        /**
         * Apply the color matrix to a color outputting the new color
         */
        public Color Apply(Color color)
        {
            return new Color(
                CalculateComponent(r, color),
                CalculateComponent(g, color),
                CalculateComponent(b, color),
                color.a
            );
        }

        private float CalculateComponent(float[] colorMatrix, Color color)
        {
            return (color.r * colorMatrix[0])
                + (color.g * colorMatrix[1])
                + (color.b * colorMatrix[2]);
        }
    }
}