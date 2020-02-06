using UnityEngine;
using Beatmap.Interfaces;

namespace Beatmap.Editor
{
    public class EditorVisualsWriter : AbstractBeatmapWriter
    {
        protected BeatmapEditor _editor;
        public EditorVisualsWriter(BeatmapEditor editor, IBeatmap beatmap)
            :base(beatmap)
        {
            _editor = editor;
        }

        protected override IDataPoint InstantiateDataPointInstance(IDataPoint initializeValuesFrom, int frameIndex)
        {
            var controller = Object.Instantiate(_editor.DataPointControllerPrefab, _editor.DataPointContainer);
            controller.transform.SetSiblingIndex(frameIndex);
            controller.Initialize(initializeValuesFrom);
            
            _editor.OnDataPointInstantiated(controller);
            
            _editor.SetDirty();
            return controller;
        }
        protected override void DestroyDataPointInstance(IDataPoint dataPoint)
        {
            //maybe use object pooling for performance if we need it

            var controller = dataPoint as EditorDataPointController;

            Object.Destroy(controller.gameObject);  //<-- have to destroy the object, not the controller!

            _editor.OnDataPointDestroyed(controller);
            _editor.SetDirty();
        }
        public void ForEachDataPointController(System.Action<EditorDataPointController> forEach)
        {
            foreach (var dataPoint in _beatmap.DataPoints)
            {
                var controller = dataPoint as EditorDataPointController;
                forEach(controller);
            }
        }
    }
}
