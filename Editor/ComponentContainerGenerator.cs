using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FurrFieldStudio.MicroInject.Components;
using Microsoft.CSharp;
using UnityEngine;

namespace FurrFieldStudio.MicroInject.Editor
{
    public class ComponentContainerGenerator
    {
        public static string GenerateContainerGenerator(string namespaceName, string className, List<DependencyBlackboardElement> data)
        {
            //setup class
            CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace(namespaceName);
            CodeTypeDeclaration classDeclaration = new CodeTypeDeclaration(className)
            {
                IsClass = true,
                TypeAttributes = TypeAttributes.Public | TypeAttributes.Sealed,
                BaseTypes = { typeof(MonoBehaviour) }
            };
                
            //class content
            foreach (DependencyBlackboardElement dependencyBlackboardElement in data)
            {
                CodeMemberField layerField = new CodeMemberField(dependencyBlackboardElement.type, dependencyBlackboardElement.varName)
                {
                    Attributes = MemberAttributes.Public
                };
                
                classDeclaration.Members.Add(layerField);
            }

            //adding namespaces
            codeNamespace.Types.Add(classDeclaration);
            codeCompileUnit.Namespaces.Add(codeNamespace);

            //generating code
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CodeGeneratorOptions options = new CodeGeneratorOptions
            {
                BracingStyle = "C"
            };

            //flush
            StringWriter writer = new StringWriter();
            codeProvider.GenerateCodeFromCompileUnit(codeCompileUnit, writer, options);
            writer.Flush();
            return writer.ToString();
        }
    }
}