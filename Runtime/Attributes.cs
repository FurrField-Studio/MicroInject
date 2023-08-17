using System;

namespace FurrFieldStudio.MicroInject
{
    [AttributeUsage(AttributeTargets.Field)]
    public class Inject : Attribute
    {
        public Inject()
        {
            
        }
    }
    
    
    #region DependencyAttributes

    [AttributeUsage(AttributeTargets.Class)]
    public class Dependency : Attribute
    {
        public Type DependencyType { get; private set; }

        public Dependency(Type dependencyType)
        {
            DependencyType = dependencyType;
        }
    }
    
    [AttributeUsage(AttributeTargets.Field)]
    public class NamedDependencyField : Attribute
    {
        public NamedDependencyField()
        {
        }
    }

    #endregion
}