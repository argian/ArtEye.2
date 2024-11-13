using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace ArtEye
{
    public class NetworkDebug : MonoBehaviour
    {
        [SerializeField] private GameObject startHostButton;
        [SerializeField] private GameObject startClientButton;
        [SerializeField] private TMP_InputField ipAddress;

        [Space]        
        [SerializeField] private GameObject disconnectButton;

    #if !(UNITY_EDITOR || DEVELOPMENT_BUILD)
        private void Awake() => Destroy(gameObject);
    #endif

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            ToggleInputs();
        }

        public void StartClient()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                                    .SetConnectionData(ipAddress.text, 7777);
            NetworkManager.Singleton.StartClient();
            ToggleInputs();
        }

        private void OnClientDisconnectCallback(ulong obj)
        {
            ToggleInputs();
        }

        public void Disconnect()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            NetworkManager.Singleton.Shutdown();
            ToggleInputs();
        }

        private void ToggleInputs()
        {
            startHostButton.SetActive(!startHostButton.activeSelf);
            startClientButton.SetActive(!startClientButton.activeSelf);
            ipAddress.gameObject.SetActive(!ipAddress.gameObject.activeSelf);

            disconnectButton.SetActive(!disconnectButton.activeSelf);
        }
    }
}