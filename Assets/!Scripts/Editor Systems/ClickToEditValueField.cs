using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

namespace Editors
{
    public class RequestSetValueEvent : UnityEvent<string> { }
    public class ClickToEditValueField : MonoBehaviour
    {
        [SerializeField] private TMP_Text _valueText;
        [SerializeField] private ClickToInputField _valueInputField;

        public RequestSetValueEvent OnRequestSetValue = new RequestSetValueEvent();

        protected virtual void Awake()
        {
            _valueInputField.BeginEditButton.onClick.AddListener(OnBeginInput);
            _valueInputField.InputField.onDeselect.AddListener(OnDeselect);
        }
        protected virtual void OnDestroy()
        {
            OnRequestSetValue.RemoveAllListeners();
        }
        public void SetValueText(string initialValue)
        {
            _valueText.text = initialValue;
        }
        protected void OnBeginInput()
        {
            _valueInputField.InputField.SetTextWithoutNotify(_valueText.text);
        }
        protected void OnDeselect(string input)
        {
            if (!string.Equals(_valueText.text, input, System.StringComparison.InvariantCulture))
                OnRequestSetValue?.Invoke(input);
        }
    }
}