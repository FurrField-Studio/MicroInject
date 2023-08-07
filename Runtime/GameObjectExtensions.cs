using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace DefaultNamespace
{
    public static class GameObjectExtensions
    {
        internal static Dictionary<GameObject, Component> bindings = new Dictionary<GameObject, Component>();
        
        public static void BindMasterComponent(this MonoBehaviour mb, Component comp)
        {
            bindings.Add(mb.gameObject, comp);
        }

        [CanBeNull]
        public static Component GetMasterComponent(GameObject go)
        {
            if (!bindings.ContainsKey(go)) return null;

            return bindings[go];
        }
    }
}