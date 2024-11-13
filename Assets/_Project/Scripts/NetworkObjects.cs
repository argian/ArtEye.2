using Unity.Netcode;

namespace ArtEye
{
    public class NetworkObjects : MonoSingleton<NetworkObjects>
    {
        public NetworkObject Root { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            name = "Network Objects";

            if (TryGetComponent<NetworkObject>(out var networkObject))
                Root = networkObject;
        }
    }
}