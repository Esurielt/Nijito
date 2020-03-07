using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Beatmap.Editor
{
    public class SubdivisionComponent : EditorComponent
    {
        const int DEFAULT_INDEX = 3;    //1/16th notes

        public TMPro.TMP_Dropdown SubdivisionDropdown;

        public Subdivision CurrentSubdivision { get; private set; }

        protected override void InitializeInternal()
        {
            List<string> optionNames = Subdivision.GetAll().Select(
                subdivision => string.Format("{0} ({1})", subdivision.Name, subdivision.Description)
                ).ToList();

            SubdivisionDropdown.ClearOptions();
            SubdivisionDropdown.AddOptions(optionNames);
            SubdivisionDropdown.SetValueWithoutNotify(DEFAULT_INDEX);
            UpdateCurrentSubdivision(DEFAULT_INDEX);
        }
        protected override void SubscribeToEventsInternal()
        {
            SubdivisionDropdown.onValueChanged.AddListener(UpdateCurrentSubdivision);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            SubdivisionDropdown.onValueChanged.RemoveAllListeners();
        }
        protected void UpdateCurrentSubdivision(int dropdownValue)
        {
            CurrentSubdivision = Subdivision.GetByIndex(dropdownValue);
            Editor.SetDirty();
        }
    }
}