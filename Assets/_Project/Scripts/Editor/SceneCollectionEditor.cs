using UnityEditor;
using UnityEngine;

namespace ArtEye.Editor
{
    [CustomEditor(typeof(SceneCollection))]
    public class SceneCollectionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();

            if (GUILayout.Button($"Open {ControlPanel.WINDOW_NAME}"))
                OpenControlPanel();
        }

        private void OpenControlPanel()
        {
            EditorApplication.ExecuteMenuItem(ControlPanel.WINDOW_PATH);
        }
    }
}