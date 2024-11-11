using Unity.Netcode;
using UnityEngine;

namespace ArtEye
{
    public class NetworkDebug : MonoBehaviour
    {
        [SerializeField] private GameObject startHostButton;
        [SerializeField] private GameObject startClientButton;
        
        [Space]        
        [SerializeField] private GameObject disconnectButton;

    #if !(UNITY_EDITOR || DEVELOPMENT_BUILD)
        private void Awake() => Destroy(gameObject);
    #endif

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            ToggleButtons();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.Singleton.StartClient();
            ToggleButtons();
        }

        private void OnClientDisconnectCallback(ulong obj)
        {
            ToggleButtons();
        }

        public void Disconnect()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            NetworkManager.Singleton.Shutdown();
            ToggleButtons();
        }

        private void ToggleButtons()
        {
            startHostButton.SetActive(!startHostButton.activeSelf);
            startClientButton.SetActive(!startClientButton.activeSelf);

            disconnectButton.SetActive(!disconnectButton.activeSelf);
        }
    }
}