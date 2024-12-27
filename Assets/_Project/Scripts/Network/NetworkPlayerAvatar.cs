using Unity.Collections;
using UnityEngine;
using XRMultiplayer;

namespace ArtEye
{
    public class NetworkPlayerAvatar : XRINetworkPlayer
    {
        protected override void SetupLocalPlayer()
        {
            m_PlayerName.Value = new FixedString128Bytes(PlayerPrefs.GetString("Name"));
            
            base.SetupLocalPlayer();

            NetworkObjects.OnActiveContainerChanged += AttachToActiveContainer;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            NetworkObjects.OnActiveContainerChanged -= AttachToActiveContainer;
        }

        private void AttachToActiveContainer()
        {
            if (NetworkObjects.Instance)
                NetworkObjects.Instance.AttachToActiveContainer(NetworkObject);
        }
    }
}