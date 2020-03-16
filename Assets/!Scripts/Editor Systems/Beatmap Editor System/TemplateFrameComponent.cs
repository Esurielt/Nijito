using SongData;
using Editors.BeatmapEditor.Commands;
using KeyCombos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editors.BeatmapEditor
{
    public class TemplateFrameComponent : EditorComponent<BeatmapEditor>
    {
        public TemplateFrameController TemplateFrameController;

        public Button TemplateCopyButton;
        public Button TemplatePasteButton;
        public Button ResetTemplateButton;

        private List<Channel.Value> _emptyFrameValues;

        protected override void InitializeInternal()
        {
            _emptyFrameValues = Editor.BeatmapWriter.TypeInstance.GetNewFrameWithDefaults().GetValues();
            TemplateFrameController.Initialize(new Frame(_emptyFrameValues));
            TemplateFrameController.SetChannels(Editor.BeatmapWriter.TypeInstance);
        }
        public override void RegisterHotkeys()
        {
            Editor.HotkeyComponent.TryRegisterHotkey("Template Frame Copy", () => CopyToTemplate(), new KeyCombo(KeyCode.C, ToggleKey.Ctrl, ToggleKey.Shift));
            Editor.HotkeyComponent.TryRegisterHotkey("Template Frame Paste", () => PasteFromTemplate(), new KeyCombo(KeyCode.V, ToggleKey.Ctrl, ToggleKey.Shift));
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
            TemplateFrameController.Frame.SetValues(_emptyFrameValues);
        }
    }
}