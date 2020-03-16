using SongData.Serialization;
using Editors;
using GracesGames.SimpleFileBrowser.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Editors
{
    public class FileBrowserComponent : EditorComponent
    {
        public Transform FileBrowserContainer;
        public FileBrowser FileBrowserPrefab;

        public void OpenFileBrowser(UnityAction<string> onFileSelectDelegate, params string[] validFileExtensions)
        {
            var fileBrowserInstance = Instantiate(FileBrowserPrefab, FileBrowserContainer);

            fileBrowserInstance.SetupFileBrowser(ViewMode.Landscape, FileBrowserContainer.gameObject);
            fileBrowserInstance.ViewMode = ViewMode.Landscape;
            fileBrowserInstance.HideIncompatibleFiles = true;

            fileBrowserInstance.OpenFilePanel(validFileExtensions);
            fileBrowserInstance.OnFileSelect.AddListener(onFileSelectDelegate); // removed in file browser code
        }
    }
}