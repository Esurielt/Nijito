using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Editors.SongMetadataEditor
{
    public class SongMetadataValueControllerEvent : UnityEvent<string, string> { }
    public class SongMetadataValueController : ClickToEditValueField
    {
        public TMP_Text CategoryText;

        public string Category { get; private set; }

        [HideInInspector] public SongMetadataValueControllerEvent OnRequestSetMetadata;

        protected override void Awake()
        {
            base.Awake();
            OnRequestSetMetadata = new SongMetadataValueControllerEvent();
        }
        public void Initialize(string category, string initialValue)
        {
            Category = category;
            
            CategoryText.text = category.ToString();
            SetValueText(initialValue);

            OnRequestSetValue.AddListener(input => OnRequestSetMetadata?.Invoke(Category, input));
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnRequestSetMetadata.RemoveAllListeners();
        }
        public void DoSetValue(string newValue)
        {
            SetValueText(newValue);
        }
    }
}