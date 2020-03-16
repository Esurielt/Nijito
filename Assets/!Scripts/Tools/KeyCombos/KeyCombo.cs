using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace KeyCombos
{
    public class KeyCombo : System.IEquatable<KeyCombo>
    {
        public KeyCode MainKey { get; private set; }

        protected readonly List<ToggleKey> _toggleKeys;
        public List<ToggleKey> GetToggleKeys() => new List<ToggleKey>(_toggleKeys);
        public int ToggleKeyCount => _toggleKeys.Count;

        public KeyCombo(KeyCode mainKey, params ToggleKey[] toggleKeys)
        {
            MainKey = mainKey;
            _toggleKeys = new List<ToggleKey>();
            foreach (var toggleKey in toggleKeys)
            {
                // Add only unique items.
                if (!_toggleKeys.Contains(toggleKey))
                {
                    _toggleKeys.Add(toggleKey);
                }
            }
        }
        public bool GetKeyComboDown() => GetKeyComboInternal(Input.GetKeyDown);
        public bool GetKeyComboUp() => GetKeyComboInternal(Input.GetKeyUp);
        public bool GetKeyComboIsPressed() => GetKeyComboInternal(Input.GetKey);
        public bool GetKeyComboDown(List<ToggleKey> engagedToggleKeys) => GetKeyComboInternal(Input.GetKeyDown, engagedToggleKeys);
        public bool GetKeyComboUp(List<ToggleKey> engagedToggleKeys) => GetKeyComboInternal(Input.GetKeyUp, engagedToggleKeys);
        public bool GetKeyComboIsPressed(List<ToggleKey> engagedToggleKeys) => GetKeyComboInternal(Input.GetKey, engagedToggleKeys);
        private bool GetKeyComboInternal(System.Predicate<KeyCode> getKeyFunction)
        {
            return GetKeyComboInternal(getKeyFunction, ToggleKey.GetEngagedToggleKeys());
        }
        private bool GetKeyComboInternal(System.Predicate<KeyCode> getKeyFunction, List<ToggleKey> engagedToggleKeys)
        {
            if (!getKeyFunction(MainKey))
                return false;

            foreach (var toggleKey in _toggleKeys)
            {
                if (!engagedToggleKeys.Contains(toggleKey))
                {
                    return false;
                }
            }

            return true;
        }
        public virtual bool Equals(KeyCombo other)
        {
            // Inefficient equality check, but calls will likely remain too infrequent to matter.

            return other != null &&
                other.MainKey == MainKey &&
                other._toggleKeys.Count == _toggleKeys.Count &&
                other._toggleKeys.TrueForAll(otherTk => _toggleKeys.Contains(otherTk));
        }
        public string GetSequenceString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _toggleKeys.Count; i++)
            {
                sb.Append(_toggleKeys[i].Name);
                sb.Append('+');
            }
            sb.Append(MainKey.ToString());
            return sb.ToString();
        }
        public override string ToString()
        {
            return GetSequenceString();
        }
    }
}