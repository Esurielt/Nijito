using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KeyCombos;

namespace Beatmap.Editor
{
    public class SubdivisionComponent : EditorComponent
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
            RegisterSingleHotkey(0, KeyCode.Alpha1);
            RegisterSingleHotkey(1, KeyCode.Alpha2);
            RegisterSingleHotkey(2, KeyCode.Alpha3);
            RegisterSingleHotkey(3, KeyCode.Alpha4);
            RegisterSingleHotkey(4, KeyCode.Alpha5);
            RegisterSingleHotkey(5, KeyCode.Alpha6);
            RegisterSingleHotkey(6, KeyCode.Alpha7);
            RegisterSingleHotkey(7, KeyCode.Alpha8);
        }
        private void RegisterSingleHotkey(int subdivisionListIndex, KeyCode mainKey)
        {
            Editor.HotkeyComponent.RegisterHotkey(
                $"Set subdivision level to {_allSubdivisions[subdivisionListIndex].Name}",
                () => ForceUpdateCurrentSubdivision(subdivisionListIndex),
                new KeyCombo(mainKey));
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