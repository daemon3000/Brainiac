using Brainiac;
using System;
using UnityEditor;
using UnityEngine;

namespace BrainiacEditor
{
	public static class BTEditorStyle
	{
		private static GUISkin m_editorSkin;
		private static Texture m_arrowUp;
		private static Texture m_arrowDown;
		private static Texture m_breakpoint;
		private static Texture m_optionsIcon;

		private static BTGraphNodeStyle m_compositeStyle;
		private static BTGraphNodeStyle m_decoratorStyle;
		private static BTGraphNodeStyle m_actionStyle;

		private static GUIStyle m_editorFooter;
		private static GUIStyle m_selectionBoxStyle;
		private static GUIStyle m_multilineTextAreaStyle;
		private static GUIStyle m_listHeaderStyle;
		private static GUIStyle m_listBackgroundStyle;
		private static GUIStyle m_listButtonStyle;
		private static GUIStyle m_listDragHandleStyle;
		private static GUIStyle m_arrowUpButtonStyle;
		private static GUIStyle m_arrowDownButtonStyle;
		private static GUIStyle m_breadcrumbLeftStyle;
		private static GUIStyle m_breadcrumbLeftActiveStyle;
		private static GUIStyle m_breadcrumbMidStyle;
		private static GUIStyle m_breadcrumbMidActiveStyle;

		public static Texture ArrowUp
		{
			get
			{
				return m_arrowUp;
			}
		}

		public static Texture ArrowDown
		{
			get
			{
				return m_arrowDown;
			}
		}

		public static Texture Breakpoint
		{
			get
			{
				return m_breakpoint;
			}
		}

		public static Texture OptionsIcon
		{
			get
			{
				return m_optionsIcon;
			}
		}

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

		public static GUIStyle ArrowUpButton
		{
			get
			{
				return m_arrowUpButtonStyle;
			}
		}

		public static GUIStyle ArrowDownButton
		{
			get
			{
				return m_arrowDownButtonStyle;
			}
		}

		public static GUIStyle EditorFooter
		{
			get
			{
				return m_editorFooter;
			}
		}

		public static GUIStyle BreadcrumbLeft
		{
			get
			{
				return m_breadcrumbLeftStyle;
			}
		}

		public static GUIStyle BreadcrumbLeftActive
		{
			get
			{
				return m_breadcrumbLeftActiveStyle;
			}
		}

		public static GUIStyle BreadcrumbMiddle
		{
			get
			{
				return m_breadcrumbMidStyle;
			}
		}

		public static GUIStyle BreadcrumbMiddleActive
		{
			get
			{
				return m_breadcrumbMidActiveStyle;
			}
		}

		public static void EnsureStyle()
		{
			if(m_editorSkin == null)
			{
				m_editorSkin = Resources.Load<GUISkin>("Brainiac/editor_style");
			}

			if(m_arrowUp == null)
			{
				m_arrowUp = Resources.Load<Texture>("Brainiac/arrow_2_up");
			}
			if(m_arrowDown == null)
			{
				m_arrowDown = Resources.Load<Texture>("Brainiac/arrow_2_down");
			}

			if(m_breakpoint == null)
			{
				m_breakpoint = Resources.Load<Texture>("Brainiac/breakpoint");
			}

			if(m_optionsIcon == null)
			{
				m_optionsIcon = Resources.Load<Texture>("Brainiac/options_icon");
			}

			if(m_compositeStyle == null)
			{
				m_compositeStyle = new BTGraphNodeStyle("flow node hex 1", "flow node hex 1 on",
														"flow node hex 6", "flow node hex 6 on",
														"flow node hex 4", "flow node hex 4 on",
														"flow node hex 3", "flow node hex 3 on", 
														new Vector2(180, 35));
			}
			if(m_decoratorStyle == null)
			{
				m_decoratorStyle = new BTGraphNodeStyle("flow node hex 1", "flow node hex 1 on",
														"flow node hex 6", "flow node hex 6 on",
														"flow node hex 4", "flow node hex 4 on",
														"flow node hex 3", "flow node hex 3 on",
														new Vector2(180, 35));
			}
			if(m_actionStyle == null)
			{
				m_actionStyle = new BTGraphNodeStyle("flow node 0", "flow node 0 on",
													"flow node 6", "flow node 6 on",
													"flow node 4", "flow node 4 on",
													"flow node 3", "flow node 3 on",
													new Vector2(180, 40));
			}

			if(m_editorFooter == null)
			{
				m_editorFooter = new GUIStyle("ProjectBrowserHeaderBgTop");
				m_editorFooter.alignment = TextAnchor.MiddleRight;
				m_editorFooter.contentOffset = new Vector2(-10, 0);
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

			if(m_arrowUpButtonStyle == null)
			{
				m_arrowUpButtonStyle = m_editorSkin.FindStyle("arrow_up");
			}
			if(m_arrowDownButtonStyle == null)
			{
				m_arrowDownButtonStyle = m_editorSkin.FindStyle("arrow_down");
			}

			if(m_breadcrumbLeftStyle == null)
			{
				m_breadcrumbLeftStyle = new GUIStyle("GUIEditor.BreadcrumbLeft");
			}
			if(m_breadcrumbLeftActiveStyle == null)
			{
				m_breadcrumbLeftActiveStyle = new GUIStyle("GUIEditor.BreadcrumbLeft");
				m_breadcrumbLeftActiveStyle.normal.background = m_breadcrumbLeftActiveStyle.active.background;
			}
			if(m_breadcrumbMidStyle == null)
			{
				m_breadcrumbMidStyle = new GUIStyle("GUIEditor.BreadcrumbMid");
			}
			if(m_breadcrumbMidActiveStyle == null)
			{
				m_breadcrumbMidActiveStyle = new GUIStyle("GUIEditor.BreadcrumbMid");
				m_breadcrumbMidActiveStyle.normal.background = m_breadcrumbMidActiveStyle.active.background;
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

		public static Vector2 GetNodeSize(Type nodeType)
		{
			if(nodeType.IsSameOrSubclass(typeof(Composite)))
			{
				return m_compositeStyle.Size;
			}
			else if(nodeType.IsSameOrSubclass(typeof(Decorator)))
			{
				return m_decoratorStyle.Size;
			}
			else if(nodeType.IsSameOrSubclass(typeof(Brainiac.Action)))
			{
				return m_actionStyle.Size;
			}

			return new Vector2(180, 40);
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
					return new Color32(221, 186, 19, 255);
				case BehaviourNodeStatus.Success:
					return Color.green;
				}
			}

			return Color.white;
		}
	}
}
