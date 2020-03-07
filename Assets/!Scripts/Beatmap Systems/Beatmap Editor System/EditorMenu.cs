using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Beatmap.Serialization;
using MessageBoxes;

namespace Beatmap.Editor
{
    public class EditorMenu : MonoBehaviour
    {
        public TMP_InputField SongNameInputField;
        public TMP_Dropdown NameDropdown;
        public TMP_Dropdown DifficultyDropdown;
        public TMP_Dropdown TypeDropdown;

        public Button CreateNewSongButton;
        public Button CreateBeatmapButton;
        public Button LoadBeatmapButton;
        public Button RefreshButton;

        protected List<string> _songNames;
        protected List<BeatmapDifficulty> _difficulties;
        protected List<BeatmapType> _typeInstances;

        public void Awake()
        {
            _songNames = PopulateDropdown(NameDropdown, BeatmapFileUtility.GetAllSongNames(), value => value);
            _difficulties = PopulateDropdown(DifficultyDropdown, BeatmapDifficulty.GetAll(), value => value.Name);
            _typeInstances = PopulateDropdown(TypeDropdown, BeatmapType.GetAll(), value => value.Name);

            CreateNewSongButton.onClick.AddListener(OnClickCreateSongButton);
            CreateBeatmapButton.onClick.AddListener(OnClickCreateBeatmapButton);
            LoadBeatmapButton.onClick.AddListener(OnClickLoadBeatmapButton);
            RefreshButton.onClick.AddListener(OnClickRefreshButton);
        }
        private void OnDestroy()
        {
            CreateNewSongButton.onClick.RemoveListener(OnClickCreateSongButton);
            CreateBeatmapButton.onClick.RemoveListener(OnClickCreateBeatmapButton);
            LoadBeatmapButton.onClick.RemoveListener(OnClickLoadBeatmapButton);
            RefreshButton.onClick.RemoveListener(OnClickRefreshButton);
        }
        public string GetSelectedName() => GetSelectedValue(_songNames, NameDropdown);
        public BeatmapDifficulty GetSelectedDifficulty() => GetSelectedValue(_difficulties, DifficultyDropdown);
        public BeatmapType GetSelectedBeatmapType() => GetSelectedValue(_typeInstances, TypeDropdown);
        protected List<TValue> PopulateDropdown<TValue>(TMP_Dropdown dropdown, List<TValue> options, System.Func<TValue, string> nameGetter)
        {
            dropdown.ClearOptions();
            var optionStrings = new List<string>();
            foreach (var option in options)
            {
                optionStrings.Add(nameGetter(option));
            }
            dropdown.AddOptions(optionStrings);
            return options;
        }
        protected TValue GetSelectedValue<TValue>(List<TValue> values, TMP_Dropdown dropdown)
        {
            int dropdownIndex = dropdown.value;
            return values[dropdownIndex];
        }
        protected void OnClickCreateSongButton()
        {
            var songName = SongNameInputField.text;
            if (BeatmapFileUtility.SongDirectoryExists(songName))
            {
                Game.MessageBox("Create New Song",
                    string.Format(
                        "A song named \"{0}\" already exists. Please select it from the Load/Create dropdown to load or create new beatmap data.",
                        songName));
            }
            else
            {
                if (BeatmapFileUtility.CreateNewSong(songName))
                {
                    Game.MessageBox("Create New Song", "Song directory was created. Hit Refresh.");
                }
            }
        }
        protected void OnClickCreateBeatmapButton()
        {
            var songName = GetSelectedName();
            var difficulty = GetSelectedDifficulty();
            var beatmapType = GetSelectedBeatmapType();

            void openWithNewBeatmap() => Game.OpenBeatmapEditor(songName, difficulty, new Beatmap(beatmapType));

            if (BeatmapFileUtility.BeatmapFileExists(songName, beatmapType, difficulty))
            {
                if (BeatmapFileUtility.TryReadBeatmap(songName, beatmapType, difficulty, out Beatmap beatmap))
                {
                    var msg = MessageTemplate.GetConfirmationBox("Create Beatmap",
                        string.Format(
                        "Song: \"{0}\"\n" +
                        "Difficulty: \"{1}\"\n" +
                        "Beatmap Type: \"{2}\"\n" +
                        "A beatmap already exists for this song and difficulty. Are you sure you want to start over?",
                        songName, difficulty.Name, beatmapType.Name),
                        openWithNewBeatmap //on yes
                        );
                    Game.MessageBox(msg);
                    return;
                }
            }
            openWithNewBeatmap();
        }
        protected void OnClickLoadBeatmapButton()
        {
            var songName = GetSelectedName();
            var difficulty = GetSelectedDifficulty();
            var beatmapType = GetSelectedBeatmapType();

            if (BeatmapFileUtility.BeatmapFileExists(songName, beatmapType, difficulty))
            {
                if (BeatmapFileUtility.TryReadBeatmap(songName, beatmapType, difficulty, out Beatmap beatmap))
                {
                    Game.OpenBeatmapEditor(songName, difficulty, beatmap);
                }
                else
                {
                    Game.MessageBox("Load Beatmap",
                        string.Format(
                        "Song: \"{0}\"\n" +
                        "Difficulty: \"{1}\"\n" +
                        "Beatmap Type: \"{2}\"\n" +
                        "There was a problem loading the beatmap for this song and difficulty.",
                        songName, difficulty.Name, beatmapType.Name)
                        );
                }
            }
            else
            {
                Game.MessageBox("Load Beatmap",
                    string.Format(
                    "Song: \"{0}\"\n" +
                    "Difficulty: \"{1}\"\n" +
                    "Beatmap Type: \"{2}\"\n" +
                    "Unable to locate a beatmap for this song and difficulty.",
                    songName, difficulty.Name, beatmapType.Name)
                    );
            }
        }
        protected void OnClickRefreshButton()
        {
            Game.OpenBeatmapEditorMenu();
        }
    }
}