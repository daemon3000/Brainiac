using UnityEditor;
using System.Collections.Generic;
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
				Memory memory = (Memory)target;
				IDictionary<string, object> dict = memory.GetMemory();
				if(dict != null)
				{
					m_inspector = new PlayTimeMemoryInspector(memory.GetMemory());
				}
			}
			else
			{
				m_inspector = new DesignTimeMemoryInspector(serializedObject);
			}
		}

		public override void OnInspectorGUI()
		{
			if(m_inspector != null)
			{
				BTEditorStyle.EnsureStyle();
				m_inspector.DrawGUI();
				Repaint();
			}
			else
			{
				EditorGUILayout.HelpBox("There is no memory to display!", MessageType.Error);
			}
		}
	}
}