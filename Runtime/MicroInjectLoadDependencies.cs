using System;
using System.Collections.Generic;
using FurrFieldStudio.MicroInject.Components;
using UnityEngine;

namespace FurrFieldStudio.MicroInject
{
    [DefaultExecutionOrder(-100)]
    public class MicroInjectLoadDependencies : MonoBehaviour
    {
        public List<Component> DependenciesContainer;
        
        private void Awake()
        {
            MicroInject mi = MicroInject.GetMicroInject(this);

            foreach (var component in DependenciesContainer)
            {
                mi.RegisterAsDependency(component);
            }
        }
    }
}