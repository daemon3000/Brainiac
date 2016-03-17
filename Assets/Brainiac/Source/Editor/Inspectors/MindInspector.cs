using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using Brainiac;

namespace BrainiacEditor
{
	[CustomEditor(typeof(Mind))]
	public class MindInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			Mind mind = (Mind)target;

			GUI.enabled = mind.BehaviourTree != null;
			if(EditorApplication.isPlaying)
			{
				if(GUILayout.Button("Live Preview", GUILayout.Height(24.0f)))
				{
					BehaviourTreeEditor.StartDebug(mind.BehaviourTree, mind.BehaviourTreeInstance);
				}
			}
			else
			{
				if(GUILayout.Button("Edit", GUILayout.Height(24.0f)))
				{
					BehaviourTreeEditor.Open(mind.BehaviourTree);
				}
			}
			GUI.enabled = true;
		}
	}
}