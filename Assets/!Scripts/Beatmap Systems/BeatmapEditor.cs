using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    public abstract class BeatmapEditor<TWriter> : MonoBehaviour where TWriter : BeatmapWriter
    {
        protected TWriter _beatmapWriter;
        protected IBeatmapFileIOHelper _fileIOHelper;

        public RectTransform UiContainer;

        private void OnEnable()
        {
            SubscribeToEvents();
        }
        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }

        public void Initialize(TWriter beatmapWriter, IBeatmapFileIOHelper fileIOHelper)
        {
            _beatmapWriter = beatmapWriter;
            _fileIOHelper = fileIOHelper;

            PopulateEditor();
        }
        protected abstract void SubscribeToEvents();
        protected abstract void UnsubscribeFromEvents();
        public abstract void PopulateEditor();
    }
}
