using System.Collections.Generic;
using UnityEngine;

namespace ArtEye
{
    [CreateAssetMenu(fileName = "SceneCollections", menuName = "ArtEye/SceneCollections")]
    public class SceneCollections : ScriptableSingleton<SceneCollections>
    {
        [field: SerializeField] public List<SceneCollection> Entries { get; private set; } = new();

        private readonly Dictionary<string, SceneLink> _sceneLinkRegistry = new();

        [RuntimeInitializeOnLoadMethod()]
        private static void OnRuntimeInitialized()
        {
            foreach (var collection in Instance.Entries)
                foreach (var link in collection.Entries)
                    Instance._sceneLinkRegistry.Add(link.ScenePath, link);
        }

        public static SceneLink GetSceneLink(string scenePath)
        {
            if (Instance._sceneLinkRegistry.TryGetValue(scenePath, out var link))
                return link;
            
            Debug.LogError($"No scene link found for '{scenePath}'");
            return null;
        }
    }
}