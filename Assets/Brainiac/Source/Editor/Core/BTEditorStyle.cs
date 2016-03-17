using Brainiac;
using System;
using UnityEditor;
using UnityEngine;

namespace BrainiacEditor
{
	public static class BTEditorStyle
	{
		private static BTGraphNodeStyle m_compositeStyle;
		private static BTGraphNodeStyle m_decoratorStyle;
		private static BTGraphNodeStyle m_actionStyle;
		private static GUIStyle m_selectionBoxStyle;
		
		public static GUIStyle SelectionBox
		{
			get
			{
				return m_selectionBoxStyle;
			}
		}

		public static void EnsureStyle(GUISkin editorSkin)
		{
			if(m_compositeStyle == null)
			{
				m_compositeStyle = new BTGraphNodeStyle("flow node hex 1", "flow node hex 1 on",
														"flow node hex 6 on", "flow node hex 6 on",
														"flow node hex 5 on", "flow node hex 5 on",
														"flow node hex 3 on", "flow node hex 3 on");
			}
			if(m_decoratorStyle == null)
			{
				m_decoratorStyle = new BTGraphNodeStyle("flow node hex 1", "flow node hex 1 on",
														"flow node hex 6 on", "flow node hex 6 on",
														"flow node hex 5 on", "flow node hex 5 on",
														"flow node hex 3 on", "flow node hex 3 on");
			}
			if(m_actionStyle == null)
			{
				m_actionStyle = new BTGraphNodeStyle("flow node 0", "flow node 0 on",
													"flow node 6 on", "flow node 6 on",
													"flow node 5 on", "flow node 5 on",
													"flow node 3 on", "flow node 3 on");
			}
			if(m_selectionBoxStyle == null)
			{
				m_selectionBoxStyle = editorSkin.FindStyle("selection_box");
				if(m_selectionBoxStyle == null)
				{
					m_selectionBoxStyle = editorSkin.box;
				}
			}
		}

		public static BTGraphNodeStyle GetNodeStyle(Type nodeType)
		{
			if(nodeType.IsSameOrSubclass(typeof(Composite)))
			{
				return m_compositeStyle;
			}
			else if(nodeType.IsSameOrSubclass(typeof(Decorator)))
			{
				return m_decoratorStyle;
			}
			else if(nodeType.IsSameOrSubclass(typeof(Brainiac.Action)))
			{
				return m_actionStyle;
			}

			return null;
		}

		public static Color GetTransitionColor(BehaviourNodeStatus? status)
		{
			if(status.HasValue)
			{
				switch(status)
				{
				case BehaviourNodeStatus.Failure:
					return Color.red;
				case BehaviourNodeStatus.Running:
					return new Color32(248, 138, 29, 255);
				case BehaviourNodeStatus.Success:
					return Color.green;
				}
			}

			return Color.white;
		}
	}
}
