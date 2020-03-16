using UnityEngine;

namespace SongData
{
    public class ValueControllerEvent : UnityEngine.Events.UnityEvent<Channel.Value> { }
    public class ValueController : MonoBehaviour
    {
        protected Channel.Value _value;

        public ValueControllerEvent OnRequestSet = new ValueControllerEvent();

        private void Awake()
        {
            OnRequestSet = new ValueControllerEvent();
        }
        private void OnDestroy()
        {
            OnRequestSet.RemoveAllListeners();
        }

        public Channel.Value GetValue() => _value;

        public virtual void SetValue(Channel.Value value)
        {
            _value = value;
        }
    }
}