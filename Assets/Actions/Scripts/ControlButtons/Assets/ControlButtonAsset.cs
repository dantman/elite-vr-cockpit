using System;
using System.ComponentModel;
using UnityEngine;
using Valve.VR;

namespace EVRC.Core.Actions
{
    /**
     * Base class for assets that represent buttons that trigger keypresses based on
     * key bound in Elite Dangerous' custom bindings file.
     */
    abstract public class ControlButtonAsset : ScriptableObject
    {
        /// When placing a controlButton, these two filters values control where a controlButton can be placed. 
        /// Filters do not actually place the button and are not involved in any logic for the active button in the scene. Filters merely allow a button to be displayed in the placement interface.
        [Header("Placement Filters")]
        /// Elite Dangerous may have multiple status flags at a time. Most buttons will be "Everything" because there's no reason to limit their placement
        /// However, some buttons only apply when a certain flag is present, for example:
        ///     InSRV flag           => all of the "_Buggy" controls
        ///     LandingGearDown flag => all of the "_Landing" controls
        [Description("Filter the statusFlags where this ControlButton can be placed. This affects the placement feature in the Desktop UI. See ControlButtonAsset class for additional details.")]
        public EDStatusFlags statusFlagFilters;

        /// <summary>
        /// GuiFocus is not often used in combination with StatusFlags. For example, the Galaxy Map GuiFocus effectively overrides whatever ship mode, it looks the same whether the user is in the Mainship, Fighter, or SRV
        /// </summary>
        [Description("Add guiFocus values to this list if the controlButton requires a certain GuiFocus. Leave the list empty if any GuiFocus is acceptable. See ControlButtonAsset class for additional details.")]
        public EDGuiFocus[] guiFocusRequired; 

        protected SteamVR_Events.Event refreshEvent = null;

        // Get the text to use for tooltips
        public abstract string GetTooltipText();

        // Get the text for the label
        public abstract string GetLabelText();

        // Get the texture to use for the button
        public abstract Texture GetTexture();

        // Set a single texture to use for the button
        public abstract void SetTexture(Texture tex);
        
        // Set the on & off textures to use for the button
        public abstract void SetTexture(Texture onTexture, Texture offTexture);

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
