#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ArtEye
{
    [CreateAssetMenu(fileName = "New SceneLink", menuName = "ArtEye/SceneLink")]
    public class SceneLink : ScriptableObject
    {
    #if UNITY_EDITOR
        [field: SerializeField] public SceneAsset SceneAsset { get; private set; }
    #endif

        [field: SerializeField, Space] public string ScenePath { get; private set; }
        
        [field: SerializeField] public bool IncludeInBuild { get; private set; }

        public void Open() => SceneLoader.Instance.LoadScene(this);

    #if UNITY_EDITOR
        public bool IsValid => ScenePath != null && AssetDatabase.AssetPathExists(ScenePath);

        private void OnValidate()
        {
            if (!SceneAsset)
                return;

            string oldPath = ScenePath;
            string newPath = AssetDatabase.GetAssetPath(SceneAsset);

            if (oldPath == newPath)
                return;

            ScenePath = newPath;

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(this), SceneAsset.name);
        }
    #endif
    }
}