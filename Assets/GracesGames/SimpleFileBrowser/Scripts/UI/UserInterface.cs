using UnityEngine;
using UnityEngine.UI;

using System;
using System.IO;

using GracesGames.Common.Scripts;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

	// The UI used in the file browser. 

	public class UserInterface : MonoBehaviour {

		// Dimension used to set the scale of the UI
		[Range(0.1f, 1.0f)] public float UserInterfaceScale = 1f;

		// Button Prefab used to create a button for each directory in the current path
		public KImprovedFileBrowserButton DirectoryButtonPrefab;

		// Button Prefab used to create a button for each file in the current path
		public KImprovedFileBrowserButton FileButtonPrefab;

		// Sprite used to represent the save button
		public Sprite SaveIcon;

		// Sprite used to represent the load button
		public Sprite LoadIcon;

		// Height of the directory and file buttons
		[Range(0.0f, 200.0f)] public int ItemButtonHeight = 50;

		// Font size used for the directory and file buttons
		[Range(0.0f, 72.0f)] public int ItemFontSize = 32;

		// Font size used for the path, load and save text
		[Range(0.0f, 72.0f)] public int PanelTextFontSize = 24;

		// Color used for the Directory Panel (and ItemPanel for Portrait mode)
		public Color DirectoryPanelColor = Color.gray;

		// Color used for the File Panel
		public Color FilePanelColor = Color.gray;

		// Color used for the directory and file texts
		public Color ItemFontColor = Color.white;

		// Game object that represents the current path
		public TMPro.TMP_Text PathText;

		// Game object and InputField that represents the name of the file to save
		public TMPro.TMP_InputField SaveFileInputField;

		// Game object (Text) that represents the name of the file to load
		public TMPro.TMP_Text LoadFileText;

		// Game object used as the parent for all the Directories of the current path
		public Transform DirectoriesContainer;

		// Game object used as the parent for all the Files of the current path
		public Transform FilesContainer;

		// Input field and variable to allow file search
		public TMPro.TMP_InputField SearchInputField;

		public Button DirectoryBackButton;
		public Button DirectoryForwardButton;
		public Button DirectoryUpButton;
		public Button CloseFileBrowserButton;

		// Button used to select a file to save/load
		public Button SelectFileButton;

		// The file browser using this user interface
		private FileBrowser _fileBrowser;

		// Setup the file browser user interface
		public void Setup(FileBrowser fileBrowser) {
			_fileBrowser = fileBrowser;
			name = "FileBrowserUI";
			transform.localScale = new Vector3(UserInterfaceScale, UserInterfaceScale, 1f);
			SetupItemPanelPrefabs();
			SetupClickListeners();
			SetupTextLabels();
			SetupContainers();
			SetupSearchInputField();
		}

		// Sets the font size and color for the directory and file texts
		private void SetupItemPanelPrefabs()
		{
			SetupPrefab(DirectoryButtonPrefab);
			SetupPrefab(FileButtonPrefab);
		}
		private void SetupPrefab(KImprovedFileBrowserButton panel)
		{
			panel.Text.fontSize = ItemFontSize;
			panel.Text.color = ItemFontColor;
			panel.GetComponent<LayoutElement>().preferredHeight = ItemButtonHeight;
		}

		// Setup click listeners for buttons
		private void SetupClickListeners() {
			// Hook up Directory Navigation methods to Directory Navigation Buttons
			DirectoryBackButton.onClick.AddListener(_fileBrowser.DirectoryBackward);
			DirectoryForwardButton.onClick.AddListener(_fileBrowser.DirectoryForward);
			DirectoryUpButton.onClick.AddListener(_fileBrowser.DirectoryUp);

			// Hook up CloseFileBrowser method to CloseFileBrowserButton
			CloseFileBrowserButton.onClick.AddListener(_fileBrowser.CloseFileBrowser);
			// Hook up SelectFile method to SelectFileButton
			SelectFileButton.onClick.AddListener(_fileBrowser.SelectFile);
		}

		// Setup path, load and save file text
		private void SetupTextLabels() {
			// and hook up onValueChanged listener to check the name using CheckValidFileName method
			SaveFileInputField.onValueChanged.AddListener(_fileBrowser.CheckValidFileName);

			// Set font size for labels and texts
			PathText.fontSize = PanelTextFontSize;
			LoadFileText.fontSize = PanelTextFontSize;
		}

		// Setup search filter
		private void SetupSearchInputField() {

			SearchInputField.pointSize = PanelTextFontSize;
			SearchInputField.onValueChanged.AddListener(_fileBrowser.UpdateSearchFilter);
		}

		// Sets the height of a GridLayoutGroup located in the game object (parent of directies and files object)
		protected void SetupContainers()
		{
			SetupContainer(DirectoriesContainer, DirectoryPanelColor);
			SetupContainer(FilesContainer, FilePanelColor);
		}
		protected void SetupContainer(Transform container, Color bgColor)
		{
			//container.GetComponent<VerticalLayoutGroup>() = ItemButtonHeight;
			container.gameObject.GetComponent<Image>().color = bgColor;
		}

		// Toggles the SelectFileButton to ensure valid file names during save
		public void ToggleSelectFileButton(bool enabled) {
			SelectFileButton.gameObject.SetActive(enabled);
		}

		// Update the path text
		public void UpdatePathText(string newPath) {
			PathText.text = $"Path: {newPath}";
		}

		// Update the file to load text
		public void UpdateLoadFileText(string newFile) {
			LoadFileText.text = $"{newFile}";
		}

		// Returns the text in the save file input field
		public String GetSaveFileText() {
			return SaveFileInputField.text;
		}

		// Updates the input field value with a file name and extension
		public void SetFileNameInputField(string fileName, string fileExtension) {
			SaveFileInputField.text = fileName + "." + fileExtension;
		}

		// Set the UI to save mode
		public void SetSaveMode(string defaultName, string fileExtension) {
			SaveFileInputField.gameObject.SetActive(true);
			LoadFileText.gameObject.SetActive(false);
			SelectFileButton.GetComponent<Image>().sprite = SaveIcon;
			// Update the input field with the default name and file extension
			SetFileNameInputField(defaultName, fileExtension);
		}

		// Set the UI to load move
		public void SetLoadMode() {
			LoadFileText.gameObject.SetActive(true);
			SelectFileButton.GetComponent<Image>().sprite = LoadIcon;
			SaveFileInputField.gameObject.SetActive(false);
		}

		// Resets the directories and files parent game objects
		public void ResetParents() {
			ResetContainer(DirectoriesContainer);
			ResetContainer(FilesContainer);
		}

		// Removes all current game objects under the parent game object
		private void ResetContainer(Transform container) {
			if (container.childCount > 0) {
				foreach (Transform child in container) {
					Destroy(child.gameObject);
				}
			}
		}

		// Creates a directory button given a directory
		public void CreateDirectoryButton(string directory) {
			var button = Instantiate(DirectoryButtonPrefab, DirectoriesContainer);
			SetupButton(button, new DirectoryInfo(directory).Name);
			// Setup FileBrowser DirectoryClick method to onClick event
			button.Button.onClick.AddListener(() => { _fileBrowser.DirectoryClick(directory); });
		}

		// Creates a file button given a file
		public void CreateFileButton(string file) {
			KImprovedFileBrowserButton button = Instantiate(FileButtonPrefab, FilesContainer);
			// When in Load mode, disable the buttons with different extension than the given file extension
			if (_fileBrowser.GetMode() == FileBrowserMode.Load) {
				DisableWrongExtensionFiles(button, file);
			}

			SetupButton(button, Path.GetFileName(file));
			// Setup FileButton script for file button (handles click and double click event)
			button.GetComponent<FileButton>().Setup(_fileBrowser, file, button.Button.interactable);
		}

		// Generic method used to extract common code for creating a directory or file button
		private void SetupButton(KImprovedFileBrowserButton button, string text) {
			button.Text.text = text;
		}

		// Disables file buttons with files that have a different file extension (than given to the OpenFilePanel)
		private void DisableWrongExtensionFiles(KImprovedFileBrowserButton button, string file) {
			if (!_fileBrowser.CompatibleFileExtension(file)) {
				button.Button.interactable = false;
			}
		}
	}
}