using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace FurrFieldStudio.MicroInject.Components
{
    [DefaultExecutionOrder(-100)]
    public class MicroInjectObject : MonoBehaviour
    {
        public bool checkChildren;
        
        public bool registerGlobally;
        
        public string objectName;
        
        public bool enableAutogenerateContainer;
        
        public string generatedFolder;
        public string generatedNamespace;
        public string generatedClassName;
        
        public List<Component> dependenciesContainer;
        
        public List<ScriptableObject> soContainer;
        
        public List<DependencyBlackboardElement> blackboardDependencies;
        
        public Component generatedContainer;

        private void Awake()
        {
            if (registerGlobally)
            {
                this.BindMasterComponent(generatedContainer);
                MicroInject.RegisterGlobalBucket(objectName, generatedContainer);
            }
        }
    }

    [Serializable]
    public class DependencyBlackboardElement
    {
        [TypeField(typeof(Object))]
        public string type;
        public string varName;
    }
}