using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArtEye
{
    public class XRRigManager : MonoSingleton<XRRigManager>
    {
        [field: SerializeField] public XROrigin XRRig { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            SetupXRRig();

            MoveXRRigAndDisableOthers();
        }

        private void SetupXRRig()
        {
            if (!XRRig)
                return;

            var name = XRRig.gameObject.name;
            XRRig = Instantiate(XRRig, transform);
            XRRig.gameObject.name = name;
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
        }

        private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
        {
            MoveXRRigAndDisableOthers();
        }

        private void MoveXRRigAndDisableOthers()
        {
            if (!XRRig)
                return;

            var xrRigs = FindObjectsByType<XROrigin>(FindObjectsSortMode.None);

            DisableOtherRigs(xrRigs, out XROrigin fallback);

            var (position, rotation) = GetTargetPositionAndRotation(fallback);

            XRRig.transform.SetPositionAndRotation(position, rotation);
        }

        private void DisableOtherRigs(XROrigin[] xrRigs, out XROrigin fallback)
        {
            fallback = null;

            foreach (var rig in xrRigs)
            {
                if (rig == XRRig)
                    continue;

                rig.gameObject.SetActive(false);

                fallback = rig;
            }
        }

        private (Vector3, Quaternion) GetTargetPositionAndRotation(XROrigin fallback)
        {
            GameObject spawn = GameObject.FindGameObjectWithTag("Spawn");

            if (spawn)
                return (spawn.transform.position, spawn.transform.rotation);
            
            if (fallback)
                return (fallback.transform.position, fallback.transform.rotation);
            
            return (Vector3.zero, Quaternion.identity);
        }
    }
}