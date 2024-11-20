using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace ArtEye
{
    public class XRRigReferences : MonoBehaviour
    {
        public Transform Origin { get; private set; }
        public Transform Head { get; private set; }
        public Transform LeftHand { get; private set; }
        public Transform RightHand { get; private set; }

        private Transform _leftController;
        private Transform _rightController;

        private Transform _leftHand;
        private Transform _rightHand;

        private void Awake()
        {
            var xrOrigin = GetComponent<XROrigin>();
            var xrInput = GetComponent<XRInputModalityManager>();

            AssignOriginAndHead(xrOrigin);

            AssignControllers(xrInput);
            AssignHands(xrInput);
        }

        private void AssignOriginAndHead(XROrigin xrOrigin)
        {
            if (!xrOrigin)
                return;

            Origin = xrOrigin.Origin.transform;
            Head = xrOrigin.Camera.transform;
        }

        private void AssignControllers(XRInputModalityManager xrInput)
        {
            if (!xrInput)
                return;

            var leftController = xrInput.leftController;
            if (leftController)
                _leftController = leftController.transform;

            var rightController = xrInput.rightController;
            if (rightController)
                _rightController = rightController.transform;
        }

        private void AssignHands(XRInputModalityManager xrInput)
        {
            if (!xrInput)
                return;

            var leftHandDriver = xrInput.leftHand.GetComponentInChildren<XRHandSkeletonDriver>();
            if (leftHandDriver)
                _leftHand = leftHandDriver.rootTransform;

            var rightHandDriver = xrInput.rightHand.GetComponentInChildren<XRHandSkeletonDriver>();
            if (rightHandDriver)
                _rightHand = rightHandDriver.rootTransform;
        }

        public void OnTrackedHandMode()
        {
            if (!_leftHand || !_rightHand)
                return;

            LeftHand = _leftHand;
            RightHand = _rightHand;
        }

        public void OnMotionControllerMode()
        {
            if (!_leftController || !_rightController)
                return;

            LeftHand = _leftController;
            RightHand = _rightController;
        }
    }
}