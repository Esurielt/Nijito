using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Beatmap.Editor;
using Beatmap.Editor.Commands;
using Beatmap.Interfaces;
using UnityEngine.Events;

namespace Beatmap.Editor
{
    public abstract class PianoRollEditor : BeatmapEditor
    {
        //Unity fields
        public RectTransform TemplateFrameContainer;
        public EditorTemplateButton TemplateButtonPrefab;
        public Button ResetTemplateButton;
        public Button TemplateCopyButton;
        public Button TemplatePasteButton;

        public Button UndoButton;
        public Button RedoButton;

        public TMPro.TMP_InputField BeatNumberInput;
        public Button AddBeatsButton;
        public Button RemoveBeatsButton;

        public ReleasableSlider ZoomSlider;
        public Button ResetZoomButton;
        public TMPro.TMP_Dropdown SubdivisionDropdown;

        //internal stuff
        protected TemplateFrameHelper _templateFrame;
        protected ZoomHelper _zoomHelper;
        protected SubdivisionHelper _subdivisionHelper;
        protected SelectionHelper _selectionHelper;

        protected override void LateInitialize()
        {
            _templateFrame = new TemplateFrameHelper(BeatmapTypeInstances.Drummer, TemplateFrameContainer, TemplateButtonPrefab);
            _templateFrame.InitializeFrame();
            
            _subdivisionHelper = new SubdivisionHelper(Writer, SubdivisionDropdown);
            SubdivisionDropdown.onValueChanged.AddListener(value => onRepaintEditor.Invoke());

            _zoomHelper = new ZoomHelper(this, ZoomSlider);
            ZoomSlider.onSliderReleased.AddListener(value => onRepaintEditor.Invoke());

            _selectionHelper = new SelectionHelper();

            SetDirty();
        }

        protected override void RegisterEvents()
        {
            base.RegisterEvents();

            //core basic events
            RegisterEvent_Basic(UndoButton.onClick, () => Undo());
            RegisterEvent_Basic(RedoButton.onClick, () => Redo());
            //save beatmap
            //open beatmap
            //leave editor (back to main screen?)

            //template events
            for (int i = 0; i < _templateFrame.ValueCount; i++)
            {
                int j = i;  //<-- avoid iterator variable, which is moved to heap after code block and becomes an object reference instead (stays at max value)
                RegisterEvent_Basic(_templateFrame.GetOnClickEvent(j), () => _templateFrame.RotateValue(j));
            }
            RegisterEvent_Basic(ResetTemplateButton.onClick, () => _templateFrame.SetValues(BeatmapTypeInstances.Drummer.GetNewFrameWithDefaults().GetValues()));
            RegisterEvent_Basic(TemplateCopyButton.onClick, () => CopySelectedDataPointToTemplate());
            RegisterEvent_Basic(TemplatePasteButton.onClick, () => PasteTemplateToSelectedDataPoint());

            //other events
            RegisterEvent_Basic(ResetZoomButton.onClick, () => _zoomHelper.ResetZoom());
            
            //repaint events
            RegisterEvent_Basic(onRepaintEditor, () => _zoomHelper.OnRepaintEditor());
            RegisterEvent_Basic(onRepaintEditor, () => _subdivisionHelper.OnRepaintEditor());

            //undoable events
            RegisterEvent_Undoable(AddBeatsButton.onClick,
                new InsertBlanksBeatsAtEndCommand(Writer, GetNumberOfBeats));
            RegisterEvent_Undoable(RemoveBeatsButton.onClick,
                new RemoveBeatsFromEndCommand(Writer, GetNumberOfBeats));
        }

        protected void PasteTemplateToSelectedDataPoint()
        {
            if (_selectionHelper.HasSelection)
                _selectionHelper.ForEach(controller => controller.SetExpectedFrame(_templateFrame));
        }
        protected void CopySelectedDataPointToTemplate()    
        {
            if (_selectionHelper.HasSelection)
                _templateFrame.SetValues(_selectionHelper.GetSelectionCopy()[0].GetValues());
        }
        protected int GetNumberOfBeats()
        {
            string strValue = BeatNumberInput.text;
            if (int.TryParse(strValue, out int intValue))
            {
                if (intValue > 1)
                    return intValue;
            }
            return 1;
        }
        public override void OnDataPointInstantiated(EditorDataPointController dataPointController)
        {
            dataPointController.onClick.AddListener(() => _selectionHelper.OnSelectDataPoint(dataPointController));
        }
        public override void OnDataPointDestroyed(EditorDataPointController dataPointController)
        {
            dataPointController.onClick.RemoveAllListeners();
            _selectionHelper.RemoveFromSelection(dataPointController);
        }

