using UnityEngine;
using UnityEditor;
using Brainiac;

namespace BrainiacEditor
{
	[CustomNodeInspector(typeof(RunBehaviour))]
	public class RunBehaviourInspector : NodeInspector
	{
		public override void OnInspectorGUI()
		{
			if(Target != null && Target is RunBehaviour)
			{
				RunBehaviour target = (RunBehaviour)Target;
				bool prevGUIState = GUI.enabled;

				DrawHeader();

				target.BehaviourTreeAsset = EditorGUILayout.ObjectField("Behaviour Tree", target.BehaviourTreeAsset, typeof(BTAsset), false) as BTAsset;
				EditorGUILayout.Space();

				if(BTEditorCanvas.Current.IsDebuging && target.BehaviourTreeAsset != null && target.BehaviourTree != null)
				{
					GUI.enabled = true;
					if(GUILayout.Button("Preview", GUILayout.Height(26.0f)))
					{
						BehaviourTreeEditor.OpenSubtreeDebug(target.BehaviourTreeAsset, target.BehaviourTree);
					}
				}
				else
				{
					GUI.enabled = target.BehaviourTreeAsset != null;
					if(GUILayout.Button("Open", GUILayout.Height(26.0f)))
					{
						BehaviourTreeEditor.OpenSubtree(target.BehaviourTreeAsset);
					}
				}
				
				GUI.enabled = prevGUIState;

				RepaintCanvas();
			}
		}
	}
}