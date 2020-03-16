using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Editors
{
    [RequireComponent(typeof(HotkeyComponent))]
    [RequireComponent(typeof(FileBrowserComponent))]
    [RequireComponent(typeof(IOComponent))]
    public abstract class EditorBase : MonoBehaviour
    {
        public TMPro.TMP_Text SongNameText;

        //repaint event
        [HideInInspector] public UnityEvent OnRepaint;
        private bool _dirtyFlag = false;

        //components
        public IOComponent IOComponent { get; protected set; }
        public HotkeyComponent HotkeyComponent { get; private set; }
        public FileBrowserComponent FileBrowserComponent { get; private set; }

        public string SongName { get; private set; }

        //internal stuff
        public bool Initialized { get; private set; }

        protected List<EditorComponent> _editorComponents = new List<EditorComponent>();

        protected bool InitializeFirst(string songName)
        {
            if (Initialized)
                return false;

            SetDirty();

            SongName = songName;
            SongNameText.text = songName;

            Initialized = true;
            return true;
        }
        protected void InitializeLast()
        {
            RegisterComponents();
            InitializeComponents();
            
            RegisterHotkeys();
            SubscribeToEvents();
        }
        private void RegisterComponents()
        {
            HotkeyComponent = RegisterEditorComponent<HotkeyComponent>();
            FileBrowserComponent = RegisterEditorComponent<FileBrowserComponent>();
            IOComponent = RegisterEditorComponent<IOComponent>();

            RegisterComponentsInternal();
        }
        protected abstract void RegisterComponentsInternal();
        protected void InitializeComponents()
        {
            _editorComponents.ForEach(com => com.Initialize(this));
        }
        private void RegisterHotkeys()
        {
            HotkeyComponent.TryRegisterHotkey("Save", () => IOComponent.Save(), new KeyCombos.KeyCombo(KeyCode.S, KeyCombos.ToggleKey.Ctrl));
            HotkeyComponent.TryRegisterHotkey("Exit Editor", () => ExitEditor(), new KeyCombos.KeyCombo(KeyCode.Q, KeyCombos.ToggleKey.Ctrl));

            _editorComponents.ForEach(com => com.RegisterHotkeys());
        }
        private void SubscribeToEvents()
        {
            _editorComponents.ForEach(com => com.SubscribeToEvents());
        }
        private void UnsubscribeFromEvents()
        {
            _editorComponents.ForEach(com => com.UnsubscribeFromEvents());
        }
        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
        protected TComponent RegisterEditorComponent<TComponent>() where TComponent : EditorComponent
        {
            var component = GetComponent<TComponent>();
            _editorComponents.Add(component);
            return component;
        }
        public void ExitEditor()
        {
            var message = MessageBoxes.MessageTemplate.GetConfirmationBox(
                "Exit Editor", "You may have unsaved changes. Are you sure you want to leave the editor?",
                () => Game.OpenEditorMainMenu(), null);
            Game.MessageBox(message);
        }
        public void SetDirty()
        {
            _dirtyFlag = true;
        }
        protected void Repaint()
        {
            _dirtyFlag = false;
            OnRepaint.Invoke();
        }
        private void Update()
        {
            if (_dirtyFlag)
                Repaint();
        }
    }
}
