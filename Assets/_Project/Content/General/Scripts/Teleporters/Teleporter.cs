using UnityEngine;

namespace ArtEye.Teleportation
{
    public class Teleporter : TeleporterBase {
        
        [field: SerializeField] public Transform TeleporterExit { get; set; }
        
        [SerializeField] private Teleporter linkedTeleporter;

        protected override void OnTeleport(GameObject playerObject)
        {
            
            if (!linkedTeleporter)
            {
                Debug.LogError("Can't teleport! No linked teleporter set!", this);
                return;
            }

            XRRigManager.Instance.MoveXRRig(linkedTeleporter.TeleporterExit);
        }
    }
}
