﻿using Beatmap.Editor.Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Beatmap.Editor
{
    public class ClipboardComponent : EditorComponent
    {
        public Button CopyButton;
        public Button PasteButton;

        protected List<List<Channel.Value>> _clipboard = new List<List<Channel.Value>>();
        public int ClipboardCount => _clipboard.Count;

        protected override void SubscribeToEventsInternal()
        {
            CopyButton.onClick.AddListener(Copy);
            PasteButton.onClick.AddListener(Paste);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            CopyButton.onClick.RemoveAllListeners();
            PasteButton.onClick.RemoveAllListeners();
        }

        public void Copy()
        {
            var frameCount = Editor.BeatmapWriter.FrameCount;
            if (frameCount > 0)
            {
                var sc = Editor.SelectionComponent;
                _clipboard.Clear();
                _clipboard.AddRange(Editor.BeatmapWriter.GetFramesValues(sc.SelectionStart, sc.SelectionLength));
            }
        }
        public void Paste()
        {
            // Only paste if there is something in the frame clipboard.
            if (ClipboardCount > 0)
            {
                // Only paste if there is a single frame selected (paste at that index).

                var sc = Editor.SelectionComponent;
                if (sc.HasSingleSelection)
                {
                    // Check for enough space to paste.
                    if (ClipboardCount <= Editor.BeatmapWriter.FrameCount - sc.SelectionStart)
                    {
                        var command = new PasteFrameClipboardCommand(Editor.BeatmapWriter,
                            () => new PasteFrameClipboardCommand.Args(_clipboard, sc.SelectionStart));

                        Editor.UndoStackComponent.ExecuteCommand(command);
                        Editor.SetDirty();
                    }
                    else
                    {
                        Game.MessageBox("Paste Selection", $"Not enough room to paste {ClipboardCount} frames. Please add more beats.");
                    }
                }
            }
        }
    }
}