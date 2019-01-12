using UnityEngine;
using UnityEngine.UI;

namespace EVRC
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollController : MonoBehaviour
    {
        public float scrollIncrement = 1f;

        private ScrollRect scrollRect;

        private void OnEnable()
        {
            scrollRect = GetComponent<ScrollRect>();
        }

        public void ScrollBy(int increment)
        {
            var viewportTransform = (RectTransform)scrollRect.viewport.transform;
            var contentTransform = (RectTransform)scrollRect.content.transform;
            float relativeScroll = scrollIncrement * increment * -1;
            float scrollHeight = contentTransform.rect.height - viewportTransform.rect.height;
            scrollRect.verticalNormalizedPosition += relativeScroll / scrollHeight;
        }
    }
}
