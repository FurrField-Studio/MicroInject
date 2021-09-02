using System;
using System.Collections.Generic;
using UnityEngine;

namespace FurrFieldStudio.MicroInject
{
    [DefaultExecutionOrder(-100)]
    public class LoadDependencies : MonoBehaviour
    {
        public List<Component> DependenciesContainer;
        
        private void Awake()
        {
            foreach (var component in DependenciesContainer)
            {
                MicroInject.RegisterAsDependency(component);
            }
        }
    }
}