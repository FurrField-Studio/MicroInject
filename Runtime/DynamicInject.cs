using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using Component = UnityEngine.Component;

namespace FurrFieldStudio.MicroInject
{
    [Serializable]
    public class DynamicInject<T> : InternalDynamicInject where T : Component
    {
        public T Value => GetValue();

        [SerializeField]
        private T InternalValue;

        public DynamicInject(Component comp)
        {
            RegisterInSystem(comp);
        }

        private T GetValue()
        {
            if (Dirty)
            {
                InternalValue = ObjectValue as T;
                Dirty = false;
            }

            return InternalValue;
        }
    }

    [Serializable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class InternalDynamicInject
    {
        public string Name
        {
            get => InternalName;
            set
            {
                NameSet(value);
                InternalName = value;
            }
        }

        [SerializeField]
        internal string InternalName = "";

        public bool IsInjected => ObjectValue != null;

#if UNITY_EDITOR
        public object ObjectValue;
#else
        internal object ObjectValue;
#endif

        internal bool Dirty;

        private bool m_Registered = false;

        private Component m_Component;
        
        public void RegisterInSystem(Component comp)
        {
            if (!string.IsNullOrEmpty(InternalName))
            {
                m_Component = comp;
                MicroInject.GetMicroInject(comp).RegisterDynamicInject(this);
                m_Registered = true;
                NameSet(InternalName);
            }
        }

        private void NameSet(string value)
        {
            if (m_Registered)
            {
                MicroInject mi = MicroInject.GetMicroInject(m_Component);
                
                if (mi.DynamicInjectFields.ContainsKey(InternalName))
                {
                    if (mi.DynamicInjectFields[InternalName].Count == 1)
                    {
                        mi.DynamicInjectFields.Remove(InternalName);
                    }
                    else
                    {
                        mi.DynamicInjectFields[InternalName].Remove(this);
                    }
                }

                if (!mi.DynamicInjectFields.ContainsKey(value))
                {
                    mi.DynamicInjectFields.Add(value, new List<InternalDynamicInject>());
                }

                mi.DynamicInjectFields[value].Add(this);

                if (mi.NamedDependencies.ContainsKey(value))
                {
                    ObjectValue = mi.NamedDependencies[value];
                }
                else
                {
                    ObjectValue = null;
                }
            
                Dirty = true;
            }
        }
    }
}