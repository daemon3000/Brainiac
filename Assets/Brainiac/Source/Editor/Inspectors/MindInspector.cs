using UnityEngine;
using UnityEditor;
using Brainiac;

namespace BrainiacEditor
{
	[CustomEditor(typeof(Mind))]
	public class MindInspector : Editor
	{
		private SerializedProperty m_behaviourTree;
		private SerializedProperty m_updateMode;
		private SerializedProperty m_updateInterval;

		private void OnEnable()
		{
			m_behaviourTree = serializedObject.FindProperty("m_behaviourTree");
			m_updateMode = serializedObject.FindProperty("m_updateMode");
			m_updateInterval = serializedObject.FindProperty("m_updateInterval");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(m_behaviourTree);
			EditorGUILayout.PropertyField(m_updateMode);
			if(m_updateMode.enumValueIndex == (int)UpdateMode.AtInterval)
			{
				EditorGUILayout.PropertyField(m_updateInterval);
			}

			serializedObject.ApplyModifiedProperties();


			Mind mind = (Mind)target;
			BTAsset btAsset = mind.GetBehaviourTree();
			BehaviourTree btInstance = mind.GetBehaviourTreeInstance();

			GUI.enabled = btAsset != null;
			if(EditorApplication.isPlaying)
			{
				if(GUILayout.Button("Preview", GUILayout.Height(24.0f)))
				{
					BehaviourTreeEditor.OpenDebug(btAsset, btInstance);
				}
			}
			else
			{
				if(GUILayout.Button("Edit", GUILayout.Height(24.0f)))
				{
					BehaviourTreeEditor.Open(btAsset);
				}
			}
			GUI.enabled = true;
		}
	}
}