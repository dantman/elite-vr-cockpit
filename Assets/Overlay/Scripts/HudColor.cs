using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml;
using UnityEngine;
using System.Text.RegularExpressions;

namespace EVRC.Core.Overlay
{
    [CreateAssetMenu(menuName = Constants.STATE_OBJECT_PATH + "/Hud Color"), System.Serializable]
    public class HudColor : ScriptableObject
    {
        public GameEvent hudColorChangedEvent;
        [Header("Colors")]
        public Color baseColor;
        public Color highlightColor;
        public Color invalidColor = Color.red;
        public Color unavailableColor;

        // In order to use these colors, you must add them to the IColorable interface (an implementing classes)
        public Color notYetConfiguredColor1;
        public Color notYetConfiguredColor2;

        [Header("HUD Color Matrix")]
        [Tooltip("Use a custom configuration from the user's GraphicsConfigurationOverride.xml file (not recommended)")]
        public bool useHudColorMatrixOverride = false;
        [SerializeField] private float[] R;
        [SerializeField] private float[] G;
        [SerializeField] private float[] B;

        private void OnEnable()
        {
            if (useHudColorMatrixOverride) LoadMatrixFromFile();
        }

        // Constructor with default colors
        public HudColor()
        {
            baseColor = Color.white;
            highlightColor = Color.yellow;
            invalidColor = Color.red;
            unavailableColor = Color.grey;
        }

        // //Generates a "bright" color for the highlight color
        // private Color GetHighlightColor(Color startColor)
        // {
        //     Color.RGBToHSV(startColor, out float h, out float s, out float v);
        //
        //     // increase value (brightness) to max
        //     v = 1f;
        //
        //     // if starting color is already bright, decrease saturation to avoid over-saturation
        //     if (s > 0.5f)
        //     {
        //         s = 0.5f;
        //     }
        //
        //     // We'll use the hue value to find a similar color
        //     h = (h + Random.Range(-0.1f, 0.1f) + 1f) % 1f;
        //
        //     // convert back to RGB
        //     Color brightColor = Color.HSVToRGB(h, s, v);
        //
        //     return brightColor;
        // }
        

        /**
         * Read the user's GraphicsConfigurationOverride.xml and parse the HUD color matrix config
         */
        private void LoadMatrixFromFile() {
            try
            {
                string RedLine;
                string GreenLine;
                string BlueLine;
                try
                {
                    var doc = XDocument.Load(Paths.GraphicsConfigurationOverridePath);
                    var defaultGuiColor = doc.Descendants("GUIColour").Descendants("Default");
                    RedLine = (from el in defaultGuiColor.Descendants("MatrixRed") select el).FirstOrDefault()?.Value;
                    GreenLine = (from el in defaultGuiColor.Descendants("MatrixGreen") select el).FirstOrDefault()?.Value;
                    BlueLine = (from el in defaultGuiColor.Descendants("MatrixBlue") select el).FirstOrDefault()?.Value;
                }
                catch (XmlException e)
                {
                    throw new HudColorMatrixSyntaxErrorException("Failed to parse XML", e);
                }

                R = ParseColorLineElement(RedLine ?? "1, 0, 0");
                G = ParseColorLineElement(GreenLine ?? "0, 1, 0");
                B = ParseColorLineElement(BlueLine ?? "0, 0, 1");
                hudColorChangedEvent.Raise();
            }
            catch (HudColorMatrixSyntaxErrorException e)
            {
                R = new float[] { 1, 0, 0 };
                G = new float[] { 0, 1, 0 };
                B = new float[] { 0, 0, 1 };

                UnityEngine.Debug.LogErrorFormat($"Failed to load your HUD Color Matrix, you have a syntax error in your graphics configuration overrides file:\n{{0}}", Paths.GraphicsConfigurationOverridePath);
                UnityEngine.Debug.LogWarning(e.Message);
                if (e.InnerException != null)
                {
                    UnityEngine.Debug.LogWarning(e.InnerException.Message);
                }
            }

            hudColorChangedEvent.Raise();
        }


        private float[] ParseColorLineElement(string line)
        {
            if (line.Trim() == "") throw new HudColorMatrixSyntaxErrorException("Matrix line was empty");
            return Regex.Split(line, ",\\s*").Select(nStr =>
            {
                if (float.TryParse(nStr, out float n))
                {
                    return n;
                }

                throw new HudColorMatrixSyntaxErrorException($"Could not parse \"{nStr}\" as a number");
            }).ToArray();
        }

        /**
         * Apply the color matrix to a color outputting the new color
         */
        public Color ApplyColorToMatrix(Color color)
        {
            return new Color(
                CalculateComponent(R, color),
                CalculateComponent(G, color),
                CalculateComponent(B, color),
                color.a
            );
        }

        private float CalculateComponent(float[] colorMatrix, Color color)
        {
            return (color.r * colorMatrix[0])
                   + (color.g * colorMatrix[1])
                   + (color.b * colorMatrix[2]);
        }

        /*
         * Used for HudColorMatrixEffect on RenderTextures
         */
        public Matrix4x4 Matrix
        {
            get
            {
                return new Matrix4x4(
                    new Vector4(R[0], R[1], R[2], 0),
                    new Vector4(G[0], G[1], G[2], 0),
                    new Vector4(B[0], B[1], B[2], 0),
                    new Vector4(0, 0, 0, 1));
            }
        }
    }
}
