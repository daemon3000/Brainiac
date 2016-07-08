using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using Brainiac;
using System;

namespace BrainiacEditor
{
	[CustomEditor(typeof(Blackboard))]
	public class BlackboardInspector : Editor
	{
		private IBlackboardInspector m_inspector;

		private void OnEnable()
		{
			if(EditorApplication.isPlaying)
			{
				Blackboard blackboard = (Blackboard)target;
				IDictionary<string, object> dict = GetRuntimeValues(blackboard);
				if(dict != null)
				{
					m_inspector = new PlayTimeBlackboardInspector(dict);
				}
			}
			else
			{
				m_inspector = new DesignTimeBlackboardInspector(serializedObject);
			}
		}

		private IDictionary<string, object> GetRuntimeValues(Blackboard blackboard)
		{
			Type type = blackboard.GetType();
			FieldInfo fi = type.GetField("m_values", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

			if(fi != null)
				return fi.GetValue(blackboard) as IDictionary<string, object>;
			else
				return null;
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
				EditorGUILayout.HelpBox("There are no values to display!", MessageType.Error);
			}
		}
	}
}