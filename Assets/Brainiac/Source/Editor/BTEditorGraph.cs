using UnityEngine;
using System;
using System.Collections;
using Brainiac;

namespace BrainiacEditor
{
	public class BTEditorGraph : ScriptableObject
	{
		private BTEditorGraphNode m_root;

		public void SetBehaviourTree(BehaviourTree behaviourTree)
		{
			if(m_root == null)
			{
				m_root = BTEditorGraphNode.Create(null, behaviourTree.Root);
			}
		}

		public void DrawGUI()
		{
			m_root.DrawGUI();
		}

		private void OnDestroy()
		{
			BTEditorGraphNode.DestroyImmediate(m_root);
		}

		public static BTEditorGraph Create()
		{
			BTEditorGraph graph = ScriptableObject.CreateInstance<BTEditorGraph>();
			graph.hideFlags = HideFlags.HideAndDontSave;

			return graph;
		}
	}
}