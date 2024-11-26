using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ArtEye
{
    public class Teleporter : TeleporterBase {
        
        [SerializeField] private Teleporter linkedTeleporter;
        
        protected override UniTask Teleport(GameObject playerObject) {
            playerObject.transform.position = linkedTeleporter.TeleporterExit.position;
            playerObject.transform.rotation = linkedTeleporter.TeleporterExit.rotation;
            return UniTask.CompletedTask;
        }
    }
}
