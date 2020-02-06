using System.Collections;
using UnityEngine;

namespace Beatmap
{
    /// <summary>
    /// Delegate for methods that modify the internal state of a beatmap reader.
    /// </summary>
    /// <param name="reader">the beatmap reader instance to modify</param>
    public delegate void ProcessorModifierDelegate(BeatmapProcessor reader);
    /// <summary>
    /// A modifier to the current state of a beatmap reader (e.g. setting a new tempo, marking a new song section, etc...)
    /// </summary>
    public class ProcessorModifier
    {
        /// <summary>
        /// Player-facing name of the processor modifier (e.g. "Set New BPM").
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Delegate method for modifying a beatmap processor instance (i.e. change current bpm, or change current section name).
        /// </summary>
        public ProcessorModifierDelegate Modifier { get; }

        //ctor
        public ProcessorModifier(string name, ProcessorModifierDelegate modifier)
        {
            Name = name;
            Modifier = modifier;
        }
    }
}