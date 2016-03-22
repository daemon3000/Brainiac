using UnityEngine;
using System;
using System.Collections;
using Brainiac;

namespace BrainiacEditor
{
	public class BTGraphNodeStyle
	{
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

		public Vector2 Size { get; private set; }

		public BTGraphNodeStyle(string standardNormalStyleName, string standardSelectedStyleName, 
								string failNormalStyleName, string failSelectedStyleName,
								string runningNormalStyleName, string runningSelectedStyleName, 
								string successNormalStyleName, string successSelectedStyleName, Vector2 size)
		{
			m_standardNormalStyleName = standardNormalStyleName;
			m_standardSelectedStyleName = standardSelectedStyleName;
			m_failNormalStyleName = failNormalStyleName;
			m_failSelectedStyleName = failSelectedStyleName;
			m_runninfNormalStyleName = runningNormalStyleName;
			m_runningSelectedStyleName = runningSelectedStyleName;
			m_successNormalStyleName = successNormalStyleName;
			m_successSelectedStyleName = successSelectedStyleName;
			Size = size;

			EnsureStyle();
		}

		public void EnsureStyle()
		{
			if(m_standardNormalStyle == null)
			{
				m_standardNormalStyle = (GUIStyle)m_standardNormalStyleName;
				m_standardNormalStyle.wordWrap = true;
			}
			if(m_standardSelectedStyle == null)
			{
				m_standardSelectedStyle = (GUIStyle)m_standardSelectedStyleName;
				m_standardSelectedStyle.wordWrap = true;
			}

			if(m_failNormalStyle == null)
			{
				m_failNormalStyle = (GUIStyle)m_failNormalStyleName;
				m_failNormalStyle.wordWrap = true;
			}
			if(m_failSelectedStyle == null)
			{
				m_failSelectedStyle = (GUIStyle)m_failSelectedStyleName;
				m_failSelectedStyle.wordWrap = true;
			}

			if(m_runninfNormalStyle == null)
			{
				m_runninfNormalStyle = (GUIStyle)m_runninfNormalStyleName;
				m_runninfNormalStyle.wordWrap = true;
			}
			if(m_runningSelectedStyle == null)
			{
				m_runningSelectedStyle = (GUIStyle)m_runningSelectedStyleName;
				m_runningSelectedStyle.wordWrap = true;
			}

			if(m_successNormalStyle == null)
			{
				m_successNormalStyle = (GUIStyle)m_successNormalStyleName;
				m_successNormalStyle.wordWrap = true;
			}
			if(m_successSelectedStyle == null)
			{
				m_successSelectedStyle = (GUIStyle)m_successSelectedStyleName;
				m_successSelectedStyle.wordWrap = true;
			}
		}

		public GUIStyle GetStyle(BehaviourNodeStatus? status, bool isSelected)
		{
			if(status.HasValue)
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
			}

			return !isSelected ? m_standardNormalStyle : m_standardSelectedStyle;
		}
	}
}