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
		private static GUIStyle m_logoStyle;

		public static GUIStyle LogoStyle
		{
			get { return m_logoStyle; }
		}

		public static void EnsureStyle()
		{
			if(m_compositeStyle == null)
			{
				m_compositeStyle = new BTGraphNodeStyle("flow node hex 1", "flow node hex 1 on", new Vector2(120, 40));
			}
			if(m_decoratorStyle == null)
			{
				m_decoratorStyle = new BTGraphNodeStyle("flow node hex 1", "flow node hex 1 on", new Vector2(120, 40));
			}
			if(m_actionStyle == null)
			{
				m_actionStyle = new BTGraphNodeStyle("flow node 0", "flow node 0 on", new Vector2(100, 30));
			}
			if(m_logoStyle == null)
			{
				m_logoStyle = new GUIStyle(EditorStyles.largeLabel);
				m_logoStyle.alignment = TextAnchor.LowerLeft;
				m_logoStyle.fontSize = 50;
				m_logoStyle.normal.textColor = new Color32(158, 255, 125, 255);
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
	}
}
