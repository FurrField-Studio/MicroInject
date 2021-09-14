namespace FurrFieldStudio.MicroInject
{
    #region InjectionAttributes

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class Inject : System.Attribute
    {
        //Used for statically named dependencies
        public string Key;
        public bool InjectInEditor;

        public Inject(string key)
        {
            Key = key;
            InjectInEditor = false;
        }
        
        public Inject(bool injectInEditor)
        {
            Key = "";
            InjectInEditor = injectInEditor;
        }
        
        public Inject(string key = "", bool injectInEditor = false)
        {
            Key = key;
            InjectInEditor = injectInEditor;
        }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class AutoInjectInEditor : System.Attribute
    {
        public AutoInjectInEditor()
        {
        }
    }

    #endregion

    #region DependencyAttributes

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class Dependency : System.Attribute
    {
        public Dependency()
        {
        }
    }
    
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class NamedDependencyField : System.Attribute
    {
        public NamedDependencyField()
        {
        }
    }

    #endregion
}