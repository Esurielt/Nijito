using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonSupers
{
    public abstract class KComponent
    {
        protected object _parent;

        public object Parent_Base { get { return _parent; } }

        public KComponent(object parent)
        {
            _parent = parent;
        }
    }

    public abstract class KComponent<T> : KComponent
    {
        public T Parent { get { return (T)_parent; } }

        public KComponent(T parent) : base(parent) { }
    }
}
