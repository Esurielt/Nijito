using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEventRegistry
{
    public class UnityEventRegistry
    {
        //'dictionary' of events and the unity actions they trigger
        protected List<KeyValuePair<UnityEvent, UnityAction>> _registry = new List<KeyValuePair<UnityEvent, UnityAction>>();

        private bool _subscribed = false;

        public string Name { get; private set; }
        public UnityEventRegistry(string name)
        {
            Name = name;
        }

        public void RegisterEvent(UnityEvent unityEvent, UnityAction unityAction)
        {
            _registry.Add(new KeyValuePair<UnityEvent, UnityAction>(unityEvent, unityAction));
        }
        public void UnregisterAllEvents()
        {
            _registry.ForEach(kvp => kvp.Key.RemoveListener(kvp.Value));
        }
        public void SubscribeToEvents()     //called on enable
        {
            if (_subscribed || _registry.Count == 0)
                return;

            Game.LogFormat(Logging.Category.SONG_DATA, "{0}: Subscribing to editor events...", Logging.Level.LOG, Name);
            foreach (var kvp in _registry)
            {
                kvp.Key.AddListener(kvp.Value);
            }

            _subscribed = true;
        }
        public void UnsubscribeFromEvents()     //called on disable
        {
            if (!_subscribed || _registry.Count == 0)
                return;

            Game.LogFormat(Logging.Category.SONG_DATA, "{0}: Unsubscribing from editor events...", Logging.Level.LOG, Name);
            foreach (var kvp in _registry)
            {
                kvp.Key.RemoveListener(kvp.Value);
            }

            _subscribed = false;
        }
    }
}