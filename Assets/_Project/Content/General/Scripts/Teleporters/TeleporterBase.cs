using System;
using UnityEngine;

namespace ArtEye.Teleportation
{
    public abstract class TeleporterBase : MonoBehaviour
    {
        public static Action<Action> OnTeleportationStart = delegate { };
        public static Action OnTeleportationEnd = delegate { };

        private static bool TeleportationInProgress { get; set; }

        public void Teleport()
        {
            if (TeleportationInProgress)
                return;

            TeleportationInProgress = true;
            
            OnTeleportationStart?.Invoke(() => {
                OnTeleport(XRRigManager.Instance.XRRig);
                OnTeleportationEnd?.Invoke();
                TeleportationInProgress = false;
            });
        }

        protected abstract void OnTeleport(GameObject playerObject);
    }
}
