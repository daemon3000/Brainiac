using UnityEngine;
using UnityEditor;
using Brainiac;

namespace BrainiacEditor
{
	[CustomEditor(typeof(AIAgent))]
	public class AIAgentInspector : Editor
	{
		private SerializedProperty m_behaviourTree;
		private SerializedProperty m_updateMode;
		private SerializedProperty m_updateInterval;
		private SerializedProperty m_body;
		private SerializedProperty m_memory;
		private SerializedProperty m_debugMode;

		private void OnEnable()
		{
			m_behaviourTree = serializedObject.FindProperty("m_behaviourTree");
			m_updateMode = serializedObject.FindProperty("m_updateMode");
			m_updateInterval = serializedObject.FindProperty("m_updateInterval");
			m_body = serializedObject.FindProperty("m_body");
			m_memory = serializedObject.FindProperty("m_memory");
			m_debugMode = serializedObject.FindProperty("m_debugMode");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			GUI.enabled = !EditorApplication.isPlaying;
			EditorGUILayout.PropertyField(m_behaviourTree);
			EditorGUILayout.PropertyField(m_memory);
			EditorGUILayout.PropertyField(m_body);
			EditorGUILayout.PropertyField(m_updateMode);
			if(m_updateMode.enumValueIndex == (int)UpdateMode.AtInterval)
			{
				EditorGUILayout.PropertyField(m_updateInterval);
			}

			GUI.enabled = true;
			GUI.color = m_debugMode.boolValue ? Color.green : Color.red;
			m_debugMode.boolValue = GUILayout.Toggle(m_debugMode.boolValue, "Debug", "Button", GUILayout.Height(24.0f));
			GUI.color = Color.white;

			serializedObject.ApplyModifiedProperties();

			AIAgent agent = (AIAgent)target;
			BTAsset btAsset = m_behaviourTree.objectReferenceValue as BTAsset;
			BehaviourTree btInstance = agent.GetBehaviourTree();

			GUI.enabled = btAsset != null;
			if(EditorApplication.isPlaying && btInstance != null)
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
