using System.Collections.Generic;
using UnityEngine;

namespace ArtEye
{
    [CreateAssetMenu(fileName = "SceneCollections", menuName = "ArtEye/SceneCollections")]
    public class SceneCollections : ScriptableSingleton<SceneCollections>
    {
        [field: SerializeField] public List<SceneCollection> Entries { get; private set; } = new();

        private readonly Dictionary<int, SceneLink> _sceneLinkRegistry = new();

        [RuntimeInitializeOnLoadMethod()]
        private static void OnRuntimeInitialized()
        {
            foreach (var collection in Instance.Entries)
                foreach (var link in collection.Entries)
                    Instance._sceneLinkRegistry.Add(link.SceneHash, link);
        }

        public static SceneLink GetSceneLink(string scenePath)
        {
            return GetSceneLink(Animator.StringToHash(scenePath));
        }

        public static SceneLink GetSceneLink(int sceneHash)
        {
            if (Instance._sceneLinkRegistry.TryGetValue(sceneHash, out var link))
                return link;

            Debug.LogError($"No scene link found", Instance);
            return null;
        }
    }
}