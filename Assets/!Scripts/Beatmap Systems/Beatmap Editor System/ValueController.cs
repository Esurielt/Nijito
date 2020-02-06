using Beatmap.Interfaces;
using UnityEngine;

namespace Beatmap
{
    public class ValueController : MonoBehaviour, IValueWrapper
    {
        public Channel.Value Value { get; private set; }     //the currently-represented channel value

        public Channel.Value GetValue() => Value;

        public virtual void SetValue(Channel.Value value)
        {
            Value = value;
        }

        public bool Equals(IValueWrapper other)
        {
            return GetValue() == other.GetValue();
        }
    }
}