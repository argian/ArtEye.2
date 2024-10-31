using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ArtEye.Editor
{
    public class ControlPanel : EditorWindow
    {
        [SerializeField] private SceneCollection general;
        [SerializeField] private SceneCollection develop;

        private bool _generalFoldout = true;
        private bool _developFoldout = true;

        private Vector2 _scroll;

        private const string WINDOW_NAME = "Control Panel";

        [MenuItem("Tools/ArtEye/" + WINDOW_NAME)]
        public static void ShowWindow()
        {
            var window = GetWindow<ControlPanel>(WINDOW_NAME);
            window.Show();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawSceneSelection();
        }

        private void DrawHeader()
        {
            EditorGUILayout.Space();

            GUILayout.Label(WINDOW_NAME, new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            });

            DrawSeparator();
        }

        private void DrawSeparator()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        private void DrawSceneSelection()
        {
            if (!(general || develop))
                return;

            GUILayout.Label("Scene Selection", new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter
            });

            EditorGUILayout.Space();

            if (GUILayout.Button("Update Scene List in Build Profiles"))
                UpdateSceneList();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            DrawSceneCollection(general, ref _generalFoldout);
            DrawSceneCollection(develop, ref _developFoldout);

            EditorGUILayout.EndScrollView();
        }

        private void UpdateSceneList()
        {
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];
            general.AddEntriesToSceneList();
            develop.AddEntriesToSceneList();

            Debug.Log("Scene List in Build Profiles has been updated (don't forget to save)");
        }

        private void DrawSceneCollection(SceneCollection sceneCollection, ref bool foldout)
        {
            if (!sceneCollection)
                return;

            EditorGUILayout.Space();

            foldout = EditorGUILayout.Foldout(foldout, sceneCollection.name, true);

            EditorGUI.indentLevel++;

            if (foldout)
            {
                foreach (var sceneLink in sceneCollection.Entries)
                {
                    if (sceneLink == null)
                        continue;

                    GUILayout.BeginHorizontal();
                    
                    GUI.enabled = false;
                    bool includeInBuild = sceneCollection.IncludeInBuild && sceneLink.IncludeInBuild;
                    EditorGUILayout.Toggle(includeInBuild, GUILayout.MaxWidth(32));
                    GUI.enabled = true;

                    if (GUILayout.Button(sceneLink.name))
                        OpenScene(sceneLink);
                    
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

                if (GUILayout.Button($"Edit {sceneCollection.name}"))
                    EditCollection(sceneCollection);
            }

            EditorGUI.indentLevel--;
        }

        private void OpenScene(SceneLink link)
        {
            if (!link.IsValid)
            {
                Debug.LogError($"Scene Link for {link.name} is invalid.", link);
                return;
            }

            if (Application.isPlaying)
                SceneLoader.Instance.LoadScene(link);
            else
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    return;

                EditorSceneManager.OpenScene(link.ScenePath, OpenSceneMode.Single);
            }
        }

        private void EditCollection(SceneCollection collection)
        {
            Selection.activeObject = collection;
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
        }
    }
}