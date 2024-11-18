using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArtEye
{
    public class NetworkObjects : NetworkSingleton<NetworkObjects>
    {
        public static Action OnActiveContainerChanged;

        [SerializeField] private SceneContainer containerPrefab;

        private SceneLink _activeScene;
        private SceneContainer _activeContainer;

        private readonly Dictionary<SceneLink, SceneContainer> _sceneContainerRegistry = new();

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            name = "Network Objects";

            if (!NetworkManager.IsClient)
                return;

            var activeScenePath = SceneManager.GetActiveScene().path;
            _activeScene = SceneCollections.GetSceneLink(activeScenePath);

            TryCreateNewContainerRpc(_activeScene.SceneHash);

            SceneLoader.OnLoadStart += OnSceneLoadStart;
            SceneLoader.OnLoadEnd += OnSceneLoadEnd;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (!NetworkManager.IsClient)
                return;

            SceneLoader.OnLoadStart -= OnSceneLoadStart;
            SceneLoader.OnLoadEnd -= OnSceneLoadEnd;
        }

        private void OnSceneLoadStart(SceneLink sceneLink)
        {
            _activeScene = sceneLink;
            TryCreateNewContainerRpc(_activeScene.SceneHash);
        }

        private void OnSceneLoadEnd()
        {
            TryUpdateActiveContainer();
        }

        [Rpc(SendTo.Server)]
        private void TryCreateNewContainerRpc(int sceneHash)
        {
            var sceneLink = SceneCollections.GetSceneLink(sceneHash);

            if (_sceneContainerRegistry.TryGetValue(sceneLink, out _))
                return;

            SceneContainer container = Instantiate(containerPrefab, transform);
            container.Init(sceneLink);
        }

        public void AttachToActiveContainer(NetworkObject networkObject)
        {
            AttachToContainerRpc(_activeContainer, networkObject);
        }

        [Rpc(SendTo.Server)]
        private void AttachToContainerRpc(NetworkBehaviourReference sceneContainerRef, 
                                          NetworkObjectReference networkObjectRef)
        {
            if (sceneContainerRef.TryGet(out SceneContainer sceneContainer))
                ((NetworkObject)networkObjectRef).TrySetParent(sceneContainer.NetworkObject);
            else
                ((NetworkObject)networkObjectRef).TrySetParent(NetworkObject);
        }

        public void AddContainer(SceneContainer container)
        {
            if (_sceneContainerRegistry.TryAdd(container.SceneLink, container))
                container.gameObject.SetActive(false);

            TryUpdateActiveContainer();
        }

        private void TryUpdateActiveContainer()
        {
            if (!_sceneContainerRegistry.TryGetValue(_activeScene, out var activeContainer))
                return;

            if (_activeContainer == activeContainer)
                return;

            if (_activeContainer)
                _activeContainer.gameObject.SetActive(false);
            
            _activeContainer = activeContainer;
            _activeContainer.gameObject.SetActive(true);
            
            OnActiveContainerChanged?.Invoke();
        }
    }
}