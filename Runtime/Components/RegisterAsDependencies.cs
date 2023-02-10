using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace FurrFieldStudio.MicroInject.Components
{
    public class RegisterAsDependencies : MonoBehaviour
    {
        public bool RegisterInEditor;

        public bool UseBucket;

        public MicroInjectBucket Bucket;
        
        private void Awake()
        {
            if (RegisterInEditor == false)
            {
                MicroInject mi;
                
                if (UseBucket)
                {
                    mi = MicroInject.GetMicroInject(this);
                }
                else
                {
                    mi = MicroInject.Instance;
                }

                Component[] components = GetComponents(typeof(Component));
                Type componentType;
                foreach (var com in components)
                {
                    componentType = com.GetType();
                    if (componentType != GetType() && com is MonoBehaviour && componentType.GetCustomAttribute<Dependency>() != null)
                    {
                        if (mi.RegisterNamedDependency(com) == false)
                        {
                            mi.RegisterAsDependency(com);
                        }
                    }
                }   
            }
        }
    }
}
