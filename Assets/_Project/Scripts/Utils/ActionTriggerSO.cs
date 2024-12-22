using System;
using UnityEngine;

namespace ArtEye
{
    [CreateAssetMenu(fileName = "New ActionTrigger", menuName = "ArtEye/ActionTrigger")]
    public class ActionTriggerSO : ScriptableObject
    {
        public event Action Performed;

        public void Perform() => Performed?.Invoke();
    }
}