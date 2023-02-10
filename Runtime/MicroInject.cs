using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using FurrFieldStudio.MicroInject.Components;
using UnityEngine;

namespace FurrFieldStudio.MicroInject
{
    public class MicroInject
    {
        public static MicroInject Instance {
            get
            {
                if (m_MicroInject == null)
                {
                    m_MicroInject = new MicroInject();
                }

                return m_MicroInject;
            }
        }

        private static MicroInject m_MicroInject;
        
        internal Dictionary<Type, object> Dependencies = new  Dictionary<Type, object>();

        //Named dependencies data
        internal Dictionary<string, object> NamedDependencies = new Dictionary<string, object>();
        
        //Dynamic dependencies data
        internal Dictionary<string, List<InternalDynamicInject>> DynamicInjectFields = new Dictionary<string, List<InternalDynamicInject>>();

        public static MicroInject GetMicroInject(Component comp) => comp.TryGetComponent(out MicroInjectBucket bucket) ? bucket.MicroInject : Instance;
        
        #region Registering
        
        public void RegisterAsDependency(object dependency)
        {
            Dependency dep = dependency.GetType().GetCustomAttribute<Dependency>();

            if (dep != null)
            {
                if (dep.DependencyType != null)
                {
                    Dependencies.Add(dep.DependencyType, dependency);
                    return;
                }
            }
            
            Dependencies.Add(dependency.GetType(), dependency);
        }
        
        public bool RegisterNamedDependency(object component)
        {
            FieldInfo[] infos = component.GetType().GetFields();
            foreach (var fi in infos)
            {
                if (fi.IsDefined(typeof(NamedDependencyField), true) && fi.FieldType == typeof(string))
                {
                    string dependencyName = (string) fi.GetValue(component);
                    if (!NamedDependencies.ContainsKey(dependencyName))
                    {
                        NamedDependencies.Add(dependencyName, component);
                        return true;
                    }
                }
            }

            return false;
        }

        internal void RegisterDynamicInject(InternalDynamicInject internalDynamicInject)
        {
            if (!DynamicInjectFields.ContainsKey(internalDynamicInject.Name))
            {
                DynamicInjectFields.Add(internalDynamicInject.Name, new List<InternalDynamicInject>());
            }
            
            DynamicInjectFields[internalDynamicInject.Name].Add(internalDynamicInject);
        }

        #endregion

        #region Injecting

        public void InjectDependencies(object toInject)
        {
            InjectDependencies(toInject, Dependencies, NamedDependencies);
        }
        
        private void InjectDependencies(object toInject, Dictionary<Type, object> dependencies, Dictionary<string, object> namedDependencies)
        {
            FieldInfo[] infos = toInject.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            List<FieldInfo> filteredInfos = new List<FieldInfo>(infos.Length);
            foreach (var fi in infos)
            {
                if (fi.IsDefined(typeof(Inject), true))
                {
                    if (fi.GetCustomAttribute<Inject>().InjectInEditor == false)
                    {
                        filteredInfos.Add(fi);
                    }
                }
            }

            InjectFilteredInternal(filteredInfos, toInject, dependencies, namedDependencies);
        }
        
#if UNITY_EDITOR
        
        public void InjectDependenciesInEditor(object toInject, Dictionary<Type, object> dependencies, Dictionary<string, object> namedDependencies)
        {
            FieldInfo[] infos = toInject.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            List<FieldInfo> filteredInfos = new List<FieldInfo>(infos.Length);
            foreach (var fi in infos)
            {
                if (fi.IsDefined(typeof(Inject), true))
                {
                    if (fi.GetCustomAttribute<Inject>().InjectInEditor)
                    {
                        filteredInfos.Add(fi);
                    }
                }
            }

            InjectFilteredInternal(filteredInfos, toInject, dependencies, namedDependencies);
        }
        
#endif

        private static void InjectFilteredInternal(List<FieldInfo> filteredInfos, object toInject, Dictionary<Type, object> dependencies, Dictionary<string, object> namedDependencies)
        {
            Inject inject;
            foreach (var fi in filteredInfos)
            {
                inject = fi.GetCustomAttribute<Inject>();
                if (string.IsNullOrEmpty(inject.Key))
                {
                    if (dependencies.ContainsKey(fi.FieldType))
                    {
                        fi.SetValue(toInject, dependencies[fi.FieldType]);
                    }
                }
                else
                {
                    if (namedDependencies.ContainsKey(inject.Key))
                    {
                        if (namedDependencies[inject.Key].GetType() == fi.FieldType)
                        {
                            fi.SetValue(toInject, namedDependencies[inject.Key]);
                        }
                    }
                }
            }
        }
        
        
        public void InjectNamedDependency(string fieldName, object component)
        {
            FieldInfo field = component.GetType().GetField(fieldName);
            Inject inject = field.GetCustomAttribute<Inject>();
            if (inject != null)
            {
                InternalInjectNamedDependency(field, inject.Key, component);
            }
        }
        
        private void InternalInjectNamedDependency(FieldInfo fieldInfo, string key, object component)
        {
            if (NamedDependencies.ContainsKey(key))
            {                
                fieldInfo.SetValue(component, NamedDependencies[key]);
            }
        }
        
        #endregion
        
        public void RebuildDependencyList()
        {
            var filteredDependencies = Dependencies.Where(kvp => kvp.Value == null).ToArray();
            Dependencies.Clear();
            foreach (var kvp in filteredDependencies)
            {
                Dependencies.Add(kvp.Key, kvp.Value);
            }
            
            var filteredNamedDependencies = NamedDependencies.Where(kvp => kvp.Value == null).ToArray();
            NamedDependencies.Clear();
            foreach (var kvp in filteredNamedDependencies)
            {
                NamedDependencies.Add(kvp.Key, kvp.Value);
            }
        }

        public void ClearMicroInjectLists()
        {
            Dependencies.Clear();
            NamedDependencies.Clear();
            DynamicInjectFields.Clear();
        }

#if UNITY_EDITOR
      
        #region DebugGet

        public Dictionary<Type, object> GetDependenciesList() => Dependencies;

        public Dictionary<string, object> GetNamedDependencies() => NamedDependencies;
        
        public Dictionary<string, List<InternalDynamicInject>> GetDynamicInjectFields() => DynamicInjectFields;

        #endregion

#endif
    }
}