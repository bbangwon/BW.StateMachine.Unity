using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;

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
            CodeTypeDeclaration machineCode = new CodeTypeDeclaration(stateMachineName);
            machineCode.IsEnum = true;
            machineCode.TypeAttributes = System.Reflection.TypeAttributes.Public;

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

                CodeTypeDeclaration stateCode = new CodeTypeDeclaration(item);
                stateCode.IsClass = true;
                stateCode.BaseTypes.Add("StateBase<" + stateMachineName + ">");
                stateCode.TypeAttributes = System.Reflection.TypeAttributes.Public;

                CodeMemberMethod enterMethod = new CodeMemberMethod();
                enterMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
                enterMethod.Name = "Enter";
                enterMethod.ReturnType = new CodeTypeReference(typeof(void));
                stateCode.Members.Add(enterMethod);

                CodeMemberMethod updateMethod = new CodeMemberMethod();
                updateMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
                updateMethod.Name = "Update";
                updateMethod.ReturnType = new CodeTypeReference(typeof(void));
                stateCode.Members.Add(updateMethod);

                CodeMemberMethod exitMethod = new CodeMemberMethod();
                exitMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
                exitMethod.Name = "Exit";
                exitMethod.ReturnType = new CodeTypeReference(typeof(void));
                stateCode.Members.Add(exitMethod);

                stateCodeNamespace.Types.Add(stateCode);
                stateUnit.Namespaces.Add(stateCodeNamespace);

                GenerateCSharpCode(stateUnit, stateFilePath);
            }
        }
    }
}
