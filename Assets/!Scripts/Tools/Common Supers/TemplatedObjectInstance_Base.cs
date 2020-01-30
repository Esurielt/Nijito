using UnityEngine;
using Sirenix.OdinInspector;

namespace CommonSupers
{
    public abstract class TemplatedObjectInstance_Base
    {
        protected TemplatedObject_Base _template;

        /// <summary>
        /// The TemplatedObject that this is an instance of. Returns as abstract superclass.
        /// </summary>
        public TemplatedObject_Base Template_Base { get { return _template; } }

        public TemplatedObjectInstance_Base(TemplatedObject_Base template)
        {
            _template = template;
        }
    }

    public abstract class TemplatedObjectInstance_Base<TemplateType> : TemplatedObjectInstance_Base where TemplateType : TemplatedObject_Base
    {
        /// <summary>
        /// The TemplatedObject that this is an instance of. Returns as concrete subclass.
        /// </summary>
        public TemplateType Template { get { return (TemplateType)_template; } }

        public TemplatedObjectInstance_Base(TemplateType template) : base(template) { }
    }
}
