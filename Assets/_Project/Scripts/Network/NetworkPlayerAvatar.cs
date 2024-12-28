using UnityEngine;
using XRMultiplayer;

namespace ArtEye
{
    public class NetworkPlayerAvatar : XRINetworkPlayer
    {
        private static Color? _localPlayerColor = null; // Not great but it works.

        protected override void SetupLocalPlayer()
        {
            base.SetupLocalPlayer();

            UpdateLocalPlayerName(PlayerPrefs.GetString("Name"));

            if (_localPlayerColor == null)
                _localPlayerColor = Random.ColorHSV(0, 1, .8f, .8f, .8f, .8f);
            
            UpdateLocalPlayerColor(_localPlayerColor.Value);
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