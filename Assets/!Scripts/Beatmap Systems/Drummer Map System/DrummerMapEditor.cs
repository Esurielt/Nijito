using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Beatmap.UI;
using UnityEngine.EventSystems;

namespace Beatmap.Drummer
{
    public class DrummerMapEditor : BeatmapEditor<DrummerMapWriter>, IPointerDownHandler
    {
        protected Channel.Value _selectedValueTool;
        public Button EmptyToolButton;
        public Button HitToolButton;

        public Button AddNewBeatButton;

        public RectTransform DataPointsContainer;
        
        public DataPointController DataPointControllerPrefab;
        protected List<DataPointController> _dataPointControllers = new List<DataPointController>();

        public override void PopulateEditor()
        {
            Game.Log(Logging.Category.BEATMAP, "Populating beatmap editor window.", Logging.Level.LOG);
            int count = _beatmapWriter.FrameCount;
            for (int i = 0; i < count; i++)
            {
                var newDataPointController = Instantiate(DataPointControllerPrefab, DataPointsContainer);
                newDataPointController.Initialize(i, _beatmapWriter.TypeInstance, _beatmapWriter.Beatmap.GetDataPoints()[i]);
                _dataPointControllers.Add(newDataPointController);
            }
        }
        public void ClearEditor()
        {
            Game.Log(Logging.Category.BEATMAP, "Clearing beatmap editor window.", Logging.Level.LOG);
            for (int i = 0; i < DataPointsContainer.childCount; i++)
            {
                Destroy(DataPointsContainer.GetChild(i).gameObject);   //safe because Unity delays destruction until after stack unwinds (we can iterate)
            }
            _dataPointControllers.Clear();
        }

        protected override void SubscribeToEvents()
        {
            EmptyToolButton.onClick.AddListener(Button_SelectEmptyTool);
            HitToolButton.onClick.AddListener(Button_SelectHitTool);
            AddNewBeatButton.onClick.AddListener(Button_AddNewBeat);
        }

        protected override void UnsubscribeFromEvents()
        {
            EmptyToolButton.onClick.RemoveListener(Button_SelectEmptyTool);
            HitToolButton.onClick.RemoveListener(Button_SelectHitTool);
            AddNewBeatButton.onClick.RemoveListener(Button_AddNewBeat);
        }

        public void Button_SelectEmptyTool()
        {
            Game.Log(Logging.Category.BEATMAP, "Selected Empty Tool.", Logging.Level.LOG);
            _selectedValueTool = _beatmapWriter.TypeInstance.ChannelFlyweights[0].ValueFlyweights[0];   //<- this is what I mean by quick and dirty
            //make me modular please ^
        }
        public void Button_SelectHitTool()
        {
            Game.Log(Logging.Category.BEATMAP, "Selected Hit Tool.", Logging.Level.LOG);
            _selectedValueTool = _beatmapWriter.TypeInstance.ChannelFlyweights[0].ValueFlyweights[1];   //<- this is what I mean by quick and dirty
        }
        public void Button_AddNewBeat()
        {
            Game.Log(Logging.Category.BEATMAP, "Added New Beat.", Logging.Level.LOG);
            _beatmapWriter.AddBeatsToEnd(1);
            ClearEditor();
            PopulateEditor();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Game.LogFormat(Logging.Category.BEATMAP, "Clicked screen at {0}.", Logging.Level.LOG, eventData.position);
            Game.LogFormat(Logging.Category.BEATMAP, "Raycast object: {0}.", Logging.Level.LOG, eventData.pointerCurrentRaycast.gameObject);
            var currentValue = eventData.pointerCurrentRaycast.gameObject.GetComponent<ValueController>();

            //this is one of the dirtier checks I've written, but it should work for now.
            if (currentValue != null && _beatmapWriter.TypeInstance.ChannelFlyweights[currentValue.ChannelIndex].ValueFlyweights.Contains(_selectedValueTool))
            {
                currentValue.UpdateValue(_selectedValueTool, _selectedValueTool.Name);
            }
        }
    }
}