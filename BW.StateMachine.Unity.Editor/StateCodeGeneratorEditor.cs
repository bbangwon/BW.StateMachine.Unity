using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace BW.StateMachine.Unity.Editor
{
    [CustomEditor(typeof(StateCodeGeneratorAsset))]
    class StateCodeGeneratorEditor : UnityEditor.Editor
    {
        private static GUIContent generatorButton = new GUIContent("Generate", "Generate StateMachine");
        ReorderableList reorderList = null;

        void OnEnable()
        {
            reorderList = new ReorderableList((target as StateCodeGeneratorAsset).stateList, typeof(List<string>));
            reorderList.onAddCallback = list => {
                list.list.Add("State_" + list.list.Count);
            };

            reorderList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "State Names");
            };

            reorderList.onChangedCallback = list =>
            {
                EditorUtility.SetDirty(target);
            };

            reorderList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = (string)reorderList.list[index];
                reorderList.list[index] = EditorGUI.TextField(rect, element);
            };
        }

        public override void OnInspectorGUI()
        {
            reorderList.DoLayoutList();
            EditorGUI.BeginDisabledGroup(reorderList.list.Count == 0);
            if (GUILayout.Button(generatorButton))
            {
                if (reorderList.list.Count >= 2 && (reorderList.list as List<string>).Distinct().Count() != reorderList.list.Count)
                {
                    EditorUtility.DisplayDialog("Error!", "State name must be unique.", "OK");
                    return;
                }

                //RegEx check
                Regex regex = new Regex(@"^[A-Za-z_]\w*\w$");
                if (!regex.IsMatch(target.name))
                {
                    EditorUtility.DisplayDialog("Error!", $"It can't set StateMachine '{target.name}'", "OK");
                    return;
                }

                foreach (var s in reorderList.list as List<string>)
                {
                    if (!regex.IsMatch(s))
                    {
                        EditorUtility.DisplayDialog("Error!", $"It can't set State '{s}'", "OK");
                        return;
                    }
                }

                if (EditorUtility.DisplayDialog("Confirm", $"Do you want '{target.name}' generate?", "Yes", "No"))
                {
                    StateCodeGenerator.Generate(Application.dataPath, target.name, reorderList.list as List<string>);
                    AssetDatabase.Refresh();
                }
            }
            EditorGUI.EndDisabledGroup();
        }       
    }
}
