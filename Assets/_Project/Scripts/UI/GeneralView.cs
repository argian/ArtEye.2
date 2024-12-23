using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ArtEye
{
    public class GeneralView : MenuView
    {
        [Space]
        [SerializeField] private TMP_InputField nameInput;

        [Space]
        [SerializeField] private TMP_InputField ipAddressInput;

        [SerializeField] private Button connectButton;
        [SerializeField] private TMP_Text connectText;

        [SerializeField] private Button hostButton;
        [SerializeField] private TMP_Text hostText;

        [Space]
        [SerializeField] private TMP_Dropdown scenesDropdown;

        private string _name;
        private string _ipAddress;
        private string _hostAddress;

        private void Awake() => Init();

        private void Init()
        {
            SetupName();
            SetupIPAddress();
            
            ResetConnection();
            
            SetHostAddress();
        }

        private void SetupName()
        {
            var name = PlayerPrefs.GetString("Name", GetDefaultName());
            nameInput.onValueChanged.AddListener(ChangeName);
            nameInput.text = name;
        }

        private string GetDefaultName() => $"User {Random.Range(0, 100)}";

        private void ChangeName(string value)
        {
            if (value == "")
                nameInput.SetTextWithoutNotify(GetDefaultName());

            _name = nameInput.text;
            PlayerPrefs.SetString("Name", _name);
        }

        private void SetupIPAddress()
        {
            _ipAddress = PlayerPrefs.GetString("IPAddress", GetDefaultIPAddress());
            ipAddressInput.onValueChanged.AddListener(ChangeIPAddress);
            ipAddressInput.text = _ipAddress;
        }

        private string GetDefaultIPAddress() => "192.168.0.1";

        private void ChangeIPAddress(string value)
        {
            if (value == "")
                ipAddressInput.SetTextWithoutNotify(GetDefaultIPAddress());

            _ipAddress = ipAddressInput.text;
            PlayerPrefs.SetString("IPAddress", _ipAddress);
        }

        public void ResetConnection()
        {
            SetConnectButton(Connect, "Connect");
            SetHostButton(Host, "Start Hosting");

            ipAddressInput.SetTextWithoutNotify(_ipAddress);
            SetInputsInteractable(true);

            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
            NetworkManager.Singleton.Shutdown();
        }

        public void OnClientDisconnect(ulong value) => ResetConnection();

        private void SetConnectButton(UnityAction action, string label)
            => SetButton(connectButton, connectText, action, label);

        private void SetHostButton(UnityAction action, string label) 
            => SetButton(hostButton, hostText, action, label);

        private void SetButton(Button button, TMP_Text tmp_text,
                               UnityAction action, string label)
        {
            tmp_text.text = label;
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action);
            button.gameObject.SetActive(true);
        }

        private void SetInputsInteractable(bool value)
        {
            nameInput.interactable = value;
            ipAddressInput.interactable = value;
        }

        private void SetHostAddress()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                _hostAddress = "OFFLINE";
                return;
            }

            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = hostEntry.AddressList.FirstOrDefault(
                ip => ip.AddressFamily == AddressFamily.InterNetwork);
            
            _hostAddress = ipAddress.ToString();
        }

        public void Connect()
        {
            SetInputsInteractable(false);
            SetConnectButton(ResetConnection, "Disconnect");
            hostButton.gameObject.SetActive(false);

            NetworkManager.Singleton.GetComponent<UnityTransport>()
                          .SetConnectionData(_ipAddress, 7777);

            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            NetworkManager.Singleton.StartClient();
        }

        public void Host()
        {
            SetInputsInteractable(false);
            SetHostButton(ResetConnection, "Stop Hosting");
            connectButton.gameObject.SetActive(false);

            ipAddressInput.SetTextWithoutNotify(_hostAddress);
            NetworkManager.Singleton.StartHost();
        }

        public void QuitApp()
        {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
        }
    }
}
