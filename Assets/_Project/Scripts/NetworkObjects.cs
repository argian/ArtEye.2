using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArtEye
{
    public class NetworkObjects : NetworkSingleton<NetworkObjects>
    {
        [SerializeField] private SceneContainer containerPrefab;

        private SceneLink _activeScene;

        private readonly Dictionary<SceneLink, SceneContainer> _sceneContainerRegistry = new();

        protected override void Awake()
        {
            base.Awake();

            name = "Network Objects";
        }

        private void Start()
        {
            if (!NetworkManager.IsClient)
                return;

            var activeScenePath = SceneManager.GetActiveScene().path;
            _activeScene = SceneCollections.GetSceneLink(activeScenePath);

            ShowContainerRpc(_activeScene.SceneHash, NetworkManager.LocalClientId);
        }

        private void OnEnable()
        {
            if (!NetworkManager.IsClient)
                return;

            SceneLoader.OnLoadStart += OnSceneLoadStart;
            SceneLoader.OnLoadEnd += OnSceneLoadEnd;
        }

        private void OnDisable()
        {
            if (!NetworkManager.IsClient)
                return;

            SceneLoader.OnLoadStart -= OnSceneLoadStart;
            SceneLoader.OnLoadEnd -= OnSceneLoadEnd;
        }

        private void OnSceneLoadStart(SceneLink sceneLink)
        {
            HideContainerRpc(_activeScene.SceneHash, NetworkManager.LocalClientId);
            _activeScene = sceneLink;
        }

        private void OnSceneLoadEnd()
        {
            ShowContainerRpc(_activeScene.SceneHash, NetworkManager.LocalClientId);
        }

        [Rpc(SendTo.Server)]
        private void ShowContainerRpc(int sceneHash, ulong clientID)
        {
            var sceneLink = SceneCollections.GetSceneLink(sceneHash);

            if (!_sceneContainerRegistry.TryGetValue(sceneLink, out var container))
            {
                container = Instantiate(containerPrefab, transform);
                container.Init(sceneLink);
            }

            container.Show(clientID);
        }

        [Rpc(SendTo.Server)]
        private void HideContainerRpc(int sceneHash, ulong clientID)
        {
            var sceneLink = SceneCollections.GetSceneLink(sceneHash);

            if (_sceneContainerRegistry.TryGetValue(sceneLink, out var container))
                container.Hide(clientID);
        }

        public void AddContainer(SceneContainer container)
        {
            _sceneContainerRegistry.TryAdd(container.SceneLink, container);
        }
    }
}