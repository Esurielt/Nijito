using KeyCombos;
using MessageBoxes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editors
{
    public abstract class IOComponent : EditorComponent
    {
        public Button SaveButton;

        public override void RegisterHotkeys()
        {
            // Done in base editor now
            //RegisterHotkey("Save Song", () => Save(), new KeyCombo(KeyCode.S, ToggleKey.Ctrl));
        }
        protected override void SubscribeToEventsInternal()
        {
            SaveButton.onClick.AddListener(Save);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            SaveButton.onClick.RemoveAllListeners();
        }
        public delegate bool DataLoader<T>(string songName, out T foundData);
        public T LoadData<T>(DataLoader<T> dataLoader, string logName)
        {
            if (dataLoader(WeakEditor.SongName, out T foundData))
            {
                Game.LogFormat(Logging.Category.SONG_DATA, "Loaded {0} successfully.", Logging.Level.LOG, logName);
            }
            else
            {
                Game.LogFormat(Logging.Category.SONG_DATA, "Failed to load {0}.", Logging.Level.LOG_WARNING, logName);
            }
            return foundData;
        }
        public void Save()
        {
            SaveArgs saveArgs = PrepareSave();
            SaveInternal(saveArgs);
        }
        protected abstract SaveArgs PrepareSave();
        private void SaveInternal(SaveArgs saveArgs)
        {
            try
            {
                if (!saveArgs.SaveDataIsValid)
                {
                    throw new System.InvalidOperationException("Editor contains invalid data.");
                }

                if (saveArgs.DataWillOverwrite)
                {
                    var msg = MessageTemplate.GetConfirmationBox("Save",
                        saveArgs.WillOverwriteMessage,
                        () => saveArgs.WriteToFileDelegate() //on yes
                        );

                    Game.MessageBox(msg);
                }
                else
                {
                    saveArgs.WriteToFileDelegate();

                    Game.MessageBox("Save", saveArgs.SuccessMessage);
                }
            }
            catch (System.Exception e)
            {
                Game.MessageBox("Editor Error", $"Unable to save editor session: {e}", new ButtonTemplate("Dang it, Eric"));
            }
        }
        protected struct SaveArgs
        {
            public bool SaveDataIsValid;
            public bool DataWillOverwrite;
            public string WillOverwriteMessage;
            public string SuccessMessage;
            public System.Action WriteToFileDelegate;
        }
    }
}