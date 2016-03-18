using Brainiac;
using System;
using UnityEditor;
using UnityEngine;

namespace BrainiacEditor
{
	public static class BTEditorStyle
	{
		private static GUISkin m_editorSkin;
		private static BTGraphNodeStyle m_compositeStyle;
		private static BTGraphNodeStyle m_decoratorStyle;
		private static BTGraphNodeStyle m_actionStyle;
		private static GUIStyle m_selectionBoxStyle;
		private static GUIStyle m_multilineTextAreaStyle;
		private static GUIStyle m_listHeaderStyle;
		private static GUIStyle m_listBackgroundStyle;
		private static GUIStyle m_listButtonStyle;
		private static GUIStyle m_listDragHandleStyle;
		private static GUIStyle m_arrowUpStyle;
		private static GUIStyle m_arrowDownStyle;

		public static GUIStyle SelectionBox
		{
			get
			{
				return m_selectionBoxStyle;
			}
		}

		public static GUIStyle MultilineTextArea
		{
			get
			{
				return m_multilineTextAreaStyle;
			}
		}

		public static GUIStyle ListHeader
		{
			get
			{
				return m_listHeaderStyle;
			}
		}

		public static GUIStyle ListBackground
		{
			get
			{
				return m_listBackgroundStyle;
			}
		}

		public static GUIStyle ListButton
		{
			get
			{
				return m_listButtonStyle;
			}
		}

		public static GUIStyle ListDragHandle
		{
			get
			{
				return m_listDragHandleStyle;
			}
		}

		public static GUIStyle ArrowUp
		{
			get
			{
				return m_arrowUpStyle;
			}
		}

		public static GUIStyle ArrowDown
		{
			get
			{
				return m_arrowDownStyle;
			}
		}

		public static void EnsureStyle()
		{
			if(m_editorSkin == null)
			{
				m_editorSkin = Resources.Load<GUISkin>("Brainiac/editor_style");
			}

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
				m_selectionBoxStyle = m_editorSkin.FindStyle("selection_box");
				if(m_selectionBoxStyle == null)
				{
					m_selectionBoxStyle = m_editorSkin.box;
				}
			}

			if(m_multilineTextAreaStyle == null)
			{
				m_multilineTextAreaStyle = new GUIStyle(EditorStyles.textField);
				m_multilineTextAreaStyle.wordWrap = true;
			}

			if(m_listHeaderStyle == null)
			{
				m_listHeaderStyle = new GUIStyle(Array.Find<GUIStyle>(GUI.skin.customStyles, obj => obj.name == "RL Header"));
				m_listHeaderStyle.normal.textColor = Color.black;
				m_listHeaderStyle.alignment = TextAnchor.MiddleLeft;
				m_listHeaderStyle.contentOffset = new Vector2(10, 0);
				m_listHeaderStyle.fontSize = 11;
			}

			if(m_listBackgroundStyle == null)
			{
				m_listBackgroundStyle = new GUIStyle("RL Background");
			}

			if(m_listButtonStyle == null)
			{
				m_listButtonStyle = new GUIStyle(Array.Find<GUIStyle>(GUI.skin.customStyles, obj => obj.name == "RL FooterButton"));
				m_listButtonStyle.alignment = TextAnchor.MiddleCenter;
			}

			if(m_listDragHandleStyle == null)
			{
				m_listDragHandleStyle = new GUIStyle("RL DragHandle");
			}

			if(m_arrowUpStyle == null)
			{
				m_arrowUpStyle = m_editorSkin.FindStyle("arrow_up");
			}
			if(m_arrowDownStyle == null)
			{
				m_arrowDownStyle = m_editorSkin.FindStyle("arrow_down");
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
