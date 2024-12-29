using UnityEngine;
using Unity.XR.CoreUtils;

namespace ArtEye
{
    public class RaySkyboxManager : ShaderPasser
    {
        [SerializeField] private SkinnedMeshRenderer treeMesh;
        [SerializeField] private ClockAnimator[] clockAnimators;
        [SerializeField] private ClockCentralizer[] clockCentralizers;
        [SerializeField] private Transform[] markers;
        [SerializeField] private int currentMarker;
        [SerializeField] private float activationDistance = 0.05f;
        [SerializeField] private AnimationCurve lerpCurve;
        
        [SerializeField] private float yScale;
        
        private XROrigin _localPlayer;
        private float _lerp;

        protected override void BakePropertyNames()
        {
            PropertyNames = new string[8];
            PropertyIDs = new int[8];

            PropertyNames[0] = "Spacing1";
            PropertyNames[1] = "Spacing2";
            PropertyNames[2] = "Spacing3";
            PropertyNames[3] = "4";
            PropertyNames[4] = "5";
            PropertyNames[5] = "6";
            PropertyNames[6] = "7";
            PropertyNames[7] = "8";
        }

        protected override void FakeStart()
        {
            currentMarker = 0;
            _lerp = 0;

            // replace it to make sure it gets local player
            _localPlayer = XRRigManager.Instance.XRRig.GetComponent<XROrigin>();

            MainMaterial.SetVector(PropertyIDs[0], new Vector4(0.1f, yScale, 0.1f, _lerp));
            MainMaterial.SetVector(PropertyIDs[1], new Vector4(0.1f, yScale, 0.1f, _lerp));
            MainMaterial.SetVector(PropertyIDs[2], new Vector4(0.1f, yScale, 0.1f, _lerp));
        }

        protected override void PassToRender()
        {
            if (currentMarker == markers.Length - 1)
                return;

            // camera pos and rot
            Vector3 playerHeadPosition = _localPlayer.Camera.transform.position;

            float distanceToNextMarker = Vector3.Distance(markers[currentMarker + 1].position, playerHeadPosition);

            _lerp = 1f - distanceToNextMarker / Vector3.Distance(markers[currentMarker].position, markers[currentMarker + 1].position);

            if (distanceToNextMarker <= activationDistance)
            {
                _lerp = 1f;
                currentMarker++;
            }

            _lerp = _lerp < 0 ? 0 : _lerp;
            _lerp = lerpCurve.Evaluate(_lerp);
            _lerp = Mathf.Clamp(_lerp, 0.1f, 1f);
            MainMaterial.SetVector(PropertyIDs[currentMarker], new Vector4(_lerp, yScale, _lerp, _lerp));

            for (int i = 0; i < clockAnimators.Length; i++)
            {
                clockAnimators[i].reanimate(_lerp);
            }

            float markerPart = 1f / (markers.Length - 1);
            for (int i = 0; i < clockCentralizers.Length; i++)
            {
                clockCentralizers[i].meltingThreshold = markerPart * currentMarker + _lerp * markerPart;
            }

            if (currentMarker % 2 > 0)
            {
                treeMesh.SetBlendShapeWeight(0, 100 - (markerPart * currentMarker + _lerp * markerPart * 400));
            }
            else
            {
                treeMesh.SetBlendShapeWeight(0, markerPart * currentMarker + _lerp * markerPart * 400);
            }

            // Debug.Log(markerPart * currentMarker + _lerp * markerPart * 100);
        }
    }
}
