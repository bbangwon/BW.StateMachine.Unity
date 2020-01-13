using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace BW.StateMachine.Unity
{
    [CreateAssetMenu(menuName = "BW.StateMachine/StateMachine Generator Asset")]
    public class StateCodeGeneratorAsset : ScriptableObject
    {
        [HideInInspector]
        private List<string> stateList = null;
    }
}
