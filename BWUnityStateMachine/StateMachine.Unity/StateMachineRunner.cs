using System;
using System.Collections.Generic;
using UnityEngine;

namespace BW.StateMachine.Unity
{
    public class StateMachineRunner : MonoBehaviour
    {
        static StateMachineRunner instance = null;
        internal static StateMachineRunner Instance => instance ?? new GameObject("BW.StateMachine.Runner").AddComponent<StateMachineRunner>();
        internal List<Action> updateActions = null;
        void Awake()
        {
            if(instance != null)
                DestroyImmediate(this);
            else
                instance = this;
        }

        internal void AddUpdateAction(Action updateAction)
        {
            if (updateActions == null)
                updateActions = new List<Action>();

            updateActions.Add(updateAction);
        }

        void Update() => updateActions?.ForEach(u => u?.Invoke());
    }
}
