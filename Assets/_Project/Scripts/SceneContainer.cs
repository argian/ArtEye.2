using Unity.Netcode;

namespace ArtEye
{
    public class SceneContainer : NetworkBehaviour
    {
        public SceneLink SceneLink { get; private set; }

        private readonly NetworkVariable<int> _sceneHash = new();

        private void Start()
        {
            SceneLink = SceneCollections.GetSceneLink(_sceneHash.Value);
            
            name = SceneLink.name;
            NetworkObjects.Instance.AddContainer(this);
        }

        public void Init(SceneLink sceneLink)
        {
            NetworkObject.Spawn();
            NetworkObject.TrySetParent(NetworkObjects.Instance.NetworkObject);

            _sceneHash.Value = sceneLink.SceneHash;
        }

        public void Attach(NetworkObject networkObject)
        {
            networkObject.TrySetParent(NetworkObject);
        }

        public void Show(ulong clientID)
        {
            NetworkObject.NetworkShow(clientID);
        }

        public void Hide(ulong clientID)
        {
            NetworkObject.NetworkHide(clientID);
        }
    }
}