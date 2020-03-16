using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editors
{
    public abstract class EditorComponent : MonoBehaviour
    {
        protected EditorBase WeakEditor { get; private set; }

        private bool _isInitialized = false;
        private bool _isSubscribed = false;

        public void Initialize(EditorBase editor)
        {
            _isInitialized = true;

            WeakEditor = editor;

            InitializeInternal();
        }        
        public virtual void RegisterHotkeys() { }
        protected void RegisterHotkey(string functionName, System.Action action, KeyCombos.KeyCombo keyCombo)
        {
            WeakEditor.HotkeyComponent.TryRegisterHotkey(functionName, action, keyCombo);
        }
        public void SubscribeToEvents()
        {
            if (_isInitialized && !_isSubscribed)
            {
                _isSubscribed = true;

                WeakEditor.OnRepaint.AddListener(Repaint);
                SubscribeToEventsInternal();
            }
        }
        public void UnsubscribeFromEvents()
        {
            if (_isInitialized && _isSubscribed)
            {
                _isSubscribed = false;

                WeakEditor.OnRepaint.RemoveListener(Repaint);
                UnsubscribeFromEventsInternal();
            }
        }
        private void Repaint()
        {
            // Room for some universal repaint stuff.

            RepaintInternal();
        }
        void OnDestroy()
        {
            CleanUpInternal();
        }

        protected virtual void InitializeInternal() { }
        protected virtual void SubscribeToEventsInternal() { }
        protected virtual void UnsubscribeFromEventsInternal() { }
        protected virtual void RepaintInternal() { }
        protected virtual void CleanUpInternal() { }
    }
    public class EditorComponent<TEditor> : EditorComponent where TEditor : EditorBase
    {
        public TEditor Editor => (TEditor)WeakEditor;
    }
}