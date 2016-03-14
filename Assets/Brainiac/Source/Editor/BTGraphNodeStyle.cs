using UnityEngine;
using System;
using System.Collections;

namespace BrainiacEditor
{
	public class BTGraphNodeStyle
	{
		private GUIStyle m_normalStyle;
		private GUIStyle m_selectedStyle;
		private Vector2 m_size;
		private string m_normalStyleName;
		private string m_selectedStyleName;

		public Vector2 Size { get { return m_size; } }

		public BTGraphNodeStyle(string normalStyle, string selectedStyle, Vector2 size)
		{
			m_normalStyleName = normalStyle;
			m_selectedStyleName = selectedStyle;
			m_size = size;

			EnsureStyle();
		}

		public void EnsureStyle()
		{
			if(m_normalStyle == null)
			{
				m_normalStyle = (GUIStyle)m_normalStyleName;
			}
			if(m_selectedStyle == null)
			{
				m_selectedStyle = (GUIStyle)m_selectedStyleName;
			}
		}

		public GUIStyle GetStyle(bool isSelected)
		{
			return isSelected ? m_selectedStyle : m_normalStyle;
		}
	}
}