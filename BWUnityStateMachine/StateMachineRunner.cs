using System.Collections.Generic;
using UnityEngine;

namespace BW.StateMachine.Unity
{
    public class StateMachineRunner : MonoBehaviour
    {
        static StateMachineRunner instance = null;
        internal static StateMachineRunner Instance => instance ?? new GameObject("BW.StateMachine.Runner").AddComponent<StateMachineRunner>();
        internal List<IUpdatable> updatables = null;
        void Awake()
        {
            if(instance != null)
                DestroyImmediate(this);
            else
                instance = this;
        }

        internal void AddUpdatable(IUpdatable updatable)
        {
            if (updatables == null)
                updatables = new List<IUpdatable>();

            updatables.Add(updatable);
        }

        void Update() => updatables?.ForEach(u => u.Update());
    }
}
