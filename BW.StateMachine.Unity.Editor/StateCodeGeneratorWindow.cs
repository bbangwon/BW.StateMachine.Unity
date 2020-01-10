using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BW.StateMachine.Unity.Editor
{
    class StateCodeGeneratorWindow : EditorWindow
    {
        private static GUIContent generatorButton = new GUIContent("Generate", "Generate StateMachine");

        string statesName = "MyState";  //DefaultStateName = MyState

        ReorderableList list = null;

        List<string> stateList = new List<string>();

        [MenuItem("BW.StateMachine/StateMachine Generator")]
        public static void ShowWindow()
        {
            GetWindow<StateCodeGeneratorWindow>(true, "StateMachine Generator");
        }

        void OnEnable()
        {
            list = new ReorderableList(stateList, typeof(List<string>));

            list.onAddCallback = (ReorderableList list) => {
                list.list.Add("State_" + list.list.Count);
            };

            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "State Names");
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = (string)list.list[index];
                //rect.y += 2;
                list.list[index] = EditorGUI.TextField(rect, element);
            };
        }

        void OnGUI()
        {
            statesName = EditorGUILayout.TextField("StateMachine Name", statesName);
            list.DoLayoutList();


            EditorGUI.BeginDisabledGroup(stateList.Count == 0);

            if (GUILayout.Button(generatorButton))
            {
                if (stateList.Count >= 2)
                {
                    if (stateList.Distinct().Count() != stateList.Count)
                    {
                        EditorUtility.DisplayDialog("Error!", "State name must be unique.", "OK");
                        return;
                    }
                }

                //Regex
                Regex regex = new Regex(@"^[A-Za-z_]\w*\w$");
                if (!regex.IsMatch(statesName))
                {
                    EditorUtility.DisplayDialog("Error!", $"It can't set StateMachine '{statesName}'", "OK");
                    return;
                }

                foreach (var item in stateList)
                {
                    if (!regex.IsMatch(item))
                    {
                        EditorUtility.DisplayDialog("Error!", $"It can't set State '{item}'", "OK");
                        return;
                    }
                }
                //~Regex

                if (EditorUtility.DisplayDialog("Confirm", $"Do you want '{statesName}' generate?", "Yes", "No"))
                {
                    StateCodeGenerator.Generate(Application.dataPath, statesName, stateList);
                    AssetDatabase.Refresh();
                }
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
