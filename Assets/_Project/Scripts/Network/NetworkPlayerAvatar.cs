using UnityEngine;
using XRMultiplayer;

namespace ArtEye
{
    public class NetworkPlayerAvatar : XRINetworkPlayer
    {
        protected override void SetupLocalPlayer()
        {
            base.SetupLocalPlayer();

            UpdateLocalPlayerName(PlayerPrefs.GetString("Name"));
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner)
                return;

            NetworkObjects.OnActiveContainerChanged += AttachToActiveContainer;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (!IsOwner)
                return;

            NetworkObjects.OnActiveContainerChanged -= AttachToActiveContainer;
        }

        private void AttachToActiveContainer()
        {
            if (NetworkObjects.Instance)
                NetworkObjects.Instance.AttachToActiveContainer(NetworkObject);
        }
    }
}