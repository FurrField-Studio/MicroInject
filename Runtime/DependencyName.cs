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
            get => InternalName;
            set
            {
                NameSet(value);
                InternalName = value;
            }
        }

        [SerializeField]
        private string InternalName = "";

        internal Component Component;

        public DynamicDependencyName(Component component)
        {
            Component = component;
        }

        private void NameSet(string value)
        {
            if (MicroInject.NamedDependencies.ContainsKey(InternalName))
            {
                MicroInject.NamedDependencies.Remove(InternalName);
            }

            if (MicroInject.DynamicInjectFields.ContainsKey(InternalName))
            {
                foreach (var idi in MicroInject.DynamicInjectFields[InternalName])
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
                    foreach (var idi in MicroInject.DynamicInjectFields[InternalName])
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