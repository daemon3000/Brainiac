using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using Brainiac;

namespace BrainiacEditor
{
	[CustomEditor(typeof(Memory))]
	public class MemoryInspector : Editor
	{
		private IMemoryInspector m_inspector;

		private void OnEnable()
		{
			if(EditorApplication.isPlaying)
			{
				m_inspector = new PlayTimeMemoryInspector();
			}
			else
			{
				m_inspector = new DesignTimeMemoryInspector(serializedObject);
			}
		}

		public override void OnInspectorGUI()
		{
			BTEditorStyle.EnsureStyle();
			m_inspector.DrawGUI();
		}
	}
}