using Unity.Netcode;

namespace ArtEye
{
    public class SceneContainer : NetworkBehaviour
    {
        public SceneLink SceneLink { get; private set; }

        private readonly NetworkVariable<int> _sceneHash = new();

        public void Init(SceneLink sceneLink)
        {
            _sceneHash.Initialize(this);
            _sceneHash.Value = sceneLink.SceneHash;

            NetworkObject.Spawn();
            NetworkObject.TrySetParent(NetworkObjects.Instance.NetworkObject);
        }

        private void Start()
        {
            SceneLink = SceneCollections.GetSceneLink(_sceneHash.Value);
            
            name = SceneLink.name;
            NetworkObjects.Instance.AddContainer(this);
        }
    }
}