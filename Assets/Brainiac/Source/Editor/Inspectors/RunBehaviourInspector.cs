using UnityEngine;
using UnityEditor;
using Brainiac;

namespace BrainiacEditor
{
	[CustomNodeInspector(typeof(RunBehaviour))]
	public class RunBehaviourInspector : NodeInspector
	{
		private BTAsset m_btAsset;

		public override void SetTarget(BehaviourNode target)
		{
			base.SetTarget(target);

			RunBehaviour action = target as RunBehaviour;
			if(action != null && action.BehaviourTreePath != null)
			{
				m_btAsset = Resources.Load<BTAsset>(action.BehaviourTreePath);
			}
			else
			{
				m_btAsset = null;
			}
		}

		public override void OnInspectorGUI()
		{
			if(Target != null && Target is RunBehaviour)
			{
				RunBehaviour target = (RunBehaviour)Target;
				BTAsset oldAsset = m_btAsset;
				bool prevGUIState = GUI.enabled;

				target.Name = EditorGUILayout.TextField("Name", target.Name);
				EditorGUILayout.LabelField("Description");
				target.Description = EditorGUILayout.TextArea(target.Description, BTEditorStyle.MultilineTextArea);

				EditorGUILayout.Space();
				m_btAsset = EditorGUILayout.ObjectField("Behaviour Tree", m_btAsset, typeof(BTAsset), false) as BTAsset;
				if(m_btAsset != oldAsset)
				{
					if(m_btAsset != null)
					{
						target.BehaviourTreePath = BTEditorUtils.GetResourcePath(m_btAsset);
						if(string.IsNullOrEmpty(target.BehaviourTreePath))
						{
							m_btAsset = null;
							EditorUtility.DisplayDialog("Warning", "The asset you selected is not in a Resources folder!", "OK");
						}
					}
					else
					{
						target.BehaviourTreePath = null;
					}
				}

				EditorGUILayout.Space();

				if(BTEditorCanvas.Current.IsDebuging && m_btAsset != null && target.BehaviourTree != null)
				{
					GUI.enabled = true;
					if(GUILayout.Button("Preview", GUILayout.Height(26.0f)))
					{
						BehaviourTreeEditor.OpenSubtreeDebug(m_btAsset, target.BehaviourTree);
					}
				}
				else
				{
					GUI.enabled = m_btAsset != null;
					if(GUILayout.Button("Open", GUILayout.Height(26.0f)))
					{
						BehaviourTreeEditor.OpenSubtree(m_btAsset);
					}
				}
				
				GUI.enabled = prevGUIState;

				if(BTEditorCanvas.Current != null)
				{
					BTEditorCanvas.Current.Repaint();
				}
			}
		}
	}
}