using UnityEngine;

namespace EVRC.Core.Actions
{
    /**
     * A base for control button assets which are boolean toggles for state that EDStateManager understands,
     * and can dynamically switch between text and textures.
     * e.g. "Landing Gear (Up)" and "Landing Gear (Down)"
     */
    abstract public class BooleanControlButtonAsset : ControlButtonAsset
    {
        public string onText;
        public string offText;
        public Texture onTexture;
        public Texture offTexture;
        public EDControlButton control;
        public string defaultKeycombo;
        public string label;

        abstract public bool IsOn();

        public override string GetTooltipText()
        {
            return IsOn() ? onText : offText;
        }

        public override string GetLabelText()
        {
            if (label != null && label != ""){
                return label;
            }
            return IsOn() ? onText : offText;
        }

        public override Texture GetTexture()
        {
            return IsOn() ? onTexture : offTexture;
        }

        public override EDControlButton GetControl()
        {
            return control;
        }

        public override KeyboardInterface.KeyCombo? GetDefaultKeycombo()
        {
            return ParseKeycomboString(defaultKeycombo);
        }

        public override void SetTexture(Texture tex)
        {
            offTexture = tex;
            onTexture = tex;
            Debug.LogWarning("Only one texture has been set for a Boolean ControlButton. Toggle will not be available.");
        }

        public override void SetTexture(Texture onTex, Texture offTex)
        {
            offTexture = offTex;
            onTexture = onTex;
        }
    }
}