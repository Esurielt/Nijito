using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KeyCombos;
using SongData;

namespace Editors.BeatmapEditor
{
    public class SubdivisionComponent : EditorComponent<BeatmapEditor>
    {
        const int DEFAULT_INDEX = 3;    //1/16th notes

        public TMPro.TMP_Dropdown SubdivisionDropdown;

        public Subdivision CurrentSubdivision { get; private set; }

        private List<Subdivision> _allSubdivisions;

        protected override void InitializeInternal()
        {
            _allSubdivisions = Subdivision.GetAll();

            List<string> optionNames = _allSubdivisions.Select(
                subdivision => string.Format("{0} ({1})", subdivision.Name, subdivision.Description)
                ).ToList();

            SubdivisionDropdown.ClearOptions();
            SubdivisionDropdown.AddOptions(optionNames);
            SubdivisionDropdown.SetValueWithoutNotify(DEFAULT_INDEX);
            UpdateCurrentSubdivision(DEFAULT_INDEX);
        }
        public override void RegisterHotkeys()
        {
            RegisterSingleHotkey(0, KeyCode.Alpha1, ToggleKey.Ctrl);
            RegisterSingleHotkey(1, KeyCode.Alpha2, ToggleKey.Ctrl);
            RegisterSingleHotkey(2, KeyCode.Alpha3, ToggleKey.Ctrl);
            RegisterSingleHotkey(3, KeyCode.Alpha4, ToggleKey.Ctrl);
            RegisterSingleHotkey(4, KeyCode.Alpha5, ToggleKey.Ctrl);
            RegisterSingleHotkey(5, KeyCode.Alpha6, ToggleKey.Ctrl);
            RegisterSingleHotkey(6, KeyCode.Alpha7, ToggleKey.Ctrl);
            RegisterSingleHotkey(7, KeyCode.Alpha8, ToggleKey.Ctrl);
        }
        private void RegisterSingleHotkey(int subdivisionListIndex, KeyCode mainKey, params ToggleKey[] toggleKeys)
        {
            Editor.HotkeyComponent.TryRegisterHotkey(
                $"Set subdivision level to {_allSubdivisions[subdivisionListIndex].Name}",
                () => ForceUpdateCurrentSubdivision(subdivisionListIndex),
                new KeyCombo(mainKey, toggleKeys));
        }
        protected override void SubscribeToEventsInternal()
        {
            SubdivisionDropdown.onValueChanged.AddListener(UpdateCurrentSubdivision);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            SubdivisionDropdown.onValueChanged.RemoveAllListeners();
        }
        private void ForceUpdateCurrentSubdivision(int dropdownValue)
        {
            SubdivisionDropdown.value = dropdownValue;
        }
        protected void UpdateCurrentSubdivision(int dropdownValue)
        {
            CurrentSubdivision = Subdivision.GetByIndex(dropdownValue);
            Editor.SetDirty();
        }
    }
}