using UnityEditor;
using UnityEngine;

namespace ArtEye.Editor
{
    [CustomEditor(typeof(NarrativePlayer))]
    public class NarrativePlayerEditor : UnityEditor.Editor
    {
        NarrativePlayer player;

        void OnEnable()
        {
            player = (NarrativePlayer)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);

            if (GUILayout.Button("Assign Text & Clip", GUILayout.Height(40)))
                AssignValues();

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Copy Values");

            if (GUILayout.Button("Text Content"))
                CopyTextContent();

            if (GUILayout.Button("Audio Clip"))
                CopyAudioClip();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Select");

            if (GUILayout.Button("Text Component"))
                SelectTextComponent();

            if (GUILayout.Button("Audio Component"))
                SelectAudioComponent();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        void CopyTextContent()
        {
            Undo.RecordObject(player, "Copy Text Content");
            player.text = player.textComponent.text;
        }

        void CopyAudioClip()
        {
            Undo.RecordObject(player, "Copy Audio Clip");
            player.clip = player.audioSource.clip;
        }

        void SelectTextComponent()
        {
            Undo.RecordObject(Selection.activeGameObject, "Select Text Component");
            Selection.activeGameObject = player.textComponent.gameObject;
        }

        void SelectAudioComponent()
        {
            Undo.RecordObject(Selection.activeGameObject, "Select Audio Component");
            Selection.activeGameObject = player.audioSource.gameObject;
        }

        void AssignValues()
        {
            player.textComponent.text = player.text;
            player.audioSource.clip = player.clip;

            PrefabUtility.RecordPrefabInstancePropertyModifications(player.textComponent);
        }
    }
}