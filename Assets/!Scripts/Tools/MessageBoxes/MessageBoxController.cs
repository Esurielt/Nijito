using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MessageBoxes
{
    public class MessageBoxController : MonoBehaviour
    {
        public TMPro.TMP_Text TitleText;
        public TMPro.TMP_Text MessageText;
        public Transform ButtonContainer;
        public Button ButtonPrefab;

        protected MessageBoxManager _manager;
        protected Stack<MessageBoxController> _parentBoxes;
        protected List<Button> _buttons = new List<Button>();

        public virtual void Initialize(MessageBoxManager manager, Stack<MessageBoxController> parentBoxes, string title, string message, params ButtonTemplate[] menuButtons)
        {
            _manager = manager;
            _parentBoxes = parentBoxes;
            TitleText.text = title;
            MessageText.text = message;

            if (menuButtons != null && menuButtons.Length > 0)
            {
                InitializeButtons(menuButtons);
            }
            else
            {
                InitializeButtons(ButtonTemplate.Ok);
            }
        }
        protected void InitializeButtons(params ButtonTemplate[] menuButtons)
        {
            foreach (var menuButton in menuButtons)
            {
                var controller = Instantiate(ButtonPrefab, ButtonContainer);
                controller.GetComponentInChildren<TMPro.TMP_Text>().text = menuButton.Name;

                foreach (var action in menuButton.GetActions(this))
                {
                    controller.onClick.AddListener(() => action());
                }

                _buttons.Add(controller);
            }
        }
        public void SpawnChildBox(MessageTemplate messageTemplate)
        {
            _parentBoxes.Push(this);
            _manager.MessageBox(messageTemplate, _parentBoxes);
        }
        public void Close()
        {
            Destroy(gameObject);
            _manager.RemoveMessage(this);

            if (_parentBoxes.Count != 0)
            {
                _parentBoxes.Pop().Close();
            }
        }
        private void OnDestroy()
        {
            foreach (var button in _buttons)
            {
                button.onClick.RemoveAllListeners();
            }
        }
    }
}