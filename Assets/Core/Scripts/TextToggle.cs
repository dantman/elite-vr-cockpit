using UnityEngine;
using UnityEngine.UI;

namespace EVRC.UI
{
    /**
     * Unity UI.Toggle subclass that also changes the text color of a button
     */
    [AddComponentMenu("EVRC/UI/Toggle", 30)]
    public class TextToggle : Toggle
    {
        public TMPro.TextMeshProUGUI text;
        public ColorBlock textColors;

        protected override void OnEnable()
        {
            base.OnEnable();
            onValueChanged.AddListener(InternalTransitionState);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            onValueChanged.RemoveListener(InternalTransitionState);
        }

        /**
         * Extra transition state handler to ensure state is changed when isOn is toggled
         */
        private void InternalTransitionState(bool isOn)
        {
            SelectionState state = currentSelectionState;
            if (IsActive() && !IsInteractable())
                state = SelectionState.Disabled;
            DoStateTransition(state, false);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (transition != Transition.ColorTint)
                base.DoStateTransition(state, instant);

            Color color;
            Color textColor;

            switch (state)
            {
                case SelectionState.Normal:
                    if (isOn)
                    {
                        color = colors.selectedColor;
                        textColor = textColors.selectedColor;
                    }
                    else
                    {
                        color = colors.normalColor;
                        textColor = textColors.normalColor;
                    }
                    break;
                case SelectionState.Highlighted:
                    color = colors.highlightedColor;
                    textColor = textColors.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    color = colors.pressedColor;
                    textColor = textColors.pressedColor;
                    break;
                case SelectionState.Selected:
                    color = colors.selectedColor;
                    textColor = textColors.selectedColor;
                    break;
                case SelectionState.Disabled:
                    color = colors.disabledColor;
                    textColor = textColors.disabledColor;
                    break;
                default:
                    color = Color.black;
                    textColor = Color.white;
                    break;
            }

            if (!gameObject.activeInHierarchy)
                return;

            switch (transition)
            {
                case Transition.ColorTint:
                    StartColorTween(color * colors.colorMultiplier, textColor * textColors.colorMultiplier, instant);
                    break;
            }
        }

        private void StartColorTween(Color targetColor, Color targetTextColor, bool instant)
        {
            if (targetGraphic != null)
            {
                targetGraphic.CrossFadeColor(targetColor, !instant ? colors.fadeDuration : 0.0f, true, true);
            }

            if (text != null)
            {
                text.CrossFadeColor(targetTextColor, !instant ? textColors.fadeDuration : 0.0f, true, true);
            }
        }
    }
}
