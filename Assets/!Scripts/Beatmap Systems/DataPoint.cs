using System.Collections.Generic;
using Beatmap.Interfaces;

namespace Beatmap
{
    /// <summary>
    /// A collection of all data at a given frame index in a beatmap (expected channel values and any beatmap reader modifications).
    /// </summary>
    public class DataPoint : IDataPoint
    {
        protected Frame _expectedFrame;
        protected List<ProcessorModifier> _modifiers;

        public DataPoint(IFrame expectedFrame, List<ProcessorModifier> modifiers)
        {
            _expectedFrame = new Frame(expectedFrame);
            _modifiers = new List<ProcessorModifier>(modifiers);
        }
        public DataPoint(IDataPoint other)
            :this(other.GetExpectedFrame(), other.GetModifiers()) { }

        public IFrame GetExpectedFrame() => _expectedFrame;
        public void SetExpectedFrame(IFrame frame) => _expectedFrame = new Frame(frame);

        public List<ProcessorModifier> GetModifiers() => _modifiers;
        public void AddModifier(ProcessorModifier modifier) => _modifiers.Add(modifier);
        public void AddModifiers(List<ProcessorModifier> modifiers) => _modifiers.AddRange(modifiers);
        public void RemoveModifier(ProcessorModifier modifier) => _modifiers.Remove(modifier);
        public void RemoveModifiers(List<ProcessorModifier> modifiers)
        {
            var removeList = new List<ProcessorModifier>(modifiers);
            _modifiers.RemoveAll(rm => removeList.Contains(rm));
        }

        public void ClearModifiers()
        {
            _modifiers.Clear();
        }

        public void ReplaceModifiers(List<ProcessorModifier> modifiers)
        {
            _modifiers = new List<ProcessorModifier>(modifiers);
        }
    }
}