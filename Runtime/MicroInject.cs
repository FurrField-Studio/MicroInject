using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace FurrFieldStudio.MicroInject
{
    public class MicroInject
    {
        private static Dictionary<Type, Component> m_Dependencies = new Dictionary<Type, Component>();
        
        //Named dependencies data
        private static Dictionary<string, Component> m_NamedDependencies = new Dictionary<string, Component>();

        #region DependenciesStuff
        
        public static void RegisterAsDependency(Component dependency)
        {
            m_Dependencies.Add(dependency.GetType(), dependency);
        }
        
        public static bool RegisterNamedDependency(Component component)
        {
            FieldInfo[] infos = component.GetType().GetFields();
            foreach (var fi in infos)
            {
                if (fi.IsDefined(typeof(NamedDependencyField), true) && fi.FieldType == typeof(string))
                {
                    m_NamedDependencies.Add((string) fi.GetValue(component), component);
                    return true;
                }
            }

            return false;
        }

        public static void ChangeNameOfNamedDependency(string name, Component dependency)
        {
            foreach (var kvp in m_NamedDependencies.ToArray())
            {
                if (kvp.Value == dependency)
                {
                    m_NamedDependencies.Remove(name);
                    m_NamedDependencies.Add(name, dependency);
                }
            }
        }
        
        #endregion

        #region Injecting

        public static void InjectDependencies(object toInject)
        {
            InjectDependencies(toInject, m_Dependencies, m_NamedDependencies);
        }
        
        private static void InjectDependencies(object toInject, Dictionary<Type, Component> dependencies, Dictionary<string, Component> namedDependencies)
        {
            FieldInfo[] infos = toInject.GetType().GetFields();
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
        
        public static void InjectDependenciesInEditor(object toInject, Dictionary<Type, Component> dependencies, Dictionary<string, Component> namedDependencies)
        {
            FieldInfo[] infos = toInject.GetType().GetFields();
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

        private static void InjectFilteredInternal(List<FieldInfo> filteredInfos, object toInject, Dictionary<Type, Component> dependencies, Dictionary<string, Component> namedDependencies)
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
        
        //Here you dont need [Inject] attribute to inject dependency, usefull for dependencies that change name during runtime (currently there is no way to auto inject dynamically named dependencies)
        public static void InjectDynamicallyNamedDependency(string fieldName, string key, object component)
        {
            InternalInjectNamedDependency(component.GetType().GetField(fieldName), key, component);
        }

        public static void InjectNamedDependency(string fieldName, object component)
        {
            FieldInfo field = component.GetType().GetField(fieldName);
            Inject inject = field.GetCustomAttribute<Inject>();
            if (inject != null)
            {
                InternalInjectNamedDependency(field, inject.Key, component);
            }
        }

        private static void InternalInjectNamedDependency(FieldInfo fieldInfo, string key, object component)
        {
            if (m_NamedDependencies.ContainsKey(key))
            {                
                fieldInfo.SetValue(component, m_NamedDependencies[key]);
            }
        }
        
        #endregion
        
        public static void RebuildDependencyList()
        {
            var filteredDependencies = m_Dependencies.Where(kvp => kvp.Value == null).ToArray();
            m_Dependencies.Clear();
            foreach (var kvp in filteredDependencies)
            {
                m_Dependencies.Add(kvp.Key, kvp.Value);
            }
            
            var filteredNamedDependencies = m_NamedDependencies.Where(kvp => kvp.Value == null).ToArray();
            m_NamedDependencies.Clear();
            foreach (var kvp in filteredNamedDependencies)
            {
                m_NamedDependencies.Add(kvp.Key, kvp.Value);
            }
        }
    }
}