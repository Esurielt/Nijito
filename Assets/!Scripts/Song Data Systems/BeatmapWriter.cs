using System.Collections.Generic;
using UnityEngine;

namespace SongData
{
    public class BeatmapWriter
    {
        //fields
        public readonly Beatmap Beatmap;
        public BeatmapType TypeInstance => Beatmap.TypeInstance;
        public Beatmap GetAsNewBeatmap()
        {
            return new Beatmap(Beatmap);
        }
        public int FrameCount => Beatmap.Frames.Count;
        
        //ctor
        public BeatmapWriter(Beatmap beatmap)
        {
            Beatmap = beatmap;
            DefaultFrame = beatmap.TypeInstance.GetNewFrameWithDefaults();
        }

        //abstract methods
        /// <summary>
        /// Redirect to a flyweight reference for a data point considered in a 'default' state.
        /// </summary>
        protected Frame DefaultFrame { get; }

        public event System.Action<Frame, int> OnFrameCreated;
        public event System.Action<int> OnFrameDestroyed;

        //getters
        public Channel.Value GetValue(int frameIndex, int channelIndex)
        {
            if (FrameExists(frameIndex))
            {
                return Beatmap.Frames[frameIndex].GetValue(channelIndex);
            }
            return null;
        }
        public List<Channel.Value> GetFrameValues(int frameIndex)
        {
            if (FrameExists(frameIndex))
            {
                return Beatmap.Frames[frameIndex].GetValues();
            }
            return null;
        }
        public Frame GetFrame(int frameIndex)
        {
            if (FrameExists(frameIndex))
            {
                return Beatmap.Frames[frameIndex];
            }
            return null;
        }
        public List<List<Channel.Value>> GetFramesValues(int startingFrameIndex, int frameCount)
        {
            if (FramesExist(startingFrameIndex, frameCount))
            {
                var twoDimensionalList = new List<List<Channel.Value>>();
                for (int i = 0; i < frameCount; i++)
                {
                    twoDimensionalList.Add(Beatmap.Frames[startingFrameIndex + i].GetValues());
                }
                return twoDimensionalList;
            }
            return null;
        }
        public List<Frame> GetFrames(int startingFrameIndex, int frameCount)
        {
            if (FramesExist(startingFrameIndex, frameCount))
                return Beatmap.Frames.GetRange(startingFrameIndex, frameCount);
            return null;
        }
        public List<Frame> GetBeats(int startingBeatIndex, int beatCount)
        {
            return GetFrames(B2F(startingBeatIndex), B2F(beatCount));
        }
        public List<Frame> GetFramesAtEnd(int frameCount)
        {
            return GetFrames(Beatmap.Frames.Count - frameCount, frameCount);
        }
        public List<Frame> GetBeatsAtEnd(int beatCount)
        {
            return GetFramesAtEnd(B2F(beatCount));
        }
        public int GetIndexOfFrame(Frame frame)
        {
            return Beatmap.Frames.IndexOf(frame);
        }

        //setters
        public void SetChannelValue(int frameIndex, int channelIndex, Channel.Value newValue)
        {
            if (FrameExists(frameIndex))
            {
                if (TypeInstance.ChannelFlyweights[channelIndex].ValueFlyweights.Contains(newValue))
                {
                    Beatmap.Frames[frameIndex].SetValue(channelIndex, newValue);
                }
            }
        }
        public void SetFrameValues(int frameIndex, List<Channel.Value> values)
        {
            if (FrameExists(frameIndex))
            {
                Beatmap.Frames[frameIndex].SetValues(values);
            }
        }
        public void SetFrameValues(int frameIndex, Frame fromFrame)
        {
            SetFrameValues(frameIndex, fromFrame.GetValues());
        }
        public void SetFramesValues(int startingFrameIndex, int frameCount, List<Channel.Value> singleFrameValues, Subdivision subdivision)
        {
            // If null subdivision was supplied, use the framerate.
            // Unity doesn't accept { subdivision ??= Subdivision.Framerate; }.
            if (subdivision == null)
            {
                subdivision = Subdivision.Framerate;
            }

            for (int i = startingFrameIndex; i < startingFrameIndex + frameCount; i++)
            {
                if (subdivision.IncludesIndex(i))
                {
                    SetFrameValues(i, singleFrameValues);
                }
            }
        }
        public void SetFramesValues(int startingFrameIndex, int frameCount, Frame fromSingleFrame, Subdivision subdivision)
        {
            SetFramesValues(startingFrameIndex, frameCount, fromSingleFrame.GetValues(), subdivision);
        }
        public void SetFramesValues(int startingFrameIndex, List<List<Channel.Value>> values)
        {
            for (int i = 0; i < values.Count; i++)  // <- outer list count (frame indexes)
            {
                SetFrameValues(startingFrameIndex + i, values[i]);
            }
        }
        public void SetFramesValues(int startingFrameIndex, List<Frame> fromFrames)
        {
            var twoDimensionalList = new List<List<Channel.Value>>();
            foreach (var frame in fromFrames)
            {
                twoDimensionalList.Add(new List<Channel.Value>(frame.GetValues()));
            }
            SetFramesValues(startingFrameIndex, twoDimensionalList);
        }

