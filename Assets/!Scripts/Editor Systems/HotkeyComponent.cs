using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KeyCombos;
using Editors;

namespace Editors
{
    public class HotkeyComponent : EditorComponent
    {
        private struct HotkeyFunction
        {
            public readonly string FunctionName;
            public readonly System.Action Action;
            public HotkeyFunction(string functionName, System.Action action)
            {
                FunctionName = functionName;
                Action = action;
            }
        }

        private readonly List<KeyValuePair<KeyCombo, HotkeyFunction>> _hotKeyRegistry = new List<KeyValuePair<KeyCombo, HotkeyFunction>>();

        public bool TryRegisterHotkey(string functionName, System.Action action, KeyCombo keyCombo)
        {
            // Toss out anything where the main key is a toggle key (not using the system correctly).
            foreach (var toggleKey in ToggleKey.GetAll())
            {
                if (toggleKey.Keys.Contains(keyCombo.MainKey))
                {
                    Game.Log(Logging.Category.SONG_DATA, $"Unable to register hotkey: Main key (Keycode.{keyCombo.MainKey}) is a reserved toggle key.",
                        Logging.Level.LOG_WARNING);
                    return false;
                }
            }

            if (_hotKeyRegistry.Exists(kvp => kvp.Key.Equals(keyCombo)))
            {
                // Keycode already registered.
                var existingHotkeyRegistryItem = _hotKeyRegistry.Find(kvp => kvp.Key.Equals(keyCombo));
                Game.LogFormat(Logging.Category.SONG_DATA,
                    "Unable to register hotkey: This key combo ({0}) is already reserved for the function: \"{1}\".",
                    Logging.Level.LOG_WARNING,
                    existingHotkeyRegistryItem.Key.GetSequenceString(),
                    existingHotkeyRegistryItem.Value.FunctionName);
                return false;
            }
            else
            {
                // Successful registration.
                var newHotkeyFunction = new HotkeyFunction(functionName, action);
                _hotKeyRegistry.Add(new KeyValuePair<KeyCombo, HotkeyFunction>(keyCombo, newHotkeyFunction));
            }
            return true;
        }
        private void Update()
        {
            var engagedToggleKeys = ToggleKey.GetEngagedToggleKeys();

            bool hotKeyIsPressed = false;
            KeyValuePair<KeyCombo, HotkeyFunction> primeSuspect = default;

            for (int i = 0; i < _hotKeyRegistry.Count; i++)
            {
                var currentItem = _hotKeyRegistry[i].Key;
                if (Input.GetKey(currentItem.MainKey))
                {
                    if (currentItem.GetKeyComboDown(engagedToggleKeys))
                    {
                        // If there is no hotkey detected yet, or if this one requires more toggle keys (Ctrl+Shift+S takes precedence over Ctrl+S)
                        if (hotKeyIsPressed == false || (currentItem.GetToggleKeys().Count > primeSuspect.Key.GetToggleKeys().Count))
                        {
                            primeSuspect = _hotKeyRegistry[i];
                            hotKeyIsPressed = true;
                        }
                    }
                }
            }
            
            if (hotKeyIsPressed)
            {
                primeSuspect.Value.Action?.Invoke();
            }
        }
        protected override void CleanUpInternal()
        {
            _hotKeyRegistry.Clear();
        }
    }
}