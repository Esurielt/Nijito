using SongData;
using System.Collections.Generic;
using System.Linq;
using UndoableCommands;

namespace Editors.BeatmapEditor.Commands
{
    public class InsertBlanksBeatsAtEndCommand : UndoableCommand_DynamicArg<BeatmapWriter, int, int>
    {
        // Technically,
        // Argument gets the selected number of beats to insert.
        // Saved info is the number of beats that were inserted.
        // But this is moot.

        public override string Description => $"Added {SavedInfo} beats at end";

        public InsertBlanksBeatsAtEndCommand(BeatmapWriter writer, DynamicValueGetter<int> argumentGetter)
            :base(writer, argumentGetter) { }

        protected override int ExecuteInContext(BeatmapWriter writer)
        {
            writer.InsertBlankBeatsAtEnd(Argument);
            return Argument;
        }
        protected override void UndoInContext(BeatmapWriter writer, int savedInfo)
        {
            writer.RemoveBeatsFromEnd(savedInfo);
        }
    }
    public class RemoveBeatsFromEndCommand : UndoableCommand_DynamicArg<BeatmapWriter, List<Frame>, int>
    {
        public override string Description => $"Removed {SavedInfo.Count} beats from end";
        public RemoveBeatsFromEndCommand(BeatmapWriter writer, DynamicValueGetter<int> argumentGetter)
            :base(writer, argumentGetter) { }

        protected override List<Frame> ExecuteInContext(BeatmapWriter writer)
        {
            var removedBeats = writer.GetBeatsAtEnd(Argument);
            writer.RemoveBeatsFromEnd(Argument);
            return removedBeats.ToList();
        }
        protected override void UndoInContext(BeatmapWriter writer, List<Frame> savedInfo)
        {
            writer.InsertFramesAtEnd(savedInfo.ToList());
        }
    }
    public class PasteFrameClipboardCommand : UndoableCommand_DynamicArg<BeatmapWriter, List<List<Channel.Value>>, PasteFrameClipboardCommand.Args>
    {
        public struct Args
        {
            public readonly List<List<Channel.Value>> ClipboardValues;  // <- can't pass Frames, since they would point to the changed values after changing
            public readonly int StartingIndex;
            public Args(List<List<Channel.Value>> clipboardValues, int startingIndex)
            {
                ClipboardValues = clipboardValues;
                StartingIndex = startingIndex;
            }
        }
        public override string Description => $"Set values of {SavedInfo.Count} frames starting at index {Argument.StartingIndex}";
        public PasteFrameClipboardCommand(BeatmapWriter writer, DynamicValueGetter<Args> argumentGetter)
            :base(writer, argumentGetter) { }

        protected override List<List<Channel.Value>> ExecuteInContext(BeatmapWriter writer)
        {
            var originalFramesValues = writer.GetFramesValues(Argument.StartingIndex, Argument.ClipboardValues.Count);  // <- outer list count
            writer.SetFramesValues(Argument.StartingIndex, Argument.ClipboardValues);
            return originalFramesValues;
        }
        protected override void UndoInContext(BeatmapWriter writer, List<List<Channel.Value>> savedInfo)
        {
            writer.SetFramesValues(Argument.StartingIndex, savedInfo);
        }
    }
    public class TemplatePasteCommand : UndoableCommand_DynamicArg<BeatmapWriter, List<List<Channel.Value>>, TemplatePasteCommand.Args>
    {
        public override string Description =>
            $"Pasted template frame starting at frame index {Argument.StartingFrameIndex} through {Argument.StartingFrameIndex + Argument.Length - 1} at subdivision \"{Argument.Subdivision.Name}\"";

        public struct Args
        {
            public readonly int StartingFrameIndex;
            public readonly int Length;
            public readonly Subdivision Subdivision;
            public readonly List<Channel.Value> TemplateFrameValues;
            public Args(int startingFrameIndex, int length, Subdivision subdivision, List<Channel.Value> templateFrameValues)
            {
                StartingFrameIndex = startingFrameIndex;
                Length = length;
                Subdivision = subdivision;
                TemplateFrameValues = templateFrameValues;
            }
        }
        public TemplatePasteCommand(BeatmapWriter writer, DynamicValueGetter<Args> argumentGetter)
            :base(writer, argumentGetter) { }

        protected override List<List<Channel.Value>> ExecuteInContext(BeatmapWriter writer)
        {
            var originalFrames = writer.GetFramesValues(Argument.StartingFrameIndex, Argument.Length);
            writer.SetFramesValues(Argument.StartingFrameIndex, Argument.Length, Argument.TemplateFrameValues, Argument.Subdivision);
            return originalFrames;
        }
        protected override void UndoInContext(BeatmapWriter writer, List<List<Channel.Value>> savedInfo)
        {
            writer.SetFramesValues(Argument.StartingFrameIndex, savedInfo);
        }
    }
    public class SetChannelValueCommand : UndoableCommand_DynamicArg<BeatmapWriter, Channel.Value, SetChannelValueCommand.Args>
    {
        public override string Description => $"Set the value of channel index {Argument.ChannelIndex} for frame index {Argument.FrameIndex} to {Argument.Value.Name}";

        public struct Args
        {
            public readonly int FrameIndex;
            public readonly int ChannelIndex;
            public readonly Channel.Value Value;
            public Args(int frameIndex, int channelIndex, Channel.Value value)
            {
                FrameIndex = frameIndex;
                ChannelIndex = channelIndex;
                Value = value;
            }
        }
        public SetChannelValueCommand(BeatmapWriter writer, DynamicValueGetter<Args> argumentGetter)
            :base(writer, argumentGetter) { }

