using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Editors.BeatmapEditor.Timeline
{
    public class AddTrackMarkerBox : MonoBehaviour
    {
        public TMP_InputField BeatIndexField;
        public TMP_Text SecondFieldLabelText;
        public TMP_InputField ValueField;
        public Button AddButton;
        public Button CancelButton;

        public void Initialize(string valueTypeName, TMP_InputField.CharacterValidation characterValidation)
        {
            SecondFieldLabelText.text = $"New {valueTypeName}";
            ValueField.characterValidation = characterValidation;

            CancelButton.onClick.AddListener(() => gameObject.SetActive(false));
        }
        private void OnDestroy()
        {
            AddButton.onClick.RemoveAllListeners();
            CancelButton.onClick.RemoveAllListeners();
        }
        public bool TryGetBeatIndex(out int beatIndex)
        {
            return int.TryParse(BeatIndexField.text, out beatIndex);
        }
        public string GetValueString()
        {
            return ValueField.text;
        }
    }
}