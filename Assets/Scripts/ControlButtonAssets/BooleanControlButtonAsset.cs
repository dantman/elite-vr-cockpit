using UnityEngine;

namespace EVRC
{
    using EDControlButton = EDControlBindings.EDControlButton;

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

        abstract public bool IsOn();

        public override string GetText()
        {
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
    }
}