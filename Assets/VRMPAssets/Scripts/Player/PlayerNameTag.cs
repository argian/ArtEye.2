using TMPro;
using UnityEngine;

namespace XRMultiplayer
{
    public class PlayerNameTag : MonoBehaviour
    {
        public ulong playerId { get => m_PlayerId; }
        ulong m_PlayerId;

        [SerializeField] bool m_WorldUp;
        [SerializeField] TMP_Text m_NameTagText;
        [SerializeField] float m_NameTextScale = .25f;

        [Header("Name Tag LOD Settings")]

        [SerializeField, Tooltip("If the avatar is further than this distance (in meters), the name tag details will be deactivated.")]
        float m_MaxDistanceThreshold = 3f;

        [SerializeField, Tooltip("If the avatar is closer than this distance (in meters), the entire name tag will be deactivated.")]
        float m_MinDistanceThreshold = 1f;

        [SerializeField, Tooltip("The GameObject that will be deactivated when the avatar is closer than the Min distance threshold.")]
        GameObject m_GameObjectToHide;

        [SerializeField, Tooltip("The GameObjects that will be deactivated when the avatar is beyond the Max distance threshold.")]
        GameObject[] m_GameObjectDetailsToDeactivate;

        XRINetworkPlayer m_Player;

        protected Camera m_Camera;

        bool m_IsMinimized = false;

        bool m_IsHidden = false;

        bool m_IsFocusedOn = false;

        private void Awake()
        {
            m_Camera = Camera.main;
        }

        void LateUpdate()
        {
            UpdateRotation();
            UpdateMinimizedState();
        }

        private void OnDestroy()
        {
            m_Player.onNameUpdated -= UpdateName;
        }

        public void SetupNameTag(XRINetworkPlayer player)
        {
            m_PlayerId = player.OwnerClientId;
            m_Player = player;

            UpdateName(player.playerName);

            m_Player.onNameUpdated += UpdateName;
        }

        void UpdateRotation()
        {
            Quaternion lookRot = Quaternion.LookRotation(m_Camera.transform.position - transform.position).normalized;

            if (m_WorldUp)
            {
                Vector3 offset = lookRot.eulerAngles;
                offset.x = 0;
                offset.z = 0;
                lookRot = Quaternion.Euler(offset);
            }

            transform.rotation = lookRot;
        }

        void UpdateMinimizedState()
        {
            var viewerDistance = Vector3.Distance(transform.position, m_Camera.transform.position);

            if (viewerDistance < m_MinDistanceThreshold)
            {
                ToggleHiddenState(true);
            }
            else
            {
                ToggleHiddenState(false);
                if (m_IsFocusedOn) return;
                if (viewerDistance > m_MaxDistanceThreshold)
                {
                    ToggleMinimizeState(true);
                }
                else
                {
                    ToggleMinimizeState(false);
                }
            }
        }

        void ToggleHiddenState(bool toggle)
        {
            if (m_IsHidden == toggle) return;
            m_IsHidden = toggle;
            m_GameObjectToHide.SetActive(!toggle);
        }

        void ToggleMinimizeState(bool toggle)
        {
            if (m_IsMinimized == toggle) return;
            m_IsMinimized = toggle;
            foreach (var go in m_GameObjectDetailsToDeactivate)
            {
                go.SetActive(!toggle);
            }
        }

        /// <summary>
        /// Called from the Callout Gaze Controller and Event Trigger.
        /// </summary>
        public void ToggleFocused(bool toggle)
        {
            if (m_IsFocusedOn == toggle) return;
            m_IsFocusedOn = toggle;
            if (m_IsFocusedOn)
            {
                ToggleMinimizeState(false);
            }
        }

        void UpdateName(string newName)
        {
            if (string.IsNullOrEmpty(newName)) return;

            m_NameTagText.text = newName;
            UpdateNameTagSize();
        }

        [ContextMenu("Update Name Tag Size")]
        void UpdateNameTagSize()
        {
            m_NameTagText.rectTransform.sizeDelta = new Vector2(m_NameTagText.preferredWidth * m_NameTextScale, m_NameTagText.rectTransform.sizeDelta.y);
        }
    }
}
