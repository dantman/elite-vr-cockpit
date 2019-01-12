using System.Collections;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EVRC
{
    public class ButtonImageCaptureController : MonoBehaviour
    {
        public Canvas captureCanvas;
        public Transform captureCameraDolly;
        public Camera captureCamera;

#if UNITY_EDITOR
        private void OnEnable()
        {
            StartCoroutine(CaptureButtonImages());
        }

        private IEnumerator CaptureButtonImages()
        {
            yield return new WaitForEndOfFrame();

            var rTex = captureCamera.targetTexture;

            for (int i = 0, l = captureCanvas.transform.childCount; i < l; ++i)
            {
                var t = (RectTransform)captureCanvas.transform.GetChild(i);
                var name = t.name;

                Debug.LogFormat("Capturing {0}", name);

                captureCameraDolly.position = t.position;

                captureCamera.Render();

                RenderTexture.active = rTex;
                var tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBAHalf, false);
                tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
                byte[] bytes = tex.EncodeToPNG();
                RenderTexture.active = null;

                var destFile = Path.Combine(Application.dataPath, "Textures", "ButtonImages", name + ".png");
                var assetPath = "Assets/Textures/ButtonImages/" + name + ".png";
                File.WriteAllBytes(destFile, bytes);

                AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceSynchronousImport);

                var importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);

                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.textureType = TextureImporterType.Sprite;
                settings.spriteMode = (int)SpriteImportMode.Single;
                settings.spriteMeshType = SpriteMeshType.FullRect;
                settings.filterMode = FilterMode.Bilinear;
                settings.sRGBTexture = true;
                settings.alphaSource = TextureImporterAlphaSource.FromInput;
                settings.alphaIsTransparency = true;
                settings.mipmapEnabled = false;


                var platformSettings = importer.GetDefaultPlatformTextureSettings();
                platformSettings.format = TextureImporterFormat.RGBAHalf;
                platformSettings.maxTextureSize = 128;
                platformSettings.textureCompression = TextureImporterCompression.Uncompressed;

                importer.SetTextureSettings(settings);
                importer.SetPlatformTextureSettings(platformSettings);
                importer.SaveAndReimport();

                yield return null;
            }

            RenderTexture.active = null;

            EditorApplication.isPlaying = false;
        }
#endif
    }
}
