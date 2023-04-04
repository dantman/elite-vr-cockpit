using UnityEngine;
using UnityEngine.UI;

namespace EVRC.UI
{
    /**
     * Unity UI.Button subclass that also changes the text color of a button
     */
    [AddComponentMenu("EVRC/UI/Button", 30)]
    public class TextButton : Button
    {
        public TMPro.TextMeshProUGUI text;
        public ColorBlock textColors;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            Color textColor;

            switch (state)
            {
                case SelectionState.Highlighted:
                    textColor = textColors.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    textColor = textColors.pressedColor;
                    break;
                case SelectionState.Selected:
                    textColor = textColors.pressedColor;
                    break;
                case SelectionState.Disabled:
                    textColor = textColors.disabledColor;
                    break;
                default:
                    textColor = textColors.normalColor;
                    break;
            }

            if (text != null)
            {
                text.color = textColor;
                text.CrossFadeColor(textColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
            }
        }
    }
}
