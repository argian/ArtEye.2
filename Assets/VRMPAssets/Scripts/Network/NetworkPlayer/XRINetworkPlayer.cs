using UnityEngine;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using Unity.Collections;
using System;

namespace XRMultiplayer
{
    /// <summary>
    /// XRINetworkPlayer class used for simple interactions.
    /// </summary>
    public class XRINetworkPlayer : NetworkBehaviour
    {
        /// <summary>
        /// Singleton Reference for the Local Player.
        /// </summary>
        public static XRINetworkPlayer LocalPlayer;

        [Header("Avatar Transform References"), Tooltip("Assign to local avatar transform.")]
        /// <summary>
        /// Non-Local player transforms.
        /// </summary>
        public Transform head;

        /// <summary>
        /// Non-Local player transforms.
        /// </summary>
        public Transform leftHand;

        /// <summary>
        /// Non-Local player transforms.
        /// </summary>
        public Transform rightHand;

        /// <summary>
        /// Action called when the player name is updated.
        /// </summary>
        public Action<string> onNameUpdated;

        /// <summary>
        /// Action called when the player color is updated.
        /// </summary>
        public Action<Color> onColorUpdated;

        /// <summary>
        /// Action called when the Local Player is finished spawning in.
        /// </summary>
        public Action onSpawnedLocal;

        /// <summary>
        /// Action called when the Local Player is finished spawning in.
        /// </summary>
        public Action onSpawnedAll;

        /// <summary>
        /// Action called when the player color is updated.
        /// </summary>
        public Action<XRINetworkPlayer> onDisconnected;

        /// <summary>
        /// Player Name string that reads from the internal NetworkVariable for the Player Name.
        /// </summary>
        public string playerName { get => m_PlayerName.Value.ToString(); }
        protected readonly NetworkVariable<FixedString128Bytes> m_PlayerName = new("", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        /// <summary>
        /// Player Color that reads from the internal NetworkVariable for the Player Color.
        /// </summary>
        public Color playerColor { get => m_PlayerColor.Value; }
        protected readonly NetworkVariable<Color> m_PlayerColor = new(Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        /// <summary>
        /// Player Name Tag.
        /// </summary>
        [Header("Player Name Tag"), SerializeField, Tooltip("Player Name Tag.")] protected bool m_UpdateObjectName = true;


        // /// <summary>
        // /// Head Renderers to change rendering mode for local players.
        // /// </summary>
        // [SerializeField, Tooltip("Head Renderers to change rendering mode for local players.")] protected Renderer[] m_HeadRends;

        /// <summary>
        /// Hand Objects to be disabled for the local player.
        /// </summary>
        [Header("Networked Hands"), SerializeField, Tooltip("Hand Objects to be disabled for the local player.")] protected GameObject[] m_handsObjects;

        /// <summary>
        /// Player Name Tag.
        /// </summary>
        [Header("Player Name Tag"), SerializeField, Tooltip("Player Name Tag.")] protected PlayerNameTag m_PlayerNameTag;

        /// <summary>
        /// Internal references to the Local Player Transforms.
        /// </summary>
        protected Transform m_LeftHandOrigin, m_RightHandOrigin, m_HeadOrigin;

        /// <summary>
        /// Reference to the local player XR Origin
        /// </summary>
        protected XROrigin m_XROrigin;

        /// <summary>
        /// If the player has been connected to the the game.
        /// </summary>
        protected bool m_InitialConnected = false;

        /// <summary>
        /// Previous position of the head.
        /// </summary>
        protected Vector3 m_PrevHeadPos;

        ///<inheritdoc/>
        protected virtual void OnEnable()
        {
            m_PlayerName.OnValueChanged += UpdatePlayerName;
            m_PlayerColor.OnValueChanged += UpdatePlayerColor;
        }

        ///<inheritdoc/>
        protected virtual void OnDisable()
        {
            m_PlayerName.OnValueChanged -= UpdatePlayerName;
            m_PlayerColor.OnValueChanged -= UpdatePlayerColor;
        }

        ///<inheritdoc/>
        protected virtual void LateUpdate()
        {
            if (!IsOwner) return;

            // Set transforms to be replicated with ClientNetworkTransforms
            leftHand.SetPositionAndRotation(m_LeftHandOrigin.position, m_LeftHandOrigin.rotation);
            rightHand.SetPositionAndRotation(m_RightHandOrigin.position, m_RightHandOrigin.rotation);
            head.SetPositionAndRotation(m_HeadOrigin.position, m_HeadOrigin.rotation);
        }

        ///<inheritdoc/>
        public override void OnDestroy()
        {
            base.OnDestroy();

            if (IsOwner)
            {
                // Local Name unsubscribe.
                //XRINetworkGameManager.LocalPlayerName.Unsubscribe(UpdateLocalPlayerName);
                //XRINetworkGameManager.LocalPlayerColor.Unsubscribe(UpdateLocalPlayerColor);
            }
            else if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient)
            {
                // Inform Network Manager that player left current session.
                //XRINetworkGameManager.Instance.PlayerLeft(NetworkObject.OwnerClientId);
            }

            // Unsubscribe from color updating.
            m_PlayerColor.OnValueChanged -= UpdatePlayerColor;
        }

        ///<inheritdoc/>
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsOwner)
            {
                // Set Local Player.
                LocalPlayer = this;
                //XRINetworkGameManager.Instance.LocalPlayerConnected(NetworkObject.OwnerClientId);

                // Get Origin and set head.
                m_XROrigin = FindFirstObjectByType<XROrigin>();
                if (m_XROrigin != null)
                {
                    m_HeadOrigin = m_XROrigin.Camera.transform;
                }
                else
                {
                    Debug.Log("No XR Rig Available", this);
                }

                SetupLocalPlayer();
            }
            CompleteSetup();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            onDisconnected?.Invoke(this);
        }