        protected override Channel.Value ExecuteInContext(BeatmapWriter writer)
        {
            var originalValue = writer.GetValue(Argument.FrameIndex, Argument.ChannelIndex);
            writer.SetChannelValue(Argument.FrameIndex, Argument.ChannelIndex, Argument.Value);
            return originalValue;
        }
        protected override void UndoInContext(BeatmapWriter writer, Channel.Value savedInfo)
        {
            writer.SetChannelValue(Argument.FrameIndex, Argument.ChannelIndex, savedInfo);
        }
    }
    public class TimelineAddTrackMarkerCommand<TValue> : UndoableCommand_DynamicArg<AudioMetadata.ValueHelper<TValue>, object, TimelineAddTrackMarkerCommand<TValue>.Args>
    {
        public override string Description => $"Added value \"{Argument.Value.ToString()}\" to audio metadata at beat index {Argument.BeatIndex}";

        public struct Args
        {
            public readonly int BeatIndex;
            public readonly TValue Value;
            public Args(int beatIndex, TValue value)
            {
                BeatIndex = beatIndex;
                Value = value;
            }
        }
        public TimelineAddTrackMarkerCommand(AudioMetadata.ValueHelper<TValue> context, DynamicValueGetter<Args> argumentGetter)
            :base(context, argumentGetter) { }

        protected override object ExecuteInContext(AudioMetadata.ValueHelper<TValue> context)
        {
            context.TryAddValue(Argument.BeatIndex, Argument.Value);
            return null;
        }

        protected override void UndoInContext(AudioMetadata.ValueHelper<TValue> context, object savedInfo)
        {
            context.TryRemoveValue(Argument.BeatIndex);
        }
    }
    public class TimelineSetTrackMarkerCommand<TValue> : UndoableCommand_DynamicArg<AudioMetadata.ValueHelper<TValue>, TValue, TimelineSetTrackMarkerCommand<TValue>.Args>
    {
        public override string Description => $"Set audio metadata value at beat index {Argument.BeatIndex} to \"{Argument.Value.ToString()}\"";

        public struct Args
        {
            public readonly int BeatIndex;
            public readonly TValue Value;
            public Args(int beatIndex, TValue value)
            {
                BeatIndex = beatIndex;
                Value = value;
            }
        }
        public TimelineSetTrackMarkerCommand(AudioMetadata.ValueHelper<TValue> context, DynamicValueGetter<Args> argumentGetter)
            :base(context, argumentGetter) { }

        protected override TValue ExecuteInContext(AudioMetadata.ValueHelper<TValue> context)
        {
            var oldValue = context.GetCurrentValue(Argument.BeatIndex);
            context.TrySetValue(Argument.BeatIndex, Argument.Value);
            return oldValue;
        }

        protected override void UndoInContext(AudioMetadata.ValueHelper<TValue> context, TValue savedInfo)
        {
            context.TrySetValue(Argument.BeatIndex, savedInfo);
        }
    }
    public class TimelineRemoveTrackMarkerCommand<TValue> : UndoableCommand_DynamicArg<AudioMetadata.ValueHelper<TValue>, TValue, int>
    {
        public override string Description => $"Removed audio metadata value at beat index {Argument.ToString()}";

        public TimelineRemoveTrackMarkerCommand(AudioMetadata.ValueHelper<TValue> context, DynamicValueGetter<int> argumentGetter)
            :base(context, argumentGetter) { }

        protected override TValue ExecuteInContext(AudioMetadata.ValueHelper<TValue> context)
        {
            var value = context.GetCurrentValue(Argument);
            context.TryRemoveValue(Argument);
            return value;
        }

        protected override void UndoInContext(AudioMetadata.ValueHelper<TValue> context, TValue savedInfo)
        {
            context.TryAddValue(Argument, savedInfo);
        }
    }
    public class TimelineSetStartingValueCommand<TValue> : UndoableCommand_DynamicArg<AudioMetadata.ValueHelper<TValue>, TValue, TValue>
    {
        public override string Description => $"Set audio metadata starting value to {Argument.ToString()}";

        public TimelineSetStartingValueCommand(AudioMetadata.ValueHelper<TValue> context, DynamicValueGetter<TValue> argumentGetter)
            :base(context, argumentGetter) { }

        protected override TValue ExecuteInContext(AudioMetadata.ValueHelper<TValue> context)
        {
            var originalValue = context.StartingValue;
            context.StartingValue = Argument;
            return originalValue;
        }

        protected override void UndoInContext(AudioMetadata.ValueHelper<TValue> context, TValue savedInfo)
        {
            context.StartingValue = savedInfo;
        }
    }
    public class SetStartingDelayCommand : UndoableCommand_DynamicArg<AudioMetadata, float, float>
    {
        public override string Description => $"Set starting delay to {Argument}";

        public SetStartingDelayCommand(AudioMetadata context, DynamicValueGetter<float> argumentGetter)
            :base(context, argumentGetter) { }

        protected override float ExecuteInContext(AudioMetadata context)
        {
            var originalStartingDelay = context.StartingDelay;
            context.StartingDelay = Argument;
            return originalStartingDelay;
        }

        protected override void UndoInContext(AudioMetadata context, float savedInfo)
        {
            context.StartingDelay = savedInfo;
        }
    }
}