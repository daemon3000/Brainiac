using UnityEngine;
using UnityEditor;


namespace BrainiacEditor
{
	public class PlayTimeMemoryInspector : IMemoryInspector
	{
		public void DrawGUI()
		{
			EditorGUILayout.HelpBox("The memory cannot be inspected during play-mode!", MessageType.Error);
		}
	}
}