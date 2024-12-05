using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace ArtEye.Editor
{
    [CustomEditor(typeof(NarrativePlayer))]
    public class NarrativePlayerEditor : UnityEditor.Editor
    {
        private NarrativePlayer _player;
        private TMP_Text _playerTextComponent;
        private AudioSource _playerAudioSource;

        void OnEnable()
        {
            _player = (NarrativePlayer)target;
            
            FieldInfo audioSourceFieldInfo = typeof(NarrativePlayer).GetField("audioSource", BindingFlags.NonPublic | BindingFlags.Instance);
            if (audioSourceFieldInfo != null)
                _playerAudioSource = (AudioSource)audioSourceFieldInfo.GetValue(_player);
            
            FieldInfo textComponentFieldInfo = typeof(NarrativePlayer).GetField("textComponent", BindingFlags.NonPublic | BindingFlags.Instance);
            if (textComponentFieldInfo != null)
                _playerTextComponent = (TMP_Text)textComponentFieldInfo.GetValue(_player);
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

        private void CopyTextContent()
        {
            Undo.RecordObject(_player, "Copy Text Content");
            
            _player.text = _playerTextComponent.text;
        }

        private void CopyAudioClip()
        {
            Undo.RecordObject(_player, "Copy Audio Clip");
            _player.clip = _playerAudioSource.clip;
        }

        private void SelectTextComponent()
        {
            Undo.RecordObject(Selection.activeGameObject, "Select Text Component");
            Selection.activeGameObject = _playerTextComponent.gameObject;
        }

        private void SelectAudioComponent()
        {
            Undo.RecordObject(Selection.activeGameObject, "Select Audio Component");
            Selection.activeGameObject = _playerAudioSource.gameObject;
        }

        private void AssignValues()
        {
            _playerTextComponent.text = _player.text;
            _playerAudioSource.clip = _player.clip;
            EditorUtility.SetDirty(_playerTextComponent);
            EditorUtility.SetDirty(_playerAudioSource);

            PrefabUtility.RecordPrefabInstancePropertyModifications(_playerTextComponent);
        }
    }
}