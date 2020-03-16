using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MessageBoxes;
using SongData;
using SongData.Serialization;

namespace Editors
{
    public class EditorMainMenu : MonoBehaviour
    {
        public Button RefreshButton;

        // Create New Song Directory fields
        public TMP_InputField SongNameInputField;
        public Button CreateNewSongButton;

        // Create/Load Beatmap fields
        public TMP_Dropdown NameDropdown1;
        public TMP_Dropdown DifficultyDropdown;
        public TMP_Dropdown TypeDropdown;

        public Button CreateBeatmapButton;
        public Button LoadBeatmapButton;

        // Edit Song Data fields
        public TMP_Dropdown NameDropdown2;

        public Button EditSongButton;

        //internal
        protected List<string> _songNames;
        protected List<BeatmapDifficulty> _difficulties;
        protected List<BeatmapType> _typeInstances;

        public void Awake()
        {
            _songNames = PopulateDropdown(NameDropdown1, SongDataFileUtility.GetAllSongNames(), value => value);
            NameDropdown2.ClearOptions();
            NameDropdown2.AddOptions(_songNames);

            _difficulties = PopulateDropdown(DifficultyDropdown, BeatmapDifficulty.GetAll(), value => value.Name);
            _typeInstances = PopulateDropdown(TypeDropdown, BeatmapType.GetAll(), value => value.Name);

            CreateNewSongButton.onClick.AddListener(OnClickCreateSongButton);

            CreateBeatmapButton.onClick.AddListener(OnClickCreateBeatmapButton);
            LoadBeatmapButton.onClick.AddListener(OnClickLoadBeatmapButton);
            RefreshButton.onClick.AddListener(OnClickRefreshButton);

            EditSongButton.onClick.AddListener(OnClickEditSongButton);
        }
        private void OnDestroy()
        {
            CreateNewSongButton.onClick.RemoveAllListeners();

            CreateBeatmapButton.onClick.RemoveAllListeners();
            LoadBeatmapButton.onClick.RemoveAllListeners();
            RefreshButton.onClick.RemoveAllListeners();

            EditSongButton.onClick.RemoveAllListeners();
        }
        public string GetSelectedName1() => GetSelectedValue(_songNames, NameDropdown1);
        public string GetSelectedName2() => GetSelectedValue(_songNames, NameDropdown2);
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
            if (SongDataFileUtility.SongDirectoryExists(songName))
            {
                Game.MessageBox("Create New Song",
                    string.Format(
                        "A song named \"{0}\" already exists. Please select it from the Load/Create dropdown to load or create new beatmap data.",
                        songName));
            }
            else
            {
                if (SongDataFileUtility.CreateNewSong(songName))
                {
                    Game.MessageBox("Create New Song", "Song directory was created. Hit Refresh.");
                }
            }
        }
        protected void OnClickCreateBeatmapButton()
        {
            var songName = GetSelectedName1();
            var difficulty = GetSelectedDifficulty();
            var beatmapType = GetSelectedBeatmapType();

            void openWithNewBeatmap() => Game.OpenBeatmapEditor(songName, difficulty, new SongData.Beatmap(beatmapType));

            if (SongDataFileUtility.BeatmapFileExists(songName, beatmapType, difficulty))
            {
                if (SongDataFileUtility.TryReadBeatmap(songName, beatmapType, difficulty, out SongData.Beatmap beatmap))
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
            var songName = GetSelectedName1();
            var difficulty = GetSelectedDifficulty();
            var beatmapType = GetSelectedBeatmapType();

            if (SongDataFileUtility.BeatmapFileExists(songName, beatmapType, difficulty))
            {
                if (SongDataFileUtility.TryReadBeatmap(songName, beatmapType, difficulty, out SongData.Beatmap beatmap))
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
        public void OnClickEditSongButton()
        {
            var songName = GetSelectedName2();

            if (SongDataFileUtility.SongDirectoryExists(songName))
            {
                Game.OpenSongEditor(songName);
            }
            else
            {
                Game.MessageBox("Load Beatmap",
                        string.Format(
                        "Song: \"{0}\"\n" +
                        "There was a problem loading the song.",
                        songName)
                        );
            }
        }
        protected void OnClickRefreshButton()
        {
            Game.OpenEditorMainMenu();
        }
    }
}