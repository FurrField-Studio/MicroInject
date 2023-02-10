using System;
using System.Collections.Generic;
using System.Reflection;
using FurrFieldStudio.MicroInject.Components;
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
                if (m_MicroInject == null)
                {
                    m_MicroInject = MicroInject.GetMicroInject(value);
                }
                
                m_InternalComponent = value;
                NameSet(m_InternalName);
            }
        }
        
        private Component m_InternalComponent = null;

        private MicroInject m_MicroInject;

        private void NameSet(string value)
        {
            if (m_MicroInject.NamedDependencies.ContainsKey(m_InternalName))
            {
                m_MicroInject.NamedDependencies.Remove(m_InternalName);
            }

            if (m_MicroInject.DynamicInjectFields.ContainsKey(m_InternalName))
            {
                foreach (var idi in m_MicroInject.DynamicInjectFields[m_InternalName])
                {
                    idi.ObjectValue = null;
                    idi.Dirty = true;
                }
            }

            if (!m_MicroInject.NamedDependencies.ContainsKey(value))
            {
                m_MicroInject.NamedDependencies.Add(value, Component);

                if (m_MicroInject.DynamicInjectFields.ContainsKey(value))
                {
                    foreach (var idi in m_MicroInject.DynamicInjectFields[m_InternalName])
                    {
                        idi.ObjectValue = Component;
                        idi.Dirty = true;
                    }
                }
                else
                {
                    m_MicroInject.DynamicInjectFields.Add(value, new List<InternalDynamicInject>());
                }
            }
        }
    }
}