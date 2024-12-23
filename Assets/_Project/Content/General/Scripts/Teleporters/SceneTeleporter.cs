using UnityEngine;

namespace ArtEye.Teleportation
{
    public class SceneTeleporter : TeleporterBase
    {
        [SerializeField] private SceneLink sceneToLoad;

        protected override void OnTeleport(GameObject playerObject)
        {
            if (!sceneToLoad)
            {
                Debug.LogError("Can't teleport! No scene to load set!", this);
                return;
            }

            SceneLoader.Instance.LoadScene(sceneToLoad);
        }
    }
}
