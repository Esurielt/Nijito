using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MessageBoxes
{
    public class MessageBoxManager : MonoBehaviour
    {
        public MessageBoxController MessageBoxPrefab;

        private readonly List<MessageBoxController> _messageList = new List<MessageBoxController>();
        public int ActiveMessageCount => _messageList.Count;

        public void MessageBox(MessageTemplate template)
        {
            MessageBox(template, new Stack<MessageBoxController>());
        }
        public void MessageBox(MessageTemplate template, Stack<MessageBoxController> parentBoxes)
        {
            var controller = Instantiate(MessageBoxPrefab, Game.Self.MessageBoxContainer);
            controller.Initialize(this, parentBoxes, template.Title, template.Message, template.Buttons);
            _messageList.Add(controller);
        }
        public void RemoveMessage(MessageBoxController controller)
        {
            _messageList.Remove(controller);
        }
    }
    public class MessageTemplate
    {
        public readonly string Title;
        public readonly string Message;
        public readonly ButtonTemplate[] Buttons;
        public MessageTemplate(string title, string message, params ButtonTemplate[] buttons)
        {
            Title = title;
            Message = message;
            Buttons = buttons;
        }

        public static MessageTemplate GetConfirmationBox(string title, string confirmationCheck, Action onYes = null, Action onNo = null)
        {
            var yesButton = new ButtonTemplate("Yes", onYes, true);
            var noButton = new ButtonTemplate("No", onNo, true);
            return new MessageTemplate(title, confirmationCheck, yesButton, noButton);
        }
    }
    public class ButtonTemplate
    {
        public readonly string Name;
        protected readonly System.Action _action;
        protected readonly bool _closeStackOnSelect;
        public ButtonTemplate(string name, Action action = null, bool closeWindow = true)
        {
            Name = name;
            _action = action;
            _closeStackOnSelect = closeWindow;
        }
        public virtual List<Action> GetActions(MessageBoxController parentBox)
        {
            var actions = new List<Action>();
            if (_action != null)
            {
                actions.Add(_action);
            }
            if (_closeStackOnSelect)
            {
                actions.Add(parentBox.Close);
            }
            return actions;
        }

        public static ButtonTemplate Ok = new ButtonTemplate("OK", null, true);
        public static ButtonTemplate Cancel = new ButtonTemplate("Cancel", null, true);
    }
    public class ButtonTemplate_ChildBox : ButtonTemplate
    {
        public readonly MessageTemplate _template;
        public ButtonTemplate_ChildBox(string name, MessageTemplate template) : base(name, null, false) => _template = template;
        public override List<Action> GetActions(MessageBoxController parentBox)
        {
            var actions = base.GetActions(parentBox);
            actions.Add(() => parentBox.SpawnChildBox(_template));
            return actions;
        }
    }
}