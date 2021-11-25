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

        public DynamicInject()
        {
            RegisterInSystem();
        }

        private T GetValue()
        {
            if (Dirty)
            {
                InternalValue = ObjectValue as T;
                Dirty = false;

                IsInjected = InternalValue != null;
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
        
        public bool IsInjected { get; internal set; } = false;

#if UNITY_EDITOR
        public Component ObjectValue;
#else
        internal Component ObjectValue;
#endif

        internal bool Dirty;

        private bool m_Registered = false;
        
        public void RegisterInSystem()
        {
            if (!string.IsNullOrEmpty(InternalName))
            {
                MicroInject.RegisterDynamicInject(this);
                m_Registered = true;
                NameSet(InternalName);
            }
        }

        private void NameSet(string value)
        {
            if (m_Registered)
            {
                if (MicroInject.DynamicInjectFields.ContainsKey(InternalName))
                {
                    if (MicroInject.DynamicInjectFields[InternalName].Count == 1)
                    {
                        MicroInject.DynamicInjectFields.Remove(InternalName);
                    }
                    else
                    {
                        MicroInject.DynamicInjectFields[InternalName].Remove(this);
                    }
                }

                if (!MicroInject.DynamicInjectFields.ContainsKey(value))
                {
                    MicroInject.DynamicInjectFields.Add(value, new List<InternalDynamicInject>());
                }

                MicroInject.DynamicInjectFields[value].Add(this);

                if (MicroInject.NamedDependencies.ContainsKey(value))
                {
                    ObjectValue = MicroInject.NamedDependencies[value];
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