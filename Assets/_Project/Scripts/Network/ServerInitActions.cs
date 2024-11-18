using Unity.Netcode;
using UnityEngine;

namespace ArtEye
{
    public class ServerInitActions : MonoBehaviour
    {
        [SerializeField] private GameObject networkObjectsPrefab;

        private void OnEnable()
        {
            if (!NetworkManager.Singleton)
                return;

            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        }

        private void OnDisable()
        {
            if (!NetworkManager.Singleton)
                return;

            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        }

        private void OnServerStarted()
        {
            if (!networkObjectsPrefab)
                return;

            var networkObjects = Instantiate(networkObjectsPrefab);

            if (networkObjects.TryGetComponent<NetworkObject>(out var networkObject))
                networkObject.Spawn();
        }
    }
}