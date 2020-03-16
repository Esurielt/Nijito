using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using SongData.Serialization;
using SongData;

namespace Editors.BeatmapEditor
{
    public class AudioPlayEvent : UnityEvent<AudioComponent.PlaySession> { }
    public class AudioComponent : EditorComponent<BeatmapEditor>
    {
        public Button AudioPlayButton;
        public Button AudioStopButton;
        public Button AudioImportButton;

        public bool HasAudio => Game.AudioSource.clip != null;
        public float AudioLength => Game.AudioSource.clip != null ? Game.AudioSource.clip.length : 0;
        public bool IsPlaying => _currentPlaySession != null && !_currentPlaySession.Complete;

        [HideInInspector] public AudioPlayEvent OnAudioPlay;
        [HideInInspector] public UnityEvent OnAudioStop;

        public AudioTimetable Timetable { get; private set; }
        private PlaySession _currentPlaySession = null;

        protected override void InitializeInternal()
        {
            OnAudioPlay = new AudioPlayEvent();

            TrySetAudio();
        }
        private void TrySetAudio()
        {
            AudioClip audio = Editor.IOComponent.LoadData<AudioClip>(SongDataFileUtility.TryGetAudio, "song audio");
            SetAudio(audio);

            if (audio == null)
            {
                Game.MessageBox("Audio", "Song audio did not load. Click the Audio Import button and select a 16-bit WAV file to import it into the project.");
            }
            else
            {
                AudioImportButton.gameObject.SetActive(false);
            }
        }
        public override void RegisterHotkeys()
        {
            Editor.HotkeyComponent.TryRegisterHotkey("Toggle Audio Playback", () => TogglePlay(), new KeyCombos.KeyCombo(KeyCode.Space));
        }
        protected override void RepaintInternal()
        {
            Timetable = new AudioTimetable(Editor.BeatmapWriter.Beatmap, Editor.TimelineComponent.AudioMetadata);
        }
        protected override void SubscribeToEventsInternal()
        {
            AudioPlayButton.onClick.AddListener(Play);
            AudioStopButton.onClick.AddListener(Stop);

            AudioImportButton.onClick.AddListener(OpenFileBrowserForAudioImport);
        }
        protected override void UnsubscribeFromEventsInternal()
        {
            AudioPlayButton.onClick.RemoveListener(Play);
            AudioStopButton.onClick.RemoveListener(Stop);

            AudioImportButton.onClick.RemoveListener(OpenFileBrowserForAudioImport);
        }
        protected override void CleanUpInternal()
        {
            Stop();
        }
        public void SetAudio(AudioClip audioClip)
        {
            Game.AudioSource.clip = audioClip;
            Game.AudioSource.time = 0;

            Game.Log(Logging.Category.SONG_DATA, "Beatmap editor audio set.", Logging.Level.LOG);
            if (audioClip == null)
                Game.Log(Logging.Category.SONG_DATA, "(Beatmap editor audio is null)", Logging.Level.LOG);
        }
        public void Play()
        {
            if (HasAudio)
            {
                if (IsPlaying)
                {
                    Stop();
                }

                var selectionComponent = Editor.SelectionComponent;
                int playbackStartIndex = selectionComponent.SelectionStart;
                int playbackEndIndex;
                if (selectionComponent.HasSingleSelection)
                {
                    playbackEndIndex = Editor.BeatmapWriter.FrameCount - 1;     //play to end of frames
                }
                else
                {
                    playbackEndIndex = selectionComponent.SelectionImpliedEnd - 1;    //play to end of selection
                }

                _currentPlaySession = new PlaySession(this, playbackStartIndex, playbackEndIndex);
                OnAudioPlay?.Invoke(_currentPlaySession);
            }
            else
            {
                Game.Log(Logging.Category.SONG_DATA, "Editor has no loaded audio clip.", Logging.Level.LOG_WARNING);
            }
        }
        public void Stop()
        {
            if (IsPlaying)
            {
                _currentPlaySession.StopSession();
                _currentPlaySession = null;
            }
            OnAudioStop?.Invoke();
        }
        public void TogglePlay()
        {
            // Used by hotkey.
            if (IsPlaying)
                Stop();
            else
                Play();
        }
        private void OpenFileBrowserForAudioImport()
        {
            Editor.FileBrowserComponent.OpenFileBrowser(userFilePath => TryImportAudioFromPath(userFilePath), "C:/Users/", "wav");
        }
        private void TryImportAudioFromPath(string filePath)
        {
            if (SongDataFileUtility.TryImportAudio(Editor.SongName, filePath))
            {
                Game.MessageBox("Audio Import", "Audio import was successful.");

                TrySetAudio();
            }
            else
            {
                // Commented out for duplicate message.
                //Game.MessageBox("Audio Import", "Audio import failed.");
            }
        }

        public class PlaySession
        {
            public int StartFrameIndex { get; }
            public int EndFrameIndex { get; }
            public float FinalTimestamp { get; }
            public AudioFrameInfo StartFrameInfo { get; }
            public AudioFrameInfo EndFrameInfo { get; }
            public float Duration { get; }
            public float CurrentTimestamp { get; private set; }
            public AudioTimetable.IndexesAndProgress CurrentIndexesAndProgress { get; private set; }
            public float AbsoluteProgress { get; private set; }
            public float ScaledProgress { get; private set; }
            public bool Complete { get; private set; }

            private readonly AudioComponent _parent;
            private readonly AudioTimetable _timetable;
            private readonly Coroutine _coroutine;
            public PlaySession(AudioComponent parent, int startFrameIndex, int endFrameIndex)
            {
                _parent = parent;
                _timetable = parent.Timetable;

                StartFrameIndex = startFrameIndex;
                EndFrameIndex = endFrameIndex;

                StartFrameInfo = _timetable.GetFrameInfo(StartFrameIndex);
                EndFrameInfo = _timetable.GetFrameInfo(EndFrameIndex);

                FinalTimestamp = EndFrameInfo.EndTimestamp;

                Duration = FinalTimestamp - StartFrameInfo.StartTimestamp;

                Complete = false;

                _coroutine = parent.StartCoroutine(PlayCoroutine());
            }
            private IEnumerator PlayCoroutine()
            {
                if (StartFrameInfo.StartTimestamp < Game.AudioSource.clip.length)
                {
                    Game.AudioSource.time = StartFrameInfo.StartTimestamp;
                    Game.AudioSource.Play();
                }
                // We don't mind if there's nothing playing.

                for (float timePassed = 0; timePassed < Duration; timePassed += Time.deltaTime)
                {
                    UpdateProgress(timePassed);
                    yield return null;  //come back next frame
                }
                
                UpdateProgress(Duration);

                Complete = true;
                StopSession();
            }
            public void StopSession()
            {
                _parent.StopCoroutine(_coroutine);
                Game.AudioSource.Stop();
                Game.AudioSource.time = 0;
            }
            private void UpdateProgress(float timePassed)
            {
                CurrentTimestamp = StartFrameInfo.StartTimestamp + timePassed;
                AbsoluteProgress = timePassed / Duration;

                CurrentIndexesAndProgress = _timetable.GetIndexesAndProgress(CurrentTimestamp);
                ScaledProgress = Mathf.Lerp(StartFrameIndex, EndFrameIndex, CurrentIndexesAndProgress.PreciseFrameIndex);
            }
        }
    }
}