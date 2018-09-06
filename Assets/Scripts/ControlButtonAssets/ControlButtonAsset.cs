using System;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using EDControlButton = EDControlsBindings.EDControlButton;

    /**
     * Base class for assets that represent buttons that trigger keypresses based on
     * key bound in Elite Dangerous' custom bindings file.
     */
    abstract public class ControlButtonAsset : ScriptableObject
    {
        public enum ButtonCategory
        {
            Cockpit,
            ShipCockpit,
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
        public abstract bool GetDefaultKeycombo(ref string key, ref string[] modifiers);

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

        protected bool ParseKeycomboString(string keystring, ref string key, ref string[] modifiers)
        {
            if (keystring == null || keystring == "") return false;

            var keys = keystring.Split('+');

            if (keys.Length > 1)
            {
                modifiers = new string[keys.Length - 1];
                Array.Copy(keys, modifiers, modifiers.Length);
            }
            else
            {
                modifiers = null;
            }

            key = keys[keys.Length - 1];

            return true;
        }
    }
}
