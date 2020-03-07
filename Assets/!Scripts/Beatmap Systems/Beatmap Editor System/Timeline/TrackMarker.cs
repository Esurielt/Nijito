using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Beatmap.Editor.Timeline
{
    public class TrackMarkerEvent : UnityEvent<string> { }
    public class TrackMarker : MonoBehaviour
    {
        [SerializeField] protected TMPro.TMP_Text ValueText;
        [SerializeField] protected Button ValueInputButton;
        [SerializeField] protected TMPro.TMP_InputField ValueInputField;

        public string StringValue
        {
            get => ValueText.text;
            set => ValueText.text = value;
        }

        [HideInInspector] public TrackMarkerEvent OnRequestSet;

        void Awake()
        {
            OnRequestSet = new TrackMarkerEvent();

            ValueInputButton.onClick.AddListener(OnClickValueInputButton);
            ValueInputField.onEndEdit.AddListener(OnEndEditValueInputField);

            LateAwake();
        }
        protected virtual void LateAwake() { }
        void OnDestroy()
        {
            OnRequestSet.RemoveAllListeners();

            ValueInputButton.onClick.RemoveAllListeners();
            ValueInputField.onEndEdit.RemoveAllListeners();

            LateOnDestroy();
        }
        protected virtual void LateOnDestroy() { }

        public void Initialize(string initialValue)
        {
            StringValue = initialValue;
        }
        public void OnClickValueInputButton()
        {
            // When the input field is opened, set the value to the same as the official value text, then activate the game object.
            ValueInputField.SetTextWithoutNotify(ValueText.text);
            ValueInputField.gameObject.SetActive(true);
        }
        public void OnEndEditValueInputField(string input)
        {
            // When the input field is done being edited, send the update event if a change was made.
            if (!string.IsNullOrEmpty(input) && !input.Equals(ValueText.text))
            {
                // Raise event that may or may not lead to visual update.
                OnRequestSet?.Invoke(input);
            }
            // Turn off the input field.
            ValueInputField.gameObject.SetActive(false);
        }
    }
}
