using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BW.StateMachine.Unity.Editor
{
    [CreateAssetMenu(menuName = "BW.StateMachine/StateMachine Generator Asset", fileName = "MyStateName")]
    class StateCodeGeneratorAsset : ScriptableObject
    {
        internal List<string> stateList = new List<string>();
    }
}
