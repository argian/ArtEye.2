using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ArtEye
{
    [CreateAssetMenu(fileName = "New SceneCollection", menuName = "ArtEye/SceneCollection")]
    public class SceneCollection : ScriptableObject
    {
        [field: SerializeField] public bool IncludeInBuild { get; private set; }

        [field: SerializeField] public List<SceneLink> Entries { get; private set; } = new();

    #if UNITY_EDITOR
        public void AddEntriesToSceneList()
        {
            if (!IncludeInBuild)
                return;

            List<EditorBuildSettingsScene> sceneList = new(EditorBuildSettings.scenes);

            foreach (var entry in Entries)
            {
                if (entry == null || !entry.IsValid)
                    continue;

                sceneList.Add(new EditorBuildSettingsScene(entry.ScenePath, entry.IncludeInBuild));
            }

            EditorBuildSettings.scenes = sceneList.ToArray();
        }
    #endif
    }
}