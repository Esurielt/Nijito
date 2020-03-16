using SongData;
using SongData.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editors.SongMetadataEditor
{
    public class SongMetadataComponent : EditorComponent<SongMetadataEditor>
    {
        public Transform SongMetadataValueControllerContainer;
        public SongMetadataValueController SongMetadataValueControllerPrefab;

        public SongMetadata SongMetadata { get; private set; }

        private readonly List<SongMetadataValueController> _valueControllers = new List<SongMetadataValueController>();

        protected override void InitializeInternal()
        {
            SongMetadata = Editor.IOComponent.LoadData<SongMetadata>(SongDataFileUtility.TryGetSongMetadata, "song metadata");
            if (SongMetadata == null)
            {
                SongMetadata = new SongMetadata();
            }

            foreach (var kvp in SongMetadata.GetAllValuesCopy())
            {
                SpawnController(kvp);
            }
        }
        protected override void SubscribeToEventsInternal()
        {
            SongMetadata.OnSetValue += DoSetMetadata;
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            SongMetadata.OnSetValue -= DoSetMetadata;
        }
        private void SpawnController(KeyValuePair<string, string> kvp)
        {
            var controller = Instantiate(SongMetadataValueControllerPrefab, SongMetadataValueControllerContainer);
            
            controller.Initialize(kvp.Key, kvp.Value);
            
            controller.OnRequestSetMetadata.AddListener(TrySetMetadata);

            _valueControllers.Add(controller);
        }
        private void TrySetMetadata(string category, string value)
        {
            // Checks? ¯\_(ツ)_/¯

            SongMetadata.SetValue(category, value);
        }
        private void DoSetMetadata(string category, string value)
        {
            var foundController = _valueControllers.Find(controller => controller.Category == category);
            if (foundController != null)
            {
                foundController.DoSetValue(value);
            }
        }
    }
}