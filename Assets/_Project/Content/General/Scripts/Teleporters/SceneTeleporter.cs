using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArtEye.Teleportation
{
    public class SceneTeleporter : TeleporterBase
    {
        [SerializeField] private SceneLink sceneToLoad;
        
        protected override async UniTask Teleport(GameObject playerObject)
        {
            if (!sceneToLoad)
            {
                Debug.LogError("Can't teleport! No scene to load set!", this);
                return;
            }

            await SceneLoader.Instance.LoadSceneAsync(sceneToLoad);
        }
    }
}