        /// <summary>
        /// Called from <see cref="XRHandPoseReplicator"/> when swapping between hand tracking and controllers.
        /// </summary>
        /// <param name="left">Transform for Left Hand.</param>
        /// <param name="right">Transform for Right Hand.</param>
        public void SetHandOrigins(Transform left, Transform right)
        {
            m_LeftHandOrigin = left;
            m_RightHandOrigin = right;
        }

        /// <summary>
        /// Hides and disables Renderers and GameObjects on the Local Player.
        /// Also sets the initial values for <see cref="m_PlayerColor"/> and <see cref="m_PlayerName"/>.
        /// Finally we subscribe to any updates for Color and Name.
        /// </summary>
        /// <remarks>Only called on the Local Player.</remarks>
        protected virtual void SetupLocalPlayer()
        {
            foreach (var hand in m_handsObjects)
            {
                hand.SetActive(false);
            }

            //m_PlayerColor.Value = XRINetworkGameManager.LocalPlayerColor.Value;
            //m_PlayerName.Value = new FixedString128Bytes(XRINetworkGameManager.LocalPlayerName.Value);
            
            //XRINetworkGameManager.LocalPlayerColor.Subscribe(UpdateLocalPlayerColor);
            //XRINetworkGameManager.LocalPlayerName.Subscribe(UpdateLocalPlayerName);

            onSpawnedLocal?.Invoke();
        }

        /// <summary>
        /// Callback for the bindable variable <see cref="XRINetworkGameManager.LocalPlayerColor"/>.
        /// </summary>
        /// <param name="color">New Color for player.</param>
        /// <remarks>Only called on Local Player.</remarks>
        protected virtual void UpdateLocalPlayerColor(Color color)
        {
            //m_PlayerColor.Value = XRINetworkGameManager.LocalPlayerColor.Value;
        }

        /// <summary>
        /// Callback for the bindable variable <see cref="XRINetworkGameManager.LocalPlayerName"/>.
        /// </summary>
        /// <param name="name">New Name for player.</param>
        /// <remarks>Only called on Local Player.</remarks>
        protected virtual void UpdateLocalPlayerName(string name)
        {
            //m_PlayerName.Value = new FixedString128Bytes(XRINetworkGameManager.LocalPlayerName.Value);
        }

        /// <summary>
        /// Called when the player object is finished being setup.
        /// </summary>
        void CompleteSetup()
        {
            // Add player to XRINetworkManager.
            //XRINetworkGameManager.Instance.PlayerJoined(NetworkObject.OwnerClientId);

            // Update Color and Name.
            UpdatePlayerColor(Color.white, m_PlayerColor.Value);
            UpdatePlayerName(new FixedString128Bytes(""), m_PlayerName.Value);

            // If we are not using a World Canvas, setup the name tag for local use.
            m_PlayerNameTag.SetupNameTag(this);

            onSpawnedAll?.Invoke();
        }
        /// <summary>
        /// Callback anytime the local player sets <see cref="m_PlayerName"/>.
        /// </summary><remarks>Invokes the callback <see cref="onNameUpdated"/>.</remarks>
        void UpdatePlayerName(FixedString128Bytes oldName, FixedString128Bytes currentName)
        {
            onNameUpdated?.Invoke(currentName.ToString());

            if (!m_InitialConnected & !string.IsNullOrEmpty(currentName.ToString()))
            {
                m_InitialConnected = true;
            }

            if (m_UpdateObjectName)
                gameObject.name = currentName.ToString();
        }

        /// <summary>
        /// Callback when the local player sets <see cref="m_PlayerColor"/>.
        /// </summary><remarks>Invokes the callback <see cref="onColorUpdated"/>.</remarks>
        void UpdatePlayerColor(Color oldColor, Color newColor)
        {
            onColorUpdated?.Invoke(newColor);
        }
    }
}
