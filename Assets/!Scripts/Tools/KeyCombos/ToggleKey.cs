using System.Collections.Generic;
using UnityEngine;

namespace KeyCombos
{
    public struct ToggleKey
    {
        // Static functionality
        private static readonly List<ToggleKey> _all = new List<ToggleKey>();
        public static List<ToggleKey> GetAll() => new List<ToggleKey>(_all);
        public static List<ToggleKey> GetEngagedToggleKeys()
        {
            var engagedToggleKeys = new List<ToggleKey>();
            foreach (var toggleKey in _all)
            {
                if (toggleKey.IsCurrentlyPressed())
                    engagedToggleKeys.Add(toggleKey);
            }
            return engagedToggleKeys;
        }

        // Static Instances
        public static readonly ToggleKey Ctrl = new ToggleKey("Ctrl", KeyCode.LeftControl, KeyCode.RightControl);
        public static readonly ToggleKey Shift = new ToggleKey("Shift", KeyCode.LeftShift, KeyCode.RightShift);
        public static readonly ToggleKey Alt = new ToggleKey("Alt", KeyCode.LeftAlt, KeyCode.RightAlt);

        // Instance members.
        public string Name;
        public readonly List<KeyCode> Keys;
        public ToggleKey(string name, params KeyCode[] keys)
        {
            Name = name;
            Keys = new List<KeyCode>(keys);

            _all.Add(this);
        }
        public bool IsCurrentlyPressed()
        {
            foreach (var key in Keys)
            {
                if (Input.GetKey(key))
                    return true;
            }
            return false;
        }
    }
}