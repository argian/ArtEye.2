using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArtEye.Teleportation
{
    public class Teleporter : TeleporterBase {
        
        [SerializeField] private Teleporter linkedTeleporter;
        
        protected override UniTask Teleport(GameObject playerObject) {
            if (!linkedTeleporter)
            {
                Debug.LogError("Can't teleport! No linked teleporter set!", this);
                return UniTask.CompletedTask;
            }

            playerObject.transform.position = linkedTeleporter.TeleporterExit.position;
            playerObject.transform.rotation = linkedTeleporter.TeleporterExit.rotation;
            return UniTask.CompletedTask;
        }
    }
}
