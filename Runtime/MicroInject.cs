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
        internal static Dictionary<Type, Component> Dependencies = new Dictionary<Type, Component>();
        
        //Named dependencies data
        internal static Dictionary<string, Component> NamedDependencies = new Dictionary<string, Component>();
        
        //Dynamic dependencies data
        internal static Dictionary<string, List<InternalDynamicInject>> DynamicInjectFields = new Dictionary<string, List<InternalDynamicInject>>();

        #region Registering
        
        public static void RegisterAsDependency(Component dependency)
        {
            Dependencies.Add(dependency.GetType(), dependency);
        }
        
        public static bool RegisterNamedDependency(Component component)
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

        internal static void RegisterDynamicInject(InternalDynamicInject internalDynamicInject)
        {
            if (!DynamicInjectFields.ContainsKey(internalDynamicInject.Name))
            {
                DynamicInjectFields.Add(internalDynamicInject.Name, new List<InternalDynamicInject>());
            }
            
            DynamicInjectFields[internalDynamicInject.Name].Add(internalDynamicInject);
        }

        #endregion

        #region Injecting

        public static void InjectDependencies(object toInject)
        {
            InjectDependencies(toInject, Dependencies, NamedDependencies);
        }
        
        private static void InjectDependencies(object toInject, Dictionary<Type, Component> dependencies, Dictionary<string, Component> namedDependencies)
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
        
        public static void InjectDependenciesInEditor(object toInject, Dictionary<Type, Component> dependencies, Dictionary<string, Component> namedDependencies)
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
            if (NamedDependencies.ContainsKey(key))
            {                
                fieldInfo.SetValue(component, NamedDependencies[key]);
            }
        }
        
        #endregion
        
        public static void RebuildDependencyList()
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

        public static void ClearMicroInjectLists()
        {
            Dependencies.Clear();
            NamedDependencies.Clear();
            DynamicInjectFields.Clear();
        }

#if UNITY_EDITOR
      
        #region DebugGet

        public static Dictionary<Type, Component> GetDependenciesList() => Dependencies;

        public static Dictionary<string, Component> GetNamedDependencies() => NamedDependencies;
        
        public static Dictionary<string, List<InternalDynamicInject>> GetDynamicInjectFields() => DynamicInjectFields;

        #endregion

#endif
    }
}