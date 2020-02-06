using Beatmap.Interfaces;

namespace Beatmap
{
    public class ValueWrapper : IValueWrapper
    {
        private Channel.Value _backingField;

        public ValueWrapper(Channel.Value backingField)
        {
            _backingField = backingField;
        }

        public bool Equals(IValueWrapper other)
        {
            return GetValue() == other.GetValue();
        }

        public Channel.Value GetValue() => _backingField;
        public void SetValue(Channel.Value value) => _backingField = value;
    }
}