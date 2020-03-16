using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    public class ClickToInputField : MonoBehaviour
    {
        public Button BeginEditButton;
        public TMP_InputField InputField;

        protected virtual void Awake()
        {
            BeginEditButton.onClick.AddListener(BeginEdit);
            InputField.onDeselect.AddListener(str => EndEdit());

            InputField.gameObject.SetActive(false);
        }
        protected virtual void OnDestroy()
        {
            BeginEditButton.onClick.RemoveAllListeners();
            InputField.onDeselect.RemoveAllListeners();
        }        
        private void BeginEdit()
        {
            InputField.gameObject.SetActive(true);
            InputField.Select();
        }
        private void EndEdit()
        {
            InputField.gameObject.SetActive(false);
        }
    }
}