        protected class TemplateFrameHelper : IFrame
        {
            protected readonly BeatmapType _typeInstance;
            protected readonly RectTransform _container;
            protected readonly EditorTemplateButton _buttonPrefab;

            protected List<EditorTemplateButton> _buttons = new List<EditorTemplateButton>();

            public TemplateFrameHelper(BeatmapType beatmapTypeInstance, RectTransform container, EditorTemplateButton templateButtonPrefab)
            {
                _typeInstance = beatmapTypeInstance;
                _container = container;
                _buttonPrefab = templateButtonPrefab;
            }
            public int ValueCount => _buttons.Count;

            public void InitializeFrame()
            {
                var defaultFrame = _typeInstance.GetNewFrameWithDefaults();

                for (int i = 0; i < defaultFrame.ValueCount; i++)
                {
                    var clone = Instantiate(_buttonPrefab, _container);
                    clone.Initialize(_typeInstance.ChannelFlyweights[i]);
                    _buttons.Add(clone);
                }

                SetValues(defaultFrame.GetValues());
            }
            public IValueWrapper GetValue(int channelIndex)
            {
                return _buttons[channelIndex];
            }

            public List<IValueWrapper> GetValues()
            {
                return new List<IValueWrapper>(_buttons);
            }

            public void SetValue(IValueWrapper value, int channelIndex)
            {
                _buttons[channelIndex].SetValue(value.GetValue());
            }

            public void SetValues(List<IValueWrapper> values)
            {
                for (int i = 0; i < _buttons.Count; i++)
                {
                    SetValue(values[i], i);
                }
            }

            public void RotateValue(int channelIndex)
            {
                //get all possible values for this channel
                var possibleValues = _typeInstance.ChannelFlyweights[channelIndex].ValueFlyweights;

                //find this value's index
                int thisValueIndex = possibleValues.IndexOf(GetValue(channelIndex).GetValue());

                //get the next value in the list (looping at the end)
                var newValue = possibleValues[(int)Mathf.Repeat(thisValueIndex + 1, possibleValues.Count)];

                //set to the next value
                SetValue(new ValueWrapper(newValue), channelIndex);
            }

            public UnityEvent GetOnClickEvent(int channelIndex)
            {
                return _buttons[channelIndex].onClick;
            }
        }
        public class SubdivisionHelper
        {
            const int DEFAULT_INDEX = 3;    //sixteenth

            protected List<SubdivisionVisuals> _subdivisionVisualsList;

            protected EditorVisualsWriter _writer;
            protected TMPro.TMP_Dropdown _dropdown;

            public SubdivisionHelper(EditorVisualsWriter writer, TMPro.TMP_Dropdown subdivisionDropdown)
            {
                _writer = writer;
                _dropdown = subdivisionDropdown;

                InitializeVisuals();
                InitalizeDropdown();
            }
            protected void InitializeVisuals()
            {
                _subdivisionVisualsList = new List<SubdivisionVisuals>()
                {
                    new SubdivisionVisuals(Subdivision.QUARTER, "Beats (1/4 notes)", new Color(.871f, .443f, .427f)),                               //red
                    new SubdivisionVisuals(Subdivision.EIGHTH, "1/2 Beats (1/8th notes)", new Color(.443f, .427f, .871f)),                          //blue
                    new SubdivisionVisuals(Subdivision.EIGHTH_TRIPLET, "1/3 Beats (1/8 note triplets)", new Color(.427f, .971f, .443f)),            //green
                    new SubdivisionVisuals(Subdivision.SIXTEENTH, "1/4 Beats (1/16 notes)", new Color(.851f, .871f, .427f)),                        //yellow
                    new SubdivisionVisuals(Subdivision.SIXTEENTH_TRIPLET, "1/6 Beats (1/16 note triplets)", new Color(.871f, .427f, .851f)),        //magenta
                    new SubdivisionVisuals(Subdivision.THIRTYSECOND, "1/8 Beats (1/32 notes)", new Color(.871f, .667f, .427f)),                     //orange
                    new SubdivisionVisuals(Subdivision.THIRTYSECOND_TRIPLET, "1/12 Beats (1/32 note triplets)", new Color(.624f, .624f, .624f)),    //gray
                    new SubdivisionVisuals(Subdivision.FRAMERATE, "Frames (1/64 note triplets)", new Color(1f, 1f, 1f)),                            //white
                };
            }
            protected void InitalizeDropdown()
            {
                List<string> optionNames = new List<string>();
                foreach (var visual in _subdivisionVisualsList)
                {
                    optionNames.Add(visual.EditorName);
                }

                _dropdown.ClearOptions();
                _dropdown.AddOptions(optionNames);
                _dropdown.value = DEFAULT_INDEX;   //will notify (will redivide)
            }
            public void OnRepaintEditor()
            {
                var dataPoints = _writer.GetDataPoints(0, _writer.FrameCount);  //all data points
                var thisListItem = _subdivisionVisualsList[_dropdown.value];    //currently painted visuals object

                //for each data point in the editor
                for (int i = 0; i < _writer.FrameCount; i++)
                {
                    var controller = dataPoints[i] as EditorDataPointController;    //handle

                    //if the subdivision level should show it
                    if (BeatmapUtility.SubdivisionIncludesIndex(thisListItem.Subdivision, i))
                    {
                        //set it active
                        controller.gameObject.SetActive(true);

                        //get highest subdivision for this frame (list is in order or priority, so it works to just grab the first one)
                        var highestSV = _subdivisionVisualsList.Find(sv => BeatmapUtility.SubdivisionIncludesIndex(sv.Subdivision, i));

                        //update the visuals of the data point
                        controller.UpdateVisuals(i, highestSV.Color);
                        var castWriter = _writer as PianoRollEditorVisualsWriter;
                        controller.UpdateSelected(false);  //super ugly, fix me
                    }
                    else
                    {
                        //otherwise, set it inactive
                        controller.gameObject.SetActive(false);
                    }
                }
            }
            protected struct SubdivisionVisuals
            {
                public readonly Subdivision Subdivision;
                public readonly string EditorName;
                public readonly Color Color;
                public SubdivisionVisuals(Subdivision subdivision, string editorName, Color color)
                {
                    Subdivision = subdivision;
                    EditorName = editorName;
                    Color = color;
                }
            }
        }
        protected class ZoomHelper
        {
            const int DEFAULT_WIDTH = 100;
            const float MIN = 0.1f;
            const float MAX = 2f;

