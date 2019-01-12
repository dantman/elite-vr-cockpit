using System;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using EDControlButton = EDControlBindings.EDControlButton;

    /**
     * Base class for assets that represent buttons that trigger keypresses based on
     * key bound in Elite Dangerous' custom bindings file.
     */
    abstract public class ControlButtonAsset : ScriptableObject
    {
        public enum ButtonCategory
        {
            [Tooltip("Any type of cockpit (Ships, Fighters, SRVs)")]
            Cockpit,
            [Tooltip("Only ship cockpits (Main ship and fighters) not SRVs")]
            ShipCockpit,
            [Tooltip("Only the main ship cockpit, not fighters or SRVs")]
            MainShipCockpit,
            [Tooltip("Only fighter cockpits, not the main ship or SRVs")]
            FighterCockpit,
            [Tooltip("Only SRV cockpits, not the main ship or fighters")]
            SRVCockpit,
        }

        public ButtonCategory category;

        protected SteamVR_Events.Event refreshEvent = null;

        // Get the text to use for tooltips
        public abstract string GetText();

        // Get the texture to use for the button
        public abstract Texture GetTexture();

        // Get the texture to use in the edit panel preview
        virtual public Sprite GetPreviewTexture()
        {
            var texture = GetTexture() as Texture2D;
            if (texture != null)
            {
                var rect = new Rect(0, 0, texture.width, texture.height);
                return Sprite.Create(texture, rect, Vector2.one * 0.5f);
            }
            return null;
        }

        // Get the control that should be used for activating and validating the button
        public abstract EDControlButton GetControl();

        // Get the default key combo that is used when a control is not bound
        public abstract KeyboardInterface.KeyCombo? GetDefaultKeycombo();

        // Listen for button updates
        public void AddRefreshListener(UnityEngine.Events.UnityAction OnRefresh)
        {
            if (refreshEvent == null)
            {
                refreshEvent = new SteamVR_Events.Event();
            }

            refreshEvent.Listen(OnRefresh);
        }

        // Remove button update listener
        public void RemoveRefreshListener(UnityEngine.Events.UnityAction OnRefresh)
        {
            if (refreshEvent != null)
            {
                refreshEvent.Remove(OnRefresh);
            }
        }

        // Trigger a refresh event
        protected void TriggerRefresh()
        {
            if (refreshEvent != null)
            {
                refreshEvent.Send();
            }
        }

        protected KeyboardInterface.KeyCombo? ParseKeycomboString(string keystring)
        {
            // @todo This is now called even when not actually using a default binding, perhaps we should memoize it
            if (keystring == null || keystring == "") return null;

            var keys = keystring.Split('+');

            string[] modifiers = null;
            if (keys.Length > 1)
            {
                modifiers = new string[keys.Length - 1];
                Array.Copy(keys, modifiers, modifiers.Length);
            }

            string key = keys[keys.Length - 1];

            return new KeyboardInterface.KeyCombo
            {
                key = key,
                modifiers = modifiers,
            };
        }
    }
}
