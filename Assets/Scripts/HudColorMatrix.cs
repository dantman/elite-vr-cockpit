using UnityEngine;

namespace EVRC
{
    /**
     * Implements a color matrix that transforms colors the same way as Elite Dangerous'
     * color matrix for the HUD.
     */
    public class HudColorMatrix
    {
        private float[] r;
        private float[] g;
        private float[] b;

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