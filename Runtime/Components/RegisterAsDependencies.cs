
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FurrFieldStudio.MicroInject.Components
{
    public class RegisterAsDependencies : MonoBehaviour
    {
        public bool RegisterInEditor;
        
        private void Awake()
        {
            if (RegisterInEditor == false)
            {
                Component[] components = GetComponents(typeof(Component));
                Type componentType;
                foreach (var com in components)
                {
                    componentType = com.GetType();
                    if (componentType != GetType() && com is MonoBehaviour && componentType.GetCustomAttribute<Dependency>() != null)
                    {
                        if (MicroInject.RegisterNamedDependency(com) == false)
                        {
                            MicroInject.RegisterAsDependency(com);
                        }
                    }
                }   
            }
        }
    }
}
