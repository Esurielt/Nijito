using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Editors.SongMetadataEditor
{
    [RequireComponent(typeof(SongMetadataIOComponent))]
    [RequireComponent(typeof(SongMetadataComponent))]
    public class SongMetadataEditor : EditorBase
    {
        public SongMetadataComponent SongMetadataComponent { get; private set; }

        public void Initialize(string songName)
        {
            InitializeFirst(songName);

            //unique stuff

            InitializeLast();
        }
        protected override void RegisterComponentsInternal()
        {
            IOComponent = RegisterEditorComponent<SongMetadataIOComponent>();
            SongMetadataComponent = RegisterEditorComponent<SongMetadataComponent>();
        }
    }
}