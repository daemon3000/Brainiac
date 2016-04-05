using UnityEngine;
using Brainiac;

namespace BrainiacEditor
{
	public class BTGraphNodeStyle
	{
		private const float VERT_MAX_NODE_WIDTH = 180;
		private const float VERT_MAX_NODE_HEIGHT = 200;
		private const float VERT_MIN_NODE_WIDTH = 60;
		private const float VERT_MIN_NODE_HEIGHT = 40;
		private const float HORZ_MIN_NODE_HEIGHT = 40;
		private const float HORZ_NODE_WIDTH = 180;
		private const float NODE_BORDER = 20;

		private static GUIContent m_content = new GUIContent();

		private GUIStyle m_standardNormalStyle;
		private GUIStyle m_standardSelectedStyle;
		private GUIStyle m_failNormalStyle;
		private GUIStyle m_failSelectedStyle;
		private GUIStyle m_runninfNormalStyle;
		private GUIStyle m_runningSelectedStyle;
		private GUIStyle m_successNormalStyle;
		private GUIStyle m_successSelectedStyle;

		private string m_standardNormalStyleName;
		private string m_standardSelectedStyleName;
		private string m_failNormalStyleName;
		private string m_failSelectedStyleName;
		private string m_runninfNormalStyleName;
		private string m_runningSelectedStyleName;
		private string m_successNormalStyleName;
		private string m_successSelectedStyleName;

		public BTGraphNodeStyle(string standardNormalStyleName, string standardSelectedStyleName, 
								string failNormalStyleName, string failSelectedStyleName,
								string runningNormalStyleName, string runningSelectedStyleName, 
								string successNormalStyleName, string successSelectedStyleName)
		{
			m_standardNormalStyleName = standardNormalStyleName;
			m_standardSelectedStyleName = standardSelectedStyleName;
			m_failNormalStyleName = failNormalStyleName;
			m_failSelectedStyleName = failSelectedStyleName;
			m_runninfNormalStyleName = runningNormalStyleName;
			m_runningSelectedStyleName = runningSelectedStyleName;
			m_successNormalStyleName = successNormalStyleName;
			m_successSelectedStyleName = successSelectedStyleName;

			EnsureStyle();
		}

		public void EnsureStyle()
		{
			if(m_standardNormalStyle == null)
			{
				m_standardNormalStyle = CreateStyle(m_standardNormalStyleName);
			}
			if(m_standardSelectedStyle == null)
			{
				m_standardSelectedStyle = CreateStyle(m_standardSelectedStyleName);
			}

			if(m_failNormalStyle == null)
			{
				m_failNormalStyle = CreateStyle(m_failNormalStyleName);
			}
			if(m_failSelectedStyle == null)
			{
				m_failSelectedStyle = CreateStyle(m_failSelectedStyleName);
			}

			if(m_runninfNormalStyle == null)
			{
				m_runninfNormalStyle = CreateStyle(m_runninfNormalStyleName);
			}
			if(m_runningSelectedStyle == null)
			{
				m_runningSelectedStyle = CreateStyle(m_runningSelectedStyleName);
			}

			if(m_successNormalStyle == null)
			{
				m_successNormalStyle = CreateStyle(m_successNormalStyleName);
			}
			if(m_successSelectedStyle == null)
			{
				m_successSelectedStyle = CreateStyle(m_successSelectedStyleName);
			}
		}

		private GUIStyle CreateStyle(string source)
		{
			GUIStyle style = (GUIStyle)source;
			style.wordWrap = true;
			style.alignment = TextAnchor.UpperCenter;

			return style;
		}

		public GUIStyle GetStyle(BehaviourNodeStatus status, bool isSelected)
		{
			switch(status)
			{
			case BehaviourNodeStatus.Failure:
				return !isSelected ? m_failNormalStyle : m_failSelectedStyle;
			case BehaviourNodeStatus.Running:
				return !isSelected ? m_runninfNormalStyle : m_runningSelectedStyle;
			case BehaviourNodeStatus.Success:
				return !isSelected ? m_successNormalStyle : m_successSelectedStyle;
			}

			return !isSelected ? m_standardNormalStyle : m_standardSelectedStyle;
		}

		public Vector2 GetSize(string content, BTEditorTreeLayout layout)
		{
			if(!string.IsNullOrEmpty(content))
			{
				if(layout == BTEditorTreeLayout.Horizontal)
					return GetSizeHorizontal(content);
				else
					return GetSizeVertical(content);
			}

			return new Vector2(180, 40);
		}

		private Vector2 GetSizeVertical(string content)
		{
			m_content.text = content;
			Vector2 size = m_standardNormalStyle.CalcSize(m_content);
			size.x = Mathf.Max(size.x, VERT_MIN_NODE_WIDTH);
			size.y = Mathf.Max(size.y, VERT_MIN_NODE_HEIGHT);
			if(size.x > VERT_MAX_NODE_WIDTH)
			{
				size.x = VERT_MAX_NODE_WIDTH;
				size.y = Mathf.Min(m_standardNormalStyle.CalcHeight(m_content, size.x), VERT_MAX_NODE_HEIGHT);
			}

			size.x += NODE_BORDER;

			float snapSize = BTEditorCanvas.Current.SnapSize * 2;
			size.x = (float)Mathf.Round(size.x / snapSize) * snapSize;
			size.y = (float)Mathf.Round(size.y / snapSize) * snapSize;

			return size;
		}

		private Vector2 GetSizeHorizontal(string content)
		{
			m_content.text = content;
			Vector2 size = new Vector2();
			size.x = HORZ_NODE_WIDTH;
			size.y = Mathf.Max(m_standardNormalStyle.CalcHeight(m_content, HORZ_NODE_WIDTH), HORZ_MIN_NODE_HEIGHT);

			float snapSize = BTEditorCanvas.Current.SnapSize * 2;
			size.x = (float)Mathf.Round(size.x / snapSize) * snapSize;

			return size;
		}
	}
}