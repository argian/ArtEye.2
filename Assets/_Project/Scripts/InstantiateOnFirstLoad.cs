using System.Collections.Generic;
using UnityEngine;

namespace ArtEye
{
    [CreateAssetMenu(fileName = "InstantiateOnFirstLoad", menuName = "ArtEye/InstantiateOnFirstLoad")]
    public class InstantiateOnFirstLoad : ScriptableSingleton<InstantiateOnFirstLoad>
    {
        [SerializeField] private List<GameObject> prefabs = new();

        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeInitialized() 
        {
            foreach (var prefab in Instance.prefabs)
            {
                var instance = Instantiate(prefab);
                instance.transform.name = prefab.name;
            }
        }
    }
}