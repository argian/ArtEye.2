using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace ArtEye
{
    public class XRRigReferences : MonoBehaviour
    {
        public Transform Origin { get; private set; }
        public Transform Head { get; private set; }
        public Transform HandLeft { get; private set; }
        public Transform HandRight { get; private set; }

        private XROrigin _xrOrigin;
        private XRInputModalityManager _xrInput;

        private void Awake()
        {
            _xrOrigin = GetComponent<XROrigin>();
            _xrInput = GetComponent<XRInputModalityManager>();

            AssignOriginAndHead();
        }

        private void AssignOriginAndHead()
        {
            if (!_xrOrigin)
                return;

            Origin = _xrOrigin.Origin.transform;
            Head = _xrOrigin.Camera.transform;
        }

        public void AssignHands()
        {
            if (!_xrInput)
                return;

            Debug.Log("Assign Hands");

            HandLeft = _xrInput.leftHand.transform;
            HandRight = _xrInput.rightHand.transform;
        }

        public void AssignControllers()
        {
            if (!_xrInput)
                return;

            Debug.Log("Assign Controllers");

            HandLeft = _xrInput.leftController.transform;
            HandRight = _xrInput.rightController.transform;
        }
    }
}