using Unity.Netcode;
using UnityEngine;

namespace ArtEye
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject gfx;

        [Space]
        [SerializeField] private Transform head;
        [SerializeField] private Transform leftHand;
        [SerializeField] private Transform rightHand;

        private GameObject _xrRig;
        private XRRigReferences _xrRigRefs;

        private void Awake()
        {
            _xrRig = XRRigManager.Instance.XRRig;
            _xrRigRefs = _xrRig.GetComponent<XRRigReferences>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            name = $"Player {OwnerClientId}";

            if (!IsOwner)
                return;

            DisableMeshRenderers();
            //AttachToActiveContainer();

            NetworkObjects.OnActiveContainerChanged += AttachToActiveContainer;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            if (!IsOwner)
                return;

            NetworkObjects.OnActiveContainerChanged -= AttachToActiveContainer;
        }

        void AttachToActiveContainer()
        {
            if (NetworkObjects.Instance)
                NetworkObjects.Instance.AttachToActiveContainer(NetworkObject);
        }

        private void DisableMeshRenderers()
        {
            var meshes = gfx.GetComponentsInChildren<MeshRenderer>();

            foreach (var mesh in meshes)
            {
                mesh.enabled = false;
            }
        }

        private void Update()
        {
            if (!IsOwner || !_xrRigRefs)
                return;

            CopyPositionAndRotation(_xrRigRefs.Origin, transform);

            CopyPositionAndRotation(_xrRigRefs.Head, head);
            CopyPositionAndRotation(_xrRigRefs.LeftHand, leftHand);
            CopyPositionAndRotation(_xrRigRefs.RightHand, rightHand);
        }

        private void CopyPositionAndRotation(Transform from, Transform to)
        {
            to.SetPositionAndRotation(from.position, from.rotation);
        }
    }
}