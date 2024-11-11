using Unity.Netcode;
using UnityEngine;

namespace ArtEye
{
    public class NetworkPlayer : NetworkBehaviour
    {
        [SerializeField] private GameObject gfx;

        [Space]
        [SerializeField] private Transform head;
        [SerializeField] private Transform handLeft;
        [SerializeField] private Transform handRight;

        private GameObject _xrRig;
        private XRRigReferences _xrRigRefs;

        private void Awake()
        {
            _xrRig = XRRigManager.Instance.XRRig;
            _xrRigRefs = _xrRig.GetComponent<XRRigReferences>();
        }

        private void Start()
        {
            if (!IsOwner)
                return;

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
            CopyPositionAndRotation(_xrRigRefs.HandLeft, handLeft);
            CopyPositionAndRotation(_xrRigRefs.HandRight, handRight);
        }

        private void CopyPositionAndRotation(Transform from, Transform to)
        {
            to.SetPositionAndRotation(from.position, from.rotation);
        }
    }
}