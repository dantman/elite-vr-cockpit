using EVRC;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class RadialWedge
{
    //public Sprite icon = null;
    //public SpriteRenderer iconRenderer = null;
    public static string defaultLabel = "Radial Menu Item";
    public string label = defaultLabel;
    public RenderTexture renderTexture = null;
    public UnityEvent onPress = new UnityEvent();  

    public void createLabelTexture()
    {
        renderTexture = new RenderTexture(128, 64, 0, RenderTextureFormat.ARGB32);
        renderTexture.wrapMode = TextureWrapMode.Clamp;
        renderTexture.antiAliasing = 4;
        renderTexture.filterMode = FilterMode.Trilinear;
        renderTexture.name = "Radial" + Regex.Replace(label, @"\s+", "") + "RenderTexture";

        renderTexture.Create();
        if (label != null && label != "")
        {
            RenderTextureTextCapture.RenderText(renderTexture, label, TMPro.TextAlignmentOptions.Top);

        }
        else
        {
            Debug.LogWarning($"Unable to render text into texture. Label is null: {this.label}");
        }
    }
}