        //resetters
        public void ResetFrameToDefault(int frameIndex)
        {
            SetFrameValues(frameIndex, DefaultFrame);
        }
        public void ResetFramesToDefault(int startingFrameIndex, int frameCount)
        {
            SetFramesValues(startingFrameIndex, frameCount, DefaultFrame, Subdivision.Framerate);
        }
        public void ResetBeatsToDefault(int startingBeatIndex, int beatCount)
        {
            ResetFramesToDefault(B2F(startingBeatIndex), B2F(beatCount));
        }
        
        //insert (create)
        protected void InsertFrame(Frame fromFrame, int frameIndex)
        {
            if (FrameExists(frameIndex) || frameIndex == Beatmap.Frames.Count) //allow for insertion at the very end
            {
                Frame newFrame = new Frame(fromFrame);
                Beatmap.Frames.Insert(frameIndex, newFrame);
                OnFrameCreated?.Invoke(newFrame, frameIndex);
            }
        }
        public void InsertFrames(Frame fromSingleFrame, int startingFrameIndex, int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
            {
               InsertFrame(fromSingleFrame, startingFrameIndex + i);
            }
        }
        public void InsertFrames(List<Frame> fromFrames, int startingFrameIndex)
        {
            for (int i = 0; i < fromFrames.Count; i++)
            {
                InsertFrame(fromFrames[i], startingFrameIndex + i);
            }
        }
        public void InsertFramesAtEnd(Frame fromSingleFrame, int frameCount)
        {
            InsertFrames(fromSingleFrame, Beatmap.Frames.Count, frameCount);
        }
        public void InsertFramesAtEnd(List<Frame> fromFrames)
        {
            InsertFrames(fromFrames, Beatmap.Frames.Count);
        }

        //insert (blank)
        protected void InsertBlankFrames(int startingFrameIndex, int frameCount)
        {
            InsertFrames(DefaultFrame, startingFrameIndex, frameCount);
        }        
        public void InsertBlankBeats(int startingBeatIndex, int beatCount)
        {
            InsertBlankFrames(B2F(startingBeatIndex), B2F(beatCount));
        }
        public void InsertBlankBeatsAtEnd(int beatCount)
        {
            InsertBlankFrames(Beatmap.Frames.Count, B2F(beatCount));
        }

        //remove
        protected void RemoveFrame(int frameIndex)
        {
            if (FrameExists(frameIndex))
            {
                Frame frame = Beatmap.Frames[frameIndex];
                Beatmap.Frames.RemoveAt(frameIndex);
                OnFrameDestroyed?.Invoke(frameIndex);
            }            
        }
        protected void RemoveFrames(int startingFrameIndex, int frameCount)
        {
            for (int i = 0; i < frameCount; i++)    //<-- there is nothing fancy here. Each will be removed from the same index in the collection as it readjusts.
            {
                RemoveFrame(startingFrameIndex);
            }
        }
        public void RemoveBeats(int startingBeatIndex, int beatCount)
        {
            RemoveFrames(B2F(startingBeatIndex), B2F(beatCount));
        }
        public void RemoveBeatsFromEnd(int beatCount)
        {
            var frameCount = B2F(beatCount);
            RemoveFrames(Beatmap.Frames.Count - frameCount, frameCount);   //<-- watch out here
        }
        
        //move
        public void MoveFrame(int startingFrameIndex, int destinationFrameIndex)
        {
            if (FrameExists(startingFrameIndex))
            {
                var dataPoint = Beatmap.Frames[startingFrameIndex];
                RemoveFrame(startingFrameIndex);

                InsertFrame(dataPoint, destinationFrameIndex);
            }
        }
        public void MoveFrames(int startingFrameIndex, int destinationFrameIndex, int frameCount)
        {
            var frames = GetFrames(startingFrameIndex, frameCount);

            if (frames != null)
            {
                RemoveFrames(startingFrameIndex, frameCount);

                InsertFrames(frames, destinationFrameIndex);
            }
        }
        public void MoveBeats(int startingBeatIndex, int destinationBeatIndex, int beatCount)
        {
            MoveFrames(B2F(startingBeatIndex), B2F(destinationBeatIndex), B2F(beatCount));
        }

        //major methods
        public void CloneBeatmap(Beatmap other)
        {
            if (other != null && Beatmap.Frames.Count == 0)    //<-- only clone when frames list is empty
            {
                var frames = other.Frames;
                InsertFrames(frames, 0);
            }
        }
        public void ClearBeatmap()
        {
            RemoveFrames(0, Beatmap.Frames.Count);     //<-- all frames removed one by one (for editor version to clean up)
        }

        //internal helper methods
        /// <summary>
        /// tiny helper method for easier readability when converting beats to frames
        /// </summary>
        protected int B2F(int beatSizeValue) => SongDataUtility.ConvertBeatsToFrames(beatSizeValue);
        protected bool FrameExists(int frameIndex) => SongDataUtility.FrameExists(Beatmap, frameIndex);
        protected bool FramesExist(int startingFrameIndex, int frameCount) => SongDataUtility.FramesExist(Beatmap, startingFrameIndex, frameCount);
    }
}
