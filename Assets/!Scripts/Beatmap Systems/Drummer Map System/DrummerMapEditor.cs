using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Beatmap.Drummer;

namespace Beatmap.Editor.Drummer
{
    public class DrummerMapEditor : BeatmapEditor, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        protected Channel.Value _selectedValueTool;
        public Button EmptyToolButton;
        public Button HitToolButton;

        public Button AddNewBeatButton;
        public Button UndoButton;
        public Button RedoButton;

        protected override void RegisterEvents()
        {
            base.RegisterEvents();

            //core basic events
            RegisterEvent_Basic(UndoButton.onClick, () => Undo());
            RegisterEvent_Basic(RedoButton.onClick, () => Redo());
                //save beatmap
                //open beatmap
                //leave editor (back to main screen?)

            //tool selection basic events
            RegisterEvent_Basic(EmptyToolButton.onClick, () => _selectedValueTool = ChannelValueInstances.Empty);
            RegisterEvent_Basic(HitToolButton.onClick, () => _selectedValueTool = ChannelValueInstances.Hit);

            //undoable events
            RegisterEvent_Undoable(AddNewBeatButton.onClick, new EditorCommands.AddBlankBeatsAtEndCommand(this, () => 1));  //always 1, but can change to get another value
        }

        public override BeatmapWriter GetNewBeatmapWriter() => new DrummerMapWriter(new DrummerMap());
        public override IBeatmapFileIOHelper GetNewFileIOHelper() => new BeatmapFileIOHelper_JSON("test");

        public void OnPointerDown(PointerEventData eventData)
        {
            Game.LogFormat(Logging.Category.BEATMAP, "Clicked screen at {0}.", Logging.Level.LOG, eventData.position);
            Game.LogFormat(Logging.Category.BEATMAP, "Raycast object: {0}.", Logging.Level.LOG, eventData.pointerCurrentRaycast.gameObject);
            var currentValue = eventData.pointerCurrentRaycast.gameObject.GetComponent<ValueController>();

            //this is one of the dirtier checks I've written, but it should work for now.
            if (currentValue != null && BeatmapWriter.TypeInstance.ChannelFlyweights[currentValue.ChannelIndex].ValueFlyweights.Contains(_selectedValueTool))
            {
                currentValue.UpdateValue(_selectedValueTool);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //
        }
    }
}