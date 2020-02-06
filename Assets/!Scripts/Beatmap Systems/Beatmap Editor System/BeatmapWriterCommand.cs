using System.Collections.Generic;
using System.Linq;

namespace Beatmap.Editor.Commands
{
    public interface IUndoableCommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }
    public abstract class BeatmapWriterCommand : IUndoableCommand
    {
        protected readonly AbstractBeatmapWriter _writer;
        public string Description { get; private set; }
        public BeatmapWriterCommand(AbstractBeatmapWriter writer, string description)
        {
            _writer = writer;
            Description = description;
        }
        public abstract void Execute();
        public abstract void Undo();
    }
    public abstract class BeatmapWriterCommand<TSavedInfo> : BeatmapWriterCommand
    {
        private TSavedInfo _savedInfo;
        public BeatmapWriterCommand(AbstractBeatmapWriter writer, string description)
            : base(writer, description) { }
        public override void Execute()
        {
            _savedInfo = ExecuteInWriter(_writer);
        }
        protected abstract TSavedInfo ExecuteInWriter(AbstractBeatmapWriter writer);    //return saved info

        public override void Undo()
        {
            UndoInWriter(_writer, _savedInfo);
        }
        protected abstract void UndoInWriter(AbstractBeatmapWriter writer, TSavedInfo savedInfo);
    }
    public class BeatmapWriterCommand_AdHoc<TSavedInfo> : BeatmapWriterCommand<TSavedInfo>
    {
        private readonly System.Func<AbstractBeatmapWriter, TSavedInfo> _executeInWriter;
        private readonly System.Action<AbstractBeatmapWriter, TSavedInfo> _undoInWriter;

        public BeatmapWriterCommand_AdHoc(AbstractBeatmapWriter writer, string description,
            System.Func<AbstractBeatmapWriter, TSavedInfo> executeInWriter,
            System.Action<AbstractBeatmapWriter, TSavedInfo> undoInWriter)
            :base(writer, description)
        {
            _executeInWriter = executeInWriter;
            _undoInWriter = undoInWriter;
        }
        protected override TSavedInfo ExecuteInWriter(AbstractBeatmapWriter writer) => _executeInWriter(writer);
        protected override void UndoInWriter(AbstractBeatmapWriter writer, TSavedInfo savedInfo) => _undoInWriter(writer, savedInfo);
    }
    public abstract class BeatmapWriterCommand_StaticArg<TSavedInfo, TStaticArg> : BeatmapWriterCommand<TSavedInfo>
    {
        protected readonly TStaticArg _argument;
        public BeatmapWriterCommand_StaticArg(AbstractBeatmapWriter writer, TStaticArg argument, string description)
            :base(writer, description)
        {
            _argument = argument;
        }
    }
    public delegate TDynamicArg DynamicValueGetter<TDynamicArg>();  //slightly more legible than System.Func<TDynamicArg>.
    public abstract class BeatmapWriterCommand_DynamicArg<TSavedInfo, TDynamicArg> : BeatmapWriterCommand_StaticArg<TSavedInfo, DynamicValueGetter<TDynamicArg>>
    {
        public BeatmapWriterCommand_DynamicArg(AbstractBeatmapWriter writer, DynamicValueGetter<TDynamicArg> argumentGetter, string description)
            : base(writer, argumentGetter, description) { }
    }
    public class InsertBlanksBeatsAtEndCommand : BeatmapWriterCommand_DynamicArg<int, int>
    {
        //argument gets the selected number of beats to insert
        //saved info is the number of beats that were inserted

        public InsertBlanksBeatsAtEndCommand(AbstractBeatmapWriter writer, DynamicValueGetter<int> argumentGetter)
            :base(writer, argumentGetter, string.Format("Added {0} beats at end", argumentGetter)) { }

        protected override int ExecuteInWriter(AbstractBeatmapWriter writer)
        {
            int currentArgValue = _argument();
            writer.InsertBlankBeatsAtEnd(currentArgValue);
            return currentArgValue;
        }

        protected override void UndoInWriter(AbstractBeatmapWriter writer, int savedInfo)
        {
            writer.RemoveBeatsFromEnd(savedInfo);
        }
    }
    public class RemoveBeatsFromEndCommand : BeatmapWriterCommand_DynamicArg<List<DataPoint>, int>
    {
        public RemoveBeatsFromEndCommand(AbstractBeatmapWriter writer, DynamicValueGetter<int> argumentGetter)
            :base(writer, argumentGetter, string.Format("Removed {0} beats from end", argumentGetter)) { }

        protected override List<DataPoint> ExecuteInWriter(AbstractBeatmapWriter writer)
        {
            int currentArgValue = _argument();
            var removedBeats = writer.GetBeatsAtEnd(currentArgValue);
            writer.RemoveBeatsFromEnd(currentArgValue);
            return removedBeats.Select(controller => new DataPoint(controller)).ToList();
        }
        protected override void UndoInWriter(AbstractBeatmapWriter writer, List<DataPoint> savedInfo)
        {
            writer.InsertDataPointsAtEnd(savedInfo.ToList<Interfaces.IDataPoint>());
        }
    }
}
