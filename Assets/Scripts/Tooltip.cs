using UnityEngine;
using Valve.VR;

namespace EVRC
{
    using Events = SteamVR_Events;

    public interface ITooltip
    {
        string GetTooltipText();
    }

    public class Tooltip : MonoBehaviour, ITooltip
    {
        [SerializeField]
        protected string text;
        [SerializeField]
        protected string suffix;

        public string Text
        {
            get { return text; }
            set { text = value; TooltipUpdate(this); }
        }

        public string Suffix
        {
            get { return suffix; }
            set { suffix = value; TooltipUpdate(this); }
        }

        public static Events.Event<ITooltip, string> TooltipUpdated = new Events.Event<ITooltip, string>();
        public static void TooltipUpdate(ITooltip tooltip)
        {
            TooltipUpdated.Send(tooltip, tooltip.GetTooltipText());
        }

        public string GetTooltipText()
        {
            if (suffix != null && suffix != "")
            {
                return string.Format("{0} ({1})", text, suffix);
            }

            return text;
        }
    }
}
