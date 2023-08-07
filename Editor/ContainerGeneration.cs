using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FurrFieldStudio.MicroInject.Components;
using UnityEditor;
using UnityEngine;

namespace FurrFieldStudio.MicroInject.Editor
{
    public static class ContainerGeneration
    {
        public static void GenerateConcreteBucket(this MicroInjectObject microInjectObject)
        {
            List<Component> components;

            if (microInjectObject.checkChildren)
            {
                components = microInjectObject.GetComponentsInChildren(typeof(Component)).ToList();
            }
            else
            {
                components = microInjectObject.GetComponents(typeof(Component)).ToList();
            }

            List<DependencyContainer> dependencies = new List<DependencyContainer>();

            Dependency dep;
            DependencyContainer dp;
            foreach (var com in components)
            {
                dp = new DependencyContainer(com);

                if (dp.Dependency != null && dp.ComponentType != typeof(MicroInjectObject) && com is MonoBehaviour)
                {
                    dependencies.Add(dp);
                }
            }

            foreach (Component comp in microInjectObject.dependenciesContainer)
            {
                dependencies.Add(new DependencyContainer(comp, true));
            }

            List<DependencyBlackboardElement> data = new List<DependencyBlackboardElement>();
            foreach (var dependency in dependencies)
            {
                if (dependency.IsNamed)
                {
                    DependencyBlackboardElement dependencyBlackboardElement = new DependencyBlackboardElement
                    {
                        type = dependency.Dependency.DependencyType.FullName,
                        varName = ToLowerCamelCase(dependency.Name)
                    };

                    data.Add(dependencyBlackboardElement);
                }
                else
                {
                    DependencyBlackboardElement dependencyBlackboardElement = new DependencyBlackboardElement
                    {
                        type = dependency.Dependency.DependencyType.FullName,
                        varName = ToLowerCamelCase(dependency.Dependency.DependencyType.Name)
                    };

                    data.Add(dependencyBlackboardElement);
                }
            }

            foreach (DependencyBlackboardElement dependencyBlackboard in microInjectObject.blackboardDependencies)
            {
                dependencyBlackboard.varName = ToLowerCamelCase(dependencyBlackboard.varName);
                data.Add(dependencyBlackboard);
            }

            string generatedFile = ComponentContainerGenerator.GenerateContainerGenerator(microInjectObject.generatedNamespace, microInjectObject.generatedClassName, data);

            string fullPath = microInjectObject.generatedFolder + "\\" + microInjectObject.generatedClassName + ".cs";
            File.WriteAllText(fullPath, generatedFile);
            AssetDatabase.ImportAsset(fullPath);
            AssetDatabase.Refresh();

            EditorUtility.RequestScriptReload();
        }

        public static void AddConreteBucketComponent(this MicroInjectObject microInjectObject)
        {
            Type generatedType = GetTypeFromAssembly(microInjectObject.generatedNamespace, microInjectObject.generatedClassName);

            microInjectObject.generatedContainer = microInjectObject.gameObject.GetComponent(generatedType);
            if (microInjectObject.generatedContainer == null)
            {
                microInjectObject.generatedContainer = microInjectObject.gameObject.AddComponent(generatedType);
            }

            foreach (var fi in generatedType.GetRuntimeFields())
            {
                if (fi.FieldType.IsSubclassOf(typeof(MonoBehaviour)) || fi.FieldType.IsSubclassOf(typeof(Component)) || fi.FieldType.IsInterface)
                {
                    Component comp = microInjectObject.GetComponent(fi.FieldType);
                    if (comp == null)
                    {
                        comp = microInjectObject.GetComponentInChildren(fi.FieldType);
                    }

                    fi.SetValue(microInjectObject.generatedContainer, comp);
                }
            }
        }

        private static Type GetTypeFromAssembly(string namespaceName, string typeName)
        {
            List<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("Assembly-CSharp")).ToList();

            foreach (var assembly in assemblies)
            {
                Type t = assembly.GetType(namespaceName + '.' + typeName, false);
                if (t != null)
                    return t;
            }

            throw new ArgumentException(
                "Type " + typeName + " doesn't exist in the current app domain");
        }

        private static string ToLowerCamelCase(string str) => Char.ToLowerInvariant(str[0]) + str.Substring(1);

        private class DependencyContainer
        {
            public readonly Type ComponentType;

            public readonly Dependency Dependency;

            public readonly bool IsNamed;

            public readonly string Name;

            public DependencyContainer(Component comp, bool autogenerateDependency = false)
            {
                ComponentType = comp.GetType();
                Dependency = ComponentType.GetCustomAttribute<Dependency>();

                foreach (var fi in ComponentType.GetRuntimeFields())
                {
                    NamedDependencyField mdf = fi.GetCustomAttribute<NamedDependencyField>();
                    IsNamed = mdf != null;

                    Name = IsNamed ? fi.GetValue(comp) as string : "";
                }

                if (!autogenerateDependency) return;
                if (Dependency == null)
                {
                    Dependency = new Dependency(ComponentType);
                }
            }
        }
    }
}