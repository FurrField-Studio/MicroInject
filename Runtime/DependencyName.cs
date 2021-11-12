using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FurrFieldStudio.MicroInject
{
    [Serializable]
    public class DynamicDependencyName
    {
        public string Name
        {
            get => m_InternalName;
            set
            {
                NameSet(value);
                m_InternalName = value;
            }
        }

        [SerializeField]
        private string m_InternalName = "";

        public Component Component
        {
            private get
            {
                return m_InternalComponent;
            }
            set
            {
                if (m_InternalComponent == null)
                {
                    m_InternalComponent = value;
                    NameSet(m_InternalName);
                }
            }
        }
        
        private Component m_InternalComponent = null;

        private void NameSet(string value)
        {
            if (MicroInject.NamedDependencies.ContainsKey(m_InternalName))
            {
                MicroInject.NamedDependencies.Remove(m_InternalName);
            }

            if (MicroInject.DynamicInjectFields.ContainsKey(m_InternalName))
            {
                foreach (var idi in MicroInject.DynamicInjectFields[m_InternalName])
                {
                    idi.ObjectValue = null;
                    idi.IsInjected = false;
                    idi.Dirty = true;
                }
            }

            if (!MicroInject.NamedDependencies.ContainsKey(value))
            {
                MicroInject.NamedDependencies.Add(value, Component);

                if (MicroInject.DynamicInjectFields.ContainsKey(value))
                {
                    foreach (var idi in MicroInject.DynamicInjectFields[m_InternalName])
                    {
                        idi.ObjectValue = Component;
                        idi.IsInjected = true;
                        idi.Dirty = true;
                    }
                }
                else
                {
                    MicroInject.DynamicInjectFields.Add(value, new List<InternalDynamicInject>());
                }
            }
        }
    }
}