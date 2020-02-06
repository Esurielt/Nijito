using Beatmap.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Beatmap
{
    public class DataPointController : Button, IDataPoint, IFrame
    {
        //Unity fields
        public ValueController ValueControllerPrefab;
        public RectTransform ValueControllerContainer;

        //internal
        protected List<ValueController> _expectedFrameValueControllers = new List<ValueController>();
        public int ValueCount => _expectedFrameValueControllers.Count;

        protected List<ProcessorModifier> _modifiers = new List<ProcessorModifier>();      //make controllers later?

        //FIX ME: reminder to make this handle modifiers too!

        public void Initialize(IDataPoint dataPoint)
        {
            var expectedFrame = dataPoint.GetExpectedFrame();
            var values = new List<IValueWrapper>(expectedFrame.GetValues());

            for (int i = 0; i < values.Count; i++)
            {
                var controller = SpawnValueController(values[i]);
                _expectedFrameValueControllers.Add(controller);
            }
        }
        protected ValueController SpawnValueController(IValueWrapper value)
        {
            var controller = Instantiate(ValueControllerPrefab, ValueControllerContainer);
            controller.SetValue(value.GetValue());
            return controller;
        }

        #region Interface Compliance
        public IFrame GetExpectedFrame()
        {
            return this;
        }

        public IValueWrapper GetValue(int channelIndex)
        {
            return _expectedFrameValueControllers[channelIndex];
        }

        public List<IValueWrapper> GetValues()
        {
            return new List<IValueWrapper>(_expectedFrameValueControllers);
        }

        public void SetExpectedFrame(IFrame frame)
        {
            SetValues(frame.GetValues());
        }

        public void SetValue(IValueWrapper value, int channelIndex)
        {
            _expectedFrameValueControllers[channelIndex].SetValue(value.GetValue());
        }

        public void SetValues(List<IValueWrapper> values)
        {
            var valueList = new List<IValueWrapper>(values);
            for (int i = 0; i < ValueCount; i++)
            {
                SetValue(valueList[i], i);
            }
        }
        
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
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DataPointController), true)]
    public class Editor_DataPointController : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
#endif
}