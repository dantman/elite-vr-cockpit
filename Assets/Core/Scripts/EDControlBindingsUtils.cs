using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using UnityEngine;

namespace EVRC.Core
{
    public static class EDControlBindingsUtils
    {

        public static Dictionary<EDControlButton, ControlButtonBinding> ParseFile(string customBindingsOptionsPath)
        {
            if (!File.Exists(customBindingsOptionsPath)) { throw new FileNotFoundException("bindings path not found"); }
            var bindings = new Dictionary<EDControlButton, ControlButtonBinding>();

            XDocument doc = XDocument.Load(customBindingsOptionsPath);
            foreach (XElement control in doc.Descendants())
            {
                if (!Enum.IsDefined(typeof(EDControlButton), control.Name.LocalName)) continue;

                var controlButton = (EDControlButton)Enum.Parse(typeof(EDControlButton), control.Name.LocalName);

                var controlBinding = new ControlButtonBinding
                {
                    Primary = ParseControlBinding(control, "Primary"),
                    Secondary = ParseControlBinding(control, "Secondary")
                };

                bindings.TryAdd(controlButton, controlBinding);

                // @todo Parse axis and options if we ever have a use for them
            }

            return bindings;
        }

        public static ControlButtonBinding.KeyBinding ParseControlBinding(XElement control, string nodeName)
        {
            var node = (from el in control.Descendants(nodeName) select el).First();
            var keyBinding = new ControlButtonBinding.KeyBinding
            {
                Device = GetAttributeValue(node, "Device"),
                Key = GetAttributeValue(node, "Key"),
                Modifiers = new HashSet<ControlButtonBinding.KeyModifier>(),
            };

            foreach (var modifier in node.Descendants("Modifier"))
            {
                keyBinding.Modifiers.Add(new ControlButtonBinding.KeyModifier
                {
                    Device = GetAttributeValue(modifier, "Device"),
                    Key = GetAttributeValue(modifier, "Key"),
                });
            }

            return keyBinding;
        }

        private static string GetAttributeValue(XElement el, string localName)
        {
            localName = localName.ToLowerInvariant();
            return el.Attributes().First(attr => attr.Name.LocalName.ToLowerInvariant() == localName).Value;
        }

        public static string EDControlFriendlyName(EDControlButton button)
        {
            string input = button.ToString();
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var result = new StringBuilder();
            var currentWord = new StringBuilder();
            char prevChar = char.MinValue;

            foreach (char c in input)
            {
                if (char.IsUpper(c))
                {
                    if (currentWord.Length > 0 && !char.IsUpper(prevChar))
                    {
                        result.Append(currentWord + " ");
                        currentWord.Clear();
                    }
                }
                else if (char.IsDigit(c))
                {
                    if (currentWord.Length > 0 && !char.IsDigit(prevChar))
                    {
                        result.Append(currentWord + " ");
                        currentWord.Clear();
                    }
                }
                else if (c == '_')
                {
                    if (currentWord.Length > 0)
                    {
                        result.Append(currentWord + " ");
                        currentWord.Clear();
                    }
                    continue;
                }
                else if (currentWord.Length >= 16)
                {
                    break;
                }

                currentWord.Append(c);
                prevChar = c;
            }

            if (currentWord.Length > 0)
            {
                result.Append(currentWord);
            }


            string MaybeShorten(string buttonName)
            {
                if (buttonName.Length < 20) { return buttonName; }

                List<(string, string)> replaceStrings = new List<(string, string)>()
                {
                    ("Exploration FSS", "FSS"),
                    ("Exploration SAA", "SAA"),
                    ("Button", "Btn"),
                    ("Commander", "Cmdr"),
                    ("Alternate", "Alt"),
                    ("Distribution", "Dist"),
                    ("Previous", "Prev"),
                    ("Combination", "Combo"),
                    ("Forward", "Fwd"),
                    ("Backward", "Back"),
                    ("Next", "Nxt"),
                    ("Power", "Pwr"),
                    ("Disable", "Kill"),
                    ("Increase", "Raise"),
                    ("Decrease", "Lower"),
                    ("Buggy", "SRV"),
                    ("Panel", "Pane"),
                    ("Vertical", "Vert"),
                    ("Cycle", ""),
                    ("Multi Crew", "Crew"),
                    ("Utility", "Util"),
                    ("Camera", "Cam"),
                    ("Reverse", "Flip"),
                    ("Aggressive", "Agg"),
                    ("Defensive", "Def"),
                    ("Lower", "Down"),
                    ("Toggle", ""),
                    ("Landing", ""),
                    ("Audio", ""),
                    ("Third Person", "Cam"),
                    ("Target", "Tgt"),
                    ("Increase", "Add"),
                    ("Decrease", "Less"),
                };

                for (int i = 0; i < replaceStrings.Count; i++)
                {
                    if (buttonName.Length < 20) { return buttonName; }
                    buttonName = buttonName.Replace(replaceStrings[i].Item1, replaceStrings[i].Item2);
                }
                return buttonName;

            }

            return MaybeShorten(result.ToString()).Trim();
        }

    }

}
