using UnityEngine;
using UnityEngine.UI;

namespace EVRC.Core
{
    /**
     * Helper that will show/hide a game object based on a Toggle's isOn
     */
    [RequireComponent(typeof(Toggle))]
    public class ToggleContentsController : MonoBehaviour
    {
        public GameObject contents;

        private Toggle toggle;

        private void OnEnable()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnValueChanged);
            Refresh();
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (toggle != null && contents != null)
            {
                contents.SetActive(toggle.isOn);
            }
        }
    }
}
