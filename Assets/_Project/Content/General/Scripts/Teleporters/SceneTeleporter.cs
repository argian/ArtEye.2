using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArtEye
{
    public class SceneTeleporter : TeleporterBase
    {
        [SerializeField] private SceneLink sceneToLoad;
        
        protected override async UniTask Teleport(GameObject playerObject)
        {
            await SceneLoader.Instance.LoadSceneAsync(sceneToLoad);
        }
    }
}
