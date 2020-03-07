using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap.Editor
{
    public abstract class EditorComponent : MonoBehaviour
    {
        public BeatmapEditor Editor { get; private set; }

        private bool _isInitialized = false;
        private bool _isSubscribed = false;

        public void Initialize(BeatmapEditor editor)
        {
            _isInitialized = true;

            Editor = editor;
            InitializeInternal();
        }        
        public virtual void RegisterHotkeys() { }
        public void SubscribeToEvents()
        {
            if (_isInitialized && !_isSubscribed)
            {
                _isSubscribed = true;

                Editor.OnRepaint.AddListener(Repaint);
                SubscribeToEventsInternal();
            }
        }
        public void UnsubscribeFromEvents()
        {
            if (_isInitialized && _isSubscribed)
            {
                _isSubscribed = false;

                Editor.OnRepaint.RemoveListener(Repaint);
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
}