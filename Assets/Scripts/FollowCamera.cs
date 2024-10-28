using UnityEngine;

namespace ArtEye
{
    public class FollowCamera : MonoBehaviour
    {
        // FIXME old udon code
        // private VRCPlayerApi LocalPlayer;

        private void Start()
        {
            // FIXME old udon code
            // LocalPlayer = Networking.LocalPlayer;
        }

        private void Update()
        {
            // FIXME old udon code
            /*
            transform.position = LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;
            transform.rotation = LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
            */
        }
    }
}