using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace BW.StateMachine.Unity.Editor
{
    public class StateCodeGenerator
    {
        public static void GenerateCSharpCode(CodeCompileUnit unit, string fileName)
        {
            string directory = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            using (StreamWriter sw = new StreamWriter(fileName, false))
            {
                IndentedTextWriter tw = new IndentedTextWriter(sw);
                provider.GenerateCodeFromCompileUnit(unit, tw, new CodeGeneratorOptions() { BracingStyle = "C", BlankLinesBetweenMembers = false });
                tw.Close();
            }
        }

        public static void Generate(string path, string stateMachineName, List<string> stateNames)
        {
            string fileFolderName = Path.Combine(path, "BW.StateMachine", stateMachineName);
            //1. Enum file Generate
            CodeCompileUnit machineUnit = new CodeCompileUnit();
            CodeNamespace codeNamespace = new CodeNamespace();

            string filePath = Path.Combine(fileFolderName, stateMachineName + ".cs");
            CodeTypeDeclaration machineCode = new CodeTypeDeclaration(stateMachineName)
            {
                IsEnum = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };

            foreach (var item in stateNames)
                machineCode.Members.Add(new CodeMemberField(stateMachineName, item));

            codeNamespace.Types.Add(machineCode);
            machineUnit.Namespaces.Add(codeNamespace);

            GenerateCSharpCode(machineUnit, filePath);

            //2. state Class Generate
            foreach (var item in stateNames)
            {
                CodeCompileUnit stateUnit = new CodeCompileUnit();
                CodeNamespace stateCodeNamespace = new CodeNamespace();
                stateCodeNamespace.Imports.Add(new CodeNamespaceImport("BW.StateMachine.Unity"));

                string stateFilePath = Path.Combine(fileFolderName, item + ".cs");

                CodeTypeDeclaration stateCode = new CodeTypeDeclaration(item)
                {
                    IsClass = true,
                    TypeAttributes = System.Reflection.TypeAttributes.Public
                };
                stateCode.BaseTypes.Add($"StateBase<{stateMachineName}>");

                stateCode.Members.Add(new CodeMemberMethod
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Override,
                    Name = "Enter",
                    ReturnType = new CodeTypeReference(typeof(void))
                });

                stateCode.Members.Add(new CodeMemberMethod
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Override,
                    Name = "Update",
                    ReturnType = new CodeTypeReference(typeof(void))
                });

                stateCode.Members.Add(new CodeMemberMethod
                {
                    Attributes = MemberAttributes.Public | MemberAttributes.Override,
                    Name = "Exit",
                    ReturnType = new CodeTypeReference(typeof(void))
                });

                stateCodeNamespace.Types.Add(stateCode);
                stateUnit.Namespaces.Add(stateCodeNamespace);

                GenerateCSharpCode(stateUnit, stateFilePath);
            }

            //3. Using Class
            CodeCompileUnit stateUseUnit = new CodeCompileUnit();
            CodeNamespace stateUseCodeNamespace = new CodeNamespace();
            stateUseCodeNamespace.Imports.Add(new CodeNamespaceImport("BW.StateMachine.Unity"));

            string stateUseFileName = stateMachineName + "Behaviour";
            string stateUseFilePath = Path.Combine(fileFolderName, stateUseFileName + ".cs");

            CodeTypeDeclaration stateUseCode = new CodeTypeDeclaration(stateUseFileName)
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };
            stateUseCode.BaseTypes.Add(typeof(MonoBehaviour));

            //Member Field             
            var stateMachineCodeType = new CodeTypeReference("StateMachine", new CodeTypeReference(stateMachineName));
            var stateMachineMemberFieldName = "stateMachine";
            var stateMachineMemberGetPropertyName = "StateMachine";

            stateUseCode.Members.Add(new CodeMemberField
            {
                Attributes = MemberAttributes.Private,
                Name = stateMachineMemberFieldName,
                Type = stateMachineCodeType
            });

            //Property
            var stateUseCodeProperty = new CodeMemberProperty
            {
                Attributes = MemberAttributes.Public,
                Name = stateMachineMemberGetPropertyName,
                HasGet = true,
                Type = stateMachineCodeType,
            };
            stateUseCodeProperty.GetStatements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), stateMachineMemberFieldName)));
            stateUseCode.Members.Add(stateUseCodeProperty);            

            CodeMemberMethod startMethod = new CodeMemberMethod
            {
                Name = "Start",
                ReturnType = new CodeTypeReference(typeof(void))
            };
            startMethod.Statements.Add(
                new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), stateMachineMemberFieldName),
                        new CodeObjectCreateExpression(stateMachineCodeType, new CodeSnippetExpression($"{stateMachineName}.{stateNames[0]}")))
                );

            stateUseCode.Members.Add(startMethod);

            stateUseCodeNamespace.Types.Add(stateUseCode);
            stateUseUnit.Namespaces.Add(stateUseCodeNamespace);

            GenerateCSharpCode(stateUseUnit, stateUseFilePath);

        }
    }
}
