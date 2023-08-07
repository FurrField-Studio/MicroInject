using System;

namespace FurrFieldStudio.MicroInject
{
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