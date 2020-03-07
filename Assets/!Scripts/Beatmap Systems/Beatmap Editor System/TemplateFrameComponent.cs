using Beatmap.Editor.Commands;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Beatmap.Editor
{
    public class TemplateFrameComponent : EditorComponent
    {
        public TemplateFrameController TemplateFrameController;

        public Button TemplateCopyButton;
        public Button TemplatePasteButton;
        public Button ResetTemplateButton;

        private Frame _emptyFrameFlyweight;

        protected override void InitializeInternal()
        {
            _emptyFrameFlyweight = Editor.BeatmapWriter.TypeInstance.GetNewFrameWithDefaults();
            TemplateFrameController.Initialize(_emptyFrameFlyweight);
            TemplateFrameController.SetChannels(Editor.BeatmapWriter.TypeInstance);
        }
        protected override void SubscribeToEventsInternal()
        {
            TemplateCopyButton.onClick.AddListener(CopyToTemplate);
            TemplatePasteButton.onClick.AddListener(PasteFromTemplate);
            ResetTemplateButton.onClick.AddListener(ResetTemplate);
        }

        protected override void UnsubscribeFromEventsInternal()
        {
            TemplateCopyButton.onClick.RemoveListener(CopyToTemplate);
            TemplatePasteButton.onClick.RemoveListener(PasteFromTemplate);
            ResetTemplateButton.onClick.RemoveListener(ResetTemplate);
        }

        public void CopyToTemplate()
        {
            var sc = Editor.SelectionComponent;
            TemplateFrameController.Frame.SetValues(Editor.BeatmapWriter.GetFrameValues(sc.SelectionStart));
        }
        public void PasteFromTemplate()
        {
            var sc = Editor.SelectionComponent;
            
            var command = new TemplatePasteCommand(
                    Editor.BeatmapWriter,
                    () => new TemplatePasteCommand.Args(
                        sc.SelectionStart,
                        sc.SelectionLength,
                        Editor.SubdivisionComponent.CurrentSubdivision,
                        TemplateFrameController.Frame.GetValues())
                    );
            Editor.UndoStackComponent.ExecuteCommand(command);
            Editor.SetDirty();
        }
        public void ResetTemplate()
        {
            TemplateFrameController.Frame.SetValues(_emptyFrameFlyweight.GetValues());
        }
    }
}