using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DevionGames.InventorySystem
{
    public interface IAction
    {
        bool isActiveAndEnabled { get; }
        void OnSequenceStart();
        void OnStart();
        ActionStatus OnUpdate();
        void OnEnd();
        void OnSequenceEnd();
    }
}