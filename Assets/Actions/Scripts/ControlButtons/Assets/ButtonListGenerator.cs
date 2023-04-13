using EVRC.Core.Overlay;
using UnityEngine;
using UnityEngine.UI;

namespace EVRC.Core.Actions
{
    public class ButtonListGenerator : MonoBehaviour
    {
        public GameObject buttonPreviewPrefab;
        public SpawnZone spawnZone;
        public ControlButtonAssetCatalog controlButtonCatalog;

        // @fixme The list of references will result in all unused listeners being registered
        // Move them from OnEnable to something that is called when the refresh listener is registered

        // @todo Make this run in edit mode (at least display previews)

        private void Start()
        {
            buttonPreviewPrefab.SetActive(false);
            foreach (var controlButton in controlButtonCatalog.controlButtons)
            {
                var buttonPreview = Instantiate(buttonPreviewPrefab);
                var image = buttonPreview.GetComponentInChildren<Image>();
                var textMesh = buttonPreview.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                var handler = buttonPreview.GetComponentInChildren<ControlButtonUIButtonHandler>();

                buttonPreview.name = controlButton.name;
                handler.controlButton = controlButton;
                handler.spawnZone = spawnZone;
                var sprite = controlButton.GetPreviewTexture();
                if (sprite)
                {
                    image.sprite = sprite;
                }
                textMesh.text = controlButton.GetTooltipText();

                buttonPreview.transform.SetParent(transform, false);
                buttonPreview.SetActive(true);
            }
        }
    }
}
