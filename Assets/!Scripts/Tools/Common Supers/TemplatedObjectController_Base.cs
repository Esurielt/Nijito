using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonSupers
{
    public abstract class TemplatedObjectController_Base
    {
        protected TemplatedObjectInstance_Base _instance;

        public TemplatedObjectInstance_Base Instance_Base { get { return _instance; } }

        public TemplatedObjectController_Base(TemplatedObjectInstance_Base instance)
        {
            _instance = instance;
        }
    }

    public abstract class TemplatedObjectController_Base<TemplateType> : TemplatedObjectController_Base where TemplateType : TemplatedObject_Base
    {
        public TemplatedObjectInstance_Base<TemplateType> Instance { get { return (TemplatedObjectInstance_Base<TemplateType>)_instance; } }

        public TemplatedObjectController_Base(TemplatedObjectInstance_Base<TemplateType> instance) : base(instance) { }

        public InstanceType GetInstance<InstanceType>() where InstanceType : TemplatedObjectInstance_Base<TemplateType>
        {
            return (InstanceType)Instance;
        }
    }
}