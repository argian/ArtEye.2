using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace ArtEye
{
    public class XRRigManager : MonoSingleton<XRRigManager>
    {
        public GameObject XRRig { get; private set; }

        [SerializeField] private GameObject XRRigPrefab;

        [Space]
        [SerializeField] private GameObject spawnPrefab;

        private Transform _spawn;

        protected override void Awake()
        {
            base.Awake();

            SetupXRRig();
        }

        public void SetupXRRig()
        {
            if (!XRRigPrefab || XRRig)
                return;

            XRRig = Instantiate(XRRigPrefab, transform);
            XRRig.name = XRRigPrefab.name;
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
            DestroyOtherRigsAndFindSpawn();
            MoveXRRigToSpawn();

            RemoveDuplicateDependencies();

            XRRig.SetActive(false);
            XRRig.SetActive(true);
        }

        private void DestroyOtherRigsAndFindSpawn()
        {
            if (!XRRigPrefab)
                return;

            var xrRigs = FindObjectsByType<XROrigin>(FindObjectsSortMode.None);

            var fallbackTransform = DestroyOtherRigs(xrRigs);

            FindOrPrepareSpawn(fallbackTransform);
        }

        private (Vector3, Quaternion) DestroyOtherRigs(XROrigin[] xrRigs)
        {
            var fallbackTransform = (Vector3.zero, Quaternion.identity);

            foreach (var rig in xrRigs)
            {
                if (rig.gameObject == XRRig)
                    continue;

                fallbackTransform = (rig.transform.position, rig.transform.rotation);

                Destroy(rig.gameObject);
            }

            return fallbackTransform;
        }

        private void FindOrPrepareSpawn((Vector3 position, Quaternion rotation) fallback)
        {
            GameObject spawn = GameObject.FindGameObjectWithTag("Spawn");

            if (!spawn)
            {
                if (spawnPrefab)
                    spawn = Instantiate(spawnPrefab, fallback.position, fallback.rotation);
                else
                {
                    spawn = new GameObject() { name = "Spawn", tag = "Spawn" };
                    spawn.transform.SetPositionAndRotation(fallback.position, fallback.rotation);
                }
            }

            _spawn = spawn.transform;
        }

        [ContextMenu("Move XR Rig To Spawn")]
        public void MoveXRRigToSpawn()
        {
            if (XRRig && _spawn)
                XRRig.transform.SetPositionAndRotation(_spawn.position, _spawn.rotation);
        }

        private void RemoveDuplicateDependencies()
        {
            var inputSystems = FindObjectsByType<XRUIInputModule>(FindObjectsSortMode.InstanceID);
            var xrInteractionManagers = FindObjectsByType<XRInteractionManager>(FindObjectsSortMode.InstanceID);

            for (int i = 1; i < inputSystems.Length; i++)
            {
                Destroy(inputSystems[i].gameObject);
            }

            for (int i = 1; i < xrInteractionManagers.Length; i++)
            {
                Destroy(xrInteractionManagers[i].gameObject);
            }
        }
    }
}