using UnityEngine;

namespace EVRC
{
    using EDControlButton = EDControlBindings.EDControlButton;

    /**
     * A control button asset with static text and textures
     */
    [CreateAssetMenu(fileName = "SimpleControlButtonAsset", menuName = "EVRC/ControlButtonAssets/SimpleControlButtonAsset", order = 1)]
    public class SimpleControlButtonAsset : ControlButtonAsset
    {
        public string text;
        public Texture texture;
        public EDControlButton control;
        public string defaultKeycombo;

        public override string GetText()
        {
            return text;
        }

        public override Texture GetTexture()
        {
            return texture;
        }

        public override EDControlButton GetControl()
        {
            return control;
        }

        public override bool GetDefaultKeycombo(ref string key, ref string[] modifiers)
        {
            return ParseKeycomboString(defaultKeycombo, ref key, ref modifiers);
        }
    }
}
