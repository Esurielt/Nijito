﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Beatmap.Interfaces;

namespace Beatmap
{
    /// <summary>
    /// A beatmap instance. This is the purely abstract concept of a beatmap, with no associated visual components (only data). This is created and passed around by the system to quickly trade data between other systems such as editors and the rhythm games themselves.
    /// </summary>
    public class Beatmap : IBeatmap
    {
        public BeatmapType TypeInstance { get; }
        public List<Frame> Frames { get; }

        public Beatmap(BeatmapType typeInstance)
        {
            TypeInstance = typeInstance;
            Frames = new List<Frame>();
        }
        public Beatmap(IBeatmap other) //clone ctor
        {
            TypeInstance = other.TypeInstance;
            Frames = new List<Frame>(other.Frames);
        }
    }
}