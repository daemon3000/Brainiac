using UnityEngine;
using UnityEditor;
using Brainiac;

namespace BrainiacEditor
{
	[CustomEditor(typeof(Agent))]
	public class AgentInspector : Editor
	{
		private SerializedProperty m_body;
		private SerializedProperty m_mind;
		private SerializedProperty m_memory;
		private SerializedProperty m_debugMode;

		private void OnEnable()
		{
			m_body = serializedObject.FindProperty("m_body");
			m_mind = serializedObject.FindProperty("m_mind");
			m_memory = serializedObject.FindProperty("m_memory");
			m_debugMode = serializedObject.FindProperty("m_debugMode");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.PropertyField(m_body);
			EditorGUILayout.PropertyField(m_mind);
			EditorGUILayout.PropertyField(m_memory);
			
			GUI.color = m_debugMode.boolValue ? Color.green : Color.red;
			m_debugMode.boolValue = GUILayout.Toggle(m_debugMode.boolValue, "Debug", "Button", GUILayout.Height(24.0f));
			GUI.color = Color.white;

			serializedObject.ApplyModifiedProperties();
		}
	}
}
