using Cysharp.Threading.Tasks;
using System;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArtEye
{
    public class SceneLoader : MonoSingleton<SceneLoader>
    {
        public static event Action<SceneLink> OnLoadStart;
        public static event Action OnLoadEnd;

        private bool _loading;

        public void LoadScene(SceneLink link)
        {
            if (!link.IncludeInBuild)
            {
            #if UNITY_EDITOR
                Debug.LogWarning("This scene isn't included in build. It works in Editor only!");
            #else
                return;
            #endif
            }

            if (_loading)
                return;

            LoadSceneAsync(link).Forget();
        }

        public async UniTask LoadSceneAsync(SceneLink link)
        {
            _loading = true;
            OnLoadStart?.Invoke(link);

        #if UNITY_EDITOR
            await EditorSceneManager.LoadSceneAsyncInPlayMode(link.ScenePath, new LoadSceneParameters(LoadSceneMode.Single));
        #else
            await SceneManager.LoadSceneAsync(link.ScenePath, LoadSceneMode.Single);
        #endif

            _loading = false;
            OnLoadEnd?.Invoke();
        } 
    }
}