using Beatmap.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Beatmap
{
    public abstract class AbstractBeatmapWriter
    {
        //fields
        protected readonly IBeatmap _beatmap;
        public int FrameCount => _beatmap.DataPoints.Count;
        
        //ctor
        public AbstractBeatmapWriter(IBeatmap beatmap)
        {
            _beatmap = beatmap;
            DefaultDataPoint = new DataPoint(beatmap.TypeInstance.GetNewFrameWithDefaults(), new List<ProcessorModifier>());

            Populate(beatmap);
        }

        //abstract methods
        /// <summary>
        /// Redirect to a flyweight reference for a data point considered in a 'default' state.
        /// </summary>
        protected DataPoint DefaultDataPoint { get; }
        /// <summary>
        /// Create a GameObject instance of a data point. No need to track it in any collection. Don't forget to move it wherever makes sense based on the given frame index.
        /// </summary>
        protected abstract IDataPoint InstantiateDataPointInstance(IDataPoint initializeValuesFrom, int frameIndex);
        /// <summary>
        /// Destroy the given data point instance. This is called after the instance has been removed from the internal collection. After this call, the instance will cease to be tracked and any remaining trace of it can be considered a memory leak. If there's simply nothing to do, don't worry about this part.
        /// </summary>
        protected abstract void DestroyDataPointInstance(IDataPoint dataPoint);

        //getters
        public IDataPoint GetDataPoint(int frameIndex)
        {
            if (FrameExists(frameIndex))
                return _beatmap.DataPoints[frameIndex];
            return null;
        }
        public List<IDataPoint> GetDataPoints(int startingFrameIndex, int frameCount)
        {
            if (FramesExist(startingFrameIndex, frameCount))
                return _beatmap.DataPoints.GetRange(startingFrameIndex, frameCount);
            return new List<IDataPoint>();
        }
        public List<IDataPoint> GetBeats(int startingBeatIndex, int beatCount)
        {
            return GetDataPoints(B2F(startingBeatIndex), B2F(beatCount));
        }
        public List<IDataPoint> GetDataPointsAtEnd(int frameCount)
        {
            return GetDataPoints(_beatmap.DataPoints.Count - frameCount, frameCount);
        }
        public List<IDataPoint> GetBeatsAtEnd(int beatCount)
        {
            return GetDataPointsAtEnd(B2F(beatCount));
        }

        //setters
        protected void UpdateDataPoint(IDataPoint toUpdate, IDataPoint fromDataPoint)
        {
            toUpdate.SetExpectedFrame(fromDataPoint.GetExpectedFrame());
            toUpdate.ClearModifiers();
            toUpdate.AddModifiers(fromDataPoint.GetModifiers());
        }
        public void SetDataPoint(int frameIndex, IDataPoint fromDataPoint)
        {
            if (FrameExists(frameIndex))
                UpdateDataPoint(_beatmap.DataPoints[frameIndex], fromDataPoint);
        }
        public void SetDataPoints(int startingFrameIndex, int frameCount, IDataPoint fromSingleDataPoint)
        {
            for (int i = 0; i < frameCount; i++)
            {
                SetDataPoint(startingFrameIndex + i, fromSingleDataPoint);
            }
        }
        public void SetDataPoints(int startingFrameIndex, List<IDataPoint> fromDataPoints)
        {
            for (int i = 0; i < fromDataPoints.Count; i++)
            {
                SetDataPoint(startingFrameIndex + i, fromDataPoints[i]);
            }
        }

        //resetters
        public void ResetDataPointToDefault(int frameIndex)
        {
            SetDataPoint(frameIndex, DefaultDataPoint);
        }
        public void ResetDataPointsToDefault(int startingFrameIndex, int frameCount)
        {
            SetDataPoints(startingFrameIndex, frameCount, DefaultDataPoint);
        }
        public void ResetBeatsToDefault(int startingBeatIndex, int beatCount)
        {
            ResetDataPointsToDefault(B2F(startingBeatIndex), B2F(beatCount));
        }
        
        //insert (create)
        protected void InsertDataPoint(IDataPoint fromDataPoint, int frameIndex)
        {
            if (FrameExists(frameIndex) || frameIndex == _beatmap.DataPoints.Count) //allow for insertion at the very end
            {
                IDataPoint newDataPoint;
                newDataPoint = InstantiateDataPointInstance(fromDataPoint, frameIndex);
                _beatmap.DataPoints.Insert(frameIndex, newDataPoint);
            }
        }
        public void InsertDataPoints(IDataPoint fromSingleDataPoint, int startingFrameIndex, int frameCount)
        {
            for (int i = 0; i < frameCount; i++)
            {
               InsertDataPoint(fromSingleDataPoint, startingFrameIndex + i);
            }
        }
        public void InsertDataPoints(List<IDataPoint> fromDataPoints, int startingFrameIndex)
        {
            for (int i = 0; i < fromDataPoints.Count; i++)
            {
                InsertDataPoint(fromDataPoints[i], startingFrameIndex + i);
            }
        }
        public void InsertDataPointsAtEnd(IDataPoint fromSingleDataPoint, int frameCount)
        {
            InsertDataPoints(fromSingleDataPoint, _beatmap.DataPoints.Count, frameCount);
        }
        public void InsertDataPointsAtEnd(List<IDataPoint> fromDataPoints)
        {
            InsertDataPoints(fromDataPoints, _beatmap.DataPoints.Count);
        }

        //insert (blank)
        protected void InsertBlankDataPoints(int startingFrameIndex, int frameCount)
        {
            InsertDataPoints(DefaultDataPoint, startingFrameIndex, frameCount);
        }        
        public void InsertBlankBeats(int startingBeatIndex, int beatCount)
        {
            InsertBlankDataPoints(B2F(startingBeatIndex), B2F(beatCount));
        }
        public void InsertBlankBeatsAtEnd(int beatCount)
        {
            InsertBlankDataPoints(_beatmap.DataPoints.Count, B2F(beatCount));
        }

        //remove
        protected void RemoveDataPoint(int frameIndex)
        {
            if (FrameExists(frameIndex))
            {
                var dataPoint = _beatmap.DataPoints[frameIndex];
                _beatmap.DataPoints.RemoveAt(frameIndex);
                DestroyDataPointInstance(dataPoint);
            }            
        }
        protected void RemoveDataPoints(int startingFrameIndex, int frameCount)
        {
            for (int i = 0; i < frameCount; i++)    //<-- there is nothing fancy here. Each will be removed from the same index in the collection as it readjusts.
            {
                RemoveDataPoint(startingFrameIndex);
            }
        }
        public void RemoveBeats(int startingBeatIndex, int beatCount)
        {
            RemoveDataPoints(B2F(startingBeatIndex), B2F(beatCount));
        }
        public void RemoveBeatsFromEnd(int beatCount)
        {
            RemoveDataPoints(_beatmap.DataPoints.Count - B2F(beatCount), B2F(beatCount));   //<-- watch out here
        }
        
        //move
        public void MoveDataPoint(int startingFrameIndex, int destinationFrameIndex)
        {
            var dataPoint = GetDataPoint(startingFrameIndex);
            RemoveDataPoint(startingFrameIndex);

            InsertDataPoint(dataPoint, destinationFrameIndex);
        }
        public void MoveDataPoints(int startingFrameIndex, int destinationFrameIndex, int frameCount)
        {
            var dataPoints = GetDataPoints(startingFrameIndex, frameCount);
            RemoveDataPoints(startingFrameIndex, frameCount);
            
            InsertDataPoints(dataPoints, destinationFrameIndex);
        }
        public void MoveBeats(int startingBeatIndex, int destinationBeatIndex, int beatCount)
        {
            MoveDataPoints(B2F(startingBeatIndex), B2F(destinationBeatIndex), B2F(beatCount));
        }

        //big methods
        public void CloneBeatmap(IBeatmap other)
        {
            Populate(other);    //<-- for clarity
        }
        protected void Populate(IBeatmap other)
        {
            if (other != null)
            {
                ClearBeatmap();

                var data = other.DataPoints;
                InsertDataPoints(data, 0);
                //FIX ME: metadata too, later
            }
        }
        public void ClearBeatmap()
        {
            RemoveDataPoints(0, _beatmap.DataPoints.Count);     //<-- all frames removed one by one (for editor version to clean up)
            //metadata too, later
        }

        //internal helper methods
        /// <summary>
        /// tiny helper method for easier readability when converting beats to frames
        /// </summary>
        private int B2F(int beatSizeValue) => BeatmapUtility.ConvertBeatsToFrames(beatSizeValue);
        private bool FrameExists(int frameIndex) => BeatmapUtility.FrameExists(_beatmap, frameIndex);
        private bool FramesExist(int startingFrameIndex, int frameCount) => BeatmapUtility.FramesExist(_beatmap, startingFrameIndex, frameCount);
    }
}
