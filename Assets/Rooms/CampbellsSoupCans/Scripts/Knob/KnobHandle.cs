using UnityEngine;

namespace ArtEye.CampbellsSoupCans
{
    public class KnobHandle : MonoBehaviour
    {
        // FIXME old udon code
        /*
        private VRCObjectSync vrcObjectSync;
        private VRCPlayerApi vrcPlayerApi;
        */
        private bool isHeld;
        [SerializeField] private Knob knob;
        [SerializeField] private Transform model;
        [SerializeField] private float minAngleLock;
        [SerializeField] private float maxAngleLock;

        private void Start()
        {
            // FIXME old udon code
            /*
            vrcObjectSync = GetComponent<VRCObjectSync>();
            vrcPlayerApi = Networking.LocalPlayer;
            */
        }

        private void Update()
        {
            if (!isHeld)
                return;

            Vector3 rotation = model.localRotation.eulerAngles;
            rotation.z = Mathf.Clamp(transform.localRotation.eulerAngles.z, minAngleLock, maxAngleLock);
            Debug.Log(rotation);

            model.localRotation = Quaternion.Euler(rotation);
            knob.SetAngle(rotation.z);
        }

        public void OnPickup()
        {
            // FIXME old udon code
            /*
            base.OnPickup();
            if (!vrcPlayerApi.isLocal)
                return;
            */

            isHeld = true;
        }

        public void OnDrop()
        {
            // FIXME old udon code
            /*
            base.OnDrop();
            if (!vrcPlayerApi.isLocal)
                return;
            */

            isHeld = false;
            // FIXME old udon code
            // vrcObjectSync.TeleportTo(model);
        }
    }
}