            protected PianoRollEditor _editor;
            protected ReleasableSlider _slider;
            public float CurrentWidth => DEFAULT_WIDTH * _slider.value;
            public ZoomHelper(PianoRollEditor editor, ReleasableSlider slider)
            {
                _editor = editor;
                _slider = slider;

                InitializeSlider();
            }
            protected void InitializeSlider()
            {
                _slider.minValue = MIN;
                _slider.maxValue = MAX;

                ResetZoom();
            }
            public void ResetZoom()
            {
                _slider.value = 1f;
                _slider.onSliderReleased.Invoke(1f);
            }
            public void OnRepaintEditor()
            {
                _editor.ForEachDataPointController(controller => controller.UpdateWidth(CurrentWidth));
            }
        }
        public class SelectionHelper
        {
            public SelectionMode CurrentMode { get; set; } = SelectionMode.SINGLE;
            protected List<EditorDataPointController> _selected = new List<EditorDataPointController>();
            public List<EditorDataPointController> GetSelectionCopy() => new List<EditorDataPointController>(_selected);
            public bool HasSelection => _selected.Count > 0;
            public bool HasSingleSelection => _selected.Count == 1;
            public bool HasMultipleSelections => _selected.Count > 1;
            public SelectionHelper()
            {

            }
            public void OnSelectDataPoint(EditorDataPointController dataPointController)
            {
                if (_selected.Contains(dataPointController))
                {
                    RemoveFromSelection(dataPointController);
                }
                else
                {
                    if (CurrentMode == SelectionMode.SINGLE)
                    {
                        ClearSelection();
                    }
                    AddToSelection(dataPointController);
                }
            }
            public bool IsSelected(EditorDataPointController dataPointController)
            {
                return _selected.Contains(dataPointController);
            }
            public void AddToSelection(EditorDataPointController dataPointController)
            {
                if (!_selected.Contains(dataPointController))
                {
                    _selected.Add(dataPointController);
                    dataPointController.UpdateSelected(true);
                }
            }
            public void RemoveFromSelection(EditorDataPointController dataPointController)
            {
                if (_selected.Contains(dataPointController))
                {
                    _selected.Remove(dataPointController);
                    dataPointController.UpdateSelected(false);
                }
            }
            public void ClearSelection()
            {
                for (int i = 0; i < _selected.Count; i++)
                {
                    _selected[i].UpdateSelected(false);
                }
                _selected.Clear();
            }
            public void ForEach(System.Action<EditorDataPointController> forEach)
            {
                if (_selected.Count > 0)
                {
                    _selected.ForEach(controller => forEach(controller));
                }
            }
            public enum SelectionMode
            {
                SINGLE = 0,
                MULTI = 1,
            }
        }
    }

    public class PianoRollEditorVisualsWriter : EditorVisualsWriter
    {
        //every time you want to change this subclass, think hard about how you could not. It works every time.
        public PianoRollEditorVisualsWriter(PianoRollEditor editor, IBeatmap beatmap) : base(editor, beatmap) { }
    }
}